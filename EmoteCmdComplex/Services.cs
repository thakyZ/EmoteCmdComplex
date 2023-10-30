using System;
using System.Diagnostics.CodeAnalysis;

using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

using NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.Base;
using NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.Game;
using NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.UI;

namespace NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex {
  public class Services : IDisposable {
    [NotNull, AllowNull]
    /// <summary>
    /// Plugin UI Manager
    /// </summary>
    internal static PluginUI PluginUI { get; private set; }

    /// <summary>
    /// Plugin UI Manager
    /// </summary>
    [NotNull, AllowNull]
    internal static Plugin Instance { get; private set; }

    /// <summary>
    /// The game's state cache.
    /// </summary>
    [NotNull, AllowNull]
    internal static GameStateCache GameStateCache { get; private set; }

    /// <summary>
    /// The configuration of the plugin.
    /// </summary>
    [NotNull, AllowNull]
    internal static Configuration Configuration { get; private set; }

    /// <summary>
    /// The window system.
    /// </summary>
    [NotNull, AllowNull]
    internal static WindowSystem WindowSystem { get; } = new WindowSystem(Plugin.Name);

    [PluginService]
    [RequiredVersion("1.0")]
    [AllowNull, NotNull]
    public static DalamudPluginInterface PluginInterface { get; private set; }

    [PluginService]
    [RequiredVersion("1.0")]
    [AllowNull, NotNull]
    public static IChatGui Chat { get; private set; }

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

    [PluginService]
    [RequiredVersion("1.0")]
    [AllowNull, NotNull]
    public static IPluginLog Log { get; private set; }

    [PluginService]
    [RequiredVersion("1.0")]
    [AllowNull, NotNull]
    public static IGameInteropProvider GameInteropProvider { get; private set; }

    public static void Init(Plugin instance) {
      Instance = instance;
      Configuration = Configuration.Load();
      GameStateCache = new GameStateCache();
#if DEBUG
      PluginUI = new PluginUI();
      PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
#endif
    }

    private bool _isDisposed;

    protected virtual void Dispose(bool disposing) {
      if (!_isDisposed && disposing) {
#if DEBUG
        PluginUI.Dispose();
        PluginInterface!.UiBuilder.Draw -= WindowSystem.Draw;
#endif
        _isDisposed = true;
      }
    }

    public void Dispose() {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }
  }
}
