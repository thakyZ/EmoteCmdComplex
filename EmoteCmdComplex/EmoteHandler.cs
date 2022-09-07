using System;
using System.Data;

using Dalamud.Game.Text.SeStringHandling.Payloads;

using EmoteCmdComplex.ActionExecutor.Strategies;
using EmoteCmdComplex.Base;
using EmoteCmdComplex.Game;
using EmoteCmdComplex.Utils;

using FFXIVClientStructs.FFXIV.Client.Game.Control;

using ImGuiNET;

using ImGuizmoNET;

using Lumina.Excel.GeneratedSheets;
using System.Linq;

namespace EmoteCmdComplex {
  public unsafe partial class EmoteCmdComplexPlugin {
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
        var emote = EmoteStrategy.GetEmoteById(emoteId);
        if (emote is null) {
          throw new ArgumentNullException($"Emote not found... ID: [{emoteId}]");
        }
        if (isTargeting) {
          if (ContainsPlaceholder(targetText)) {
            _ = Service.Framework.RunOnFrameworkThread(delegate {
              ChatUtils.SendSanitizedChatMessage($"/em {targetText}");
              ChatUtils.SendSanitizedChatMessage($"{emote.TextCommand?.Value?.Command} motion");
            });
          } else {
            LogError("Target placeholder not detected in the target text argument.");
          }
        } else {
          _ = Service.Framework.RunOnFrameworkThread(delegate {
            ChatUtils.SendSanitizedChatMessage($"/em {singleText}");
            ChatUtils.SendSanitizedChatMessage($"{emote.TextCommand?.Value?.Command} motion");
          });
        }
      } else {
        if (isTargeting) {
          if (ContainsPlaceholder(targetText)) {
            _ = Service.Framework.RunOnFrameworkThread(delegate {
              ChatUtils.SendSanitizedChatMessage($"/em {targetText}");
            });
          } else {
            LogError("Target placeholder not detected in the target text argument.");
          }
        } else {
          _ = Service.Framework.RunOnFrameworkThread(delegate {
            ChatUtils.SendSanitizedChatMessage($"/em {singleText}");
          });
        }
      }
    }
  }
}
