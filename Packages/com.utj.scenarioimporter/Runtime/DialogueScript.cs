using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.ScenarioImporter
{
    /// <summary>
    /// ScriptableObject class for managing scenario conversations and events.
    /// This class stores different sections and messages within a scenario
    /// and provides functionality to retrieve specific scenario sections based on tags.
    /// </summary>
    [RenamedFrom("Scenario.ScenarioData")]
    [RenamedFrom("Utj.ScenarioImporter.DialogueScript")]
    public class DialogueScript : ScriptableObject
    {
        /// <summary>
        /// List of DialogueEntry
        /// </summary>
        [FormerlySerializedAs("Messages")] 
        [RenamedFrom("Messages")]
        public List<DialogueEntry> DialogueEntries = new();

        /// <summary>
        /// Get the range of a scenario
        /// </summary>
        /// <param name="tag">Tag</param>
        /// <param name="start">Start point</param>
        /// <param name="end">End point</param>
        public void GetSliceIndex(string tag, out int start, out int end)
        {
            var size = DialogueEntries.Count;
            start = DialogueEntries.FindIndex(0, c => c.Tag == tag);
            end = DialogueEntries.FindIndex(start + 1, c => string.IsNullOrEmpty(c.Tag) == false) ;
            if (end < 0)
                end = DialogueEntries.Count;
        }

        /// <summary>
        /// Get dialogue entries with a specified tag
        /// </summary>
        /// <param name="tag">Tag</param>
        /// <returns>List of messages within the scenario</returns>
        [RenamedFrom("GetSlicedScenario")]
        [RenamedFrom("GetMessages")]
        public List<DialogueEntry> GetDialogueEntriesWithTag(string tag)
        {
            GetSliceIndex(tag, out var start, out var end);
            return DialogueEntries.GetRange(start, end - start);
        }
    }
}
