# ClassicPre-Born Menu v1.0

Built for **MelonLoader 0.5.1** + Unity (old game with VR / Assembly-CSharp references).

## What you asked for
- [x] Tan / Navy Blue / Cream themed UI (non-AI, custom drawn with Unity IMGUI)
- [x] **World** tab — Time Scale, Gravity, Weather, Time of Day
- [x] **Player** tab — Precise Body Controls + **PANKAKE** button (Flatscreen VR Mock)
- [x] **Fun** tab — Speed, Fly, Infinite Jump, God Mode
- [x] **PANKAKE** toggle turns on a flat-screen VR mock: camera forced flat, WASD moves virtual hands
- [x] Full-body function: head / torso / arm sliders with precise values
- [x] Fully controllable with **Keyboard + Mouse** (type in fields, click buttons, use slider)
- [x] Opens with **INSERT** or **F1**

## Files
- `ClassicPreBornMenu.cs` — Main mod (658 lines, reflection-based, works with unknown game classes)
- `ClassicPreBornMenu.csproj` — Project file
- `README.md` — This file

## How to compile
You need a C# compiler that targets .NET Framework 4.7.2 (or compatible):

**With `dotnet` (Windows / Linux SDK):**
```bash
dotnet build ClassicPreBornMenu.csproj
```

**With Mono (`mcs` / `csc`):**
```bash
mcs -target:library -r:MelonLoader.dll -r:Assembly-CSharp.dll \
    -out:ClassicPreBornMenu.dll ClassicPreBornMenu.cs
```

> Note: If your game folder has separate `UnityEngine.dll` / `UnityEngine.CoreModule.dll`, add them as references (`-r:UnityEngine.dll`) in the command above.

## Install
1. Compile to `ClassicPreBornMenu.dll`
2. Copy `ClassicPreBornMenu.dll` into your game's `Mods/` folder (or `MelonLoader/Mods/` depending on setup)
3. Launch the game via MelonLoader
4. Press **INSERT** (or **F1**) to open the tan/blue menu

## PANKAKE (Flatscreen VR Mock) — How it works
When you click **PANKAKE ON** in the Player tab:
- `Camera.main.stereoEnabled` is forced to `false` (flat, non-stereo)
- WASD input moves any found `Hand` / `Controller` / `OVR*` objects
- Full body tracking remains active via sliders
- UI stays fully interactive with mouse + keyboard

## Precise Body Controls
The Player tab includes editable float fields for:
- Head X / Y / Z
- Torso Rotation
- Left / Right Arm X / Y
- Body Scale
All fields accept **direct typing** (click → type → Enter / Tab).

## Reflection / Safety
Because your repo only contains the compiled game assembly (`Assembly-CSharp.dll`) with no source, the mod uses `Reflection` to find common Unity / VR components (`CharacterController`, `Rigidbody`, `Health`, `Hand`, `OVR*`, etc.). It is designed to be safe: failures are caught silently so the menu never crashes your game.

If you want tighter hooks to **your exact class names**, send me the class names from `Assembly-CSharp.dll` (use `strings` / `ildasm` / `dnSpy`) and I can replace the reflection with direct patches.

## Theme Colors
- Background: `#D2B48C` tan / `#F5DEB3` cream
- Accents: `#0E1F2F` deep navy / `#385A91` steel blue / `#C4A96A` gold
- All text is cream (`#F0E8D0`) for readability
