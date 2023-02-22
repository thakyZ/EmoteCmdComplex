using System;
using System.Linq;

using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin;

using NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.ActionExecutor.Strategies;
using NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.Base;
using NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.Game;
using NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.UI;

using FFXIVClientStructs.FFXIV.Client.Game.Control;

/// <summary>
/// Main plugin implementation.
/// </summary>
namespace NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex {
  /// <summary>
  /// Main plugin Class.
  /// </summary>
  public partial class EmoteCmdComplexPlugin : IDalamudPlugin {
    internal static EmoteCmdComplexPlugin Instance { get; private set; } = null!;
    /// <summary>
    /// The name of the plugin.
    /// </summary>
    public string Name => "Emote Command Complex";
    /// <summary>
    /// The command for the plugin.
    /// </summary>
    private const string CommandName = "/xlem";
    /// <summary>
    /// Plugin UI Manager
    /// </summary>
    private static PluginUI PluginUI = null!;
    /// <summary>
    /// The game's state cache.
    /// </summary>
    internal GameStateCache GameStateCache { get; }
    /// <summary>
    /// The plugin's name.
    /// </summary>
    public static string PluginName = "EmoteCmdComplex";
    /// <summary>
    /// The configuration of the plugin.
    /// </summary>
    public Configuration Configuration { get; }
    /// <summary>
    /// The signature helper class.
    /// </summary>
    public SigHelper SigHelper { get; }
    /// <summary>
    /// The window system.
    /// </summary>
    public WindowSystem WindowSystem { get; } = new WindowSystem(PluginName);
    /// <summary>
    /// The instanced <see cref="DalamudPluginInterface"/>
    /// </summary>
    private DalamudPluginInterface PluginInterface { get; }

    /// <summary>
    /// The constructor for the plugin.
    /// </summary>
    /// <param name="pluginInterface">Dalamud Plugin Interface.</param>
    public EmoteCmdComplexPlugin(DalamudPluginInterface pluginInterface) {
      pluginInterface.Create<Services>();
      Instance = this;
      this.PluginInterface = pluginInterface;
      this.Configuration = Configuration.Load();
      Services.Commands.AddHandler(CommandName, new CommandInfo(OnCommand) {
        HelpMessage = "Custom emote messages while using emote.\n/xlem (<t>) \"(Text for non-target mode)\" \"(Text for targeted mode)\""
      });
      unsafe {
        // Passes to _targetSystem in another partial class that needs to be unsafe: EmoteCmdComplex.EmoteHandler.cs
        _targetSystem = (TargetSystem*)Services.Targets.Address;
      }
      this.SigHelper = new SigHelper();
      this.GameStateCache = new GameStateCache();

      // Add the Plugin interface when built on debug system.
#if DEBUG
      PluginUI = new PluginUI();
      Services.PluginInterface!.UiBuilder.Draw += this.WindowSystem.Draw;
      Services.PluginInterface!.UiBuilder.OpenConfigUi += DrawConfigUI;
      DrawConfigUI();
#endif
    }

    /// <summary>
    /// The dispose function.
    /// VS2022 and Sonar Lint don't like the way it's written, so just ignore the warnings.
    /// </summary>
    public void Dispose() {
      this.Configuration.Save();
#if DEBUG
      PluginUI.Dispose();
      Services.PluginInterface!.UiBuilder.Draw -= this.WindowSystem.Draw;
      Services.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
#endif
      _ = Services.Commands.RemoveHandler(CommandName);
    }

    /// <summary>
    /// A function that gets executed after the command is sent.
    /// </summary>
    /// <param name="command">The name of the command.</param>
    /// <param name="args">The args of the command in one string.</param>
    private void OnCommand(string command, string args) {
      // Split the arguments via double quotes.
      // Borrowed from https://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
      var resultArgs = args.Split('"')
        .Select((element, index) => index % 2 == 0 // If even index
                ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) // Split the item
                : new string[] { element }) // Keep the entire item
        .SelectMany(element => element).ToList();

      // If the result arguments are not surrounded in quotes or there were way too many arguments passed.
      if (resultArgs.Count > 3) {
        LogError("Please surround the two messages in double quotes.\nExample: /xlem \"soup can sam\" \"soup can sam toward\" tea");
        return;
      }
      // If the arguments are exactly three
      if (resultArgs.Count == 3) {
        // If the last command contains a space in it, then return error.
        if (resultArgs[^1].Contains(' ')) {
          LogError("Please do not space out the emote command name.");
          return;
        }
        // Check if the player has the emote unlocked and if not return error.
        if (GameStateCache.IsEmoteUnlocked(EmoteStrategy.GetEmoteByName(resultArgs[^1]))) {
          RunCustomEmote(resultArgs[0], resultArgs[1], EmoteStrategy.GetEmoteByName(resultArgs[^1]));
        } else {
          LogError($"The emote, {resultArgs[^1]} isn't obtained by your account.");
        }
      }
      // If no emote supplied then execute without emote.
      else if (resultArgs.Count == 2) {
        RunCustomEmote(resultArgs[0], resultArgs[1]);
      }
    }

    // Logging functions
    // Borrowed from https://github.com/Bluefissure/MapLinker/blob/master/MapLinker/MapLinker.cs:147
    /// <summary>
    /// Log via a static method. Does not print to chat.
    /// </summary>
    /// <param name="message">The message to log.</param>
    private static void LogStatic(string message) {
      PluginLog.Log($"[INF] {message}");
    }
    /// <summary>
    /// Log via Dalamud log and chat.
    /// </summary>
    /// <param name="message">The message to log.</param>
    private static void Log(string message) {
      if (!EmoteCmdComplexPlugin.Instance.Configuration.Debug) {
        LogStatic(message);
        return;
      }
      PluginLog.Log($"[DBG] {message}");
      Services.Chat.Print($"{message}");
    }
    /// <summary>
    /// Log an error via Dalamud log and chat.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void LogError(string message) {
      PluginLog.LogError($"[ERR] {message}");
      Services.Chat.PrintError($"{message}");
    }

    /// <summary>
    /// Draw the configuration UI on demand.
    /// </summary>
    private void DrawConfigUI() {
      PluginUI.Draw();
      PluginUI.SettingsVisible = true;
    }
  }
}
