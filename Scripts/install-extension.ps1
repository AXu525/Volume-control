# Edge 浏览器扩展安装脚本
# 使用方法：右键点击 -> 使用 PowerShell 运行

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  音量控制 - Edge 浏览器扩展安装脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查 Edge 浏览器是否安装
$edgePath = Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\msedge.exe" -ErrorAction SilentlyContinue

if (-not $edgePath) {
    Write-Host "错误：未找到 Edge 浏览器" -ForegroundColor Red
    Write-Host "请先安装 Microsoft Edge 浏览器" -ForegroundColor Yellow
    Read-Host "按 Enter 键退出"
    exit 1
}

Write-Host "✓ 找到 Edge 浏览器" -ForegroundColor Green

# 获取扩展目录路径
$extensionPath = Join-Path (Split-Path $PSScriptRoot -Parent) "VolumeControl"

if (-not (Test-Path $extensionPath)) {
    Write-Host "错误：未找到扩展目录" -ForegroundColor Red
    Write-Host "期望路径：$extensionPath" -ForegroundColor Yellow
    Read-Host "按 Enter 键退出"
    exit 1
}

Write-Host "✓ 找到扩展目录：$extensionPath" -ForegroundColor Green

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  安装步骤" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "请按照以下步骤手动安装扩展：" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. 打开 Edge 浏览器" -ForegroundColor White
Write-Host "2. 在地址栏输入：edge://extensions/" -ForegroundColor White
Write-Host "3. 打开右上角的"开发人员模式"开关" -ForegroundColor White
Write-Host "4. 点击"加载解压缩的扩展"按钮" -ForegroundColor White
Write-Host "5. 选择以下目录：" -ForegroundColor White
Write-Host "   $extensionPath" -ForegroundColor Green
Write-Host "6. 确认安装" -ForegroundColor White
Write-Host ""

# 尝试打开 Edge 扩展页面
$openPage = Read-Host "是否自动打开 Edge 扩展页面？(Y/N)"
if ($openPage -eq "Y" -or $openPage -eq "y") {
    Start-Process "edge://extensions/"
    Write-Host "已打开 Edge 扩展页面" -ForegroundColor Green
}

Write-Host ""
Write-Host "安装完成后，您可以在 Edge 工具栏上看到"音量控制"图标" -ForegroundColor Cyan
Write-Host ""
Read-Host "按 Enter 键退出"
