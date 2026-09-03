#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
SECRETS_DIR="${ROOT_DIR}/secrets"

mkdir -p "${SECRETS_DIR}"
chmod 700 "${SECRETS_DIR}"

PASSWORD_FILE="${SECRETS_DIR}/db_password.txt"

if [ ! -f "${PASSWORD_FILE}" ]; then
  if command -v openssl >/dev/null 2>&1; then
    openssl rand -hex 24 | tr -d '\n' > "${PASSWORD_FILE}"
  else
    head -c 24 /dev/urandom | base64 | tr -dc 'a-zA-Z0-9' | head -c 24 > "${PASSWORD_FILE}"
  fi
  chmod 600 "${PASSWORD_FILE}"
  echo "✔ Successfully generated ${PASSWORD_FILE} with 0600 permissions."
else
  echo "✔ Secret file ${PASSWORD_FILE} already exists."
fi
