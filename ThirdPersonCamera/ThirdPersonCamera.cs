using ProjectM;
using UnityEngine;
using Wetstone.API;

namespace ThirdPersonCamera
{
    public static class ThirdPersonCamera
    {
        public static bool isMenuOpen;
        public static bool isCombatModeActive;

        private static Keybinding keybinding;

        public static void Initialize()
        {
            keybinding = KeybindManager.Register(new()
            {
                Id = "izastic.combatmode",
                Category = "Third Person Camera",
                Name = "Combat Mode",
                DefaultKeybinding = KeyCode.BackQuote
            });
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

        public static void UpdateCursor(ref InputState inputState)
        {
            bool rotateCameraPressed = inputState.IsInputPressed(InputFlag.RotateCamera);
            bool visible = isMenuOpen || (!isCombatModeActive && !rotateCameraPressed);

            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = visible;
        }
    }
}
