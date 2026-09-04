[CmdletBinding()]
param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir
$SecretsDir = Join-Path $RootDir "secrets"

if (-not (Test-Path -Path $SecretsDir)) {
    New-Item -ItemType Directory -Path $SecretsDir -Force | Out-Null
}

$PasswordFile = Join-Path $SecretsDir "db_password.txt"

if ($Force -or (-not (Test-Path -Path $PasswordFile))) {
    $bytes = New-Object byte[] 24
    [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
    $hexPassword = [System.BitConverter]::ToString($bytes).Replace("-", "").ToLower()

    [System.IO.File]::WriteAllText($PasswordFile, $hexPassword, [System.Text.Encoding]::ASCII)
    Write-Host "✔ Successfully generated $PasswordFile" -ForegroundColor Green
} else {
    Write-Host "✔ Secret file $PasswordFile already exists (use -Force to regenerate)." -ForegroundColor Green
}

