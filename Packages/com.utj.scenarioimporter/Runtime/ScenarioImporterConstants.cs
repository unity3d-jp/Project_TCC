namespace Unity.ScenarioImporter
{

    /// <summary>
    /// Format for identifying scenarios
    /// </summary>
    public static class ScenarioImporterConstants
    {
        /// <summary>
        /// File extension that ScenarioImporter recognizes as Dialogue.
        /// For example, "MyScenario.scenario".
        /// </summary>
        public const string Extensions = "scenario";
        
        /// <summary>
        /// Comment out the text from "//" to the end of the line
        /// </summary>
        public const string Comment = "//";

        /// <summary>
        /// Character used to separate arguments
        /// </summary>
        public const string SplitArgument = ",";

        /// <summary>
        /// Tag used to split text. Note that this is not a regular expression.
        /// </summary>
        public const string SplitText = "---";

        /// <summary>
        /// Format for recognizing text as an event using regular expressions.
        /// name: Event name
        /// value: Arguments
        /// </summary>
        /// <example>
        /// @image character_1, smile
        /// Event Name: image
        /// Argument 1: character_1
        /// Argument 2: smile
        /// </example>
        public const string EventWithArgument = "@(?<name>.+)[ ]+(?<value>.+)[ ]*";

        /// <summary>
        /// Format for recognizing text as a tag.
        /// </summary>
        /// <example>
        /// #Tag Tag Name
        /// </example>
        public const string TagFormat = "#Tag[ ]+(?<name>.+)[ ]*";
        
        /// <summary>
        /// Format for recognizing text as an event using regular expressions.
        /// </summary>
        /// <example>
        /// @hide
        /// </example>
        public const string EventWithoutArgument = "@(?<name>.+)[ ]*";
    }
}