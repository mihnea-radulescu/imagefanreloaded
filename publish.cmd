@echo off

set project_file=ImageFanReloaded/ImageFanReloaded.csproj

set platform_windows_arm64=win-arm64
set platform_windows_x64=win-x64

dotnet publish %project_file% --output publish/%platform_windows_arm64% -p:DebugType=none -p:PublishSingleFile=true -p:PublishAot=true -p:StripSymbols=true --self-contained true --configuration Release --framework net10.0 --runtime %platform_windows_arm64%

dotnet publish %project_file% --output publish/%platform_windows_x64% -p:DebugType=none -p:PublishSingleFile=true -p:PublishAot=true -p:StripSymbols=true --self-contained true --configuration Release --framework net10.0 --runtime %platform_windows_x64%
