using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoteCmdComplex {
  public static class ChatUtils {
    // borrowed base logic from ChatTwo by ascclemens
    public static void SendSanitizedChatMessage(string text, bool commandOnly = true) {
      var plugin = EmoteCmdComplexPlugin.Instance;

      if (commandOnly && !text.StartsWith("/")) {
        throw new ArgumentException("The specified message message does not start with a slash while in command-only mode.");
      }

      // sanitization rules
      text = text.Replace("\n", " ");
      text = plugin.SigHelper.GetSanitizedString(text);

      plugin.SigHelper.SendChatMessage(text);
    }
  }
}
