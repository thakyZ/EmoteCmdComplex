using System;
using System.IO;
using System.Linq;

using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;

using EmoteCmdComplex.ActionExecutor.Strategies;
using EmoteCmdComplex.Base;
using EmoteCmdComplex.Game;
using EmoteCmdComplex.UI;

using FFXIVClientStructs;
using FFXIVClientStructs.FFXIV.Client.Game.Control;

/// <summary>
/// Main plugin implementation.
/// </summary>
namespace EmoteCmdComplex {
  /// <summary>
  /// Main plugin Class.
  /// </summary>
  public partial class EmoteCmdComplexPlugin : IDalamudPlugin {
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
    /// The constructor for the plugin.
    /// </summary>
    /// <param name="pluginInterface">Dalamud Plugin Interface.</param>
    /// <param name="commandManager">Dalamud Command Manager.</param>
    /// <param name="chat">Dalamud Chat Manager.</param>
    /// <param name="targetManager">Dalamud Target Manager.</param>
    public EmoteCmdComplexPlugin(DalamudPluginInterface pluginInterface) {
      Service.Initialize(pluginInterface);
      Service.PluginInterface!.UiBuilder.Draw += Service.WindowSystem.Draw;
      Service.PluginInterface!.UiBuilder.OpenConfigUi += DrawConfigUI;
      Service.Configuration = Configuration.Load();
      PluginUI = new PluginUI();
      Service.Commands.AddHandler(CommandName, new CommandInfo(OnCommand) {
        HelpMessage = "Custom emote messages while using emote.\n/xlem (<t>) \"(Text for non-target mode)\" \"(Text for targeted mode)\""
      });
      unsafe {
        // Passes to _targetSystem in another partial class that needs to be unsafe: EmoteCmdComples.EmoteHandler.cs
        _targetSystem = (TargetSystem*)Service.Targets.Address;
      }

      // Add the Plugin interface when built on debug system.
#if (DEBUG)
      DrawConfigUI();
#endif
    }

    /// <summary>
    /// The dispose function.
    /// VS2022 and Sonar Lint don't like the way it's written, so just ignore the warnings.
    /// </summary>
    public void Dispose() {
      PluginUI.Dispose();
      Service.Configuration.Save();
      Service.PluginInterface!.UiBuilder.Draw -= Service.WindowSystem.Draw;
      Service.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
      _ = Service.Commands.RemoveHandler(CommandName);
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
        LogError($"Please surround the two messages in double quotes.\nExample: /xlem \"soup can sam\" \"soup can sam toward\" tea");
        return;
      }
      // If the arguments are exactly three
      if (resultArgs.Count == 3) {
        // If the last command contains a space in it, then return error.
        if (resultArgs[resultArgs.Count - 1].Contains(' ')) {
          LogError($"Please do not space out the emote command name.");
          return;
        }
        // Check if the player has the emote unlocked and if not return error.
        if (Service.GameStateCache.IsEmoteUnlocked(EmoteStrategy.GetEmoteByName(resultArgs[resultArgs.Count - 1]))) {
          RunCustomEmote(resultArgs[0], resultArgs[1], EmoteStrategy.GetEmoteByName(resultArgs[resultArgs.Count - 1]));
        } else {
          LogError($"The emote, {resultArgs[resultArgs.Count - 1]} isn't obtained by your account.");
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
      if (!Service.Configuration.Debug) {
        LogStatic(message);
        return;
      }
      PluginLog.Log($"[DBG] {message}");
      Service.Chat.Print($"{message}");
    }
    /// <summary>
    /// Log an error via Dalamud log and chat.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void LogError(string message) {
      PluginLog.LogError($"[ERR] {message}");
      Service.Chat.PrintError($"{message}");
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
