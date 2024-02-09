using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Unity.ScenarioImporter
{
    /// <summary>
    /// Class representing a single dialogue entry in the game.
    /// This includes the text of the dialogue, related events, and the tag it belongs to.
    /// This class is used to represent the flow of scenarios and dialogues.
    /// </summary>
    [System.Serializable]
    [RenamedFrom("Scenario.Message")]
    [RenamedFrom("Utj.ScenarioImporter.DialogueEntry")]
    public class DialogueEntry
    {
        /// <summary>
        /// Text content of the message
        /// </summary>
        [Multiline]
        public string Text;

        /// <summary>
        /// Events stored within the message
        /// </summary>
        public List<DialogueEvent> Events = new();

        /// <summary>
        /// Tag to which the message belongs
        /// </summary>
        public string Tag;
    }
}