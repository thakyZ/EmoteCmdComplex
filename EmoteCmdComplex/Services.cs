using System.Diagnostics.CodeAnalysis;

using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex {
  public class Services {
    [PluginService]
    [RequiredVersion("1.0")]
    [AllowNull, NotNull]
    public static DalamudPluginInterface PluginInterface { get; private set; }
    [PluginService]
    [RequiredVersion("1.0")]
    [AllowNull, NotNull]
    public static IChatGui Chat { get; private set; } = null!;
    [PluginService]
    [RequiredVersion("1.0")]
    [AllowNull, NotNull]
    public static ICommandManager Commands { get; private set; }
    [PluginService]
    [RequiredVersion("1.0")]
    [AllowNull, NotNull]
    public static ITargetManager Targets { get; private set; }
    [PluginService]
    [RequiredVersion("1.0")]
    [AllowNull, NotNull]
    public static IClientState ClientState { get; private set; }
    [PluginService]
    [RequiredVersion("1.0")]
    [AllowNull, NotNull]
    public static ISigScanner SigScanner { get; private set; }
    [PluginService]
    [RequiredVersion("1.0")]
    [AllowNull, NotNull]
    public static IDataManager DataManager { get; private set; }
    [PluginService]
    [RequiredVersion("1.0")]
    [AllowNull, NotNull]
    public static IFramework Framework { get; private set; }
  }
}
