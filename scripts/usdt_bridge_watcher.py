#!/usr/bin/env python3
import argparse
import hashlib
import hmac
import json
import os
import sys
import time
import uuid
from datetime import datetime, timezone
from decimal import Decimal, InvalidOperation
from pathlib import Path
from urllib import error, parse, request

DEFAULT_BEP20_TOKEN_CONTRACT = "0x55d398326f99059ff775485246999027b3197955"
DEFAULT_BSCSCAN_API_URL = "https://api.etherscan.io/v2/api"
DEFAULT_BSC_CHAIN_ID = 56
DEFAULT_BSC_RPC_URL = "https://bsc-dataseed.binance.org"
DEFAULT_CHAIN_DATA_PROVIDER = "rpc_balance"
DEFAULT_RPC_BLOCK_STEP = 200
DEFAULT_RPC_MIN_BLOCK_STEP = 1
DEFAULT_RPC_REQUEST_DELAY_MS = 120
DEFAULT_INITIAL_LOOKBACK_BLOCKS = 5000
DEFAULT_TOKEN_DECIMALS = -1

TRANSFER_TOPIC = "0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55aebf523b3ef"
DECIMALS_METHOD_DATA = "0x313ce567"
BALANCE_OF_METHOD_DATA_PREFIX = "0x70a08231"
SYNTHETIC_COUNTERPARTY = "0x0000000000000000000000000000000000000001"


def log(message: str) -> None:
    now = datetime.now(timezone.utc).strftime("%Y-%m-%d %H:%M:%S")
    print(f"[{now} UTC] {message}", flush=True)


def load_env_file(path: Path) -> None:
    if not path.exists():
        return
    for line in path.read_text(encoding="utf-8").splitlines():
        raw = line.strip()
        if not raw or raw.startswith("#") or "=" not in raw:
            continue
        key, value = raw.split("=", 1)
        key = key.strip()
        value = value.strip().strip('"').strip("'")
        if key and key not in os.environ:
            os.environ[key] = value


def read_required_env(key: str) -> str:
    value = os.getenv(key, "").strip()
    if not value:
        raise RuntimeError(f"Missing required env: {key}")
    return value


def read_int_env(key: str, default: int) -> int:
    value = os.getenv(key, "").strip()
    if not value:
        return default
    try:
        return int(value)
    except ValueError:
        return default


def read_state(path: Path) -> dict:
    if not path.exists():
        return {"processed": {}}
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except Exception:
        return {"processed": {}}


def write_state(path: Path, state: dict) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(state, ensure_ascii=False, indent=2), encoding="utf-8")


def trim_state(state: dict, max_items: int = 5000) -> None:
    processed = state.get("processed", {})
    if len(processed) <= max_items:
        return
    items = sorted(processed.items(), key=lambda x: x[1], reverse=True)[:max_items]
    state["processed"] = dict(items)


def safe_decimal(value) -> Decimal:
    if value is None:
        return Decimal("0")
    if isinstance(value, (int, float)):
        return Decimal(str(value))
    text = str(value).strip()
    if not text:
        return Decimal("0")
    return Decimal(text)


def format_decimal(value: Decimal) -> str:
    text = format(value.normalize(), "f")
    if "." in text:
        text = text.rstrip("0").rstrip(".")
    return text or "0"


def normalize_evm_address(value: str) -> str:
    return (value or "").strip().lower()


def parse_token_amount(item: dict) -> Decimal:
    direct = item.get("tokenAmount")
    if direct is not None:
        try:
            return safe_decimal(direct)
        except Exception:
            return Decimal("0")

    raw_amount = item.get("value")
    if raw_amount is None:
        return Decimal("0")

    amount_text = str(raw_amount).strip()
    if not amount_text:
        return Decimal("0")

    token_decimals = item.get("tokenDecimal")
    try:
        decimals = int(str(token_decimals)) if token_decimals is not None else 18
    except ValueError:
        decimals = 18

    try:
        raw_int = Decimal(amount_text)
    except InvalidOperation:
        return Decimal("0")

    if decimals <= 0:
        return raw_int

    scale = Decimal(10) ** decimals
    return raw_int / scale


def parse_confirmations(item: dict) -> int:
    raw = item.get("confirmations")
    if raw is None:
        return 0
    try:
        return int(str(raw))
    except ValueError:
        return 0


def parse_timestamp(item: dict) -> int:
    raw = item.get("timeStamp")
    if raw is None:
        raw = item.get("blockNumber")
        if raw is None:
            return 0
    try:
        return int(str(raw))
    except ValueError:
        return 0


def parse_tx_hash(item: dict) -> str:
    return (item.get("hash") or "").strip().lower()


def parse_from_address(item: dict) -> str:
    return normalize_evm_address(item.get("from") or "")


def parse_to_address(item: dict) -> str:
    return normalize_evm_address(item.get("to") or "")


def parse_token_contract(item: dict) -> str:
    return normalize_evm_address(item.get("contractAddress") or "")


def parse_block_number(item: dict):
    raw = item.get("blockNumber")
    if raw is None:
        return None
    try:
        return int(str(raw))
    except ValueError:
        return None


def parse_log_index(item: dict) -> str:
    raw = item.get("logIndex")
    if raw is None:
        raw = item.get("transactionIndex")
    if raw is None:
        return ""
    return str(raw).strip()


def build_event_key(item: dict) -> str:
    tx_hash = parse_tx_hash(item)
    if not tx_hash:
        return ""
    log_index = parse_log_index(item)
    if log_index:
        return f"{tx_hash}:{log_index}"
    return tx_hash


def parse_hex_int(raw: str) -> int:
    text = str(raw or "").strip().lower()
    if not text:
        return 0
    if text.startswith("0x"):
        text = text[2:]
    if not text:
        return 0
    return int(text, 16)


def parse_state_int(value, default: int) -> int:
    try:
        return int(str(value))
    except Exception:
        return default


def address_to_topic(address: str) -> str:
    normalized = normalize_evm_address(address)
    if normalized.startswith("0x"):
        normalized = normalized[2:]
    return "0x" + ("0" * 24) + normalized


def topic_to_address(topic: str) -> str:
    text = (topic or "").strip().lower()
    if text.startswith("0x"):
        text = text[2:]
    if len(text) < 40:
        return ""
    return "0x" + text[-40:]


def encode_balance_of_data(address: str) -> str:
    normalized = normalize_evm_address(address)
    if normalized.startswith("0x"):
        normalized = normalized[2:]
    if len(normalized) != 40:
        raise RuntimeError(f"Invalid address for balanceOf(): {address}")
    return BALANCE_OF_METHOD_DATA_PREFIX + ("0" * 24) + normalized


def raw_to_token_amount(raw_amount: int, token_decimals: int) -> Decimal:
    amount = Decimal(int(raw_amount))
    if token_decimals <= 0:
        return amount
    scale = Decimal(10) ** token_decimals
    return amount / scale


def build_synthetic_tx_hash(*parts) -> str:
    joined = "|".join(str(p) for p in parts)
    return "0x" + hashlib.sha256(joined.encode("utf-8")).hexdigest()


def commit_balance_checkpoint(state: dict, item: dict) -> None:
    next_balance_raw = item.get("_next_balance_raw")
    next_safe_block = item.get("_next_safe_block")
    if next_balance_raw is None or next_safe_block is None:
        return
    state["last_confirmed_balance_raw"] = str(next_balance_raw)
    state["last_balance_safe_block"] = parse_state_int(next_safe_block, -1)
    token_decimals = item.get("tokenDecimal")
    if token_decimals is not None:
        try:
            token_decimals_int = int(str(token_decimals))
            if 0 <= token_decimals_int <= 36:
                state["last_token_decimals"] = token_decimals_int
        except Exception:
            pass


def rpc_call(rpc_url: str, method: str, params, timeout_sec: int):
    payload = json.dumps(
        {"jsonrpc": "2.0", "id": 1, "method": method, "params": params},
        separators=(",", ":"),
    ).encode("utf-8")

    req = request.Request(
        rpc_url,
        data=payload,
        method="POST",
        headers={
            "Content-Type": "application/json",
            "Accept": "application/json",
            "User-Agent": "aha-usdt-bridge/1.0",
        },
    )

    with request.urlopen(req, timeout=timeout_sec) as resp:
        body = json.loads(resp.read().decode("utf-8"))

    if not isinstance(body, dict):
        raise RuntimeError("RPC invalid response shape")
    if body.get("error") is not None:
        raise RuntimeError(str(body.get("error")))
    return body.get("result")


def fetch_token_decimals_rpc(config: dict) -> int:
    configured = int(config.get("token_decimals", DEFAULT_TOKEN_DECIMALS))
    if configured >= 0:
        return configured

    try:
        result = rpc_call(
            config["bsc_rpc_url"],
            "eth_call",
            [{"to": config["token_contract"], "data": DECIMALS_METHOD_DATA}, "latest"],
            config["http_timeout"],
        )
        decimals = parse_hex_int(result)
        if decimals < 0 or decimals > 36:
            raise RuntimeError(f"Unexpected token decimals: {decimals}")
        config["token_decimals"] = decimals
        return decimals
    except Exception as ex:
        log(f"RPC decimals() failed ({ex}); fallback to 18")
        config["token_decimals"] = 18
        return 18


def hex_block(value: int) -> str:
    return hex(max(int(value), 0))


def map_rpc_log_to_item(log_item: dict, latest_block: int, token_decimals: int) -> dict:
    block_number = parse_hex_int(log_item.get("blockNumber"))
    log_index = parse_hex_int(log_item.get("logIndex"))
    confirmations = max(0, latest_block - block_number + 1)

    topics = log_item.get("topics") or []
    from_address = topic_to_address(topics[1] if len(topics) > 1 else "")
    to_address = topic_to_address(topics[2] if len(topics) > 2 else "")
    amount_raw = parse_hex_int(log_item.get("data"))

    return {
        "hash": (log_item.get("transactionHash") or "").strip().lower(),
        "from": from_address,
        "to": to_address,
        "contractAddress": normalize_evm_address(log_item.get("address") or ""),
        "blockNumber": str(block_number),
        "timeStamp": str(block_number),
        "confirmations": str(confirmations),
        "value": str(amount_raw),
        "tokenDecimal": str(token_decimals),
        "logIndex": str(log_index),
    }


def is_rpc_limit_error(ex: Exception) -> bool:
    text = str(ex or "").lower()
    return (
        "limit exceeded" in text
        or "-32005" in text
        or "query returned more than" in text
        or "response size exceeded" in text
    )


def fetch_token_balance_raw_rpc(config: dict, block_number: int) -> int:
    result = rpc_call(
        config["bsc_rpc_url"],
        "eth_call",
        [
            {
                "to": config["token_contract"],
                "data": encode_balance_of_data(config["deposit_address"]),
            },
            hex_block(block_number),
        ],
        config["http_timeout"],
    )
    return parse_hex_int(result)


def fetch_logs_range_rpc(
    rpc_url: str,
    token_contract: str,
    deposit_topic: str,
    from_block: int,
    to_block: int,
    timeout_sec: int,
):
    inbound_logs = rpc_call(
        rpc_url,
        "eth_getLogs",
        [
            {
                "fromBlock": hex_block(from_block),
                "toBlock": hex_block(to_block),
                "address": token_contract,
                "topics": [TRANSFER_TOPIC, None, deposit_topic],
            }
        ],
        timeout_sec,
    ) or []

    outbound_logs = rpc_call(
        rpc_url,
        "eth_getLogs",
        [
            {
                "fromBlock": hex_block(from_block),
                "toBlock": hex_block(to_block),
                "address": token_contract,
                "topics": [TRANSFER_TOPIC, deposit_topic],
            }
        ],
        timeout_sec,
    ) or []

    return inbound_logs, outbound_logs


def fetch_bsc_transfers_rpc_logs(config: dict, state: dict):
    timeout_sec = config["http_timeout"]
    rpc_url = config["bsc_rpc_url"]

    try:
        latest_block = parse_hex_int(rpc_call(rpc_url, "eth_blockNumber", [], timeout_sec))
    except error.HTTPError as ex:
        raw = ex.read().decode("utf-8", errors="ignore").strip()
        suffix = f" body={raw[:220]}" if raw else ""
        log(f"RPC HTTP error {ex.code}.{suffix}")
        return []
    except error.URLError as ex:
        log(f"RPC network error: {ex}")
        return []
    except Exception as ex:
        log(f"RPC latest block failed: {ex}")
        return []

    safe_tip = latest_block - max(config["min_confirmations"], 1) + 1
    if safe_tip < 0:
        return []

    start_block = int(state.get("last_scanned_block", 0) or 0)
    if start_block <= 0:
        lookback = max(config["initial_lookback_blocks"], 100)
        start_block = max(safe_tip - lookback + 1, 0)

    if start_block > safe_tip:
        return []

    token_decimals = fetch_token_decimals_rpc(config)
    state["last_token_decimals"] = token_decimals
    deposit_topic = address_to_topic(config["deposit_address"])
    step = max(config["rpc_block_step"], 20)
    min_step = max(config["rpc_min_block_step"], 1)
    req_delay_ms = max(config["rpc_request_delay_ms"], 0)
    events = []

    current_block = start_block
    while current_block <= safe_tip:
        current_step = min(step, safe_tip - current_block + 1)
        range_skipped = False

        while True:
            to_block = min(current_block + current_step - 1, safe_tip)
            try:
                inbound_logs, outbound_logs = fetch_logs_range_rpc(
                    rpc_url=rpc_url,
                    token_contract=config["token_contract"],
                    deposit_topic=deposit_topic,
                    from_block=current_block,
                    to_block=to_block,
                    timeout_sec=timeout_sec,
                )
                break
            except Exception as ex:
                if is_rpc_limit_error(ex) and current_step > min_step:
                    reduced = max(min_step, current_step // 2)
                    if reduced >= current_step:
                        reduced = current_step - 1
                    if reduced < min_step:
                        reduced = min_step
                    log(
                        f"RPC limit exceeded for block range {current_block}-{to_block}. "
                        f"Reduce step {current_step} -> {reduced}."
                    )
                    current_step = reduced
                    continue

                if is_rpc_limit_error(ex) and current_step <= min_step:
                    log(
                        f"RPC limit exceeded even at min step={min_step} "
                        f"for block {current_block}. Skip this block."
                    )
                    current_block += 1
                    range_skipped = True
                    inbound_logs = []
                    outbound_logs = []
                    break

                log(f"RPC getLogs failed for block range {current_block}-{to_block}: {ex}")
                return []

        if range_skipped:
            continue

        for raw_log in inbound_logs:
            events.append(map_rpc_log_to_item(raw_log, latest_block, token_decimals))
        for raw_log in outbound_logs:
            events.append(map_rpc_log_to_item(raw_log, latest_block, token_decimals))

        if req_delay_ms > 0:
            time.sleep(req_delay_ms / 1000.0)

        current_block = to_block + 1

    deduped = {}
    for item in events:
        key = build_event_key(item)
        if not key:
            continue
        deduped[key] = item

    sorted_items = sorted(
        deduped.values(),
        key=lambda x: (
            parse_block_number(x) or 0,
            int(parse_log_index(x) or "0"),
            parse_tx_hash(x),
        ),
    )

    state["last_scanned_block"] = safe_tip + 1
    return sorted_items


def fetch_bsc_transfers_rpc_balance(config: dict, state: dict):
    timeout_sec = config["http_timeout"]
    rpc_url = config["bsc_rpc_url"]

    try:
        latest_block = parse_hex_int(rpc_call(rpc_url, "eth_blockNumber", [], timeout_sec))
    except error.HTTPError as ex:
        raw = ex.read().decode("utf-8", errors="ignore").strip()
        suffix = f" body={raw[:220]}" if raw else ""
        log(f"RPC HTTP error {ex.code}.{suffix}")
        return []
    except error.URLError as ex:
        log(f"RPC network error: {ex}")
        return []
    except Exception as ex:
        log(f"RPC latest block failed: {ex}")
        return []

    safe_tip = latest_block - max(config["min_confirmations"], 1) + 1
    if safe_tip < 0:
        return []

    token_decimals = fetch_token_decimals_rpc(config)
    state["last_token_decimals"] = token_decimals

    try:
        current_raw = fetch_token_balance_raw_rpc(config, safe_tip)
    except error.HTTPError as ex:
        raw = ex.read().decode("utf-8", errors="ignore").strip()
        suffix = f" body={raw[:220]}" if raw else ""
        log(f"RPC HTTP error {ex.code}.{suffix}")
        return []
    except error.URLError as ex:
        log(f"RPC network error: {ex}")
        return []
    except Exception as ex:
        log(f"RPC balanceOf() failed: {ex}")
        return []

    previous_raw_text = str(state.get("last_confirmed_balance_raw", "")).strip()
    previous_safe_block = parse_state_int(state.get("last_balance_safe_block"), -1)
    if not previous_raw_text:
        state["last_confirmed_balance_raw"] = str(current_raw)
        state["last_balance_safe_block"] = safe_tip
        state["last_token_decimals"] = token_decimals
        baseline_amount = raw_to_token_amount(current_raw, token_decimals)
        log(
            f"RPC balance baseline initialized at block {safe_tip}. "
            f"balance={format_decimal(baseline_amount)}"
        )
        return []

    if safe_tip <= previous_safe_block:
        return []

    previous_raw = parse_state_int(previous_raw_text, current_raw)
    delta_raw = current_raw - previous_raw

    if delta_raw == 0:
        state["last_confirmed_balance_raw"] = str(current_raw)
        state["last_balance_safe_block"] = safe_tip
        state["last_token_decimals"] = token_decimals
        return []

    direction = "in" if delta_raw > 0 else "out"
    delta_abs_raw = abs(delta_raw)
    delta_amount = raw_to_token_amount(delta_abs_raw, token_decimals)
    delta_text = format_decimal(delta_amount)

    synthetic_tx = build_synthetic_tx_hash(
        "rpc_balance",
        config["deposit_address"],
        config["token_contract"],
        safe_tip,
        previous_raw,
        current_raw,
        direction,
    )

    previous_amount_text = format_decimal(raw_to_token_amount(previous_raw, token_decimals))
    current_amount_text = format_decimal(raw_to_token_amount(current_raw, token_decimals))

    log(
        f"RPC balance delta block={safe_tip} direction={direction} amount={delta_text} "
        f"(prev={previous_amount_text}, current={current_amount_text})"
    )

    if direction == "in":
        from_address = SYNTHETIC_COUNTERPARTY
        to_address = config["deposit_address"]
    else:
        from_address = config["deposit_address"]
        to_address = SYNTHETIC_COUNTERPARTY

    return [
        {
            "hash": synthetic_tx,
            "from": from_address,
            "to": to_address,
            "contractAddress": config["token_contract"],
            "blockNumber": str(safe_tip),
            "timeStamp": str(safe_tip),
            "confirmations": str(max(config["min_confirmations"], 1)),
            "value": str(delta_abs_raw),
            "tokenDecimal": str(token_decimals),
            "tokenAmount": delta_text,
            "logIndex": "0",
            "_next_balance_raw": str(current_raw),
            "_next_safe_block": str(safe_tip),
        }
    ]


def fetch_bsc_transfers(
    api_base: str,
    api_key: str,
    chain_id: int,
    deposit_address: str,
    token_contract: str,
    batch_size: int,
    timeout_sec: int,
):
    query = parse.urlencode(
        {
            "chainid": chain_id,
            "module": "account",
            "action": "tokentx",
            "address": deposit_address,
            "contractaddress": token_contract,
            "page": 1,
            "offset": batch_size,
            "sort": "desc",
            "apikey": api_key,
        }
    )
    url = f"{api_base.rstrip('/')}?{query}"
    req = request.Request(
        url,
        headers={
            "Accept": "application/json",
            "User-Agent": "aha-usdt-bridge/1.0",
        },
    )
    try:
        with request.urlopen(req, timeout=timeout_sec) as resp:
            payload = json.loads(resp.read().decode("utf-8"))
    except error.HTTPError as ex:
        raw = ex.read().decode("utf-8", errors="ignore").strip()
        if ex.code == 401:
            log("Scan API HTTP 401 Unauthorized. Check BSCSCAN_API_KEY/ETHERSCAN_API_KEY in scripts/usdt_bridge.env.")
        else:
            suffix = f" body={raw[:220]}" if raw else ""
            log(f"BSCScan HTTP error {ex.code}.{suffix}")
        return []
    except error.URLError as ex:
        log(f"BSCScan network error: {ex}")
        return []
    except Exception as ex:
        log(f"BSCScan response parse error: {ex}")
        return []

    if not isinstance(payload, dict):
        log("BSCScan returned invalid payload type.")
        return []

    result = payload.get("result")
    if isinstance(result, list):
        return result

    status = str(payload.get("status", "")).strip()
    message = str(payload.get("message", "")).strip()
    if status == "0":
        detail = str(result).strip() if result is not None else ""
        if "deprecated V1 endpoint" in detail:
            log("Provider rejected V1 endpoint. Set BSCSCAN_API_URL=https://api.etherscan.io/v2/api and BSC_CHAIN_ID=56.")
            return []
        if "Free API access is not supported for this chain" in detail:
            log("Etherscan free plan does not support BSC for this endpoint. Set CHAIN_DATA_PROVIDER=rpc (balance mode).")
            return []
        if detail:
            log(f"BSCScan status=0 message={message} detail={detail}")
        else:
            log(f"BSCScan status=0 message={message}")
    else:
        log("BSCScan returned no transaction list.")

    return []


def post_credit(api_url: str, api_key: str, signing_secret: str, payload: dict, timeout_sec: int):
    body_text = json.dumps(payload, ensure_ascii=False, separators=(",", ":"))
    body = body_text.encode("utf-8")
    timestamp = str(int(time.time()))
    nonce = uuid.uuid4().hex
    signing_input = f"{timestamp}\n{nonce}\n{body_text}"
    signature = hmac.new(
        signing_secret.encode("utf-8"),
        signing_input.encode("utf-8"),
        hashlib.sha256,
    ).hexdigest()

    req = request.Request(
        api_url,
        data=body,
        method="POST",
        headers={
            "Content-Type": "text/plain; charset=utf-8",
            "X-Bridge-Key": api_key,
            "X-Bridge-Timestamp": timestamp,
            "X-Bridge-Nonce": nonce,
            "X-Bridge-Signature": signature,
            "User-Agent": "aha-usdt-bridge/1.0",
        },
    )
    try:
        with request.urlopen(req, timeout=timeout_sec) as resp:
            text = resp.read().decode("utf-8")
            return resp.getcode(), json.loads(text) if text else {}
    except error.HTTPError as ex:
        text = ex.read().decode("utf-8", errors="ignore")
        try:
            body_json = json.loads(text) if text else {}
        except Exception:
            body_json = {"raw": text}
        return ex.code, body_json


def process_once(config: dict, state: dict):
    provider = (config.get("chain_data_provider") or DEFAULT_CHAIN_DATA_PROVIDER).strip().lower()
    if provider == "etherscan":
        transfers = fetch_bsc_transfers(
            api_base=config["bscscan_api_url"],
            api_key=config["bscscan_api_key"],
            chain_id=config["bsc_chain_id"],
            deposit_address=config["deposit_address"],
            token_contract=config["token_contract"],
            batch_size=config["batch_size"],
            timeout_sec=config["http_timeout"],
        )
    elif provider == "rpc_logs":
        transfers = fetch_bsc_transfers_rpc_logs(config, state)
    else:
        transfers = fetch_bsc_transfers_rpc_balance(config, state)

    if not transfers:
        log("No transfer data returned from provider.")
        return

    transfers_sorted = sorted(transfers, key=parse_timestamp)

    for item in transfers_sorted:
        tx_hash = parse_tx_hash(item)
        if not tx_hash:
            continue

        event_key = build_event_key(item)
        dedupe_key = event_key or tx_hash
        if dedupe_key in state["processed"] or tx_hash in state["processed"]:
            continue

        token_contract = parse_token_contract(item)
        if token_contract and token_contract != config["token_contract"]:
            continue

        from_address = parse_from_address(item)
        to_address = parse_to_address(item)
        deposit_address = config["deposit_address"]

        if to_address == deposit_address and from_address == deposit_address:
            state["processed"][dedupe_key] = datetime.now(timezone.utc).isoformat()
            state["processed"][tx_hash] = datetime.now(timezone.utc).isoformat()
            trim_state(state)
            continue

        if to_address == deposit_address:
            direction = "in"
        elif from_address == deposit_address:
            direction = "out"
        else:
            continue

        confirmations = parse_confirmations(item)
        if confirmations < config["min_confirmations"]:
            continue

        amount = parse_token_amount(item)
        if amount <= 0:
            continue

        payload = {
            "tx_hash": tx_hash,
            "direction": direction,
            "chain": "BSC",
            "token_contract": token_contract or config["token_contract"],
            "from_address": from_address,
            "to_address": to_address,
            "block_number": parse_block_number(item),
            "confirmations": confirmations,
            "token_amount": format(amount.normalize(), "f"),
            "usdt_amount": format(amount.normalize(), "f"),
            "observed_at_utc": datetime.now(timezone.utc).isoformat(),
        }

        code, body = post_credit(
            api_url=config["bridge_api_url"],
            api_key=config["bridge_api_key"],
            signing_secret=config["bridge_signing_secret"],
            payload=payload,
            timeout_sec=config["http_timeout"],
        )

        ok = bool(body.get("ok")) if isinstance(body, dict) else False
        status = body.get("status", "") if isinstance(body, dict) else ""
        message = body.get("message", "") if isinstance(body, dict) else ""

        if code == 200 and ok:
            log(
                f"Applied tx={tx_hash} key={dedupe_key} direction={direction} "
                f"amount={payload['token_amount']} status={status}"
            )
            if provider == "rpc_balance":
                commit_balance_checkpoint(state, item)
            state["processed"][dedupe_key] = datetime.now(timezone.utc).isoformat()
            state["processed"][tx_hash] = datetime.now(timezone.utc).isoformat()
            trim_state(state)
            continue

        if code == 200 and status == "already_processed":
            log(f"Skip already processed tx={tx_hash} key={dedupe_key}")
            if provider == "rpc_balance":
                commit_balance_checkpoint(state, item)
            state["processed"][dedupe_key] = datetime.now(timezone.utc).isoformat()
            state["processed"][tx_hash] = datetime.now(timezone.utc).isoformat()
            trim_state(state)
            continue

        log(
            f"Bridge rejected tx={tx_hash} key={dedupe_key} direction={direction} "
            f"http={code} status={status} message={message}"
        )


def build_config() -> dict:
    deposit_address = normalize_evm_address(read_required_env("BSC_DEPOSIT_ADDRESS"))
    token_contract = normalize_evm_address(
        os.getenv("BSC_TOKEN_CONTRACT", "").strip()
        or os.getenv("BSC_USDT_CONTRACT", DEFAULT_BEP20_TOKEN_CONTRACT).strip()
    )
    provider = os.getenv("CHAIN_DATA_PROVIDER", DEFAULT_CHAIN_DATA_PROVIDER).strip().lower()
    if provider in ("rpc", "rpc_balance"):
        provider = "rpc_balance"
    elif provider in ("rpc_logs", "rpclogs", "rpc-log", "rpc_log"):
        provider = "rpc_logs"
    elif provider != "etherscan":
        provider = DEFAULT_CHAIN_DATA_PROVIDER

    bscscan_api_url = os.getenv("BSCSCAN_API_URL", "").strip() or os.getenv("ETHERSCAN_API_URL", "").strip() or DEFAULT_BSCSCAN_API_URL
    bscscan_api_key = os.getenv("BSCSCAN_API_KEY", "").strip() or os.getenv("ETHERSCAN_API_KEY", "").strip()

    config = {
        "bridge_api_url": read_required_env("USDT_BRIDGE_API_URL"),
        "bridge_api_key": read_required_env("USDT_BRIDGE_API_KEY"),
        "bridge_signing_secret": read_required_env("USDT_BRIDGE_SIGNING_SECRET"),
        "chain_data_provider": provider,
        "bscscan_api_url": bscscan_api_url,
        "bscscan_api_key": bscscan_api_key,
        "bsc_chain_id": read_int_env("BSC_CHAIN_ID", DEFAULT_BSC_CHAIN_ID),
        "bsc_rpc_url": os.getenv("BSC_RPC_URL", DEFAULT_BSC_RPC_URL).strip(),
        "rpc_block_step": read_int_env("RPC_BLOCK_STEP", DEFAULT_RPC_BLOCK_STEP),
        "rpc_min_block_step": read_int_env("RPC_MIN_BLOCK_STEP", DEFAULT_RPC_MIN_BLOCK_STEP),
        "rpc_request_delay_ms": read_int_env("RPC_REQUEST_DELAY_MS", DEFAULT_RPC_REQUEST_DELAY_MS),
        "initial_lookback_blocks": read_int_env("INITIAL_LOOKBACK_BLOCKS", DEFAULT_INITIAL_LOOKBACK_BLOCKS),
        "token_decimals": read_int_env("TOKEN_DECIMALS", DEFAULT_TOKEN_DECIMALS),
        "deposit_address": deposit_address,
        "token_contract": token_contract,
        "min_confirmations": read_int_env("MIN_CONFIRMATIONS", 20),
        "poll_seconds": read_int_env("POLL_SECONDS", 30),
        "batch_size": read_int_env("BATCH_SIZE", 50),
        "http_timeout": read_int_env("HTTP_TIMEOUT", 20),
        "state_file": os.getenv("STATE_FILE", "./scripts/usdt_bridge_state.json").strip(),
    }

    if provider == "etherscan" and not bscscan_api_key:
        raise RuntimeError("Missing required env: BSCSCAN_API_KEY (or ETHERSCAN_API_KEY) for etherscan mode")

    return config


def main():
    parser = argparse.ArgumentParser(description="AhaSale Token -> Point bridge watcher (BSC BEP20)")
    parser.add_argument("--env-file", default="scripts/usdt_bridge.env", help="Path env file")
    parser.add_argument("--once", action="store_true", help="Run one poll cycle and exit")
    args = parser.parse_args()

    load_env_file(Path(args.env_file))

    try:
        config = build_config()
    except RuntimeError as ex:
        log(str(ex))
        return 1

    state_path = Path(config["state_file"])
    state = read_state(state_path)
    if "processed" not in state or not isinstance(state["processed"], dict):
        state = {"processed": {}}

    log(
        "Watcher started. "
        f"Provider={config['chain_data_provider']} "
        f"Endpoint={config['bridge_api_url']} Address={config['deposit_address']} "
        f"TokenContract={config['token_contract']}"
    )

    while True:
        try:
            process_once(config, state)
            write_state(state_path, state)
        except Exception as ex:
            log(f"Watcher error: {ex}")

        if args.once:
            break
        time.sleep(max(config["poll_seconds"], 5))

    return 0


if __name__ == "__main__":
    sys.exit(main())
