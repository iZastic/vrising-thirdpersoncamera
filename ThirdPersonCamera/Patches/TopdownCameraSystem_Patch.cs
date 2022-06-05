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
            cameraState->ZoomSettings.MaxPitch = 1.57f;
            cameraState->ZoomSettings.MinPitch = 0.17f;
            cameraState->ZoomSettings.MaxZoom = 15f;
            cameraState->ZoomSettings.MinZoom = 2f;

            // Update camera standard zoom settings to equal new zoom settings
            cameraData->StandardZoomSettings = cameraState->ZoomSettings;

            // Update look at offset to view over the shoulder when zoomed in
            var zoomed = cameraState->Current.Zoom > 0;
            var lookAtLerp = zoomed ? cameraState->Current.Zoom / cameraState->ZoomSettings.MaxZoom : 0;
            cameraState->Current.NormalizedLookAtOffset.y = zoomed ? Mathf.Lerp(1f, 0f, lookAtLerp) : 0;
            cameraState->Current.NormalizedLookAtOffset.x = zoomed ? Mathf.Lerp(0.3f, 0f, lookAtLerp) : 0;

            UpdateCameraInputsOriginal!(_this, cameraData, freezeMouseInputs, cameraState);
        }
    }
}
