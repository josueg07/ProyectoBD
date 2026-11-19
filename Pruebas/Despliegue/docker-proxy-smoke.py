"""Isolated real-image checks. Creates no SQL service and uses no existing volume."""
import json
import subprocess
import time
import uuid

IMAGE = "proyectobd-web:https-cicd-test"
PREFIX = "proyectobd-smoke-" + uuid.uuid4().hex[:8]
NETWORK = PREFIX + "-network"
WEB = PREFIX + "-web"
PROXY = PREFIX + "-proxy"
LABEL = "proyectobd.ephemeral-test=" + PREFIX
created = []


def docker(*args):
    return subprocess.check_output(["docker", *args], stderr=subprocess.STDOUT).decode().strip()


def run(name, *args):
    docker("run", "-d", "--name", name, "--label", LABEL, "--network", NETWORK, *args)
    created.append(name)


def call(peer, path="/", forwarded=True):
    args = ["exec", peer, "curl", "-sS", "--max-time", "10", "-D", "-", "-H", "Host: proyectobd.duckdns.org"]
    if forwarded:
        args += ["-H", "X-Forwarded-Proto: https", "-H", "X-Forwarded-For: 203.0.113.10",
                 "-H", "X-Forwarded-Host: evil.invalid"]
    args += ["http://" + WEB + ":8080" + path]
    result = docker(*args).replace("\r", "")
    head, _, body = result.partition("\n\n")
    lines = head.splitlines()
    return int(lines[0].split()[1]), {k.lower(): v.strip() for k, v in (line.split(":", 1) for line in lines[1:] if ":" in line)}, body


try:
    docker("network", "create", "--label", LABEL, NETWORK)
    subnet = json.loads(docker("network", "inspect", NETWORK))[0]["IPAM"]["Config"][0]["Subnet"]
    run(PROXY, "--network-alias", "traefik-test", "--entrypoint", "sleep", IMAGE, "600")

    def start_web():
        run(WEB, "-e", "TRUSTED_PROXY_HOSTS=traefik-test", "-e", "TRUSTED_PROXY_NETWORKS=" + subnet,
            "-e", "ASPNETCORE_HTTPS_PORT=443", IMAGE)
        for _ in range(30):
            try:
                if call(PROXY, "/health")[0] == 200:
                    return
            except subprocess.CalledProcessError:
                pass
            time.sleep(1)
        raise RuntimeError("Isolated web did not become ready")

    start_web()
    status, headers, _ = call(PROXY)
    assert status == 302 and headers["location"].startswith("https://proyectobd.duckdns.org/Account/Login"), (status, headers.get("location"))
    assert "strict-transport-security" in headers
    print("PASS actual cookie challenge redirects to HTTPS, forwarded host ignored, HSTS present")
    assert call(WEB)[0] == 400
    print("PASS a different container on the same network cannot impersonate Traefik")
    assert call(WEB, "/health", False)[0] == 200
    print("PASS internal health probe remains available")
    status, _, body = call(PROXY, "/deployment.json")
    assert status == 200 and json.loads(body)["revision"] == "cicd-local-test"
    print("PASS image carries the expected non-sensitive revision marker")
    docker("rm", "-f", WEB)
    created.remove(WEB)
    start_web()
    assert call(PROXY)[1]["location"].startswith("https://")
    print("PASS HTTPS configuration survives recreation of the isolated web container")
    print("No SQL service, production container or existing volume was touched.")
finally:
    for name in reversed(created):
        # Only delete resources this invocation created and still owns.
        actual = docker("inspect", name, "--format", '{{index .Config.Labels "proyectobd.ephemeral-test"}}')
        if actual == PREFIX:
            docker("rm", "-f", name)
    try:
        if docker("network", "inspect", NETWORK, "--format", '{{index .Labels "proyectobd.ephemeral-test"}}') == PREFIX:
            docker("network", "rm", NETWORK)
    except subprocess.CalledProcessError:
        pass
