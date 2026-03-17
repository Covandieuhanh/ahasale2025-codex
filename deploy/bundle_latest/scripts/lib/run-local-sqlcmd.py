#!/usr/bin/env python3
import os
import subprocess
import sys


def main() -> int:
    db_container = os.environ.get("DB_CONTAINER", "ahasale_local_db")
    local_db_name = os.environ.get("LOCAL_DB_NAME", "ahasale_local")
    login_timeout = os.environ.get("DB_LOGIN_TIMEOUT", "5")
    query_timeout = os.environ.get("DB_QUERY_TIMEOUT", "15")
    overall_timeout = int(os.environ.get("SQLCMD_TIMEOUT_SECONDS", "30"))

    cmd = [
        "docker",
        "exec",
        "-i",
        db_container,
        "/opt/mssql-tools18/bin/sqlcmd",
        "-b",
        "-C",
        "-S",
        "localhost",
        "-U",
        "sa",
        "-P",
        "AhaSaleLocal#2026",
        "-l",
        login_timeout,
        "-t",
        query_timeout,
        "-d",
        local_db_name,
        *sys.argv[1:],
    ]

    stdin = None if sys.stdin.isatty() else sys.stdin.buffer
    try:
        completed = subprocess.run(cmd, stdin=stdin, timeout=overall_timeout)
        return completed.returncode
    except subprocess.TimeoutExpired:
        print(
            f"Timed out after {overall_timeout}s while waiting for docker sqlcmd against {local_db_name}.",
            file=sys.stderr,
        )
        return 124


if __name__ == "__main__":
    raise SystemExit(main())
