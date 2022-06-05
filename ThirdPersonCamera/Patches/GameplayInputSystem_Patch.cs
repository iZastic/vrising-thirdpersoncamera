using HarmonyLib;
using ProjectM;
using System;
using System.Timers;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ThirdPersonCamera
{
    [HarmonyPatch]
    public class GameplayInputSystem_Patch
    {
        private static float cursorLockDelay = 150;
        private static DateTime cursorLockStartTime;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameplayInputSystem), nameof(GameplayInputSystem.HandleInput))]
        public static void HandleInput(GameplayInputSystem __instance, ref InputState inputState)
        {
            // Reset cursor lock start time when rotate camera button is pressed
            if (inputState.IsInputDown(InputFlag.RotateCamera))
                cursorLockStartTime = DateTime.Now;

            ThirdPersonCamera.Update();
            ThirdPersonCamera.UpdateRotateCameraPressed(ref inputState);

            bool rotateCameraPressed = inputState.IsInputPressed(InputFlag.RotateCamera);
            if (!rotateCameraPressed || (DateTime.Now - cursorLockStartTime).TotalMilliseconds >= cursorLockDelay)
                ThirdPersonCamera.UpdateCursor(ref inputState);

            // If rotate camera is not pressed then return
            if (!rotateCameraPressed) return;

            try
            {
                // Get top down camera entity
                Entity camera = __instance.EntityManager.CreateEntityQuery(new ComponentType[] {
                    ComponentType.ReadOnly<LocalToWorld>(),
                    ComponentType.ReadOnly<TopdownCamera>()
                }).GetSingletonEntity();
                // Get character entity
                Entity character = EntitiesHelper.GetLocalCharacterEntity(__instance.EntityManager);

                // Get world postiion and rotation
                var characterLocation = __instance.EntityManager.GetComponentData<LocalToWorld>(character);
                var cameraLocation = __instance.EntityManager.GetComponentData<LocalToWorld>(camera);

                // Get distance from character location to mouse world postion for aiming
                var distance = math.distance(inputState.MouseWorldPosition, characterLocation.Position);
                // Get aim position from character position to the aim distance in the direction the camera is facing
                var target = characterLocation.Position + (cameraLocation.Forward * distance);
                // Reset y value
                target.y = inputState.MouseWorldPosition.y;

                inputState.MouseWorldPosition = target;
            }
            catch (Exception)
            {
                // HandleInput can be called before the world is fully loaded
                // This causes errors with getting the EntityManager
                // These errors can be ignored
            }
        }
    }
}
