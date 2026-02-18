# Gorilla Tag Mod Menu (BepInEx)

This repository contains source code for a Gorilla Tag BepInEx plugin with:
- VR menu toggle on right-hand A button
- In-VR pressable menu buttons (trigger)
- Noclip toggle
- Speed boost toggle
- Kick-gun toggle (master-client only)

## Files
- `GorillaTagModMenu/GorillaTagModMenu.csproj`
- `GorillaTagModMenu/Plugin.cs`

## Build to DLL (on your PC)
1. Install .NET SDK 8.0+ and BepInEx.
2. Edit `GorillaTagModMenu/GorillaTagModMenu.csproj` and add `<Reference>` paths for your local game DLLs.
3. Build:
   ```bash
   dotnet build GorillaTagModMenu/GorillaTagModMenu.csproj -c Release
   ```
4. Your output DLL will be at:
   - `GorillaTagModMenu/bin/Release/netstandard2.1/GorillaTagModMenu.dll`
5. Copy that DLL into your Gorilla Tag `BepInEx/plugins` folder.

Use mods responsibly and only where all players consent.
