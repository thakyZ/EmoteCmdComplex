using System;
using System.Linq;

using Dalamud.Game.Command;
using Dalamud.Plugin;

using FFXIVClientStructs.FFXIV.Client.Game.Control;

using NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.ActionExecutor.Strategies;
using NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.Game;

// cSpell:ignoreRegExp /(?<=\/)xlem/

/// <summary>
/// Main plugin implementation.
/// </summary>
namespace NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex {
  /// <summary>
  /// Main plugin Class.
  /// </summary>
  public partial class Plugin : IDalamudPlugin {
    /// <summary>
    /// The static instance of the name of the plugin.
    /// </summary>
    public static string Name => "EmoteCmdComplex";
    /// <summary>
    /// The command for the plugin.
    /// </summary>
    private const string CommandName = "/xlem";

    /// <summary>
    /// The constructor for the plugin.
    /// </summary>
    /// <param name="pluginInterface">Dalamud Plugin Interface.</param>
    public Plugin(IDalamudPluginInterface pluginInterface) {
      pluginInterface.Create<Services>();
      Services.Init(this);
      Services.Commands.AddHandler(CommandName, new CommandInfo(OnCommand) {
        HelpMessage = "Custom emote messages while using emote.\n/xlem (<t>) \"(Text for non-target mode)\" \"(Text for targeted mode)\""
      });
      unsafe {
        // Passes to _targetSystem in another partial class that needs to be unsafe: EmoteCmdComplex.EmoteHandler.cs
        _targetSystem = TargetSystem.Instance();
      }

      // Add the Plugin interface when built on debug system.
#if DEBUG
      Services.PluginInterface!.UiBuilder.OpenConfigUi += DrawConfigUI;
      DrawConfigUI();
#endif
    }

    /// <summary>
    /// The dispose function.
    /// VS2022 and Sonar Lint don't like the way it's written, so just ignore the warnings.
    /// </summary>
    public void Dispose() {
      Services.Configuration.Save();
#if DEBUG
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
                ? element.Split(' ', StringSplitOptions.RemoveEmptyEntries) // Split the item
                : [ element ]) // Keep the entire item
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
      Services.Log.Info($"[INF] {message}");
    }
    /// <summary>
    /// Log via Dalamud log and chat.
    /// </summary>
    /// <param name="message">The message to log.</param>
    private static void Log(string message) {
      if (!Services.Configuration.Debug) {
        LogStatic(message);
        return;
      }
      Services.Log.Info($"[DBG] {message}");
      Services.Chat.Print($"{message}");
    }
    /// <summary>
    /// Log an error via Dalamud log and chat.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void LogError(string message) {
      Services.Log.Error($"[ERR] {message}");
      Services.Chat.PrintError($"{message}");
    }

    /// <summary>
    /// Draw the configuration UI on demand.
    /// </summary>
    private void DrawConfigUI() {
      Services.PluginUI.Draw();
      Services.PluginUI.SettingsVisible = true;
    }
  }
}
