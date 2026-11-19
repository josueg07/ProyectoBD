"""Publish a full source snapshot with a pinned image AFTER registry publication.

Uses a temporary git index, a normal fast-forward push and no working-tree edits.
No production API or credentials are needed beyond the GitHub Actions token.
"""
import argparse
import json
import os
from pathlib import Path
import re
import subprocess
import tempfile

RELEASE_BRANCH = "deploy/production"
COMPOSE_PATH = "deploy/docker/compose.dokploy.yaml"


def git(*args, data=None, env=None):
    return subprocess.check_output(["git", *args], input=data, env=env).decode().strip()


def pin_image(source, image):
    if not re.fullmatch(r"josueg07/proyectobd-web@sha256:[0-9a-f]{64}", image):
        raise ValueError("Expected the published josueg07 image with an immutable SHA256 digest")
    source, count = re.subn(r"(?m)^(  web:\r?\n    image: )[^\r\n]+", rf"\g<1>{image}", source)
    if count != 1:
        raise ValueError("Expected exactly one web image immediately after the web service")
    return source


def publish(revision, image, push):
    if os.environ.get("GITHUB_OUTPUT"):
        with open(os.environ["GITHUB_OUTPUT"], "a") as output:
            output.write("released=false\n")
    if not re.fullmatch(r"[0-9a-f]{40}", revision):
        raise ValueError("Expected a full source commit SHA")
    if git("rev-parse", "HEAD") != revision:
        raise ValueError("Checkout must match the image source revision")
    source = git("show", f"{revision}:{COMPOSE_PATH}") + "\n"
    pinned = pin_image(source, image)
    # The GitHub integration follows this branch, never main's early push event.
    head = git("ls-remote", "origin", "refs/heads/main").split()
    if not head or head[0] != revision:
        print("Source is no longer main HEAD; not releasing an outdated build.")
        return
    previous = git("ls-remote", "origin", f"refs/heads/{RELEASE_BRANCH}").split()
    if previous:
        subprocess.check_call(["git", "fetch", "--no-tags", "origin", f"refs/heads/{RELEASE_BRANCH}"])
    parents = [previous[0]] if previous else []
    if revision not in parents:
        parents.append(revision)
    with tempfile.TemporaryDirectory(prefix="proyectobd-release-") as directory:
        env = dict(os.environ, GIT_INDEX_FILE=str(Path(directory) / "index"))
        env.update(GIT_AUTHOR_NAME="ProyectoBD CI", GIT_AUTHOR_EMAIL="ci@proyectobd.invalid",
                   GIT_COMMITTER_NAME="ProyectoBD CI", GIT_COMMITTER_EMAIL="ci@proyectobd.invalid")
        git("read-tree", revision, env=env)
        files = {COMPOSE_PATH: pinned, "deploy/release.json": json.dumps({"revision": revision, "image": image}) + "\n"}
        for path, content in files.items():
            blob = git("hash-object", "-w", "--stdin", data=content.encode(), env=env)
            git("update-index", "--add", "--cacheinfo", f"100644,{blob},{path}", env=env)
        tree = git("write-tree", env=env)
        arguments = ["commit-tree", tree]
        for parent in parents:
            arguments += ["-p", parent]
        commit = git(*arguments, data=f"Deploy source {revision}\n".encode(), env=env)
    print(f"Release {commit}: source {revision}, image {image}")
    if push:
        # No force. Concurrent external updates fail rather than overwrite history.
        subprocess.check_call(["git", "push", "origin", f"{commit}:refs/heads/{RELEASE_BRANCH}"])
        if os.environ.get("GITHUB_OUTPUT"):
            with open(os.environ["GITHUB_OUTPUT"], "a") as output:
                output.write("released=true\n")
    else:
        print("Dry run: release branch was NOT pushed.")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--revision", required=True)
    parser.add_argument("--image", required=True)
    parser.add_argument("--push", action="store_true")
    args = parser.parse_args()
    publish(args.revision, args.image, args.push)
