using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.ScenarioImporter;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Unity.ScenarioImporter
{
    /// <summary>
    /// Provides a custom editor for the ScenarioData object.
    /// Displays scenario data in the Inspector.
    /// </summary>
    [CustomEditor(typeof(DialogueScript))]
    public class ScenarioDataEditor : Editor
    {
        /// <summary>
        /// Override the Inspector GUI to display scenario messages.
        /// </summary>
        public override void OnInspectorGUI()
        {
            foreach (var o in targets)
            {
                var scenario = (DialogueScript)o;
                DisplayScenarioMessages(scenario);
            }
        }

        /// <summary>
        /// Retrieve messages from scenario data and display them.
        /// </summary>
        /// <param name="scenario">Scenario data to display</param>
        private void DisplayScenarioMessages(DialogueScript scenario)
        {
            var style = CreateBoldLabelStyle();
            foreach (var message in scenario.DialogueEntries)
            {
                DisplayMessage(message, style);
            }
        }

        /// <summary>
        /// Create a bold label style.
        /// </summary>
        /// <returns>GUIStyle for bold labels</returns>
        private GUIStyle CreateBoldLabelStyle()
        {
            return new GUIStyle
            {
                fontStyle = FontStyle.Bold
            };
        }

        /// <summary>
        /// Display scenario messages using the given message and style.
        /// </summary>
        /// <param name="dialogueEntry">Message to display</param>
        /// <param name="style">GUI style to use</param>
        private void DisplayMessage(DialogueEntry dialogueEntry, GUIStyle style)
        {
            EditorGUI.indentLevel = 0;
            if (!string.IsNullOrEmpty(dialogueEntry.Tag))
            {
                EditorGUILayout.LabelField(dialogueEntry.Tag, style);
            }
            EditorGUILayout.TextArea(dialogueEntry.Text);

            DisplayMessageEvents(dialogueEntry);
        }

        /// <summary>
        /// Display events related to a message.
        /// </summary>
        /// <param name="dialogueEntry">Message to display events for</param>
        private void DisplayMessageEvents(DialogueEntry dialogueEntry)
        {
            EditorGUI.indentLevel = 1;
            foreach (var ev in dialogueEntry.Events)
            {
                DisplayEvent(ev);
            }
        }

        /// <summary>
        /// Display a scenario event.
        /// </summary>
        /// <param name="ev">Event to display</param>
        private void DisplayEvent(DialogueEvent ev)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.TextField(ev.EventName, GUILayout.Width(100));
                var eventArgsString = BuildEventArgsString(ev.EventArgs);
                EditorGUILayout.LabelField(eventArgsString);
            }
        }

        /// <summary>
        /// Build a string for event arguments.
        /// </summary>
        /// <param name="eventArgs">Array of event arguments</param>
        /// <returns>Constructed event arguments string</returns>
        private string BuildEventArgsString(in string[] eventArgs)
        {
            var builder = new StringBuilder();
            foreach (var arg in eventArgs)
            {
                builder.AppendFormat("  {0},", arg);
            }
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 1, 1);
            }
            return builder.ToString();
        }
    }
}
