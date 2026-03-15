# Mullai Installation Script for Windows
# Detects Architecture, downloads the appropriate binary,
# and updates the PATH for the user.

$ErrorActionPreference = "Stop"

# Configuration
$VERSION = "v0.0.1"
$INSTALL_DIR = Join-Path $HOME ".mullai"
$BIN_DIR = Join-Path $INSTALL_DIR "bin"
$BINARY_NAME = "Mullai.exe"

# Detect OS
# This script is for Windows (PowerShell)
$OS = "win"

# Detect Architecture
$ARCH_NAME = [System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture.ToString().ToLower()
if ($ARCH_NAME -eq "arm64") {
    $ARCH = "arm64"
} elseif ($ARCH_NAME -eq "x64") {
    $ARCH = "x64"
} else {
    Write-Host "Unsupported Architecture: $ARCH_NAME" -ForegroundColor Red
    exit 1
}

# Construct Download URL
# Pattern: Mullai_v0.0.1_win_arm64
$DOWNLOAD_URL = "https://github.com/agentmatters/mullai-bot/releases/download/$VERSION/Mullai_${VERSION}_${OS}_${ARCH}"

Write-Host "Installing Mullai $VERSION for $OS-$ARCH..." -ForegroundColor Cyan

# Create directories
if (-not (Test-Path $BIN_DIR)) {
    New-Item -ItemType Directory -Path $BIN_DIR -Force | Out-Null
}

# Download binary
Write-Host "Downloading Mullai from $DOWNLOAD_URL..." -ForegroundColor Gray
try {
    Invoke-WebRequest -Uri $DOWNLOAD_URL -OutFile (Join-Path $BIN_DIR $BINARY_NAME) -UseBasicParsing
} catch {
    Write-Host "Failed to download Mullai. Please check your internet connection and the release URL." -ForegroundColor Red
    exit 1
}

Write-Host "Mullai has been installed to $(Join-Path $BIN_DIR $BINARY_NAME)" -ForegroundColor Green

# Path persistence
$CURRENT_PATH = [Environment]::GetEnvironmentVariable("Path", "User")
if ($CURRENT_PATH -notlike "*$BIN_DIR*") {
    Write-Host "Updating User PATH with Mullai directory..." -ForegroundColor Cyan
    $NEW_PATH = "$CURRENT_PATH;$BIN_DIR"
    [Environment]::SetEnvironmentVariable("Path", $NEW_PATH, "User")
    Write-Host "Success! Please restart your terminal for changes to take effect." -ForegroundColor Green
} else {
    Write-Host "Mullai directory is already in your User PATH." -ForegroundColor Gray
}

Write-Host "Installation complete! Try running 'Mullai' in a new PowerShell session." -ForegroundColor Green
