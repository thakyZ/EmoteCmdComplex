# Emote Command Complex

Complex custom emotes now with targets!

## Main Points

* Slash command to do various things.
  * Emotes with custom text
  * Emotes with custom text and targeting
  * Emotes with custom text and/or targeting at the same time!

## To Use

### Building

1. Open up `EmoteCmdComplex.sln` in your C# editor of choice (likely [Visual Studio 2022](https://visualstudio.microsoft.com) or [JetBrains Rider](https://www.jetbrains.com/rider/)).
2. Build the solution. By default, this will build a `Debug` build, but you can switch to `Release` in your IDE.
3. The resulting plugin can be found at `SamplEmoteCmdComplexePlugin/obj/x64/Debug/EmoteCmdComplex.dll` (or `Release` if appropriate.)

### Activating in-game

1. Launch the game and use `/xlsettings` in chat or `xlsettings` in the Dalamud Console to open up the Dalamud settings.
    * In here, go to `Experimental`, and add the full path to the `EmoteCmdComplex.dll` to the list of Dev Plugin Locations.
2. Next, use `/xlplugins` (chat) or `xlplugins` (console) to open up the Plugin Installer.
    * In here, go to `Dev Tools > Installed Dev Plugins`, and the `EmoteCmdComplex` should be visible. Enable it.
3. You should now be able to use `/xlem` (chat) or `xlem` (console)!

Note that you only need to add it to the Dev Plugin Locations once (Step 1); it is preserved afterwards. You can disable, enable, or load your plugin on startup through the Plugin Installer.

### Reconfiguring for your own uses

Basically, just replace all references to `EmoteCmdComplex` in all of the files and filenames with your desired name. You'll figure it out 😁

Dalamud will load the JSON file (by default, `Data/EmoteCmdComplex.json`) next to your DLL and use it for metadata, including the description for your plugin in the Plugin Installer. Make sure to update this with information relevant to _your_ plugin!
