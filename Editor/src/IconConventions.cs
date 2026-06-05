using InputPrompts.Runtime;

namespace InputPrompts.Editor
{
    /// <summary>
    /// Shared Kenney naming conventions: controlPath -> ordered candidate sprite names.
    /// Single source of truth, used by both the IconSet inspector and the installer.
    /// Edit freely (rename a candidate if your sprites use different names).
    /// </summary>
    public static class IconConventions
    {
        #region Public API

        public static (string control, string[] candidates)[] ForFamily(DeviceFamily family)
        {
            switch (family)
            {
                case DeviceFamily.Xbox:
                    return new (string, string[])[]
                    {
                        ("buttonSouth",     new[] { "xbox_button_color_a" }),
                        ("buttonEast",      new[] { "xbox_button_color_b" }),
                        ("buttonWest",      new[] { "xbox_button_color_x" }),
                        ("buttonNorth",     new[] { "xbox_button_color_y" }),
                        ("start",           new[] { "xbox_button_menu", "xbox_button_start" }),
                        ("select",          new[] { "xbox_button_view", "xbox_button_back" }),
                        ("leftShoulder",    new[] { "xbox_lb" }),
                        ("rightShoulder",   new[] { "xbox_rb" }),
                        ("leftTrigger",     new[] { "xbox_lt" }),
                        ("rightTrigger",    new[] { "xbox_rt" }),
                        ("leftStickPress",  new[] { "xbox_stick_l_press", "xbox_stick_l" }),
                        ("rightStickPress", new[] { "xbox_stick_r_press", "xbox_stick_r" }),
                        ("leftStick",       new[] { "xbox_stick_l" }),
                        ("rightStick",      new[] { "xbox_stick_r" }),
                        ("dpad/up",         new[] { "xbox_dpad_up" }),
                        ("dpad/down",       new[] { "xbox_dpad_down" }),
                        ("dpad/left",       new[] { "xbox_dpad_left" }),
                        ("dpad/right",      new[] { "xbox_dpad_right" }),
                    };

                case DeviceFamily.PlayStation:
                    return new (string, string[])[]
                    {
                        ("buttonSouth",     new[] { "playstation_button_color_cross" }),
                        ("buttonEast",      new[] { "playstation_button_color_circle" }),
                        ("buttonWest",      new[] { "playstation_button_color_square" }),
                        ("buttonNorth",     new[] { "playstation_button_color_triangle" }),
                        ("start",           new[] { "playstation_button_options" }),
                        ("select",          new[] { "playstation_button_create", "playstation_button_share" }),
                        ("leftShoulder",    new[] { "playstation_trigger_l1" }),
                        ("rightShoulder",   new[] { "playstation_trigger_r1" }),
                        ("leftTrigger",     new[] { "playstation_trigger_l2" }),
                        ("rightTrigger",    new[] { "playstation_trigger_r2" }),
                        ("leftStickPress",  new[] { "playstation_stick_l_press", "playstation_stick_l" }),
                        ("rightStickPress", new[] { "playstation_stick_r_press", "playstation_stick_r" }),
                        ("leftStick",       new[] { "playstation_stick_l" }),
                        ("rightStick",      new[] { "playstation_stick_r" }),
                        ("dpad/up",         new[] { "playstation_dpad_up" }),
                        ("dpad/down",       new[] { "playstation_dpad_down" }),
                        ("dpad/left",       new[] { "playstation_dpad_left" }),
                        ("dpad/right",      new[] { "playstation_dpad_right" }),
                    };

                case DeviceFamily.KeyboardMouse:
                    return new (string, string[])[]
                    {
                        ("space",        new[] { "keyboard_space" }),
                        ("enter",        new[] { "keyboard_enter", "keyboard_return" }),
                        ("escape",       new[] { "keyboard_escape" }),
                        ("leftShift",    new[] { "keyboard_shift", "keyboard_shift_l" }),
                        ("leftCtrl",     new[] { "keyboard_ctrl", "keyboard_control" }),
                        ("tab",          new[] { "keyboard_tab" }),
                        ("w",            new[] { "keyboard_w" }),
                        ("a",            new[] { "keyboard_a" }),
                        ("s",            new[] { "keyboard_s" }),
                        ("d",            new[] { "keyboard_d" }),
                        ("e",            new[] { "keyboard_e" }),
                        ("q",            new[] { "keyboard_q" }),
                        ("f",            new[] { "keyboard_f" }),
                        ("c",            new[] { "keyboard_c" }),
                        ("leftButton",   new[] { "mouse_left" }),
                        ("rightButton",  new[] { "mouse_right" }),
                        ("middleButton", new[] { "mouse_middle" }),
                    };

                default:
                    return null;
            }
        }

        #endregion
    }
}