using ProjectM;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Wetstone.API;

namespace ThirdPersonCamera
{
    public static class ThirdPersonCamera
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        public static bool isMenuOpen;
        public static bool isCombatModeActive;

        private static Keybinding keybinding;
        private static IntPtr gameHandle;

        public static void Initialize()
        {
            keybinding = KeybindManager.Register(new()
            {
                Id = "izastic.combatmode",
                Category = "Third Person Camera",
                Name = "Combat Mode",
                DefaultKeybinding = KeyCode.BackQuote
            });

            gameHandle = FindWindow(null, "VRising");
        }

        public static void Uninitialize()
        {
            KeybindManager.Unregister(keybinding);
        }

        public static void Update()
        {
            if (keybinding.IsPressed && !isMenuOpen && !isCombatModeActive)
                isCombatModeActive = true;
            else if (keybinding.IsPressed && isCombatModeActive)
                isCombatModeActive = false;
        }

        public static void UpdateRotateCameraPressed(ref InputState inputState)
        {
            if (!isMenuOpen && isCombatModeActive && !inputState.IsInputPressed(InputFlag.RotateCamera))
                inputState.InputsPressed |= InputFlag.RotateCamera;
        }

        public static void UpdateCursorVisible(ref InputState inputState)
        {
            bool rotateCameraPressed = inputState.IsInputPressed(InputFlag.RotateCamera);
            bool visible = isMenuOpen || (!isCombatModeActive && !rotateCameraPressed);

            Cursor.visible = visible;
        }

        public static void UpdateCursorPosition()
        {
            // Get the window bounds so that the cursor position is set correctly if in windowed mode
            Rect rect = new Rect();
            GetWindowRect(gameHandle, ref rect);

            SetCursorPos(rect.Left + Screen.width / 2 + Plugin.aimOffsetX.Value, rect.Top + Screen.height / 2 - Plugin.aimOffsetY.Value);
        }
    }
}
