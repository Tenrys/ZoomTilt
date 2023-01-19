using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using System.Reflection;
using Dalamud.Interface.Windowing;
using ZoomTilt.Windows;
using Dalamud.Game;
using System;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using System.Diagnostics;
using Dalamud.Logging;
using ZoomTilt.Structures;

namespace ZoomTilt {
  public sealed unsafe class Plugin : IDalamudPlugin {
    public string Name => "ZoomTilt";
    private const string CommandName = "/zoomtilt";

    private DalamudPluginInterface pluginInterface { get; init; }
    private CommandManager commandManager { get; init; }
    public Configuration Configuration { get; init; }
    private Framework framework { get; init; }
    public WindowSystem WindowSystem = new("ZoomTilt");
    private ConfigModule* configModule { get; init; }
    private CameraManager* cameraManager { get; init; }

    public Plugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] CommandManager commandManager,
        [RequiredVersion("1.0")] Framework framework,
        [RequiredVersion("1.0")] SigScanner sigScanner) {
      this.pluginInterface = pluginInterface;
      this.commandManager = commandManager;
      this.framework = framework;
      this.configModule = ConfigModule.Instance();
      this.cameraManager = cameraManager = (CameraManager*)sigScanner.GetStaticAddressFromSig("4C 8D 35 ?? ?? ?? ?? 85 D2"); // g_ControlSystem_CameraManager

      Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
      Configuration.Initialize(pluginInterface);

      // you might normally want to embed resources and load them from the manifest stream
      // var imagePath = Path.Combine(pluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
      // var goatImage = pluginInterface.UiBuilder.LoadImage(imagePath);

      WindowSystem.AddWindow(new MainWindow(this));
      // WindowSystem.AddWindow(new ConfigWindow(this));

      commandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
        HelpMessage = "A useful message to display in /xlhelp"
      });

      pluginInterface.UiBuilder.Draw += DrawUI;
      pluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

      this.framework.Update += Update;
    }

    public void Dispose() {
      WindowSystem.RemoveAllWindows();

      commandManager.RemoveHandler(CommandName);

      framework.Update -= Update;
    }

    public void Update(Framework framework) {
      if (!Configuration.Enabled) return;

      // var now = (DateTime.Now - Process.GetCurrentProcess().StartTime).TotalMilliseconds;
      // var tiltOffset = Math.Abs(Math.Sin(now * 0.005f)) * 100;
      var currentZoom = cameraManager->WorldCamera->CurrentZoom;
      var minZoom = cameraManager->WorldCamera->MinZoom;
      var maxZoom = cameraManager->WorldCamera->MaxZoom;
      var minTilt = Configuration.MinZoomTilt;
      var maxTilt = Configuration.MaxZoomTilt;
      // Calculate the tilt offset based on the current zoom level, raning from the min and max tilt values
      var tiltOffset = (int)(
        (
          (currentZoom - minZoom) * (maxTilt - minTilt) / (maxZoom - minZoom)
        )
        + minTilt
      );
      var currentTiltOffset = configModule->GetIntValue(ConfigOption.TiltOffset);
      if (currentTiltOffset != tiltOffset) {
        PluginLog.Log($"ZoomTilt: current zoom: {currentZoom}, old: {currentTiltOffset}, new: {tiltOffset}");
      }
      configModule->SetOption(ConfigOption.TiltOffset, tiltOffset);
    }

    private void OnCommand(string command, string args) {
      // in response to the slash command, just display our main ui
      DrawConfigUI();
    }

    private void DrawUI() {
      WindowSystem.Draw();
    }

    public void DrawConfigUI() {
      WindowSystem.GetWindow("ZoomTilt")!.IsOpen = true;
    }
  }
}
