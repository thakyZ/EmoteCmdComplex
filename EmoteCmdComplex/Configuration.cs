//-----------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Neko Boi Nick">
// Copyright (c) Neko Boi Nick. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

using Dalamud.Configuration;
using Dalamud.Plugin;

/// <summary>
/// Main plugin configuration implementation.
/// </summary>
namespace EmoteCmdComplex {
  /// <summary>
  /// Main plugin configuration class.
  /// </summary>
  [Serializable]
  public class Configuration : IPluginConfiguration {
    /// <summary>
    /// Gets or sets the version of the configuration file.
    /// </summary>
    public int Version { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether the debug logging system is enabled.
    /// </summary>
    public bool Debug { get; set; } = false;

    // the below exist just to make saving less cumbersome

    /// <summary>
    /// The plugin interface.
    /// </summary>
    [NonSerialized]
    private DalamudPluginInterface? pluginInterface;

    /// <summary>
    /// Initializes the configuration.
    /// </summary>
    /// <param name="pluginInterface">The plugin interface from the main plugin implementation.</param>
    public void Initialize(DalamudPluginInterface pluginInterface) {
      this.pluginInterface = pluginInterface;
    }

    /// <summary>
    /// Saves the plugin configuration to file.
    /// </summary>
    public void Save() {
      this.pluginInterface!.SavePluginConfig(this);
    }
  }
}
