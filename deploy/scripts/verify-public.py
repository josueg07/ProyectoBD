"""Verify the actual public image revision, HTTPS redirects and spoofed headers."""
import argparse
import json
import time
import urllib.error
import urllib.parse
import urllib.request


class NoRedirect(urllib.request.HTTPRedirectHandler):
    def redirect_request(self, req, fp, code, msg, headers, newurl):
        return None


def request(url, headers=None):
    opener = urllib.request.build_opener(NoRedirect)
    req = urllib.request.Request(url, headers=headers or {})
    try:
        response = opener.open(req, timeout=15)
    except urllib.error.HTTPError as error:
        response = error
    with response:
        return response.status, response.headers, response.read(1024 * 1024)


def verify(base, revision=None):
    parsed = urllib.parse.urlsplit(base)
    if parsed.scheme != "https" or not parsed.netloc or parsed.username or parsed.password:
        raise ValueError("Use an HTTPS origin without credentials")
    base = base.rstrip("/")
    for forged in [{}, {"X-Forwarded-Proto": "http", "X-Forwarded-Host": "evil.invalid",
                       "X-Forwarded-For": "127.0.0.1", "Forwarded": "proto=http;host=evil.invalid"},
                   {"X-Forwarded-Proto": "https, http", "X-Forwarded-For": "10.0.1.7"}]:
        status, headers, _ = request(base + "/", forged)
        target = urllib.parse.urlsplit(headers.get("Location", ""))
        if status != 302 or target.scheme != "https" or target.netloc != parsed.netloc or target.path != "/Account/Login":
            raise RuntimeError("Root must redirect to HTTPS login on the official host, including with forged forwarded headers")
    status, _, body = request(base + "/health")
    if status != 200 or body.decode().strip() != "Healthy":
        raise RuntimeError("Public health check failed")
    status, _, _ = request(base + "/Account/Login")
    if status != 200:
        raise RuntimeError("HTTPS login page failed")
    if revision:
        status, _, body = request(base + "/deployment.json", {"Cache-Control": "no-cache"})
        if status != 200 or json.loads(body).get("revision") != revision:
            raise RuntimeError("Public image revision has not reached the expected commit")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--base-url", required=True)
    parser.add_argument("--revision")
    parser.add_argument("--attempts", type=int, default=1)
    args = parser.parse_args()
    for attempt in range(max(1, args.attempts)):
        try:
            verify(args.base_url, args.revision)
            print("PASS public HTTPS, login, health, forged headers" + (f", revision {args.revision}" if args.revision else ""))
            break
        except (RuntimeError, ValueError, OSError) as error:
            if attempt + 1 >= args.attempts:
                raise SystemExit(f"FAIL public verification: {error}")
            print(f"Waiting for release ({attempt + 1}/{args.attempts})")
            time.sleep(5)
