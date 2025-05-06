using System;

using Dalamud.Configuration;

/// <summary>
/// Main plugin configuration implementation.
/// </summary>
namespace NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.Base {
  /// <summary>
  /// Main plugin configuration class.
  /// </summary>
  [Serializable]
  public class Configuration : IPluginConfiguration {
    /// <summary>
    /// Gets or sets the version of the configuration file.
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Gets or sets a value indicating whether the debug logging system is enabled.
    /// </summary>
    public bool Debug { get; set; } = false;

    /// <summary>
    /// Initializes the configuration.
    /// </summary>
    public static Configuration Load() {
      if (Services.PluginInterface.GetPluginConfig() is Configuration config) {
        return config;
      }

      config = new Configuration();
      config.Save();
      return config;
    }

    /// <summary>
    /// Saves the plugin configuration to file.
    /// </summary>
    public void Save() {
      Services.PluginInterface!.SavePluginConfig(this);
    }
  }
}
