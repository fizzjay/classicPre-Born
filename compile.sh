#!/bin/bash
# Compile ClassicPreBornMenu.dll against MelonLoader 0.5.1 and game assembly
# Requires mono-devel (mcs) or dotnet SDK

if command -v dotnet &> /dev/null; then
    echo "Using dotnet..."
    dotnet build ClassicPreBornMenu.csproj -c Release
    echo "Output: bin/Release/net472/ClassicPreBornMenu.dll"
elif command -v mcs &> /dev/null; then
    echo "Using mcs (Mono)..."
    mcs -target:library \
        -r:MelonLoader.dll \
        -r:Assembly-CSharp.dll \
        -out:ClassicPreBornMenu.dll \
        ClassicPreBornMenu.cs
    echo "Output: ClassicPreBornMenu.dll"
else
    echo "No C# compiler found. Install dotnet SDK or mono-devel."
    exit 1
fi
