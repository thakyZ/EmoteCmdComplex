using System;
using System.Data;
using System.Linq;

using FFXIVClientStructs.FFXIV.Client.Game.Control;

using NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.ActionExecutor.Strategies;
using NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.Utils;

namespace NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex {
  public unsafe partial class Plugin {
    private readonly TargetSystem* _targetSystem;
    private static bool ContainsPlaceholder(string text) {
      var placeholders = new string[] {
        "<t>", "<target>"
      };
      foreach (var _ in from placeholder in placeholders where text.Contains(placeholder) select new { }) {
        return true;
      }

      return false;
    }
    private void RunCustomEmote(string singleText, string targetText, uint emoteId = 0) {
      var isTargeting = _targetSystem->GetCurrentTarget() is not null;

      if (emoteId != 0) {
        var emote = EmoteStrategy.GetEmoteById(emoteId) ?? throw new ArgumentNullException($"Emote not found... ID: [{emoteId}]");

        if (isTargeting) {
          if (ContainsPlaceholder(targetText)) {
            _ = Services.Framework.RunOnFrameworkThread(delegate {
              ChatUtils.GetInstance().SendSanitizedChatMessage($"/em {targetText}");
              ChatUtils.GetInstance().SendSanitizedChatMessage($"{emote.TextCommand?.Value?.Command} motion");
            });
          } else {
            LogError("Target placeholder not detected in the target text argument.");
          }
        } else {
          _ = Services.Framework.RunOnFrameworkThread(delegate {
            ChatUtils.GetInstance().SendSanitizedChatMessage($"/em {singleText}");
            ChatUtils.GetInstance().SendSanitizedChatMessage($"{emote.TextCommand?.Value?.Command} motion");
          });
        }
      } else {
        if (isTargeting) {
          if (ContainsPlaceholder(targetText)) {
            _ = Services.Framework.RunOnFrameworkThread(delegate {
              ChatUtils.GetInstance().SendSanitizedChatMessage($"/em {targetText}");
            });
          } else {
            LogError("Target placeholder not detected in the target text argument.");
          }
        } else {
          _ = Services.Framework.RunOnFrameworkThread(delegate {
            ChatUtils.GetInstance().SendSanitizedChatMessage($"/em {singleText}");
          });
        }
      }
    }
  }
}
