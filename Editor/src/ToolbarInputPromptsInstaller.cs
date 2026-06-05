using UnityEditor;
using UnityEditor.Toolbars;

namespace InputPrompts.Editor
{
    /// <summary>
    /// Adds an "Input Prompts" button to the main toolbar that opens the installer
    /// window. Mirrors the project's existing ToolbarCreateFeature pattern.
    /// </summary>
    public class ToolbarInputPromptsInstaller : EditorWindow
    {
        [MainToolbarElement("Input Prompts", defaultDockPosition = MainToolbarDockPosition.Left)]
        public static MainToolbarButton ShowButton()
        {
            MainToolbarContent content = new MainToolbarContent(
                "Input Prompts", "Install the Input Prompts feature");

            return new MainToolbarButton(content, OpenWindow);
        }

        private static void OpenWindow()
        {
            InputPromptsInstaller.ShowWindow();
        }
    }
}