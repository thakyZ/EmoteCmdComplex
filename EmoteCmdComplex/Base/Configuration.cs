using System;

using Dalamud.Configuration;
using Dalamud.Plugin;

/// <summary>
/// Main plugin configuration implementation.
/// </summary>
namespace EmoteCmdComplex.Base {
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
    /// <param name="pluginInterface">The plugin interface from the main plugin implementation.</param>
    public static Configuration Load() {
        if (Service.PluginInterface.GetPluginConfig() is Configuration config)
        {
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
      Service.PluginInterface!.SavePluginConfig(this);
    }
  }
}
