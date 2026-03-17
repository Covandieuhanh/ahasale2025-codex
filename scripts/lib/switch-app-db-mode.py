#!/usr/bin/env python3
import pathlib
import re
import sys

ROOT = pathlib.Path(__file__).resolve().parents[2]
MODE_MAP = {
    "local": ROOT / "config" / "db-modes" / "local.connectionStrings.xml",
    "shared-online": ROOT / "config" / "db-modes" / "shared-online.connectionStrings.xml",
}
CONFIG_FILES = [ROOT / "Web.config", ROOT / "Web.local.config"]


def usage() -> int:
    print("Usage: switch-app-db-mode.py <local|shared-online>", file=sys.stderr)
    return 2


def main() -> int:
    if len(sys.argv) != 2:
        return usage()
    mode = sys.argv[1].strip().lower()
    snippet_path = MODE_MAP.get(mode)
    if snippet_path is None:
        return usage()

    snippet = snippet_path.read_text(encoding="utf-8").strip()
    pattern = re.compile(r"<connectionStrings>.*?</connectionStrings>", re.IGNORECASE | re.DOTALL)

    for config_path in CONFIG_FILES:
        text = config_path.read_text(encoding="utf-8")
        updated, count = pattern.subn(snippet, text, count=1)
        if count != 1:
            raise RuntimeError(f"Could not replace <connectionStrings> block in {config_path}")
        config_path.write_text(updated, encoding="utf-8")
        print(f"Updated {config_path} -> {mode}")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
