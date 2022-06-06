using BepInEx.IL2CPP.Hook;
using ProjectM;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Wetstone.Util;

namespace ThirdPersonCamera
{
#nullable enable
    public class TopdownCameraSystem_Patch
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void UpdateCameraInputs(IntPtr _this, TopdownCamera* cameraData, bool freezeMouseInputs, TopdownCameraState* cameraState);
        private static UpdateCameraInputs? UpdateCameraInputsOriginal;
        private static FastNativeDetour? UpdateCameraInputsDetour;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void HandleInput(IntPtr _this, InputState* inputState);
        private static HandleInput? HandleInputOriginal;
        private static FastNativeDetour? HandleInputDetour;

        public static unsafe void RegisterHooks()
        {
            if (UpdateCameraInputsDetour == null)
            {
                UpdateCameraInputsDetour = NativeHookUtil.Detour(typeof(TopdownCameraSystem), "UpdateCameraInputs", UpdateCameraInputsHook, out UpdateCameraInputsOriginal);
            }

            if (HandleInputDetour == null)
            {
                HandleInputDetour = NativeHookUtil.Detour(typeof(TopdownCameraSystem), "HandleInput", HandleInputHook, out HandleInputOriginal);
            }
        }

        public static void UnRegisterHooks()
        {
            if (UpdateCameraInputsDetour != null)
            {
                UpdateCameraInputsDetour.Dispose();
                UpdateCameraInputsDetour = null;
            }

            if (HandleInputDetour != null)
            {
                HandleInputDetour.Dispose();
                HandleInputDetour = null;
            }
        }

        public static unsafe void HandleInputHook(IntPtr _this, InputState* inputState)
        {
            ThirdPersonCamera.UpdateRotateCameraPressed(ref *inputState);
            HandleInputOriginal!(_this, inputState);
        }

        private static unsafe void UpdateCameraInputsHook(IntPtr _this, TopdownCamera* cameraData, bool freezeMouseInputs, TopdownCameraState* cameraState)
        {
            // Force updating the current camera states zoom settings
            cameraState->ZoomSettings.MaxPitch = Plugin.cameraMaxPitch.Value;
            cameraState->ZoomSettings.MinPitch = Plugin.cameraMinPitch.Value;
            cameraState->ZoomSettings.MaxZoom = Plugin.cameraMaxZoom.Value;
            cameraState->ZoomSettings.MinZoom = Plugin.cameraMinZoom.Value;

            // Update camera standard zoom settings to equal new zoom settings
            cameraData->StandardZoomSettings = cameraState->ZoomSettings;

            // Update look at offset to view over the shoulder when zoomed in
            var behindCharacter = cameraState->Current.Zoom > 0;
            var lookAtLerp = (cameraState->Current.Zoom - cameraState->ZoomSettings.MinZoom) / (cameraState->ZoomSettings.MaxZoom - cameraState->ZoomSettings.MinZoom);
            var lookOffsetX = behindCharacter ? Mathf.Lerp(Plugin.lookAtOffsetX.Value, 0f, lookAtLerp) : 0f;
            var lookOffsetY = behindCharacter ? Mathf.Lerp(Plugin.lookAtOffsetY.Value, 0f, lookAtLerp) : 0.8f;

            cameraState->LastTarget.NormalizedLookAtOffset.x = lookOffsetX * 2;
            cameraState->LastTarget.NormalizedLookAtOffset.y = lookOffsetY * 2;

            UpdateCameraInputsOriginal!(_this, cameraData, freezeMouseInputs, cameraState);
        }
    }
}
