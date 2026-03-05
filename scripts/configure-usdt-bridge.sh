#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ENV_FILE="$ROOT_DIR/scripts/usdt_bridge.env"
WEB_CONFIG="$ROOT_DIR/Web.config"

DEFAULT_TOKEN_CONTRACT="0x55d398326f99059ff775485246999027b3197955"
DEFAULT_CHAIN_ID="56"
DEFAULT_API_URL="https://api.etherscan.io/v2/api"
DEFAULT_RPC_URL="https://bsc-dataseed.binance.org"
DEFAULT_TREASURY="vitonggianhangdoitac"
DEFAULT_POINT_RATE="1"
DEFAULT_MIN_CONFIRMATIONS="20"

deposit_address=""
etherscan_api_key=""
bridge_api_key=""
bridge_signing_secret=""
treasury_account=""
point_rate="$DEFAULT_POINT_RATE"
min_confirmations="$DEFAULT_MIN_CONFIRMATIONS"
token_contract="$DEFAULT_TOKEN_CONTRACT"
chain_data_provider="rpc"
bsc_rpc_url="$DEFAULT_RPC_URL"
restart_services="true"

usage() {
  cat <<'EOF'
Usage:
  ./scripts/configure-usdt-bridge.sh \
    --deposit-address 0x... \
    [--provider rpc|rpc_balance|rpc_logs|etherscan] \
    [--etherscan-api-key YOUR_KEY] \
    [--rpc-url https://bsc-dataseed.binance.org] \
    [--bridge-api-key INTERNAL_KEY] \
    [--bridge-signing-secret INTERNAL_SECRET] \
    [--treasury-account ACCOUNT] \
    [--point-rate 1000] \
    [--min-confirmations 20] \
    [--token-contract 0x55d398326f99059ff775485246999027b3197955] \
    [--no-restart]

Notes:
  - If --bridge-api-key or --bridge-signing-secret are omitted, secure random values will be generated.
  - This script updates both files:
      scripts/usdt_bridge.env
      Web.config
EOF
}

escape_sed_replacement() {
  printf '%s' "$1" | sed -e 's/[&|\\]/\\&/g'
}

escape_regex() {
  printf '%s' "$1" | sed -e 's/[][(){}.^$*+?|\/]/\\&/g'
}

set_env_value() {
  local key="$1"
  local value="$2"
  local escaped
  escaped="$(escape_sed_replacement "$value")"
  if grep -q "^${key}=" "$ENV_FILE"; then
    sed -i '' -E "s|^${key}=.*$|${key}=${escaped}|" "$ENV_FILE"
  else
    printf '\n%s=%s\n' "$key" "$value" >> "$ENV_FILE"
  fi
}

set_web_config_value() {
  local key="$1"
  local value="$2"
  local key_regex
  local value_escaped
  key_regex="$(escape_regex "$key")"
  value_escaped="$(escape_sed_replacement "$value")"

  if ! grep -q "key=\"$key\"" "$WEB_CONFIG"; then
    echo "Missing key in Web.config: $key" >&2
    exit 1
  fi

  sed -i '' -E \
    "s|(<add[[:space:]]+key=\"$key_regex\"[[:space:]]+value=\")[^\"]*(\"[[:space:]]*/>)|\\1${value_escaped}\\2|" \
    "$WEB_CONFIG"
}

get_web_config_value() {
  local key="$1"
  sed -n -E "s|.*<add[[:space:]]+key=\"$key\"[[:space:]]+value=\"([^\"]*)\"[[:space:]]*/>.*|\\1|p" "$WEB_CONFIG" | head -n 1
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --deposit-address)
      deposit_address="${2:-}"; shift 2 ;;
    --etherscan-api-key|--bscscan-api-key)
      etherscan_api_key="${2:-}"; shift 2 ;;
    --bridge-api-key)
      bridge_api_key="${2:-}"; shift 2 ;;
    --bridge-signing-secret)
      bridge_signing_secret="${2:-}"; shift 2 ;;
    --treasury-account)
      treasury_account="${2:-}"; shift 2 ;;
    --point-rate)
      point_rate="${2:-}"; shift 2 ;;
    --min-confirmations)
      min_confirmations="${2:-}"; shift 2 ;;
    --token-contract)
      token_contract="${2:-}"; shift 2 ;;
    --provider)
      chain_data_provider="${2:-}"; shift 2 ;;
    --rpc-url)
      bsc_rpc_url="${2:-}"; shift 2 ;;
    --no-restart)
      restart_services="false"; shift ;;
    -h|--help)
      usage; exit 0 ;;
    *)
      echo "Unknown argument: $1" >&2
      usage
      exit 1 ;;
  esac
done

if [[ ! -f "$ENV_FILE" ]]; then
  echo "Missing env file: $ENV_FILE" >&2
  exit 1
fi

if [[ ! -f "$WEB_CONFIG" ]]; then
  echo "Missing Web.config: $WEB_CONFIG" >&2
  exit 1
fi

if [[ -z "$deposit_address" ]]; then
  read -r -p "Nhap BSC deposit address (0x...): " deposit_address
fi

chain_data_provider="$(echo "$chain_data_provider" | tr '[:upper:]' '[:lower:]')"
case "$chain_data_provider" in
  rpc|rpc_balance)
    chain_data_provider="rpc"
    ;;
  rpc_logs|rpclogs|rpc-log|rpc_log)
    chain_data_provider="rpc_logs"
    ;;
  etherscan)
    ;;
  *)
    echo "Invalid --provider. Use rpc, rpc_balance, rpc_logs or etherscan." >&2
    exit 1
    ;;
esac

if [[ "$chain_data_provider" == "etherscan" && -z "$etherscan_api_key" ]]; then
  read -r -s -p "Nhap Etherscan API key (BSC chainid=56): " etherscan_api_key
  echo
fi

if [[ -z "$deposit_address" ]]; then
  echo "Missing required value: deposit address." >&2
  exit 1
fi

if [[ "$chain_data_provider" == "etherscan" && -z "$etherscan_api_key" ]]; then
  echo "Missing required value: etherscan api key for etherscan provider." >&2
  exit 1
fi

if [[ -z "$treasury_account" ]]; then
  treasury_account="$(get_web_config_value "USDTBridge.TreasuryAccount")"
  if [[ -z "$treasury_account" ]]; then
    treasury_account="$DEFAULT_TREASURY"
  fi
fi

if [[ -z "$bridge_api_key" ]]; then
  bridge_api_key="$(openssl rand -hex 24)"
fi

if [[ -z "$bridge_signing_secret" ]]; then
  bridge_signing_secret="$(openssl rand -hex 32)"
fi

if [[ ! "$deposit_address" =~ ^0x[0-9a-fA-F]{40}$ ]]; then
  echo "Invalid --deposit-address format. Expect EVM address 0x + 40 hex chars." >&2
  exit 1
fi

if [[ ! "$token_contract" =~ ^0x[0-9a-fA-F]{40}$ ]]; then
  echo "Invalid --token-contract format. Expect EVM address 0x + 40 hex chars." >&2
  exit 1
fi

if [[ ! "$min_confirmations" =~ ^[0-9]+$ ]]; then
  echo "Invalid --min-confirmations. Expect positive integer." >&2
  exit 1
fi

if ! [[ "$point_rate" =~ ^[0-9]+([.][0-9]+)?$ ]]; then
  echo "Invalid --point-rate. Expect number, e.g. 1000 or 1000.5." >&2
  exit 1
fi

# Update watcher env
set_env_value "USDT_BRIDGE_API_URL" "http://localhost:8081/admin/api/usdt-bridge-credit.aspx"
set_env_value "USDT_BRIDGE_API_KEY" "$bridge_api_key"
set_env_value "USDT_BRIDGE_SIGNING_SECRET" "$bridge_signing_secret"
set_env_value "BSC_DEPOSIT_ADDRESS" "$deposit_address"
set_env_value "BSC_TOKEN_CONTRACT" "$token_contract"
set_env_value "BSC_USDT_CONTRACT" "$token_contract"
set_env_value "CHAIN_DATA_PROVIDER" "$chain_data_provider"
set_env_value "BSC_RPC_URL" "$bsc_rpc_url"
set_env_value "RPC_BLOCK_STEP" "200"
set_env_value "RPC_MIN_BLOCK_STEP" "1"
set_env_value "RPC_REQUEST_DELAY_MS" "120"
set_env_value "INITIAL_LOOKBACK_BLOCKS" "5000"
set_env_value "TOKEN_DECIMALS" "-1"
set_env_value "BSCSCAN_API_URL" "$DEFAULT_API_URL"
if [[ -n "$etherscan_api_key" ]]; then
  set_env_value "BSCSCAN_API_KEY" "$etherscan_api_key"
fi
set_env_value "BSC_CHAIN_ID" "$DEFAULT_CHAIN_ID"
set_env_value "MIN_CONFIRMATIONS" "$min_confirmations"

# Update web bridge config
set_web_config_value "USDTBridge.Enabled" "true"
set_web_config_value "USDTBridge.ApiKey" "$bridge_api_key"
set_web_config_value "USDTBridge.ApiKeys" "$bridge_api_key"
set_web_config_value "USDTBridge.RequireHmacSignature" "true"
set_web_config_value "USDTBridge.RequireJsonContentType" "false"
set_web_config_value "USDTBridge.SigningSecret" "$bridge_signing_secret"
set_web_config_value "USDTBridge.AllowedChain" "BSC"
set_web_config_value "USDTBridge.DepositAddress" "$deposit_address"
set_web_config_value "USDTBridge.TokenContract" "$token_contract"
set_web_config_value "USDTBridge.MinConfirmations" "$min_confirmations"
set_web_config_value "USDTBridge.PointRatePerUSDT" "$point_rate"
set_web_config_value "USDTBridge.PointRatePerToken" "$point_rate"
set_web_config_value "USDTBridge.TreasuryAccount" "$treasury_account"
set_web_config_value "USDTBridge.StrictWalletOnlyMinting" "true"

if [[ "$restart_services" == "true" ]]; then
  (
    cd "$ROOT_DIR"
    docker compose restart web admin >/dev/null
  )
fi

mask() {
  local v="$1"
  local n=${#v}
  if (( n <= 8 )); then
    printf '********'
    return
  fi
  printf '%s****%s' "${v:0:4}" "${v:n-4:4}"
}

echo "Token bridge configured successfully."
echo "Updated:"
echo " - $ENV_FILE"
echo " - $WEB_CONFIG"
echo "Bridge API key: $(mask "$bridge_api_key")"
echo "Bridge signing secret: $(mask "$bridge_signing_secret")"
echo "Deposit address: $deposit_address"
echo "Data provider: $chain_data_provider"
if [[ "$chain_data_provider" == "rpc" || "$chain_data_provider" == "rpc_logs" ]]; then
  echo "BSC RPC URL: $bsc_rpc_url"
else
  echo "Etherscan/BSC key: $(mask "$etherscan_api_key")"
fi
echo "Min confirmations: $min_confirmations"
echo "Point rate per token: $point_rate"
if [[ "$restart_services" == "true" ]]; then
  echo "Web services restarted: web, admin"
fi

echo
echo "Next:"
echo "  python3 scripts/usdt_bridge_watcher.py --env-file scripts/usdt_bridge.env --once"
echo "  python3 scripts/usdt_bridge_watcher.py --env-file scripts/usdt_bridge.env"
