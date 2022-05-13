using System;

namespace EmoteCmdComplex.Utils {
  /// <summary>
  /// Chat utilities to send sanitized chat messages to FFXIV.
  /// Borrowed from: https://github.com/KazWolfe/XIVDeck/blob/main/FFXIVPlugin/Utils/ChatUtils.cs
  /// </summary>
  public static class ChatUtils {
    // Borrowed base logic from ChatTwo by ascclemens
    public static void SendSanitizedChatMessage(string text, bool commandOnly = true) {
      var plugin = EmoteCmdComplexPlugin.Instance;

      if (commandOnly && !text.StartsWith("/")) {
        throw new ArgumentException($"The specified message {text} does not start with a slash while in command-only mode.");
      }

      // Sanitization rules
      text = text.Replace("\n", " ");
      text = plugin.SigHelper.GetSanitizedString(text);

      plugin.SigHelper.SendChatMessage(text);
    }
  }
}
