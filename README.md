# MiscToolsForMD

## What is This

A Muse Dash Mod which contains many features.

## What feature does it have

~~1. Realtime accuracy calculator (a little buggy but I can't fix it now, maybe you can help me.)~~

~~2. Key indicator (which key do you press and how many times do you press that key)~~

~~3. Realtime lyric display (under developing)~~
Everything is under rewriting and porting to net6 for MelonLoader v0.6. We will try our best to keep all features working.

## How to use it

Install mod loader from [here](https://github.com/LavaGang/MelonLoader), install binary file as a mod, start game and you can see changes.

## How to get binary

You can clone this repo, copy `Directory.Build.props.example` to `Directory.Build.props` and edit it, set game folder(for example, `C:\Program Files\Steam\steamapps\common\Muse Dash`), open in Visual Studio and recover the project. All libraries are in `%Game Folder%\MelonLoader\Managed` after you started game with MelonLoader at least once but it should be detected automatically if you edit `Directory.Build.props` correctly.  
After recovering project successfully, you can build this project and get dll files. Put them in the `Mods` folder and start your game, you will find the changes.

## Known issues

- Accuracy calculation in some cases is not same with game's result, we are working on it now.

## Contributing

1. Please creating feature at branch `dev`, we use branch `main` as stable version.
2. Please use Visual Studio or other IDE supports Visual Studio's `.editorconfig`. Because we defined our code style in `.editorconfig` file, using Visual Studio can make your code like us. You can use Visual Studio Community for free. If you still want to choose other IDE which does not support the `.editorconfig` file, please keep our code style yourself:
    - Most of the names are using Pascal Style, interface must starts with letter `I` like `IMyInterface`, `MyAwesomeClass` and `MyFunction`.
    - Property's name should use Camel Style like `propertyAlpha` and `betaProperty`.
3. Please make sure most warnings in Visual Studio are solved. Solving warnings may help reducing hidden bugs.
