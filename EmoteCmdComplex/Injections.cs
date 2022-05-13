using System.Diagnostics.CodeAnalysis;

using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Libc;
using Dalamud.IoC;
using Dalamud.Plugin;

// disable nullable warnings as all of these are injected. if they're missing, we have serious issues.
#pragma warning disable CS8618

namespace EmoteCmdComplex {
  public class Injections {
    [PluginService] public static ChatGui Chat { get; private set; }
    [PluginService] public static DataManager DataManager { get; private set; }
    [PluginService] public static Framework Framework { get; private set; }
    [PluginService] public static SigScanner SigScanner { get; private set; }
  }
}
