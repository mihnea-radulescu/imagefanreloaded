#!/bin/sh

dotnet publish ImageFanReloaded/ImageFanReloaded.csproj --output publish/linux-arm64 -p:DebugType=none -p:PublishSingleFile=true -p:PublishReadyToRun=true --self-contained true --configuration Release --framework net10.0 --runtime linux-arm64

dotnet publish ImageFanReloaded/ImageFanReloaded.csproj --output publish/linux-x64 -p:DebugType=none -p:PublishSingleFile=true -p:PublishAot=true -p:StripSymbols=true --self-contained true --configuration Release --framework net10.0 --runtime linux-x64

dotnet publish ImageFanReloaded/ImageFanReloaded.csproj --output publish/win-arm64 -p:DebugType=none -p:PublishSingleFile=true -p:PublishReadyToRun=true --self-contained true --configuration Release --framework net10.0 --runtime win-arm64

dotnet publish ImageFanReloaded/ImageFanReloaded.csproj --output publish/win-x64 -p:DebugType=none -p:PublishSingleFile=true -p:PublishReadyToRun=true --self-contained true --configuration Release --framework net10.0 --runtime win-x64
