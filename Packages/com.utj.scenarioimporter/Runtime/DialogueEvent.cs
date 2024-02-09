using Unity.VisualScripting;

namespace Unity.ScenarioImporter
{
    /// <summary>
    /// Struct representing a specific event that occurs during the scenario.
    /// This includes the event's name and associated arguments.
    /// This struct is used to add dynamic elements to dialogues and scenarios.
    /// </summary>
    [System.Serializable]
    [RenamedFrom("Scenario.MessageEvent")]
    [RenamedFrom("Utj.ScenarioImporter.DialogueEvent")]
    public struct DialogueEvent
    {
        /// <summary>
        /// Name of the event
        /// </summary>
        public string EventName;

        /// <summary>
        /// Arguments for the event
        /// </summary>
        public string[] EventArgs;
    }
}