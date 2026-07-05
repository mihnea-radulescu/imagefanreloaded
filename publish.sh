#!/bin/sh

platform_linux_arm64=linux-arm64
platform_linux_x64=linux-x64

dotnet publish ImageFanReloaded/ImageFanReloaded.csproj --output publish/$platform_linux_arm64 -p:DebugType=none -p:PublishSingleFile=true -p:PublishAot=true -p:StripSymbols=true -p:ObjCopyName=aarch64-linux-gnu-objcopy --self-contained true --configuration Release --framework net10.0 --runtime $platform_linux_arm64

dotnet publish ImageFanReloaded/ImageFanReloaded.csproj --output publish/$platform_linux_x64 -p:DebugType=none -p:PublishSingleFile=true -p:PublishAot=true -p:StripSymbols=true --self-contained true --configuration Release --framework net10.0 --runtime $platform_linux_x64
