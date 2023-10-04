# ZoomTilt

https://user-images.githubusercontent.com/3979239/213533348-c1cd5a31-73bc-439e-8a39-7b8451590e39.mp4

Change the 3rd person camera angle character config setting depending how zoomed in you are.

Zoom out, and you will be able to see more of the world. Zoom in, the camera will be focused on your character.

## Contributions

If you can find a better way to adjust the 3rd person camera angle so that we can avoid rewriting the configuration option every frame, please let me know, or make a fork and send in a pull request, and I will gladly accept it.

I hacked this together by looking at code from [Cammy](https://github.com/UnknownX7/Cammy) and [SimpleTweaks](https://github.com/Caraxi/SimpleTweaks).

## To Use

### Installation

Add this URL to your list of Custom Plugin Repositories, and install the plugin using the plugin browser.

`https://raw.githubusercontent.com/Tenrys/ZoomTilt/master/pluginmaster.json`

You can type `/zoomtilt` in the chat to open the settings window, the plugin will work on its own.

The command `/zoomtilt toggle` allows for toggling the plugin on and off, for use with QoLBar and whatnot.

## Development

### Building

1. Open up `ZoomTilt.sln` in your C# editor of choice (likely [Visual Studio 2022](https://visualstudio.microsoft.com) or [JetBrains Rider](https://www.jetbrains.com/rider/)).
  - [Visual Studio Code](code.visualstudio.com/) works with the C# extension.
2. Build the solution. By default, this will build a `Debug` build, but you can switch to `Release` in your IDE.
  - Use [Task Explorer](https://marketplace.visualstudio.com/items?itemName=spmeesseman.vscode-taskexplorer) for Visual Studio Code.
3. The resulting plugin can be found at `ZoomTilt/bin/x64/Debug/ZoomTilt.dll` (or `Release` if appropriate.)

### Activating in-game

1. Launch the game and use `/xlsettings` in chat or `xlsettings` in the Dalamud Console to open up the Dalamud settings.
    * In here, go to `Experimental`, and add the full path to the `ZoomTilt.dll` to the list of Dev Plugin Locations.
2. Next, use `/xlplugins` (chat) or `xlplugins` (console) to open up the Plugin Installer.
    * In here, go to `Dev Tools > Installed Dev Plugins`, and the `ZoomTilt` should be visible. Enable it.
3. You should now be able to use `/zoomtilt` (chat) or `zoomtilt` (console)!

Note that you only need to add it to the Dev Plugin Locations once (Step 1); it is preserved afterwards. You can disable, enable, or load your plugin on startup through the Plugin Installer.
