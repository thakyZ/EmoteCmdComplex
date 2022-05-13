using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

using Lumina.Excel.GeneratedSheets;

namespace EmoteCmdComplex {
  public unsafe partial class EmoteCmdComplexPlugin {
    private readonly TargetSystem* _targetSystem;

    private static ExecutableAction GetExecutableAction(Emote emote) {
      return new ExecutableAction {
        ActionId = (uint)emote.RowId,
        TextCommand = (string)emote.TextCommand?.Value?.Command?.ToString()
      };
    }

    private void RunCustomEmote(string singleText, string targetText, uint emoteId = 0) {
      var isTargeting = _targetSystem->GetCurrentTarget() is not null;

      if (emoteId != 0) {
        var emote = GetEmoteById(emoteId);
        if (emote is null) {
          throw new ArgumentNullException($"Emote not found... ID: [{emoteId}]");
        }
        if (isTargeting) {
          _ = TickScheduler.Schedule(delegate {
            ChatUtils.SendSanitizedChatMessage($"/em {targetText} <t>");
            ChatUtils.SendSanitizedChatMessage($"{emote.TextCommand?.Value?.Command} motion");
          });
        } else {
          _ = TickScheduler.Schedule(delegate {
            ChatUtils.SendSanitizedChatMessage($"/em {singleText}");
            ChatUtils.SendSanitizedChatMessage($"{emote.TextCommand?.Value?.Command} motion");
          });
        }
      } else {
        if (isTargeting) {
          _ = TickScheduler.Schedule(delegate {
            ChatUtils.SendSanitizedChatMessage($"/em {targetText} <t>");
          });
        } else {
          _ = TickScheduler.Schedule(delegate {
            ChatUtils.SendSanitizedChatMessage($"/em {singleText}");
          });
        }
      }
    }
  }
}
