#!/bin/bash

# Mullai Installation Script
# Detects OS and Architecture, downloads the appropriate binary,
# and updates the PATH for the user.

set -e

# Configuration
VERSION="v0.0.1"
INSTALL_DIR="$HOME/.mullai"
BIN_DIR="$INSTALL_DIR/bin"
BINARY_NAME="Mullai"

# Detect OS
OS_NAME=$(uname -s | tr '[:upper:]' '[:lower:]')
if [ "$OS_NAME" == "darwin" ]; then
    OS="macos"
elif [ "$OS_NAME" == "linux" ]; then
    OS="linux"
else
    echo "Unsupported OS: $OS_NAME"
    exit 1
fi

# Detect Architecture
ARCH_NAME=$(uname -m)
if [[ "$ARCH_NAME" == "arm64" || "$ARCH_NAME" == "aarch64" ]]; then
    ARCH="arm64"
elif [ "$ARCH_NAME" == "x86_64" ]; then
    ARCH="x64"
else
    echo "Unsupported Architecture: $ARCH_NAME"
    exit 1
fi

# Construct Download URL
# Pattern: Mullai_v0.0.1_linux_arm64
# Note: Releasing version specific name as per user's request: Mullai_v0.0.1_linux_arm64
DOWNLOAD_URL="https://github.com/agentmatters/mullai-bot/releases/download/${VERSION}/Mullai_${VERSION}_${OS}_${ARCH}"

echo "Installing Mullai ${VERSION} for ${OS}-${ARCH}..."

# Create directories
mkdir -p "$BIN_DIR"

# Download binary
echo "Downloading Mullai from ${DOWNLOAD_URL}..."
if curl -L --progress-bar "$DOWNLOAD_URL" -o "$BIN_DIR/$BINARY_NAME"; then
    chmod +x "$BIN_DIR/$BINARY_NAME"
else
    echo "Failed to download Mullai. Please check your internet connection and the release URL."
    exit 1
fi

echo "Mullai has been installed to $BIN_DIR/$BINARY_NAME"

# Path persistence
SHELL_RC=""
case "$SHELL" in
    */zsh)
        SHELL_RC="$HOME/.zshrc"
        ;;
    */bash)
        SHELL_RC="$HOME/.bashrc"
        ;;
    *)
        echo "Could not detect shell configuration file. Please add $BIN_DIR to your PATH manually."
        ;;
esac

if [ -n "$SHELL_RC" ]; then
    if ! grep -q "$BIN_DIR" "$SHELL_RC"; then
        echo "Updating $SHELL_RC with Mullai path..."
        echo "" >> "$SHELL_RC"
        echo "# Mullai path" >> "$SHELL_RC"
        echo "export PATH=\"\$PATH:$BIN_DIR\"" >> "$SHELL_RC"
        echo "Success! Please restart your terminal or run: source $SHELL_RC"
    else
        echo "Mullai directory is already in your PATH in $SHELL_RC"
    fi
fi

echo "Installation complete! Try running 'Mullai' in a new terminal session."
