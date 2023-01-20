using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using ZoomTilt.Windows;
using Dalamud.Game;
using System;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using ZoomTilt.Structures;
using Dalamud.Game.Gui;
using Dalamud.Logging;

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
    private ChatGui chat { get; init; }

    public Plugin(
      [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
      [RequiredVersion("1.0")] CommandManager commandManager,
      [RequiredVersion("1.0")] Framework framework,
      [RequiredVersion("1.0")] SigScanner sigScanner,
      [RequiredVersion("1.0")] ChatGui chat
    ) {
      this.pluginInterface = pluginInterface;
      this.commandManager = commandManager;
      this.framework = framework;
      this.configModule = ConfigModule.Instance();
      this.cameraManager = (CameraManager*)sigScanner.GetStaticAddressFromSig("4C 8D 35 ?? ?? ?? ?? 85 D2"); // g_ControlSystem_CameraManager
      this.chat = chat;

      Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
      Configuration.Initialize(pluginInterface);

      // you might normally want to embed resources and load them from the manifest stream
      // var imagePath = Path.Combine(pluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
      // var goatImage = pluginInterface.UiBuilder.LoadImage(imagePath);

      WindowSystem.AddWindow(new MainWindow(this));

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


    public static double Lerp(double delta, double from, double to) {
      return from + ((to - from) * delta);
    }
    // private static double EaseInOutSine(double x) {
    //   return -(Math.Cos(Math.PI * x) - 1) / 2;
    // }
    // private static double EaseOutCubic(double x) {
    //   return 1 - Math.Pow(1 - x, 3);
    // }
    public static double EaseOutQuad(double x) => 1 - ((1 - x) * (1 - x));

    private float desiredTiltOffset;
    private float currentTiltOffset;
    public void Update(Framework framework) {
      if (!Configuration.Enabled) return;

      var currentZoom = cameraManager->WorldCamera->CurrentZoom;
      var minZoom = cameraManager->WorldCamera->MinZoom;
      var maxZoom = cameraManager->WorldCamera->MaxZoom;
      var minTilt = Configuration.MinZoomTilt;
      var maxTilt = Configuration.MaxZoomTilt;
      var tiltOffset = (int)(
        (
          (maxTilt - minTilt) * EaseOutQuad((currentZoom - minZoom) / (maxZoom - minZoom))
        )
        + minTilt
      );
      desiredTiltOffset = tiltOffset;
      currentTiltOffset = (float)Lerp(
        framework.UpdateDelta.TotalMilliseconds / 100f,
        currentTiltOffset, desiredTiltOffset
      );
      configModule->SetOption(ConfigOption.TiltOffset, (int)Math.Round(currentTiltOffset));
    }

    /* This didn't work
    private readonly float minLookAtHeightOffset = -0.342871f;
    private readonly float maxLookAtHeightOffset = -0.889871f;
    3rd Person Camera Angle: 0
    Look at Height Offset: -0.342871

    3rd Person Camera Angle: 100
    Look at Height Offset: -0.891649
                           -0.889871 is more accurate?
    public void Update(Framework framework) {
      if (!Configuration.Enabled) {
        if (desiredLookAtHeightOffset != minLookAtHeightOffset) {
          desiredLookAtHeightOffset = minLookAtHeightOffset;
          cameraManager->WorldCamera->LookAtHeightOffset = desiredLookAtHeightOffset;
        }
        return;
      }

      var currentZoom = cameraManager->WorldCamera->CurrentZoom;
      var minZoom = cameraManager->WorldCamera->MinZoom;
      var maxZoom = cameraManager->WorldCamera->MaxZoom;
      var minTilt = Configuration.MinZoomTilt;
      var maxTilt = Configuration.MaxZoomTilt;
      var currentTiltOffset = configModule->GetIntValue(ConfigOption.TiltOffset);
      var tiltOffset = (int)(
        (
          (currentZoom - minZoom) * (maxTilt - minTilt) / (maxZoom - minZoom)
        )
        + minTilt
      );
      var lookAtHeightOffset = (
        minLookAtHeightOffset
        + (maxLookAtHeightOffset - minLookAtHeightOffset)
        * (Math.Clamp(currentTiltOffset - 50, -50, 50) / 50f)
        * (float)tiltOffset / (float)maxTilt
      );
      desiredLookAtHeightOffset = lookAtHeightOffset;
      currentLookAtHeightOffset = (float)Lerp((DateTime.Now - framework.LastUpdate).TotalMilliseconds, currentLookAtHeightOffset, desiredLookAtHeightOffset);
      PluginLog.Log($"currentTiltOffset: {currentTiltOffset}");
      PluginLog.Log($"tiltOffset: {tiltOffset}");
      PluginLog.Log($"maxTilt: {maxTilt}");
      PluginLog.Log($"lookAtHeightOffset: {lookAtHeightOffset}");
      // PluginLog.Log($"ZoomTilt: tiltOffset: {tiltOffset}, currentLookAtHeightOffset: {currentLookAtHeightOffset}, desiredLookAtHeightOffset: {desiredLookAtHeightOffset}");
      cameraManager->WorldCamera->LookAtHeightOffset = currentLookAtHeightOffset;
      PluginLog.Log($"camera: {cameraManager->WorldCamera->LookAtHeightOffset}");
    }
    */

    private void OnCommand(string command, string args) {
      // in response to the slash command, just display our main ui
      if (args.Trim() == "") {
        DrawConfigUI();
        return;
      }

      var argArray = args.Split(" ");

      if (argArray[0] == "toggle") {
        Configuration.Enabled = !Configuration.Enabled;
        chat.Print($"ZoomTilt {(Configuration.Enabled ? "enabled" : "disabled")}");
        return;
      }
    }

    private void DrawUI() {
      WindowSystem.Draw();
    }

    public void DrawConfigUI() {
      WindowSystem.GetWindow("ZoomTilt")!.IsOpen = true;
    }
  }
}
