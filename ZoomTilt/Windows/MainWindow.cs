using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;

namespace ZoomTilt.Windows;

public class MainWindow : Window, IDisposable {
  // private TextureWrap goatImage;
  private readonly Plugin plugin;

  public MainWindow(Plugin plugin/* , TextureWrap goatImage */) : base(
    "ZoomTilt", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse
  ) {
    var defaultSize = new Vector2(384, 112);
    Size = defaultSize;
    SizeConstraints = new WindowSizeConstraints {
      MinimumSize = defaultSize,
      MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
    };

    // this.goatImage = goatImage;
    this.plugin = plugin;
  }

  public void Dispose() {
    GC.SuppressFinalize(this);
    // this.goatImage.Dispose();
  }

  public override void Draw() {
    var enabled = plugin.Configuration.Enabled;
    var minZoomTilt = plugin.Configuration.MinZoomTilt;
    var maxZoomTilt = plugin.Configuration.MaxZoomTilt;

    var save = false;

    save |= ImGui.Checkbox("Enabled", ref enabled);

    // ImGui.TextUnformatted("Min Zoom Tilt");
    // ImGui.SameLine();
    save |= ImGui.SliderInt("Min Zoom Tilt", ref minZoomTilt, 0, 100);
    // ImGui.TextUnformatted("Max Zoom Tilt");
    // ImGui.SameLine();
    save |= ImGui.SliderInt("Max Zoom Tilt", ref maxZoomTilt, 0, 100);

    if (save) {
      plugin.Configuration.Enabled = enabled;
      plugin.Configuration.MinZoomTilt = minZoomTilt;
      plugin.Configuration.MaxZoomTilt = maxZoomTilt;
      plugin.Configuration.Save();
    }

    // ImGui.Text($"The random config bool is {this.plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");

    // if (ImGui.Button("Show Settings")) {
    //   this.plugin.DrawConfigUI();
    // }

    // ImGui.Spacing();

    // ImGui.Text("Have a goat:");
    // ImGui.Indent(55);
    // ImGui.Image(this.goatImage.ImGuiHandle, new Vector2(this.goatImage.Width, this.goatImage.Height));
    // ImGui.Unindent(55);
  }
}
