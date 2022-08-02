using System;
using System.Data;

using EmoteCmdComplex.ActionExecutor.Strategies;
using EmoteCmdComplex.Base;
using EmoteCmdComplex.Game;
using EmoteCmdComplex.Utils;

using FFXIVClientStructs.FFXIV.Client.Game.Control;

namespace EmoteCmdComplex {
  public unsafe partial class EmoteCmdComplexPlugin {
    private readonly TargetSystem* _targetSystem;

    private void RunCustomEmote(string singleText, string targetText, uint emoteId = 0) {
      var isTargeting = _targetSystem->GetCurrentTarget() is not null;

      if (emoteId != 0) {
        var emote = EmoteStrategy.GetEmoteById(emoteId);
        if (emote is null) {
          throw new ArgumentNullException($"Emote not found... ID: [{emoteId}]");
        }
        if (isTargeting) {
          _ = Injections.Framework.RunOnFrameworkThread(duler.Schedule(delegate {
            ChatUtils.SendSanitizedChatMessage($"/em {targetText} <t>");
            ChatUtils.SendSanitizedChatMessage($"{emote.TextCommand?.Value?.Command} motion");
          });
        } else {
          _ = Injections.Framework.RunOnFrameworkThread(delegate {
            ChatUtils.SendSanitizedChatMessage($"/em {singleText}");
            ChatUtils.SendSanitizedChatMessage($"{emote.TextCommand?.Value?.Command} motion");
          });
        }
      } else {
        if (isTargeting) {
          _ = Injections.Framework.RunOnFrameworkThread(delegate {
            ChatUtils.SendSanitizedChatMessage($"/em {targetText} <t>");
          });
        } else {
          _ = Injections.Framework.RunOnFrameworkThread(delegate {
            ChatUtils.SendSanitizedChatMessage($"/em {singleText}");
          });
        }
      }
    }
  }
}
