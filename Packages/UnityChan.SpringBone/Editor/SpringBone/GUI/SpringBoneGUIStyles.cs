using UnityEngine;

namespace UTJ
{
    public static class SpringBoneGUIStyles
    {
        public static GUIStyle LabelStyle { get; private set; }
        public static GUIStyle HeaderLabelStyle { get; private set; }
        public static GUIStyle ButtonStyle { get; private set; }
        public static GUIStyle MiddleLeftJustifiedLabelStyle { get; private set; }
        public static GUIStyle ToggleStyle { get; private set; }

        public static void ReacquireStyles()
        {
            if (LabelStyle == null)
            {
                LabelStyle = new GUIStyle(GUI.skin.label);
                HeaderLabelStyle = new GUIStyle(UnityEditor.EditorStyles.boldLabel);
                ButtonStyle = new GUIStyle(GUI.skin.button);
                MiddleLeftJustifiedLabelStyle = new GUIStyle(GUI.skin.label);
                ToggleStyle = new GUIStyle(GUI.skin.toggle);
            }

            const int FontSize = 14;
            LabelStyle.fontSize = FontSize;
            HeaderLabelStyle.fontSize = FontSize;
            ButtonStyle.fontSize = FontSize;
            MiddleLeftJustifiedLabelStyle.fontSize = FontSize;
            ToggleStyle.fontSize = FontSize;

            HeaderLabelStyle.alignment = TextAnchor.MiddleCenter;
            MiddleLeftJustifiedLabelStyle.alignment = TextAnchor.MiddleLeft;
        }
    }
}