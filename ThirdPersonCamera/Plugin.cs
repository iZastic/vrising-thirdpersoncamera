using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace ThirdPersonCamera
{
    [BepInProcess("VRising.exe")]
    [BepInDependency("xyz.molenzwiebel.wetstone")]
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource Logger;

        private static Harmony harmony;

        public override unsafe void Load()
        {
            Logger = Log;

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
