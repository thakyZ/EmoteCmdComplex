using System;
using System.Runtime.InteropServices;
using System.Text;

using Dalamud.Utility.Signatures;

using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.System.String;

using NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.Game.Structs;

// cSpell:word ascclemens

namespace NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.Utils {
  /// <summary>
  /// Chat utilities to send sanitized chat messages to FFXIV.
  /// Borrowed from: https://github.com/KazWolfe/XIVDeck/blob/cca5a774ab3a739cc5e2f89b4e42171be1a805b0/FFXIVPlugin/Game/Chat/ChatHelper.cs
  /// > Code heavily borrowed from ascclemens' XivCommon
  /// > https://git.anna.lgbt/ascclemens/XivCommon/src/branch/main/XivCommon/Functions/Chat.cs
  /// </summary>
  public sealed unsafe class ChatUtils {
    private static ChatUtils? _instance;

    public static ChatUtils GetInstance() {
      return _instance ??= new ChatUtils();
    }

    private static class Signatures {
      internal const string SendChatMessage = "48 89 5C 24 ?? 57 48 83 EC 20 48 8B FA 48 8B D9 45 84 C9";
      internal const string SanitizeChatString = "E8 ?? ?? ?? ?? EB 0A 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8D 8D";
    }

    private ChatUtils() {
      Services.InteropProvider.InitializeFromAttributes(this);
    }

    [Signature(Signatures.SanitizeChatString, Fallibility = Fallibility.Fallible)]
    private readonly delegate* unmanaged<Utf8String*, int, nint, void> _sanitizeChatString = null!;

    // UIModule, message, unused, byte
    [Signature(Signatures.SendChatMessage, Fallibility = Fallibility.Fallible)]
    private readonly delegate* unmanaged<nint, nint, nint, byte, void> _processChatBoxEntry = null!;

    private string SanitizeString(string input) {
      var uString = Utf8String.FromString(input);

      this._sanitizeChatString(uString, 0x27F, nint.Zero);
      var output = uString->ToString();

      uString->Dtor();
      IMemorySpace.Free(uString);

      return output;
    }

    private void SendChatMessage(string message) {
      if (this._processChatBoxEntry == null) {
        throw new InvalidOperationException("Could not find the signature for SendChatMessage!");
      }

      var messageBytes = Encoding.UTF8.GetBytes(message);

      switch (messageBytes.Length) {
        case 0:
          throw new ArgumentException("Message cannot be empty", nameof(message));
        case > 500:
          throw new ArgumentException("Message cannot exceed 500 byte limit", nameof(message));
      }

      var payloadMem = Marshal.AllocHGlobal(400);
      Marshal.StructureToPtr(new ChatPayload(messageBytes), payloadMem, false);

      this._processChatBoxEntry((nint)Framework.Instance()->GetUIModule(), payloadMem, nint.Zero, 0);

      Marshal.FreeHGlobal(payloadMem);
    }

    public void SendSanitizedChatMessage(string text, bool commandOnly = true) {
      if (commandOnly && !text.StartsWith('/')) {
        throw new ArgumentException($"The specified message \"{text}\" does not start with a slash while in command-only mode.");
      }

      // Sanitization rules
      text = text.Replace("\n", " ");
      text = this.SanitizeString(text);

      this.SendChatMessage(text);
    }
  }
}
