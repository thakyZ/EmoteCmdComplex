//-----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Neko Boi Nick">
// Copyright (c) Neko Boi Nick. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using Dalamud.Utility.Signatures;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.GeneratedSheets;
using FFXIVClientStructs;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using ImGuizmoNET;
using System.Xml.Linq;

/// <summary>
/// Main plugin implementation.
/// </summary>
namespace EmoteCmdComplex {
  /// <summary>
  /// Main plugin Class.
  /// </summary>
  public partial class EmoteCmdComplexPlugin : IDalamudPlugin {
    internal static EmoteCmdComplexPlugin Instance = null!;
    public GameStateCache GameStateCache { get; }
    public SigHelper SigHelper { get; }
    public string Name => "Emote Command Complex";

    private const string CommandName = "/xlem";

    private DalamudPluginInterface PluginInterface {
      get; init;
    }
    private CommandManager CommandManager {
      get; init;
    }
    private Configuration Configuration {
      get; init;
    }
    private PluginUI PluginUi {
      get; init;
    }

    private readonly TargetManager _targetManager;

    public EmoteCmdComplexPlugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] CommandManager commandManager,
        TargetManager targetManager) {
      this.PluginInterface = pluginInterface;
      this.CommandManager = commandManager;
      pluginInterface.Create<Injections>();
      Resolver.Initialize(Injections.SigScanner.SearchBase);
      Instance = this;

      this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
      this.Configuration.Initialize(this.PluginInterface);

      // you might normally want to embed resources and load them from the manifest stream
      var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "EmoteCmdComplex.png");
      var pluginIcon = this.PluginInterface.UiBuilder.LoadImage(imagePath);
      this.PluginUi = new PluginUI(this.Configuration, pluginIcon);
      this.SigHelper = new SigHelper();

      this.GameStateCache = GameStateCache.Load();
      _targetManager = targetManager;
      unsafe {
        _targetSystem = (TargetSystem*)_targetManager.Address;
      }

      _ = this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
        HelpMessage = "Custom emote messages while using emote.\n/xlem (<t>) \"(Text for non-target mode)\" \"(Text for targeted mode)\""
      });

      this.PluginInterface.UiBuilder.Draw += DrawUI;
      this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }

    public void Dispose() {
      this.PluginUi.Dispose();
      _ = this.CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args) {
      var resultArgs = args.Split('"')
        .Select((element, index) => index % 2 == 0 // If even index
                ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) // Split the item
                : new string[] { element }) // Keep the entire item
        .SelectMany(element => element).ToList();

      if (resultArgs.Count > 3) {
        LogError($"Please surround the two messages in double quotes.\nExample: /xlem tea \"soup can sam\" \"soup can sam toward\"");
        return;
      }
      if (resultArgs.Count == 3 && resultArgs[resultArgs.Count - 1].Contains(' ')) {
        LogError($"Please do not space out the emote command name.");
        return;
      }
      if (resultArgs.Count == 3 && GameStateCache.IsEmoteUnlocked(GetEmoteByName(resultArgs[resultArgs.Count - 1]))) {
        RunCustomEmote(resultArgs[0], resultArgs[1], GetEmoteByName(resultArgs[resultArgs.Count - 1]));
      } else {
        RunCustomEmote(resultArgs[0], resultArgs[1]);
      }
    }
    private static Emote? GetEmoteById(uint id) {
      return Injections.DataManager.Excel.GetSheet<Emote>()!.GetRow(id);
    }

    private uint GetEmoteByName(string name) {
      GameStateCache.Refresh();
      var emotes = GameStateCache.UnlockedEmotes!.Select(GetExecutableAction).ToList();

      if (emotes == null || emotes.Count == 0) {
        return 0;
      }

      return emotes.Find(e => e.TextCommand == $"/{name}").ActionId;
    }

    private static bool CheckFullString(string arg) {
      if (arg[0] == '"' && arg[arg.Length - 1] == '"') {
        return true;
      }
      return false;
    }

    private static void LogStatic(string message) {
      // if (!Config.PrintMessage)
      //   return;
      var msg = $"{message}";
      PluginLog.Log(msg);
      // ChatGui.Print(msg);
    }

    private void Log(string message) {
      // if (!Config.PrintMessage)
      //   return;
      var msg = $"{message}";
      PluginLog.Log(msg);
      // ChatGui.Print(msg);
    }
    public void LogError(string message) {
      // if (!Config.PrintError)
      //   return;
      var msg = $"{message}";
      PluginLog.LogError(msg);
      // ChatGui.PrintError(msg);
    }

    private void DrawUI() {
      this.PluginUi.Draw();
    }

    private void DrawConfigUI() {
      this.PluginUi.SettingsVisible = true;
    }
  }
}
