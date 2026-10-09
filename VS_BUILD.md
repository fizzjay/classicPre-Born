# Build in Visual Studio (easiest)

1. Make sure these 3 DLL files are in this folder (same place as .cs file):
   - MelonLoader.dll
   - Assembly-CSharp.dll
   - UnityEngine.dll (copy from your game folder, usually inside `Managed/` or `MelonLoader/`)

2. Open `ClassicPreBornMenu.csproj` in Visual Studio 2022 / 2019.

3. If references show yellow warning:
   - Solution Explorer -> References -> Add Reference -> Browse
   - Pick MelonLoader.dll, Assembly-CSharp.dll, UnityEngine.dll

4. Build -> Build Solution (Ctrl+Shift+B)
5. Output: `bin\Release\ClassicPreBornMenu.dll`

If UnityEngine.dll isn't in this folder, find it in your game install folder and add it via Browse.
