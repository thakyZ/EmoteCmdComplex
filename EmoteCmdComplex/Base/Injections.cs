using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Gui;
using Dalamud.IoC;

// disable nullable warnings as all of these are injected. if they're missing, we have serious issues.
#pragma warning disable CS8618
namespace EmoteCmdComplex.Base {
  /// <summary>
  /// The injection for the Plugin Services.
  /// Borrowed from: https://github.com/KazWolfe/XIVDeck/blob/main/FFXIVPlugin/Base/Injections.cs
  /// </summary>
  public class Injections {
    [PluginService] public static ChatGui Chat { get; private set; }
    [PluginService] public static TargetManager TargetManager { get; private set; }
    [PluginService] public static DataManager DataManager { get; private set; }
    [PluginService] public static Framework Framework { get; private set; }
    [PluginService] public static SigScanner SigScanner { get; private set; }
  }
}
