#!/usr/bin/env bash
set -euo pipefail
ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
python3 - "$ROOT_DIR/Web.config" "$ROOT_DIR/Web.local.config" <<'PY'
import re
import sys
for path in sys.argv[1:]:
    text = open(path, 'r', encoding='utf-8').read()
    host = re.search(r"Data Source=([^;\"']+)", text, re.I)
    db = re.search(r"Initial Catalog=([^;\"']+)", text, re.I)
    user = re.search(r"User ID=([^;\"']+)", text, re.I)
    host_v = host.group(1).strip() if host else "?"
    db_v = db.group(1).strip() if db else "?"
    user_v = user.group(1).strip() if user else "?"
    if host_v.lower() in {"db,1433", "db", "localhost", "127.0.0.1", ".", "(local)"}:
        mode = "local"
    elif host_v == "112.78.2.206":
        mode = "shared-online"
    else:
        mode = "custom"
    print(f"{path}: mode={mode}, host={host_v}, db={db_v}, user={user_v}")
PY
