# Gorilla Tag Mod Menu (BepInEx)

This repository contains source code for a Gorilla Tag BepInEx plugin project.

> Note: I can't generate a compiled `.dll` binary in this environment because the .NET SDK is not installed. The project is configured so that when you build locally, the output file name is `ReadonsMenu.dll`.

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
   - `GorillaTagModMenu/bin/Release/netstandard2.1/ReadonsMenu.dll`
5. Copy that DLL into your Gorilla Tag `BepInEx/plugins` folder.

A `.dll` is a compiled binary file, so source code from `README.md` / `.csproj` cannot literally be embedded as plain text in one file. The correct way is to keep source files in the project and compile them into `ReadonsMenu.dll`.
