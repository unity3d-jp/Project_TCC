using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UTJ
{
    public static class GUIElements
    {
        public const float RowHeight = 30f;
        public const float Spacing = 8f;

        // GUIStyles often die when toggling play, etc. so don't store them in the elements        

        public interface IElement
        {
            float Height { get; set; }
            void DoUI(float x, float y, float width);
        }

        public class Separator : IElement
        {
            public Separator(float height = Spacing)
            {
                Height = height;
            }

            public float Height { get; set; }

            public void DoUI(float x, float y, float width)
            {
                // Do nothing
            }
        }

        public class Label : IElement
        {
            public Label(string caption, System.Func<GUIStyle> styleProvider = null, float height = RowHeight)
            {
                Caption = caption;
                StyleProvider = styleProvider;
                Height = height;
            }

            public float Height { get; set; }
            public string Caption { get; set; }
            public System.Func<GUIStyle> StyleProvider { get; set; }

            public void DoUI(float x, float y, float width)
            {
                var rect = new Rect(x, y, width, Height);
                if (StyleProvider != null)
                {
                    GUI.Label(rect, Caption, StyleProvider()); // EditorGUIStyles.HeaderLabelStyle);
                }
                else
                {
                    GUI.Label(rect, Caption);
                }
            }
        }

        public class Button : IElement
        {
            public Button
            (
                string caption,
                System.Action onPress,
                Texture iconTexture = null,
                System.Func<GUIStyle> captionStyleProvider = null,
                float height = RowHeight
            )
            {
                Caption = caption;
                OnPress = onPress;
                IconTexture = iconTexture;
                CaptionStyleProvider = captionStyleProvider;
                Height = height;
            }

            public float Height { get; set; }
            public string Caption { get; set; }
            public System.Action OnPress { get; set; }
            public Texture IconTexture { get; set; }
            public System.Func<GUIStyle> CaptionStyleProvider { get; set; }

            public void DoUI(float x, float y, float width)
            {
                var rect = new Rect(x, y, width, Height);
                if (GUI.Button(rect, "")) { OnPress(); }
                var iconSize = Mathf.RoundToInt(rect.height - Spacing * 2);
                DrawIcon(rect, IconTexture, iconSize);
                var labelOffset = iconSize + Spacing * 1.5f;
                var labelRect = new Rect(rect.x + labelOffset, rect.y, rect.width - labelOffset - 2f, rect.height);
                if (CaptionStyleProvider != null)
                {
                    GUI.Label(labelRect, Caption, CaptionStyleProvider()); // EditorGUIStyles.MiddleLeftJustifiedLabelStyle);
                }
                else
                {
                    GUI.Label(labelRect, Caption);
                }
            }

            private static void DrawIcon(Rect buttonRect, Texture iconTexture, int maxIconSize = 16)
            {
                if (iconTexture == null) { return; }

                const int IconPadding = 4;

                var iconSize = Mathf.Min(buttonRect.height - IconPadding * 2, maxIconSize);
                var iconOffset = (buttonRect.height - iconSize) / 2;
                var x = buttonRect.x + iconOffset;
                var iconRect = new Rect(x, buttonRect.y + iconOffset, iconSize, iconSize);
                GUI.DrawTexture(iconRect, iconTexture);
            }
        }

        public class Toggle : IElement
        {
            public Toggle
            (
                string label,
                System.Func<bool> getCurrentValue,
                System.Action<bool> onChange,
                System.Func<GUIStyle> styleProvider = null,
                float height = RowHeight
            )
            {
                Label = label;
                GetCurrentValue = getCurrentValue;
                OnChange = onChange;
                StyleProvider = styleProvider;
                Height = height;
            }

            public float Height { get; set; }
            public string Label { get; set; }
            public System.Func<bool> GetCurrentValue { get; set; }
            public System.Action<bool> OnChange { get; set; }
            public System.Func<GUIStyle> StyleProvider { get; set; }

            public void DoUI(float x, float y, float width)
            {
                var rect = new Rect(x, y, width, Height);
                var currentValue = GetCurrentValue();
                var newValue = currentValue;
                if (StyleProvider != null)
                {
                    newValue = GUI.Toggle(rect, currentValue, " " + Label, StyleProvider()); // EditorGUIStyles.ToggleStyle);
                }
                else
                {
                    newValue = GUI.Toggle(rect, currentValue, " " + Label);
                }

                var valueChanged = newValue != currentValue;
                if (valueChanged) { OnChange(newValue); }
            }
        }

        public class Row : IElement
        {
            public Row(IEnumerable<IElement> children, float rowHeight = RowHeight)
            {
                Elements = children.ToArray();
                Height = rowHeight;
            }

            public IElement[] Elements { get; set; }
            public float Height { get; set; }

            public void DoUI(float x, float y, float width)
            {
                var elementCount = Elements.Length;
                var elementWidth = GetAutoElementSize(width, Spacing, elementCount);
                var height = Height;
                for (int buttonIndex = 0; buttonIndex < elementCount; buttonIndex++)
                {
                    Elements[buttonIndex].Height = height;
                    Elements[buttonIndex].DoUI(x, y, elementWidth);
                    x += elementWidth + Spacing;
                }
            }

            private static float GetAutoElementSize
            (
                float containerSize,
                float spacing,
                int itemCount
            )
            {
                if (itemCount < 1) { return 0; }
                return (containerSize - spacing * (itemCount - 1)) / itemCount;
            }
        }

        public class Column : IElement
        {
            public Column
            (
                IEnumerable<IElement> children, 
                bool showBackground = true, 
                float padding = 4f, 
                float rowSpacing = Spacing
            )
            {
                Elements = children.ToArray();
                ShowBackground = showBackground;
                Padding = padding;
                RowSpacing = rowSpacing;
            }

            public IElement[] Elements { get; set; }
            public bool ShowBackground { get; set; }
            public float Padding { get; set; }
            public float RowSpacing { get; set; }
            public float Height
            {
                get
                {
                    return Elements.Sum(item => item.Height) 
                        + RowSpacing * (Elements.Length - 1)
                        + Padding * 2f;
                }
                set { /* Do nothing; depends on children */ }
            }

            public void DoUI(float x, float y, float width)
            {
                if (ShowBackground)
                {
                    var backgroundRect = new Rect(x, y, width, Height);
                    GUI.Box(backgroundRect, "");
                }

                var elementY = y + Padding;
                var elementX = x + Padding;
                var elementWidth = width - Padding * 2f;
                var elementCount = Elements.Length;
                for (int elementIndex = 0; elementIndex < elementCount; elementIndex++)
                {
                    Elements[elementIndex].DoUI(elementX, elementY, elementWidth);
                    elementY += Elements[elementIndex].Height + RowSpacing;
                }
            }
        }
    }
}