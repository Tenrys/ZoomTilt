using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace ZoomTilt {
  [Serializable]
  public class Configuration : IPluginConfiguration {
    public int Version { get; set; } = 0;

    public bool Enabled { get; set; } = true;
    public int MinZoomTilt { get; set; } = 0;
    public int MaxZoomTilt { get; set; } = 100;

    // the below exists just to make saving less cumbersome
    [NonSerialized]
    private DalamudPluginInterface? pluginInterface;

    public void Initialize(DalamudPluginInterface pluginInterface) {
      this.pluginInterface = pluginInterface;
    }

    public void Save() {
      pluginInterface!.SavePluginConfig(this);
    }
  }
}
