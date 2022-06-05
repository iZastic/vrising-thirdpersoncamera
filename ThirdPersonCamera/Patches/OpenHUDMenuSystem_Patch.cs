using HarmonyLib;
using ProjectM.UI;

namespace ThirdPersonCamera.Patches
{
    [HarmonyPatch]
    public class OpenHUDMenuSystem_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(OpenHUDMenuSystem), nameof(OpenHUDMenuSystem.OnUpdate))]
        public static void OnUpdate(OpenHUDMenuSystem __instance)
        {
            ThirdPersonCamera.isMenuOpen = __instance.CurrentMenuType != HUDMenuType.None;
        }
    }
}