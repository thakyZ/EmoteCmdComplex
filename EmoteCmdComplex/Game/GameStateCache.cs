using System;
using System.Collections.Generic;
using System.Linq;

using Dalamud.Memory;
using Dalamud.Utility.Signatures;

using NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.Base;

using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

using Lumina.Excel.GeneratedSheets;

namespace NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.Game {
  /// <summary>
  /// The game state data. Loaded from binary signatures.
  /// Borrowed from: https://github.com/KazWolfe/XIVDeck/blob/main/FFXIVPlugin/Game/GameStateCache.cs
  /// </summary>
  public unsafe class GameStateCache {
    //private static class Signatures {
    //  internal const string IsEmoteUnlocked = "E8 ?? ?? ?? ?? 84 C0 74 A4";
    //}

    //[Signature(Signatures.IsEmoteUnlocked, Fallibility = Fallibility.Fallible)]
    //private readonly delegate* unmanaged<UIState*, uint, byte, byte> _isEmoteUnlocked = null;
    private static bool IsUnlockLinkUnlockedOrQuestCompleted(uint unlockLinkOrQuestId) {
        return UIState.Instance()->IsUnlockLinkUnlockedOrQuestCompleted(unlockLinkOrQuestId, 1);
    }

    public IReadOnlyList<Emote>? UnlockedEmotes { get; private set; }

    /*internal bool IsEmoteUnlocked(uint emoteId) {
      if (this._isEmoteUnlocked == null) return false;

      var emote = Services.DataManager.Excel.GetSheet<Emote>()!.GetRow(emoteId);
      if (emote == null || emote.Order == 0) return false;

      return emote.UnlockLink == 0 || (this._isEmoteUnlocked(UIState.Instance(), emote.UnlockLink, 1) > 0);
    }*/

    internal static bool IsEmoteUnlocked(Emote? emote) {
        // Work around showing emotes if nobody is logged in.
        if (!Services.ClientState.IsLoggedIn) return false;
        // WARNING: This is a reimplementation of UIState#IsEmoteUnlocked, but designed to hopefully be a bit faster and
        // more reliable. As a result, this is not exactly faithful to how the game does it, but the logic is the same.
        // Particularly:
        // 1. IsEmoteUnlocked will check Emote#EmoteCategory, but we're using Emote#Order as it's more in line with how
        //    the emote UI works.
        // 2. IsEmoteUnlocked uses its own (inlined) checks rather than IULUOQC. However, this inlined version is (for
        //    now) functionally identical to IULUOQC with the default arguments.
        // Both of these decisions *should* be safe, but are being recorded here for posterity for when Square decides
        // to blow all this up.
        if (emote == null || emote.Order == 0) return false;

        // HACK - We need to handle GC emotes as a special case
        switch (emote.RowId) {
            case 55 when PlayerState.Instance()->GrandCompany != 1: // Maelstrom
            case 56 when PlayerState.Instance()->GrandCompany != 2: // Twin Adders
            case 57 when PlayerState.Instance()->GrandCompany != 3: // Immortal Flames
                return false;
        }

        return emote.UnlockLink == 0 || IsUnlockLinkUnlockedOrQuestCompleted(emote.UnlockLink);
    }

    internal static bool IsEmoteUnlocked(uint emoteId) {
      var emote = Services.DataManager.Excel.GetSheet<Emote>()!.GetRow(emoteId);
      if (emote is null) {
        return false;
      }
      return IsEmoteUnlocked(emote);
    }

    internal GameStateCache() {
      SignatureHelper.Initialise(this);
      this.Refresh();
    }

    //public void Refresh() {
    //  if (this._isEmoteUnlocked != null) {
    //    this.UnlockedEmotes = Services.DataManager.GetExcelSheet<Emote>()!
    //        .Where(x => this.IsEmoteUnlocked(x.RowId)).ToList();
    //  }
    //}

    public static GameStateCache Load() {
      return new();
    }

    internal void Refresh() {
        this.UnlockedEmotes = Services.DataManager.GetExcelSheet<Emote>()!
            .Where(x => x.IsUnlocked()).ToList();
    }
  }
  public static class LuminaExtensions {
    public static bool IsUnlocked(this Emote emote) {
      return GameStateCache.IsEmoteUnlocked(emote);
    }
  }
}
