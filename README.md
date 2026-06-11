# 音量精确控制工具

一个可以精确输入数值调节音量的工具，解决用户在调节网页或音乐软件音量时无法精确控制的问题。

## 功能特点

- **系统音量控制**：精确控制主音量（0-100）
- **应用音量控制**：单独控制每个应用的音量（如 Spotify、网易云等）
- **浏览器标签页音量控制**：通过 Edge 扩展控制单个网页标签页的音量
- **托盘常驻**：最小化到系统托盘，一键调节
- **绿色免安装**：下载解压即用，删除文件夹即卸载

## 项目结构

```
音量调节/
├── AudioVolumeControl.sln         # Visual Studio 解决方案
├── AudioVolumeControl/            # 核心音频控制库
│   ├── SystemVolumeController.cs  # 系统主音量控制
│   ├── AudioSessionManager.cs     # 音频会话管理
│   └── VolumeCalculator.cs        # 音量值转换
├── App/                           # WPF 托盘程序
│   ├── MainWindow.xaml/.cs        # 主界面
│   ├── TrayIcon.cs                # 托盘图标
│   └── SettingsManager.cs         # 设置管理
├── VolumeControl/                 # Edge 浏览器扩展
│   ├── manifest.json
│   ├── popup.html/js
│   └── content.js
└── 卸载说明.txt                   # 卸载步骤文档
```

## 安装和使用

### 系统托盘程序

1. **编译项目**
   - 使用 Visual Studio 2022 或更高版本
   - 打开 `AudioVolumeControl.sln`
   - 选择 Release 配置
   - 生成解决方案

2. **运行程序**
   - 编译后在 `App/bin/Release/net6.0-windows/` 目录下找到 `AudioVolumeControl.exe`
   - 双击运行

3. **使用方法**
   - 程序启动后会显示主界面
   - 在下拉框中选择控制目标（系统主音量或某个应用）
   - 在输入框中输入音量值（0-100），点击"应用"或按回车
   - 也可以使用滑块调节音量
   - 点击"最小化"按钮最小化到托盘
   - 托盘图标左键点击：显示/隐藏主窗口
   - 托盘图标右键菜单：快捷操作

### Edge 浏览器扩展

1. **安装扩展**
   - 打开 Edge 浏览器
   - 地址栏输入 `edge://extensions/`
   - 打开"开发人员模式"
   - 点击"加载解压缩的扩展"
   - 选择 `VolumeControl` 文件夹

2. **使用方法**
   - 打开包含音频/视频的网页（如 B站、YouTube、音乐网站等）
   - 点击工具栏上的"音量控制"图标
   - 在弹出窗口中输入音量值（0-100）
   - 点击"应用"或按回车

## 技术栈

- **核心库**：C# + .NET 6.0 + NAudio
- **托盘程序**：WPF
- **浏览器扩展**：JavaScript + Manifest V3

## 权限说明

### 系统托盘程序
- 音频设备访问权限：用于控制系统音量
- 进程枚举权限：用于识别运行中的应用

### 浏览器扩展
- `activeTab`：访问当前标签页
- `scripting`：注入内容脚本
- `<all_urls>`：在所有网页上运行

## 安全说明

- 浏览器扩展**不使用** `tabCapture` API
- 浏览器扩展**不修改**网页内容
- 浏览器扩展**只操作**媒体元素的 `volume` 属性
- 不会收集任何用户数据

## 卸载说明

详见 [卸载说明.txt](卸载说明.txt)

## 常见问题

### Q: 程序无法启动？
A: 请确保系统已安装 .NET 6.0 运行时。可以从 https://dotnet.microsoft.com/download/dotnet/6.0 下载。

### Q: 浏览器扩展无法控制音量？
A: 请确保：
1. 网页中有 `<audio>` 或 `<video>` 元素
2. 扩展已正确安装并启用
3. 已授予扩展所需的权限

### Q: 找不到应用的音频会话？
A: 有些应用可能在播放音频后才会显示在音频会话列表中。请先播放一段音频，然后重新打开程序。

## 开发说明

### 编译要求
- Visual Studio 2022 或更高版本
- .NET 6.0 SDK
- Windows 10/11

### 依赖项
- NAudio 2.1.0（音频处理库）
- CommunityToolkit.Mvvm 8.2.0（MVVM 框架）

## 许可证

MIT License

## 作者

AXu
