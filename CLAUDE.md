# 音量精确控制工具 - 项目规范

## 项目概述
一个可以精确输入数值调节音量的工具，解决用户在调节网页或音乐软件音量时无法精确控制的问题。

## 技术栈
- C# WPF（.NET 6.0）
- Windows Core Audio API
- Edge 浏览器扩展（Manifest V3）

## 项目结构
```
D:\Projects\音量调节\
├── CLAUDE.md                          # 本文件
├── README.md                          # 项目说明
├── 卸载说明.txt                       # 卸载步骤文档
├── AudioVolumeControl.sln             # Visual Studio 解决方案
├── AudioVolumeControl/                # 核心音频控制库
│   ├── AudioVolumeControl.csproj
│   ├── SystemVolumeController.cs      # 系统主音量控制
│   ├── AudioSessionManager.cs         # 音频会话枚举和控制
│   └── VolumeCalculator.cs            # 音量值转换计算
├── App/                               # WPF 托盘程序
│   ├── App.csproj
│   ├── App.xaml / App.xaml.cs
│   ├── MainWindow.xaml / .cs
│   ├── TrayIcon.cs
│   ├── SettingsManager.cs
│   └── Resources/
├── VolumeControl/                     # Edge 浏览器扩展
│   ├── manifest.json
│   ├── popup.html
│   ├── popup.js
│   ├── content.js
│   └── icons/
└── Scripts/                           # 脚本
    └── install-extension.ps1
```

## 功能清单
1. 系统主音量控制（0-100，精确到整数）
2. 单个应用音量控制（音乐软件等）
3. Edge 浏览器标签页音量控制
4. 系统托盘常驻
5. 绿色免安装（删除文件夹即卸载）

## 不包含的功能
- 开机自启动
- 全局快捷键（可选，后续版本考虑）

## 代码规范
- 使用中文注释
- 变量名使用英文
- 每个类和方法都要有 XML 文档注释
- 错误处理要完善，不能崩溃

## 卸载规范
- 卸载时必须清理所有注册表项
- 卸载时必须注销所有热键
- 浏览器扩展需要用户手动删除
- 提供详细的卸载说明文档

## 安全规范
- 浏览器扩展不使用 tabCapture
- 不修改网页内容
- 只操作音量属性
