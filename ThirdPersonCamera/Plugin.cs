using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace ThirdPersonCamera
{
    [Wetstone.API.Reloadable]
    [BepInProcess("VRising.exe")]
    [BepInDependency("xyz.molenzwiebel.wetstone")]
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource Logger;
        public static ConfigEntry<float> lookAtOffsetX;
        public static ConfigEntry<float> lookAtOffsetY;
        public static ConfigEntry<int> aimOffsetX;
        public static ConfigEntry<int> aimOffsetY;
        public static ConfigEntry<int> mouseLockDelay;

        private static Harmony harmony;

        public override unsafe void Load()
        {
            Logger = Log;

            lookAtOffsetX = Config.Bind("Default", "LookAtOffsetX", .3f, "(World Coordinates) Used to offset the camera look at position when zoomed in");
            lookAtOffsetY = Config.Bind("Default", "LookAtOffsetY", .8f, "(World Coordinates) Used to offset the camera look at position when zoomed in");
            aimOffsetX = Config.Bind("Default", "AimOffsetX", 0, "(Screen Pixels) Used to offset the mouse aim position from the center of the screen when in combat mode");
            aimOffsetY = Config.Bind("Default", "AimOffsetY", (int)(Screen.height * 0.1f), "(Screen Pixels) Used to offset the mouse aim position from the center of the screen when in combat mode");
            mouseLockDelay = Config.Bind("Default", "MouseLockDelay", 150, "(Milliseconds) Delay before combat mode is used when holding down the camera rotate button");

            // Initialize custom classes
            ThirdPersonCamera.Initialize();

            // Register native hooks
            TopdownCameraSystem_Patch.RegisterHooks();

            // Patch with harmony
            harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public override bool Unload()
        {
            // Unpatch harmony
            harmony.UnpatchSelf();

            // Unregister native hooks
            TopdownCameraSystem_Patch.UnRegisterHooks();

            // Uninitialize custom classes
            ThirdPersonCamera.Uninitialize();

            return true;
        }
    }
}
