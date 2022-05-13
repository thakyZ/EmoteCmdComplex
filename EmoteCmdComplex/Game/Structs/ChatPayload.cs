using System;
using System.Runtime.InteropServices;

namespace EmoteCmdComplex.Game.Structs {
  /// <summary>
  /// The payload for what to send to chat.
  /// Borrowed from: https://github.com/KazWolfe/XIVDeck/blob/main/FFXIVPlugin/Game/Structs/ChatPayload.cs
  /// </summary>
  [StructLayout(LayoutKind.Explicit)]
  internal readonly struct ChatPayload : IDisposable {
    [FieldOffset(0)] private readonly IntPtr _stringPtr;
    [FieldOffset(16)] private readonly ulong _stringLength;
    [FieldOffset(8)] private readonly ulong _unknown1 = 64;
    [FieldOffset(24)] private readonly ulong _unknown2 = 0;

    internal ChatPayload(byte[] stringBytes) {
      this._stringPtr = Marshal.AllocHGlobal(stringBytes.Length + 30);
      Marshal.Copy(stringBytes, 0, this._stringPtr, stringBytes.Length);
      Marshal.WriteByte(this._stringPtr + stringBytes.Length, 0);

      this._stringLength = (ulong)stringBytes.Length + 1;
    }

    public void Dispose() {
      Marshal.FreeHGlobal(this._stringPtr);
    }
  }
}
