using System.Linq;

using NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.Base;

using Lumina.Excel.GeneratedSheets;
using NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.Game;
using Dalamud.Logging;

namespace NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.ActionExecutor.Strategies {
  /// <summary>
  /// Emote utility file and functions.
  /// Borrowed from https://github.com/KazWolfe/XIVDeck/blob/main/FFXIVPlugin/ActionExecutor/Strategies/EmoteStrategy.cs
  /// </summary>
  internal static class EmoteStrategy {
    /// <summary>
    /// Gets the executable action from a <see cref="GameStateCache"/>.
    /// </summary>
    /// <param name="emote">The emote data.</param>
    /// <returns>A <list type="ExecutableAction"/></returns>
    private static ExecutableAction GetExecutableAction(Emote emote) {
      return new ExecutableAction {
        ActionId = emote.RowId, // unit
        TextCommand = emote.TextCommand.Value is not null ? emote.TextCommand.Value.Command.ToString() : string.Empty // string
      };
    }
    internal static Emote? GetEmoteById(uint id) {
      return Services.DataManager.Excel.GetSheet<Emote>()!.GetRow(id);
    }

    internal static uint GetEmoteByName(string name) {
      if (EmoteCmdComplexPlugin.Instance.GameStateCache is null || EmoteCmdComplexPlugin.Instance.GameStateCache.UnlockedEmotes is null) {
        return 0;
      }
      EmoteCmdComplexPlugin.Instance.GameStateCache.Refresh();
      var emotes = EmoteCmdComplexPlugin.Instance.GameStateCache.UnlockedEmotes.Select(GetExecutableAction).ToList();

      if (emotes == null || emotes.Count == 0) {
        return 0;
      }

      PluginLog.Information(string.Join(",", emotes));

      var actionId = emotes.Find(match: e => e.TextCommand is not null && e.TextCommand == $"/{name}");
      return actionId is not null ? actionId.ActionId : 0;
    }
  }
}
