using System;

using Lumina.Excel.GeneratedSheets;

using Newtonsoft.Json;

namespace EmoteCmdComplex.ActionExecutor {
  /// <summary>
  /// A class for returning Action data from Lumina.
  /// Borrowed from https://github.com/KazWolfe/XIVDeck/blob/main/FFXIVPlugin/ActionExecutor/ExecutableAction.cs
  /// </summary>
  [Serializable]
  internal class ExecutableAction {
    /// <summary>
    /// The ID of the action. Corresponds to the RowID.
    /// </summary>
    [JsonProperty("id")] internal uint ActionId;
    /// <summary>
    /// The string for the text command. Corresponds to <see cref="Action.TextCommand.Value().ToString()"/>
    /// </summary>
    [JsonProperty("textCommand")] internal string TextCommand = String.Empty;
  }
}
