# MiscToolsForMD

## What is This

A Muse Dash Mod which contains many features.

## What feature does it have

1. Realtime accuracy calculator (a little buggy but I can't fix it now, maybe you can help me.)

2. Key indicator (which key do you press and how many times do you press that key)

3. Realtime lyric display (under developing)

## How to use it

Install mod loader from [here](https://github.com/LavaGang/MelonLoader), install binary file as a mod, start game and you can see changes.

## How to get binary

You can clone this repo, copy `Directory.Build.props.example` to `Directory.Build.props` and edit it, set game folder(for example, `C:\Program Files\Steam\steamapps\common\Muse Dash`), open in Visual Studio and recover the project. All libraries are in `%Game Folder%\MelonLoader\Managed` after you started game with MelonLoader at least once but it should be detected automatically if you edit `Directory.Build.props` correctly.  
After recovering project successfully, you can build it and get `MiscToolsForMD.dll` and `MiscToolsForMD.SDK.dll` at `%Repo Folder%\bin\%Build Type%\`. Both files are needed, put `MiscToolsForMD.dll` at `Mods` folder and put `MiscToolsForMD.SDK.dll` at `UserLibs` folder.

## Why create MiscToolsForMD.SDK and how to use it

Code inside it contains a data provider which allows you get realtime play info. I think this may be useful for someone. `MiscToolsForMD` mod now also depends on this.  
Just add it to your project as a reference, all APIs are under namespace `MiscToolsForMD.SDK`.  
**[WARN]** The SDK is under heavy development and may not work as expected. Also it needs a qualified document, but I am working on fixing accuracy calculating now so you may have to explore it yourself.  

## Contributing

Please creating feature at branch dev, we use branch main as stable version.
