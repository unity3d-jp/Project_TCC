using UTJ.GameObjectExtensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTJ
{
    public class FindGameObjectsWindow : EditorWindow
    {
        [MenuItem("UTJ/名前で選択窓")]
        public static void ShowWindow()
        {
            var window = GetWindow<FindGameObjectsWindow>("名前で選択");
            window.OnShow();
        }

        public static IEnumerable<GameObject> FindObjectsByComponent
        (
            string componentPattern, 
            IEnumerable<string> ignorePatterns = null
        )
        {
            var gameObjects = GameObjectUtil.GetAllGameObjects();
            var matchingObjects = FindObjectsWithComponentByPattern(gameObjects, componentPattern);
            if (ignorePatterns != null)
            {
                foreach (var ignorePattern in ignorePatterns)
                {
                    matchingObjects = RemoveObjectsByPattern(matchingObjects, ignorePattern);
                }
            }
            return matchingObjects;
        }

        public static IEnumerable<GameObject> FindObjectsByPattern
        (
            IEnumerable<string> namePatterns, 
            IEnumerable<string> ignorePatterns,
            string componentPattern
        )
        {
            if (namePatterns == null || !namePatterns.Any())
            {
                return !System.String.IsNullOrEmpty(componentPattern)
                    ? FindObjectsByComponent(componentPattern, ignorePatterns)
                    : new List<GameObject>();
            }

            // The first pattern is required
            // If there any patterns after the first, then at least one must match
            var requiredPattern = "*" + namePatterns.First() + "*";
            var additionalPatterns = namePatterns.Skip(1)
                .Select(pattern => "*" + pattern + "*")
                .ToArray();

            System.Func<string, bool> isNameAMatch = (name) =>
            {
                return StringUtil.GlobMatch(name, requiredPattern)
                    && (additionalPatterns.Length == 0
                        || additionalPatterns.Any(pattern => StringUtil.GlobMatch(name, pattern)));
            };

            var matchingObjects = GameObjectUtil.GetAllGameObjects()
                .Where(gameObject => isNameAMatch(gameObject.name));

            if (ignorePatterns != null)
            {
                foreach (var ignorePattern in ignorePatterns)
                {
                    matchingObjects = RemoveObjectsByPattern(matchingObjects, ignorePattern);
                }
            }

            if (!System.String.IsNullOrEmpty(componentPattern))
            {
                matchingObjects = FindObjectsWithComponentByPattern(matchingObjects, componentPattern);
            }

            return matchingObjects;
        }

        // private

        private const int SearchFrameCount = 2;
        private const string SearchNameField = "SearchNameField";

        private string objectPattern = "";
        private string ignorePattern = "";
        private string componentPattern = "";
        private int searchCountdown;
        private bool isInitialShow;

        private static bool HasExactComponentMatch(GameObject gameObject, string componentTypeToFind)
        {
            var componentTypes = gameObject.GetComponents<Component>()
                .Where(component => component != null)
                .Select(component => component.GetType().ToString().ToLowerInvariant());
            var dottedComponentTypeToFind = "." + componentTypeToFind;
            return componentTypes.Any(type => 
                type == componentTypeToFind
                || type.EndsWith(dottedComponentTypeToFind));
        }

        private static IEnumerable<GameObject> FindObjectsWithComponentByPattern
        (
            IEnumerable<GameObject> sourceObjects,
            string componentPattern
        )
        {
            // Remove all whitespace
            string[] whitespaceList = { " ", "\t", "　", "\r", "\n" };
            foreach (var whitespaceString in whitespaceList)
            {
                componentPattern = componentPattern.Replace(whitespaceString, "");
            }

            // For components, if there are any objects whose components match exactly (whole word),
            // then only return those
            componentPattern = componentPattern.ToLowerInvariant();
            var exactMatches = sourceObjects.Where(gameObject => 
                HasExactComponentMatch(gameObject, componentPattern));
            if (exactMatches.Any())
            {
                return exactMatches;
            }

            // Otherwise do a find-anywhere search
            componentPattern = "*" + componentPattern + "*";
            return sourceObjects
                .Where(gameObject => gameObject.GetComponents<Component>().Any(
                    component => component != null && StringUtil.GlobMatch(component.GetType().ToString(), componentPattern)));
        }

        private static IEnumerable<GameObject> RemoveObjectsByPattern
        (
            IEnumerable<GameObject> sourceObjects,
            string removePattern
        )
        {
            removePattern = "*" + removePattern + "*";
            return sourceObjects
                .Where(gameObject => !StringUtil.GlobMatch(gameObject.name, removePattern));
        }
        
        private void ReacquireData()
        {
            if (objectPattern == null) { objectPattern = ""; }
            if (ignorePattern == null) { ignorePattern = ""; }
            if (componentPattern == null) { componentPattern = ""; }
        }

        private void FindAndSelectObjects()
        {
            const string WhitespaceString = " \t\r\n　";
            var whitespaceList = WhitespaceString.ToCharArray();

            var objectPatterns = objectPattern.Split(whitespaceList, System.StringSplitOptions.RemoveEmptyEntries);
            var ignorePatterns = ignorePattern.Split(whitespaceList, System.StringSplitOptions.RemoveEmptyEntries);
            Selection.objects = FindObjectsByPattern(objectPatterns, ignorePatterns, componentPattern).ToArray();
            searchCountdown = 0;
        }

        private void RestartSearchCountdown()
        {
            searchCountdown = SearchFrameCount;
        }

        private static void SetRectYPositions(IList<Rect> rects, float yPosition)
        {
            for (var rectIndex = 0; rectIndex < rects.Count; ++rectIndex)
            {
                var rect = rects[rectIndex];
                rect.y = yPosition;
                rects[rectIndex] = rect;
            }
        }

        private static void DoTextFieldWithClearButton
        (
            ref TextFieldPositions fieldPositions,
            string label,
            ref string textItem,
            System.Action onTextChanged,
            string textFieldControlName = null
        )
        {
            GUI.Label(fieldPositions.GetLabelRect(), label);

            if (textFieldControlName != null)
            {
                GUI.SetNextControlName(textFieldControlName);
            }

            var newText = GUI.TextField(fieldPositions.GetFieldRect(), textItem);
            if (newText != textItem)
            {
                textItem = newText;
                onTextChanged();
            }

            if (GUI.Button(fieldPositions.GetButtonRect(), "×") && textItem.Length > 0)
            {
                textItem = "";
                onTextChanged();
            }
        }

        private void OnSelectionChange()
        {
            Repaint();
        }

        private void OnInspectorUpdate()
        {
            if (searchCountdown > 0)
            {
                searchCountdown--;
                if (searchCountdown == 0)
                {
                    FindAndSelectObjects();
                }
            }
        }

        private static Rect[] GetRowRects(float x, float y, IList<float> widths, float height, float spacing)
        {
            var rectCount = widths.Count;
            var rects = new Rect[rectCount];
            for (int rectIndex = 0; rectIndex < rectCount; rectIndex++)
            {
                rects[rectIndex] = new Rect(x, y, widths[rectIndex], height);
                x += widths[rectIndex] + spacing;
            }
            return rects;
        }

        private struct TextFieldPositions
        {
            public float x;
            public float y;
            public float labelWidth;
            public float fieldWidth;
            public float buttonWidth;
            public float height;
            public float spacing;

            public Rect GetLabelRect() { return new Rect(x, y, labelWidth, height); }
            public Rect GetFieldRect() { return new Rect(x + labelWidth + spacing, y, fieldWidth, height); }
            public Rect GetButtonRect() { return new Rect(x + labelWidth + fieldWidth + spacing * 2f, y, buttonWidth, height); }
        }

        private static TextFieldPositions GetTextFieldPositions(float x, float y, float rowWidth, float rowHeight, float spacing)
        {
            const int LabelMinWidth = 40;
            const int LabelMaxWidth = 120;
            const float ButtonWidth = 20f;
            var labelWidth = Mathf.Clamp(rowWidth * 0.25f, LabelMinWidth, LabelMaxWidth);
            return new TextFieldPositions
            {
                x = x,
                y = y,
                labelWidth = labelWidth,
                fieldWidth = rowWidth - labelWidth - ButtonWidth - spacing * 2f,
                buttonWidth = ButtonWidth,
                height = rowHeight,
                spacing = spacing
            };
        }

        private void OnGUI()
        {
            ReacquireData();

            const float RowHeight = 30f;
            const float Spacing = 8f;
            const float RowOffset = RowHeight + Spacing;

            var rowWidth = position.width - Spacing * 2f;
            var fieldPositions = GetTextFieldPositions(Spacing, Spacing, rowWidth, RowHeight, Spacing);
            DoTextFieldWithClearButton(ref fieldPositions, "名前", ref objectPattern, RestartSearchCountdown, SearchNameField);
            fieldPositions.y += RowOffset;
            DoTextFieldWithClearButton(ref fieldPositions, "無視", ref ignorePattern, RestartSearchCountdown);
            fieldPositions.y += RowOffset;
            DoTextFieldWithClearButton(ref fieldPositions, "コンポーネント", ref componentPattern, RestartSearchCountdown);
            fieldPositions.y += RowOffset;

            var rowRect = new Rect(fieldPositions.x, fieldPositions.y, rowWidth, RowHeight);
            if (GUI.Button(rowRect, "選択")) { FindAndSelectObjects(); }

            rowRect.y += RowOffset;
            var message = Selection.objects.Length.ToString() + " 個のオブジェクトを選択中";
            GUI.Label(rowRect, message);

            if (isInitialShow)
            {
                EditorGUI.FocusTextInControl(SearchNameField);
                isInitialShow = false;
            }
        }

        private void OnShow()
        {
            searchCountdown = 0;
            isInitialShow = true;
        }
    }
}
