# FolderToPDF

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0_Windows-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![WPF-UI](https://img.shields.io/badge/UI-WPF--UI_Fluent-0078D4?style=flat&logo=windows&logoColor=white)](https://github.com/lepoco/wpfui)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=flat)](LICENSE.txt)

FolderToPDF is a desktop utility for Windows designed to scan, sanitize, calculate AI token counts, and package software repositories into token-optimized PDF, Markdown, and TXT archives. It is tailored for large-language-model prompt engineering, code reviews, and architectural documentation.

---

## Preview

![FolderToPDF Interface](assets/app-preview.png)

---

## Features

### Modern Tokenization & Telemetry
- Exact byte-pair encoding (BPE) powered by SharpToken (`o200k_base` and `cl100k_base`).
- Calibrated tokenizer algorithms:
  - OpenAI GPT-4o, o1, o3 (`o200k_base`)
  - Anthropic Claude 3.5 & 3.7 Sonnet
  - Google Gemini 2.0 & 1.5 Pro / Flash
  - Meta Llama 3, 3.1, 3.3
  - DeepSeek-V3 & DeepSeek-R1
- Real-time context load indicators for 128k (GPT-4o, Llama) and 200k (Claude, o1) windows.

### Compact Single-Screen Layout
- Designed to fit standard displays (1080p, 1440p, 4K) without vertical scrolling.
- Fluent design system with native dark/light theme support and drag-and-drop folder ingestion.

### Integrated Document & PDF Viewer
- Built-in Microsoft Edge WebView2 rendering engine for generated PDF files with zoom, search, thumbnail navigation, and print support.
- Monospace reader for Markdown and plain-text archives.
- Direct in-app preview from the main scan view and export history table.

### Export History & Project Management
- Automatic indexing of exported documents with search and filtering by project name, format, or directory.
- Shortcuts to open files in external default viewers, reveal in Windows Explorer, or copy file paths.
- Aggregate metrics tracking total processed tokens, file counts, and storage footprint.

### Secret Scrubbing & Comment Cleaning
- Pattern-based automated scrubbing for sensitive credentials (API keys, RSA/SSH private keys, JWT tokens, database passwords, emails) before export.
- Non-destructive comment stripper supporting C#, TypeScript, JavaScript, Python, Go, Rust, Java, C++, SQL, HTML, and YAML while preserving string literals.

---

## Output Formats

| Format | Engine | Use Case |
|---|---|---|
| **PDF Document** | QuestPDF Fluent API | Formatted documentation with directory tree map, syntax-styled blocks, and page numbers. |
| **Markdown Archive** | CommonMark / GFM | GitHub-flavored Markdown with language-tagged fenced code blocks for LLM contexts. |
| **Plain Text (TXT)** | Stream-based | Compact ASCII-delimited plain-text archive for CLI tools and offline review. |
| **Copy Prompt** | Native Clipboard | Copies formatted Markdown prompt directly to the clipboard for instant pasting. |

---

## Architecture

FolderToPDF follows Clean Architecture and MVVM patterns with strict layer decoupling:

```
FolderToPDF/
├── Core/                              # Domain models, business logic, interfaces, services
│   ├── Enums/                         # TokenizerModel, OutputFormat, AppThemeMode
│   ├── Models/                        # Domain entities and configuration models
│   ├── Interfaces/                    # Service contracts (ITokenCounterService, etc.)
│   └── Services/                      # Pure domain service implementations
├── UI/                                # Presentation layer
│   ├── Converters/                    # XAML value converters
│   ├── ViewModels/                    # CommunityToolkit.Mvvm ViewModels
│   └── Views/                         # Windows 11 Fluent XAML views
└── tests/FolderToPDF.Tests/           # xUnit automated test suite (57 tests)
```

- Each file adheres to a strict size budget under 400 lines of code.
- Zero compiler warnings with XML documentation generation enabled.

---

## Installation & Builds

### Prerequisites
- Windows 10 (version 1809+) or Windows 11 (x64)
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (only required if building from source)

### Run from Source
```powershell
git clone https://github.com/vtstv/FolderToPDF.git
cd FolderToPDF
dotnet run
```

### Run Tests
```powershell
dotnet test
```

### Build Executable

#### Self-Contained Single-File (No .NET installation required)
Bundles the .NET runtime and native libraries into a standalone executable:
```powershell
dotnet publish FolderToPDF.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish\standalone
```

#### Framework-Dependent Single-File (Requires .NET 8 Desktop Runtime)
Creates a lightweight executable that utilizes the system's installed .NET 8 runtime:
```powershell
dotnet publish FolderToPDF.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish\framework-dependent
```

---

## License

- **Author**: Murr ([github.com/vtstv](https://github.com/vtstv))
- **License**: [MIT License](LICENSE.txt)
