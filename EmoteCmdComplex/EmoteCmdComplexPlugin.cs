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
    /// The instance of the plugin.
    /// </summary>
    internal static EmoteCmdComplexPlugin Instance = null!;
    /// <summary>
    /// The <see cref="GameStateCache"/> which stores the binary data in the game.
    /// </summary>
    internal GameStateCache GameStateCache { get; }
    /// <summary>
    /// The <see cref="SigHelper"/> which helps with writing/reading binary signatures.
    /// </summary>
    public SigHelper SigHelper { get; }
    /// <summary>
    /// The name of the plugin.
    /// </summary>
    public string Name => "Emote Command Complex";
    /// <summary>
    /// The command for the plugin.
    /// </summary>
    private const string CommandName = "/xlem";
    /// <summary>
    /// The plugin interface to be set via the constructor.
    /// </summary>
    private DalamudPluginInterface PluginInterface {
      get; init;
    }
    /// <summary>
    /// The command manager to make commands for the plugin onto Dalamud.
    /// </summary>
    private CommandManager CommandManager {
      get; init;
    }
    /// <summary>
    /// The instance for the configuration manager.
    /// </summary>
    private Configuration Configuration {
      get; init;
    }
    /// <summary>
    /// The instance for the chat gui.
    /// </summary>
    [PluginService]
    internal ChatGui ChatGui {
      get; private set;
    }
    /// <summary>
    /// The instance of the plugin ui manager.
    /// </summary>
    private PluginUI PluginUi {
      get; init;
    }
    /// <summary>
    /// Target manager for checking if the player is targeting someone or not.
    /// </summary>
    private readonly TargetManager _targetManager;

    /// <summary>
    /// The constructor for the plugin.
    /// </summary>
    /// <param name="pluginInterface">Dalamud Plugin Interface.</param>
    /// <param name="commandManager">Dalamud Command Manager.</param>
    /// <param name="chat">Dalamud Chat Manager.</param>
    /// <param name="targetManager">Dalamud Target Manager.</param>
    public EmoteCmdComplexPlugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] CommandManager commandManager,
        TargetManager targetManager) {
      // Initialize the Plugin and Command Interface/Manager.
      this.PluginInterface = pluginInterface;
      this.CommandManager = commandManager;
      // Create the injection instances
      // Borrowed from https://github.com/KazWolfe/XIVDeck/blob/main/FFXIVPlugin/XIVDeckPlugin.cs:37
      pluginInterface.Create<Injections>();
      Resolver.Initialize(Injections.SigScanner.SearchBase);

      // Initialize the instance
      // Ignore the warning.
      Instance = this;

      // Create the configuration instance.
      this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
      this.Configuration.Initialize(this.PluginInterface);

      // Initialize the plugin icon.
      // ---------------------------
      // You might normally want to embed resources and load them from the manifest stream
      // TODO: Heed this suggestion.
      var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "EmoteCmdComplex.png");
      var pluginIcon = this.PluginInterface.UiBuilder.LoadImage(imagePath);
      this.PluginUi = new PluginUI(this.Configuration, pluginIcon);

      // Initialize the signature helper.
      this.SigHelper = new SigHelper();

      // Initialize the Game State Cache.
      this.GameStateCache = GameStateCache.Load();

      // Initialize the Chat GUI.
      // Borrowrd from https://github.com/Bluefissure/MapLinker/blob/master/MapLinker/MapLinker.cs:51
      this.ChatGui = Injections.Chat;

      // Set the target system.
      // Borrowed from https://github.com/fitzchivalrik/compass/blob/master/Compass/Compass.cs:74
      _targetManager = Injections.TargetManager;
      unsafe {
        // Passes to _targetSystem in another partial class that needs to be unsafe: EmoteCmdComples.EmoteHandler.cs
        _targetSystem = (TargetSystem*)_targetManager.Address;
      }

      // Add the command to the Dalamud instance.
      _ = this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
        HelpMessage = "Custom emote messages while using emote.\n/xlem (<t>) \"(Text for non-target mode)\" \"(Text for targeted mode)\""
      });

      // Add the Plugin interface when built on debug system.
#if DEBUG
      this.PluginInterface.UiBuilder.Draw += DrawUI;
      this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
#endif
    }

    /// <summary>
    /// The dispose function.
    /// VS2022 and Sonar Lint don't like the way it's written, so just ignore the warnings.
    /// </summary>
    public void Dispose() {
      this.PluginUi.Dispose();
      _ = this.CommandManager.RemoveHandler(CommandName);
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
        if (GameStateCache.IsEmoteUnlocked(EmoteStrategy.GetEmoteByName(resultArgs[resultArgs.Count - 1]))) {
          RunCustomEmote(resultArgs[0], resultArgs[1], EmoteStrategy.GetEmoteByName(resultArgs[resultArgs.Count - 1]));
        } else {
          LogError($"The emote, {resultArgs[resultArgs.Count - 1]} isn't obtained by your account.");
        }
      }
      // If no emote supplied then execute without emote.
      else if (resultArgs.Count == 2) {
        RunCustomEmote(resultArgs[0], resultArgs[1]);
      }
      // If all else return error.
      else {
        return;
      }
    }

    /// <summary>
    /// Check the full string for quotes. Command arguments should be passed to this.
    /// No longer needed.
    /// </summary>
    /// <param name="arg">The command argument.</param>
    /// <returns><see cref="true"/> if the command argument is surrounded by double quotes.</returns>
    private static bool CheckFullString(string arg) {
      if (arg[0] == '"' && arg[arg.Length - 1] == '"') {
        return true;
      }
      return false;
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
    private void Log(string message) {
      if (!Configuration.Debug) {
        LogStatic(message);
        return;
      }
      PluginLog.Log($"[DBG] {message}");
      ChatGui.Print($"{message}");
    }
    /// <summary>
    /// Log an error via Dalamud log and chat.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void LogError(string message) {
      PluginLog.LogError($"[ERR] {message}");
      ChatGui.PrintError($"{message}");
    }

    /// <summary>
    /// Draw the UI on demand.
    /// </summary>
    private void DrawUI() {
      this.PluginUi.Draw();
    }

    /// <summary>
    /// Draw the configuration UI on demand.
    /// </summary>
    private void DrawConfigUI() {
      this.PluginUi.SettingsVisible = true;
    }
  }
}
