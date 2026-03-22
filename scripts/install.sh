#!/bin/bash

# Mullai Installation Script
# Detects OS and Architecture, downloads the appropriate binary,
# and updates the PATH for the user.

set -e

# Configuration
REPO="agentmatters/mullai-bot"
VERSION=${1:-""}
VERSION_SUFFIX=""

# Fetch latest version if not provided
if [ -z "$VERSION" ]; then
    echo "Fetching latest version from GitHub..."
    # Try /latest first (stable only)
    LATEST_JSON=$(curl -s "https://api.github.com/repos/$REPO/releases/latest")
    VERSION=$(echo "$LATEST_JSON" | grep '"tag_name":' | sed -E 's/.*"([^"]+)".*/\1/')
    
    # Fallback to general releases list if /latest is not found (includes pre-releases)
    if [ -z "$VERSION" ] || [[ "$LATEST_JSON" == *"Not Found"* ]]; then
        echo "No stable release found. Checking all releases (including pre-releases)..."
        VERSION=$(curl -s "https://api.github.com/repos/$REPO/releases" | grep -m 1 '"tag_name":' | sed -E 's/.*"([^"]+)".*/\1/')
        if [ -n "$VERSION" ] && [[ ! "$VERSION" == *"-preview" ]]; then
            VERSION_SUFFIX="-preview"
        fi
    fi

    if [ -z "$VERSION" ]; then
        echo "Error: Could not fetch any version from GitHub. Please specify a version as an argument."
        exit 1
    fi
fi

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
# Pattern: Mullai_v0.0.1-preview_linux_arm64.zip
DOWNLOAD_URL="https://github.com/agentmatters/mullai-bot/releases/download/${VERSION}/Mullai_${VERSION}${VERSION_SUFFIX}_${OS}_${ARCH}.zip"

echo "Installing Mullai ${VERSION} for ${OS}-${ARCH}..."

# Create directories
mkdir -p "$BIN_DIR"
mkdir -p "$INSTALL_DIR/temp"

# Download and extract
echo "Downloading Mullai from ${DOWNLOAD_URL}..."
TEMP_ZIP="$INSTALL_DIR/temp/mullai.zip"
EXTRACT_DIR="$INSTALL_DIR/temp/extracted"

if curl -L --progress-bar "$DOWNLOAD_URL" -o "$TEMP_ZIP"; then
    echo "Extracting Mullai..."
    rm -rf "$EXTRACT_DIR"
    mkdir -p "$EXTRACT_DIR"
    unzip -q -o "$TEMP_ZIP" -d "$EXTRACT_DIR"
    
    # Install CLI
    mv "$EXTRACT_DIR/cli/Mullai" "$BIN_DIR/$BINARY_NAME"
    chmod +x "$BIN_DIR/$BINARY_NAME"
    
    # Install Web App
    echo "Installing Mullai Web App..."
    rm -rf "$INSTALL_DIR/web"
    mv "$EXTRACT_DIR/web" "$INSTALL_DIR/web"
    chmod +x "$INSTALL_DIR/web/Mullai.Web"
    
    # Cleanup temp
    rm -rf "$INSTALL_DIR/temp"
else
    echo "Failed to download Mullai. Please check your internet connection and the release URL."
    exit 1
fi

echo "Mullai CLI has been installed to $BIN_DIR/$BINARY_NAME"

# Service Installation
if [ "$OS" == "linux" ]; then
    if command -v systemctl >/dev/null 2>&1; then
        echo "Setting up Mullai Web as a systemd service..."
        SERVICE_FILE="/etc/systemd/system/mullai-web.service"
        sudo tee $SERVICE_FILE > /dev/null <<EOF
[Unit]
Description=Mullai Web Service
After=network.target

[Service]
ExecStart=$INSTALL_DIR/web/Mullai.Web
WorkingDirectory=$INSTALL_DIR/web
Restart=always
User=$USER

[Install]
WantedBy=multi-user.target
EOF
        sudo systemctl daemon-reload
        sudo systemctl enable mullai-web
        sudo systemctl restart mullai-web
        echo "Mullai Web service installed and started."
    fi
elif [ "$OS" == "macos" ]; then
    echo "Setting up Mullai Web as a launchd agent..."
    PLIST_DIR="$HOME/Library/LaunchAgents"
    PLIST_FILE="$PLIST_DIR/com.mullai.bot.web.plist"
    mkdir -p "$PLIST_DIR"
    cat > "$PLIST_FILE" <<EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>Label</key>
    <string>com.mullai.bot.web</string>
    <key>ProgramArguments</key>
    <array>
        <string>$INSTALL_DIR/web/Mullai.Web</string>
    </array>
    <key>RunAtLoad</key>
    <true/>
    <key>KeepAlive</key>
    <true/>
    <key>StandardOutPath</key>
    <string>$INSTALL_DIR/web/out.log</string>
    <key>StandardErrorPath</key>
    <string>$INSTALL_DIR/web/err.log</string>
</dict>
</plist>
EOF
    launchctl unload "$PLIST_FILE" 2>/dev/null || true
    launchctl load "$PLIST_FILE"
    echo "Mullai Web agent installed and started."
fi

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
