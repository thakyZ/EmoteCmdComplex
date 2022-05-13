using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FFXIVClientStructs.FFXIV.Client.UI.Misc;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EmoteCmdComplex {
  [Serializable]
  public class ExecutableAction {
    [JsonProperty("id")] public uint ActionId;
    [JsonProperty("textcommand")] public string TextCommand;
  }
}
