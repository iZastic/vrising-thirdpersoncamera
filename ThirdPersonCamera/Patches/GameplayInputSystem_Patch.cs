using HarmonyLib;
using ProjectM;
using System;

namespace ThirdPersonCamera
{
    [HarmonyPatch]
    public class GameplayInputSystem_Patch
    {
        private static DateTime cursorLockStartTime;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameplayInputSystem), nameof(GameplayInputSystem.HandleInput))]
        public static void HandleInput(GameplayInputSystem __instance, ref InputState inputState)
        {
            // Reset cursor lock start time when rotate camera button is pressed
            if (inputState.IsInputDown(InputFlag.RotateCamera))
                cursorLockStartTime = DateTime.Now;

            ThirdPersonCamera.Update();

            if (inputState.IsInputUp(InputFlag.ToggleActionWheel) || inputState.IsInputUp(InputFlag.ToggleEmoteWheel))
                ThirdPersonCamera.isMenuOpen = false;

            if (inputState.IsInputPressed(InputFlag.ToggleActionWheel) || inputState.IsInputPressed(InputFlag.ToggleEmoteWheel))
                ThirdPersonCamera.isMenuOpen = true;

            ThirdPersonCamera.UpdateRotateCameraPressed(ref inputState);

            bool rotateCameraPressed = inputState.IsInputPressed(InputFlag.RotateCamera);
            if (!rotateCameraPressed || (DateTime.Now - cursorLockStartTime).TotalMilliseconds >= Plugin.mouseLockDelay.Value)
                ThirdPersonCamera.UpdateCursorVisible(ref inputState);

            if (!rotateCameraPressed) return;
            ThirdPersonCamera.UpdateCursorPosition();
        }
    }
}
