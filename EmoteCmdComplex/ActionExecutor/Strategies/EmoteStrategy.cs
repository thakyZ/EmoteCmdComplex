using System.Linq;

using EmoteCmdComplex.Base;

using Lumina.Excel.GeneratedSheets;
using EmoteCmdComplex.Game;

namespace EmoteCmdComplex.ActionExecutor.Strategies {
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
      return Injections.DataManager.Excel.GetSheet<Emote>()!.GetRow(id);
    }

    internal static uint GetEmoteByName(string name) {
      Service.GameStateCache!.Refresh();
      var emotes = Service.GameStateCache!.UnlockedEmotes!.Select(GetExecutableAction).ToList();

      if (emotes == null || emotes.Count == 0) {
        return 0;
      }

      var actionId = emotes.Find(match: e => e.TextCommand is not null && e.TextCommand == $"/{name}");
      return actionId is not null ? actionId.ActionId : 0;
    }
  }
}
