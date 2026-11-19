import importlib.util
import json
import os
from pathlib import Path
import subprocess
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[2]
SCRIPT = ROOT / "deploy/scripts/publish-release.py"
spec = importlib.util.spec_from_file_location("release", SCRIPT)
release = importlib.util.module_from_spec(spec)
spec.loader.exec_module(release)
IMAGE = "josueg07/proyectobd-web@sha256:" + "a" * 64


def git(where, *args):
    return subprocess.check_output(["git", "-C", str(where), *args], stderr=subprocess.PIPE).decode().strip()


class ReleaseTests(unittest.TestCase):
    def test_pin_only_web(self):
        source = (ROOT / release.COMPOSE_PATH).read_text()
        pinned = release.pin_image(source, IMAGE)
        self.assertIn("    image: " + IMAGE, pinned)
        self.assertEqual(source.replace("josueg07/proyectobd-web:latest", IMAGE), pinned)
        self.assertIn("sql-data:/var/opt/mssql", pinned)
        self.assertIn("dp-keys:/var/dpkeys", pinned)

    def test_refuse_mutable_or_other_registry(self):
        for bad in ["josueg07/proyectobd-web:latest", IMAGE.replace("josueg07", "attacker")]:
            with self.assertRaises(ValueError):
                release.pin_image("  web:\n    image: old\n", bad)

    def test_fast_forward_release_contains_matching_source_and_image(self):
        with tempfile.TemporaryDirectory(prefix="proyectobd-ci-test-") as temp:
            base = Path(temp)
            remote, work = base / "remote.git", base / "work"
            subprocess.check_call(["git", "init", "--bare", str(remote)], stdout=subprocess.DEVNULL)
            subprocess.check_call(["git", "init", "-b", "main", str(work)], stdout=subprocess.DEVNULL)
            git(work, "config", "user.name", "Test")
            git(work, "config", "user.email", "test@example.invalid")
            path = work / release.COMPOSE_PATH
            path.parent.mkdir(parents=True)
            path.write_text((ROOT / release.COMPOSE_PATH).read_text())
            (work / "visible.txt").write_text("version one")
            git(work, "add", ".")
            git(work, "commit", "-m", "initial")
            git(work, "remote", "add", "origin", str(remote))
            git(work, "push", "origin", "main")
            first = git(work, "rev-parse", "HEAD")
            subprocess.check_call(["python", str(SCRIPT), "--revision", first, "--image", IMAGE, "--push"], cwd=work)
            initial_release = git(work, "ls-remote", "origin", "refs/heads/deploy/production").split()[0]
            self.assertEqual(json.loads(git(remote, "show", "deploy/production:deploy/release.json"))["revision"], first)
            self.assertIn(IMAGE, git(remote, "show", "deploy/production:" + release.COMPOSE_PATH))
            (work / "visible.txt").write_text("version two")
            git(work, "add", ".")
            git(work, "commit", "-m", "visible change")
            git(work, "push", "origin", "main")
            second = git(work, "rev-parse", "HEAD")
            subprocess.check_call(["python", str(SCRIPT), "--revision", second, "--image", IMAGE, "--push"], cwd=work)
            git(remote, "merge-base", "--is-ancestor", initial_release, "deploy/production")
            self.assertEqual("version two", git(remote, "show", "deploy/production:visible.txt"))
            self.assertEqual(second, git(work, "rev-parse", "HEAD"))
            self.assertEqual("", git(work, "status", "--porcelain"))
            before = git(remote, "rev-parse", "deploy/production")
            git(work, "checkout", "--detach", first)
            subprocess.check_call(["python", str(SCRIPT), "--revision", first, "--image", IMAGE, "--push"], cwd=work)
            self.assertEqual(before, git(remote, "rev-parse", "deploy/production"))


if __name__ == "__main__":
    unittest.main()
