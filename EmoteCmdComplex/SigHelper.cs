using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;

using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace EmoteCmdComplex {
  public unsafe class SigHelper : IDisposable {
    private static class Signatures {
      internal const string SendChatMessage = "48 89 5C 24 ?? 57 48 83 EC 20 48 8B FA 48 8B D9 45 84 C9";
      internal const string SanitizeChatString = "E8 ?? ?? ?? ?? EB 0A 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8D 8D";
    }

    /***** functions *****/
    [Signature(Signatures.SanitizeChatString, Fallibility = Fallibility.Fallible)]
    private readonly delegate* unmanaged<Utf8String*, int, IntPtr, void> _sanitizeChatString = null!;

    // UIModule, message, unused, byte
    [Signature(Signatures.SendChatMessage, Fallibility = Fallibility.Fallible)]
    private readonly delegate* unmanaged<IntPtr, IntPtr, IntPtr, byte, void> _processChatBoxEntry = null!;

    /***** the actual class *****/

    internal SigHelper() {
      SignatureHelper.Initialise(this);
    }

    public void Dispose() {
      GC.SuppressFinalize(this);
    }

    public string GetSanitizedString(string input) {
      var uString = Utf8String.FromString(input);

      this._sanitizeChatString(uString, 0x27F, IntPtr.Zero);
      var output = uString->ToString();

      uString->Dtor();
      IMemorySpace.Free(uString);

      return output;
    }

    public void SendChatMessage(string message) {
      if (this._processChatBoxEntry is null) {
        throw new InvalidOperationException("Signature for ProcessChatBoxEntry/SendMessage not found!");
      }

      var messageBytes = Encoding.UTF8.GetBytes(message);

      switch (messageBytes.Length) {
        case 0:
          throw new ArgumentException("Message cannot be empty", nameof(message));
        case > 500:
          throw new ArgumentException("Message exceeds 500char limit", nameof(message));
      }

      var payloadMem = Marshal.AllocHGlobal(400);
      Marshal.StructureToPtr(new ChatPayload(messageBytes), payloadMem, false);

      this._processChatBoxEntry((IntPtr)Framework.Instance()->GetUiModule(), payloadMem, IntPtr.Zero, 0);

      Marshal.FreeHGlobal(payloadMem);
    }
  }
}
