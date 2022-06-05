using HarmonyLib;
using ProjectM.UI;
using UnityEngine;

namespace ThirdPersonCamera.Patches
{
    [HarmonyPatch]
    public class EscapeMenuView_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EscapeMenuView), nameof(EscapeMenuView.OnEnable))]
        public static void OnEnable(EscapeMenuView __instance)
        {
            ThirdPersonCamera.isMenuOpen = true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(EscapeMenuView), nameof(EscapeMenuView.OnDestroy))]
        public static void OnDestroy(EscapeMenuView __instance)
        {
            ThirdPersonCamera.isMenuOpen = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EscapeMenuView), nameof(EscapeMenuView.OnButtonClick_LeaveGame))]
        public static void OnButtonClick_LeaveGame()
        {
            ThirdPersonCamera.isMenuOpen = false;
            ThirdPersonCamera.isCombatModeActive = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
