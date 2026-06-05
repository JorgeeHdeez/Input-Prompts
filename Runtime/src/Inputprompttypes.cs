using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.Switch;

namespace InputPrompts.Runtime
{
    /// <summary>
    /// Device family, used to pick the matching icon set (InputIconSet).
    /// </summary>
    public enum DeviceFamily
    {
        Unknown = 0,
        KeyboardMouse = 1,
        GenericGamepad = 2,
        Xbox = 3,
        PlayStation = 4,
        Switch = 5,
    }


    /// <summary>
    /// Result of an icon resolution: a sprite, and/or a text fallback
    /// (e.g. "Space", "W/A/S/D") used when no sprite is found.
    /// </summary>
    public readonly struct InputIconResult
    {
        public InputIconResult(Sprite sprite, string fallback)
        {
            Sprite = sprite;
            Fallback = fallback;
        }

        public readonly Sprite Sprite;

        public readonly string Fallback;

        public bool HasSprite => Sprite != null;
    }


    /// <summary>
    /// Pure (stateless) classification of a device into a family.
    /// No mutable static state: compliant with Power of 10 / NASA JPL rules
    /// (the rule bans global mutable state, not pure functions).
    /// Classification is done by TYPE (DualSense derives from DualShockGamepad,
    /// etc.), which is reliable, unlike comparing layout name strings.
    /// </summary>
    public static class InputDeviceFamilies
    {
        #region Public API

        public static DeviceFamily Classify(InputDevice device)
        {
            if (device == null) return DeviceFamily.Unknown;

            if (device is Keyboard || device is Mouse) return DeviceFamily.KeyboardMouse;

            if (device is Gamepad)
            {
                if (device is DualShockGamepad) return DeviceFamily.PlayStation;
                if (device is XInputController) return DeviceFamily.Xbox;
                if (device is SwitchProControllerHID) return DeviceFamily.Switch;

                return DeviceFamily.GenericGamepad;
            }

            return DeviceFamily.Unknown;
        }

        // True for any gamepad, regardless of brand.
        public static bool IsGamepad(DeviceFamily family)
        {
            return family == DeviceFamily.GenericGamepad
                || family == DeviceFamily.Xbox
                || family == DeviceFamily.PlayStation
                || family == DeviceFamily.Switch;
        }

        #endregion
    }
}