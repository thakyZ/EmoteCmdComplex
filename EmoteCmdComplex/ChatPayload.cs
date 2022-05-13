using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EmoteCmdComplex {
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
