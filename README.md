# SpriteSheetCreator

A powerful .NET WPF application for creating sprite sheets from sequences of images. Built with .NET 8 and CommunityToolkit.Mvvm.

## Features

- **Batch Processing**: Load multiple image files from a folder.
- **Customizable Layout**: Configure columns, rows, and start frame index.
- **Cropping**: Advanced crop settings to trim whitespace or focus on specific areas.
- **Preview**: Real-time preview of the generated frames.
- **Export Options**: 
  - Adjustable padding and spacing.
  - Support for different output formats (PNG).
  - Configurable frame rate for preview.

## Requirements

- Windows OS
- .NET 8.0 Runtime

## Getting Started

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/979569650/SpriteSheetCreator.git
   ```
2. Open the solution `SpriteSheetCreator.sln` in Visual Studio 2022 or your preferred .NET IDE.

### Building

You can build the project using the .NET CLI:

```bash
dotnet build
```

### Running

Run the application from the `src/SpriteSheetCreator.App` directory:

```bash
dotnet run --project src/SpriteSheetCreator.App/SpriteSheetCreator.App.csproj
```

## Project Structure

- **src/SpriteSheetCreator.App**: The main WPF application containing Views and ViewModels.
- **src/SpriteSheetCreator.Core**: Core logic library containing models and image processing services.

## Technologies Used

- **.NET 8**: The latest LTS version of the .NET platform.
- **WPF (Windows Presentation Foundation)**: For the desktop user interface.
- **CommunityToolkit.Mvvm**: For implementing the Model-View-ViewModel pattern efficiently.

## License

This project is licensed under the MIT License.
