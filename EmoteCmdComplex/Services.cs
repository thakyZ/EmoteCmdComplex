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
    private static readonly Services _instance;

    /// <summary>
    /// Plugin UI Manager
    /// </summary>
    [NotNull, AllowNull]
    public  PluginUI _PluginUI { get; set; }
    public static PluginUI PluginUI  => _instance._PluginUI;

    /// <summary>
    /// Plugin UI Manager
    /// </summary>
    [NotNull, AllowNull]
    public Plugin _Instance { get; set; }
    public static Plugin Instance  => _instance._Instance;

    /// <summary>
    /// The game's state cache.
    /// </summary>
    [NotNull, AllowNull]
    public GameStateCache _GameStateCache { get; set; }
    public static GameStateCache GameStateCache => _instance._GameStateCache;

    /// <summary>
    /// The configuration of the plugin.
    /// </summary>
    [NotNull, AllowNull]
    public Configuration _Configuration { get; set; }
    public static Configuration Configuration => _instance._Configuration;

    /// <summary>
    /// The window system.
    /// </summary>
    [NotNull, AllowNull]
    public WindowSystem _WindowSystem { get; } = new WindowSystem(Plugin.Name);
    public static WindowSystem WindowSystem => _instance._WindowSystem;

    [PluginService]
    [AllowNull, NotNull]
    public IDalamudPluginInterface _PluginInterface { get; init; }
    public static IDalamudPluginInterface PluginInterface => _instance._PluginInterface;

    [PluginService]
    [AllowNull, NotNull]
    public IChatGui _Chat { get; init; }
    public static IChatGui Chat => _instance._Chat;

    [PluginService]
    [AllowNull, NotNull]
    public ICommandManager _Commands { get; init; }
    public static ICommandManager Commands => _instance._Commands;

    [PluginService]
    [AllowNull, NotNull]
    public ITargetManager _Targets { get; init; }
    public static ITargetManager Targets => _instance._Targets;

    [PluginService]
    [AllowNull, NotNull]
    public IClientState _ClientState { get; init; }
    public static IClientState ClientState => _instance._ClientState;

    [PluginService]
    [AllowNull, NotNull]
    public ISigScanner _SigScanner { get; init; }
    public static ISigScanner SigScanner => _instance._SigScanner;

    [PluginService]
    [AllowNull, NotNull]
    public IDataManager _DataManager { get; init; }
    public static IDataManager DataManager => _instance._DataManager;

    [PluginService]
    [AllowNull, NotNull]
    public IFramework _Framework { get; init; }
    public static IFramework Framework => _instance._Framework;

    [PluginService]
    [AllowNull, NotNull]
    public IPluginLog _Log { get; init; }
    public static IPluginLog Log => _instance._Log;

    [PluginService]
    [AllowNull, NotNull]
    public IGameInteropProvider _InteropProvider { get; init; }
    public static IGameInteropProvider InteropProvider => _instance._InteropProvider;

    static Services() {
      _instance = new();
    }

    public static void Init(Plugin instance) {
      _instance._Instance = instance;
      _instance._Configuration = Configuration.Load();
      _instance._GameStateCache = new GameStateCache();
#if DEBUG
      _instance._PluginUI = new PluginUI();
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
