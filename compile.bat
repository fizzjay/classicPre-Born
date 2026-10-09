@echo off
echo Compiling ClassicPreBornMenu.dll ...
if exist "C:\Program Files\dotnet\dotnet.exe" (
    "C:\Program Files\dotnet\dotnet.exe" build ClassicPreBornMenu.csproj -c Release
    echo Done (dotnet).
) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
    rem try msbuild
    echo MSBuild path found — build with: msbuild ClassicPreBornMenu.csproj /p:Configuration=Release
) else (
    echo No dotnet found. Try mono mcs:
    mcs -target:library -r:MelonLoader.dll -r:Assembly-CSharp.dll -out:ClassicPreBornMenu.dll ClassicPreBornMenu.cs
    echo Done (mcs).
)
