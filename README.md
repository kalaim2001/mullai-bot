# Mullai — Advanced AI Assistant

Mullai is an open-source, locally-hosted AI assistant built on .NET. It integrates with multiple LLM providers and provides a powerful, extensible framework for AI-driven automation and assistance.

---

## Key Features

- Multi-Provider LLM Support: Works with Mistral, Groq, OpenAI, and more.
- Tool Integration: Execute code, run shell commands, and interact with APIs.
- Natural Language UI: Control your system using conversational language.
- Extensible Architecture: Easily add new providers, tools, and features.
- Local-First: Runs on your machine, ensuring privacy and offline capabilities.
- Cross-Platform: Built on .NET, runs on Windows, macOS, and Linux.

---

## Project Structure

```
Mullai/
├── Mullai/               # Core library and logic
├── Mullai.TUI/           # Text-based User Interface (previously Mullai.Console)
├── Mullai.Web/            # Web interface (future)
├── Mullai.Tests/         # Unit and integration tests
├── docs/                 # Documentation
├── samples/              # Example configurations and scripts
└── README.md             # This file
```

---

## Getting Started

### Prerequisites

- .NET 8.0+ SDK
- (Optional) API keys for LLM providers (see Configuration)

### Installation

1. Clone the repository:
   ```sh
   git clone https://github.com/yourusername/mullai.git
   cd mullai
   ```

2. Build the solution:
   ```sh
   dotnet build
   ```

3. Run the TUI application:
   ```sh
   dotnet run --project Mullai.TUI
   ```

---

## Configuration

Mullai uses `appsettings.json` for configuration. Here's a basic example:

```json
{
  "Mullai": {
    "DefaultProvider": "Mistral",
    "Providers": {
      "Mistral": {
        "ApiKey": "your-mistral-api-key",
        "Model": "mistral-tiny"
      },
      "Groq": {
        "ApiKey": "your-groq-api-key",
        "Model": "llama3-70b-8192"
      }
    },
    "Tools": {
      "EnableShell": true,
      "EnableCodeInterpreter": true
    }
  }
}
```

> Note: Never commit your `appsettings.json` with API keys to version control. Use `appsettings.Development.json` (added to `.gitignore`) for local development.

---

## Usage

### Running the TUI

Start the Text-Based User Interface:
```sh
dotnet run --project Mullai.TUI
```

Once running, you can:
- Chat with the AI assistant
- Execute tools (shell commands, code interpretation, etc.)
- Switch between different LLM providers
- Manage conversation history

### Basic Commands

| Command               | Description                          |
|-----------------------|--------------------------------------|
| /help                  | Show available commands              |
| /exit                  | Exit the application                 |
| /providers             | List available LLM providers         |
| /provider <name>       | Switch to a specific provider        |
| /tools                 | List available tools                 |
| /clear                 | Clear the conversation history       |

---

## Extending Mullai

### Adding a New LLM Provider

1. Implement the `IMullaiProvider` interface.
2. Register your provider in the DI container (see `Mullai.Providers`).
3. Add configuration support in `appsettings.json`.
4. Update `models.json` with your provider's models.

### Adding a New Tool

1. Implement the `ITool` interface.
2. Register your tool in the `ToolRegistry`.
3. Document the tool's usage in the README.

### Creating a Custom Agent

1. Inherit from `AgentBase`.
2. Define the agent's instructions, tools, and skills.
3. Register the agent in the `AgentFactory`.

---

## Development

### Building from Source

```sh
git clone https://github.com/yourusername/mullai.git
cd mullai
dotnet restore
dotnet build
```

### Running Tests

```sh
dotnet test
```

### Code Style

- Use 4 spaces for indentation (no tabs).
- Follow Allman brace style (braces on new lines).
- Use PascalCase for classes/methods and camelCase for variables.
- Limit lines to 120 characters.
- Document public APIs with XML comments.

---

## Contributing

We welcome contributions! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

---

## License

Mullai is licensed under the [MIT License](LICENSE).