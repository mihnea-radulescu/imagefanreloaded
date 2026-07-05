@echo off

set platform_win_arm64=win-arm64
set platform_win_x64=win-x64

dotnet publish ImageFanReloaded/ImageFanReloaded.csproj --output publish/%platform_win_arm64% -p:DebugType=none -p:PublishSingleFile=true -p:PublishAot=true -p:StripSymbols=true --self-contained true --configuration Release --framework net10.0 --runtime %platform_win_arm64%

dotnet publish ImageFanReloaded/ImageFanReloaded.csproj --output publish/%platform_win_x64% -p:DebugType=none -p:PublishSingleFile=true -p:PublishAot=true -p:StripSymbols=true --self-contained true --configuration Release --framework net10.0 --runtime %platform_win_x64%
