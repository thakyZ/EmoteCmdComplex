﻿using System;
using System.Numerics;

using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;

using NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.Base;

using ImGuiNET;

namespace NekoBoiNick.FFXIV.DalamudPlugin.EmoteCmdComplex.UI {
  // It is good to have this be disposable in general, in case you ever need it
  // to do any cleanup
  public class PluginUI : Window, IDisposable {
    private readonly Configuration configuration;

    // this extra boolean exists for ImGui, since you can't ref a property
    private bool visible;
    public bool Visible {
      get { return this.visible; }
      set { this.visible = value; }
    }

    private bool settingsVisible;
    public bool SettingsVisible {
      get { return this.settingsVisible; }
      set { this.settingsVisible = value; }
    }

    // passing in the image here just for simplicity
    public PluginUI() : base($"{EmoteCmdComplexPlugin.PluginName} Settings") {
      EmoteCmdComplexPlugin.Instance.WindowSystem.AddWindow(this);
      this.configuration = EmoteCmdComplexPlugin.Instance.Configuration;
    }

    public void Dispose() {
      EmoteCmdComplexPlugin.Instance.Configuration.Save();
      EmoteCmdComplexPlugin.Instance.WindowSystem.RemoveWindow(this);
    }

    public override void Draw() {
      if (!IsOpen && !settingsVisible)
        return;

      // can't ref a property, so use a local copy
      var configValue = this.configuration.Debug;
      if (ImGui.Checkbox("Debug", ref configValue)) {
        this.configuration.Debug = configValue;
        // can save immediately on change, if you don't want to provide a "Save and Close" button
        this.configuration.Save();
      }
      ImGui.Indent();
      if (EmoteCmdComplexPlugin.Instance.Configuration.Debug) {
        ImGui.TextColored(ImGuiColors.HealerGreen, fmt: "Debug Enabled");
      } else {
        ImGui.TextColored(ImGuiColors.DalamudRed, "Debug Disabled");
      }
      ImGui.Unindent();
      ImGui.Spacing();
    }

    public override void OnClose() {
      settingsVisible = false;
      EmoteCmdComplexPlugin.Instance.Configuration.Save();
    }
  }
}
