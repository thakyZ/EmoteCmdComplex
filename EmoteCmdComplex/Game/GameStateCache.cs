using System.Collections.Generic;
using System.Linq;

using Dalamud.Utility.Signatures;

using EmoteCmdComplex.Base;

using FFXIVClientStructs.FFXIV.Client.Game.UI;

using Lumina.Excel.GeneratedSheets;

namespace EmoteCmdComplex.Game {
  /// <summary>
  /// The game state data. Loaded from binary signatures.
  /// Borrowed from: https://github.com/KazWolfe/XIVDeck/blob/main/FFXIVPlugin/Game/GameStateCache.cs
  /// </summary>
  public unsafe class GameStateCache {

    private static class Signatures {
      internal const string IsEmoteUnlocked = "E8 ?? ?? ?? ?? 84 C0 74 A4";
    }

    [Signature(Signatures.IsEmoteUnlocked, Fallibility = Fallibility.Fallible)]
    private readonly delegate* unmanaged<UIState*, uint, byte, byte> _isEmoteUnlocked = null;

    public IReadOnlyList<Emote>? UnlockedEmotes { get; private set; }

    internal bool IsEmoteUnlocked(uint emoteId) {
      if (this._isEmoteUnlocked == null) return false;

      var emote = Injections.DataManager.Excel.GetSheet<Emote>()!.GetRow(emoteId);
      if (emote == null || emote.Order == 0) return false;

      return emote.UnlockLink == 0 || (this._isEmoteUnlocked(UIState.Instance(), emote.UnlockLink, 1) > 0);
    }

    private GameStateCache() {
      SignatureHelper.Initialise(this);
    }

    public void Refresh() {
      if (this._isEmoteUnlocked != null) {
        this.UnlockedEmotes = Injections.DataManager.GetExcelSheet<Emote>()!
            .Where(x => this.IsEmoteUnlocked(x.RowId)).ToList();
      }
    }

    public static GameStateCache Load() {
      return new();
    }
  }
}
