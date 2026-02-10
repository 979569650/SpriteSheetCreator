# SpriteSheetCreator

一个强大的 .NET WPF 应用程序，用于从图像序列创建精灵表（Sprite Sheets）。基于 .NET 8 和 CommunityToolkit.Mvvm 构建。

## 功能特性

- **批量处理**：从文件夹加载多个图像文件。
- **自定义布局**：配置列数、行数和起始帧索引。
- **裁剪功能**：高级裁剪设置，用于去除空白或聚焦特定区域。
- **实时预览**：实时预览生成的帧动画效果。
- **导出选项**：
  - 可调节的填充（Padding）和间距（Spacing）。
  - 支持多种输出格式（如 PNG）。
  - 可配置预览帧率。

## 系统要求

- Windows 操作系统
- .NET 8.0 运行时

## 快速开始

### 安装

1. 克隆仓库：
   ```bash
   git clone https://github.com/979569650/SpriteSheetCreator.git
   ```
2. 在 Visual Studio 2022 或您喜欢的 .NET IDE 中打开解决方案 `SpriteSheetCreator.sln`。

### 构建

您可以使用 .NET CLI 构建项目：

```bash
dotnet build
```

### 运行

从 `src/SpriteSheetCreator.App` 目录运行应用程序：

```bash
dotnet run --project src/SpriteSheetCreator.App/SpriteSheetCreator.App.csproj
```

## 项目结构

- **src/SpriteSheetCreator.App**：包含视图（Views）和视图模型（ViewModels）的主 WPF 应用程序。
- **src/SpriteSheetCreator.Core**：包含模型和图像处理服务的核心逻辑库。

## 技术栈

- **.NET 8**：最新的 .NET 长期支持（LTS）版本。
- **WPF (Windows Presentation Foundation)**：用于构建桌面用户界面。
- **CommunityToolkit.Mvvm**：用于高效实现 MVVM（Model-View-ViewModel）模式。

## 许可证

本项目采用 MIT 许可证。
