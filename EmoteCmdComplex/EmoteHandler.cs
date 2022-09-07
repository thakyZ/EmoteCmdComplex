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
        "<t>", "<target>", "<tt>", "<t2t>", "<me>", "<0>", "<r>", "<reply>", "<1>", "<2>", "<3>", "<4>", "<5>", "<6>", "<7>", "<8>", "<f>", "<focus>", "<lt>", "<lasttarget>", "<le>", "<lastenemy>", "<la>", "<lastattacker>", "<c>", "<comp>", "<b>", "<buddy>", "<pet>", "<attack1>", "<attack2>", "<attack3>", "<attack4>", "<attack5>", "<bind1>", "<bind2>", "<bind3>", "<ignore1>", "<ignore2>", "<square>", "<circle>", "<cross>", "<triange>", "<mo>", "<mouseover>", "<targethpp>", "<thpp>", "<focushpp>", "<fhpp>", "<targetclass>", "<tclass>", "<targetjob>", "<tjob>", "<focusclass>", "<fclass>", "<focusjob>", "<fjob>"
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
            EmoteCmdComplexPlugin.LogError("Target placeholder not detected in the target text argument.");
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
            EmoteCmdComplexPlugin.LogError("Target placeholder not detected in the target text argument.");
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
