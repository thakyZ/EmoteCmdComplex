using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;

using EmoteCmdComplex.Base;
using EmoteCmdComplex.Game;

namespace EmoteCmdComplex {
  public class Service {
    public static void Initialize(DalamudPluginInterface pluginInterface) {
      _ = pluginInterface.Create<Service>();
    }

    public const string PluginName = "EmoteCmdComplex";

    [PluginService][RequiredVersion("1.0")] public static DalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService][RequiredVersion("1.0")] public static GameStateCache GameStateCache { get; private set; } = null!;
    [PluginService][RequiredVersion("1.0")] public static SigHelper SigHelper { get; private set; } = null!;
    [PluginService][RequiredVersion("1.0")] public static ChatGui Chat { get; private set; } = null!;
    [PluginService][RequiredVersion("1.0")] public static CommandManager Commands { get; private set; } = null!;
    [PluginService][RequiredVersion("1.0")] public static TargetManager Targets { get; private set; } = null!;
    public static Configuration Configuration { get; set; } = null!;
    public static WindowSystem WindowSystem { get; } = new WindowSystem(PluginName);
  } 
}
