using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Unity.ScenarioImporter
{
    public class ScenarioToDialogueConverter
    {
        public List<DialogueEntry> Dialogue { get; private set; } = new();
        
        /// <summary>
        /// Constructor. Extracts dialogue entries from text data, splitting the text into segments
        /// based on a specified delimiter and storing them as dialogue entries.
        /// </summary>
        /// <param name="textData">The text data to parse.</param>
        public ScenarioToDialogueConverter( string textData)
        {
            textData = NormalizeNewLines(textData);
            textData = RemoveComments(textData);
            textData = RemoveEndLine(textData);

            var splitText = textData.Split( $"{Environment.NewLine}{ScenarioImporterConstants.SplitText}{Environment.NewLine}" );
            
            foreach (var text in splitText)
            {
                var newText = RemoveNewLines(text);
                var message = GetMessage(newText);
                Dialogue.Add(message);
            }
        }

        /// <summary>
        /// Normalizes newline characters within the text data to ensure consistency across platforms.
        /// </summary>
        /// <param name="textData">The text data to normalize.</param>
        /// <returns>Normalized text data with consistent newline characters.</returns>
        private static string NormalizeNewLines(string textData)
        {
            // First replace \r\n (Windows) with \n (Unix), and then \r (Mac) with \n
            textData = textData.Replace("\r\n", "\n");
            textData = textData.Replace("\r", "\n");
            // Now replace all \n (Unix) with Environment.NewLine (Windows if on Windows)
            textData = textData.Replace("\n", Environment.NewLine);
            return textData;
        }

        /// <summary>
        /// Removes trailing newline characters from the text data.
        /// </summary>
        /// <param name="textData">The text data to process.</param>
        /// <returns>The text data without trailing newline characters.</returns>
        private static string RemoveEndLine(string textData)
        {
            return Regex.Replace(textData, $"{Environment.NewLine}+ *$", string.Empty);
        }

        /// <summary>
        /// Removes comment lines from the text data based on a predefined comment symbol.
        /// </summary>
        /// <param name="textData">The text data to clean.</param>
        /// <returns>The text data without comment lines.</returns>
        private static string RemoveComments(string textData)
        {
            var builder = new StringBuilder();

            foreach (var str in textData.Split(Environment.NewLine))
            {
                // remove comments line.
                var result = Regex.Replace(str, ScenarioImporterConstants.Comment + ".*", Environment.NewLine);

                if (string.IsNullOrEmpty(result) == false)
                    builder.AppendLine(result);
            }

            return builder.ToString();
        }
        
        /// <summary>
        /// Extracts dialogue entries from the given text, identifying events, tags, and the main body text.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>A dialogue entry with extracted events, tags, and text.</returns>
        private static DialogueEntry GetMessage(string text)
        {
            var message = new DialogueEntry();
            var events = new List<DialogueEvent>();

            // update texts.
            text = PickupEventWithArgument(text, ref events);
            text = PickupEvent(text, ref events);
            text = PickupTag(text, ref message);
            text = RemoveEmptyLine(text);
            
            message.Events = events;
            message.Text = text;

            return message;
        }

        /// <summary>
        /// Extracts formatted text from the input text that matches an Tag.
        /// </summary>
        /// <param name="text">input text</param>
        /// <param name="dialogueEntry">update message</param>
        /// <returns>Text excluding the sentences identified as tag.</returns>
        private static string PickupTag(string text, ref DialogueEntry dialogueEntry)
        {
            var pattern = $"{ScenarioImporterConstants.TagFormat}{Environment.NewLine}";
            var match = Regex.Match(text, pattern);
            if (match.Success)
            {
                dialogueEntry.Tag = match.Groups["name"].Value;
                text = text.Replace(match.Value, string.Empty);
            }

            return text;
        }
        
        /// <summary>
        /// Extracts formatted text from the input text that matches an event
        /// and stores the discovered events in a collection called <see cref="events"/>".
        /// This method targets events with arguments.
        /// </summary>
        /// <param name="text">text message</param>
        /// <param name="events">List of Events</param>
        /// <returns>Text excluding the sentences identified as events.</returns>
        private static string PickupEventWithArgument(string text, ref List<DialogueEvent> events)
        {
            var pattern = $"{ScenarioImporterConstants.EventWithArgument}{Environment.NewLine}";
            foreach (Match match in Regex.Matches(text, pattern, RegexOptions.RightToLeft))
            {
                var ev = new DialogueEvent();
                var str = match.Value;
                text = text.Replace(str, string.Empty);

                ev.EventName = match.Groups["name"].Value;

                var matchValue = match.Groups["value"];

                if (matchValue.Success)
                {
                    var value = matchValue.Value;
                    ev.EventArgs = SplitArgument(value);
                }

                events.Add(ev);
            }

            return text;
        }
        
        /// <summary>
        /// Extracts formatted text from the input text that matches an event
        /// and stores the discovered events in a collection called <see cref="events"/>".
        /// </summary>
        /// <param name="text">text message</param>
        /// <param name="events">List of Events</param>
        /// <returns>Text excluding the sentences identified as events.</returns>
        private static string PickupEvent(string text, ref List<DialogueEvent> events)
        {
            var pattern = $"{ScenarioImporterConstants.EventWithoutArgument}{Environment.NewLine}";
            foreach (Match match in Regex.Matches(text, pattern))
            {
                var ev = new DialogueEvent();
                var str = match.Value;
                text = text.Replace(str, string.Empty);

                ev.EventName = match.Groups["name"].Value;
                events.Add(ev);
            }

            return text;
        }
        
        /// <summary>
        /// Splits event text and removes any leading or trailing spaces from each element,
        /// for use as an argument.
        /// </summary>
        /// <param name="value">text</param>
        /// <returns>split </returns>
        private static string[] SplitArgument(string value)
        {
            var split = value.Split(ScenarioImporterConstants.SplitArgument);

            for (var i = 0; i < split.Length; i++)
            {
                var str = split[i];
                str = Regex.Replace(str, $"^ *", "");
                str = Regex.Replace(str, $" *$", "");
                
                split[i] = str;
            }

            return split;
        }
        
        /// <summary>
        /// Removes the first and last line breaks from a text.
        /// </summary>
        /// <param name="text">text</param>
        /// <returns>removed text</returns>
        private static string RemoveNewLines(string text)
        {
            // remove newlines at the beginning of a sentence
            text = Regex.Replace(text, $"^ *{Environment.NewLine}+", string.Empty);
            // delete newlines at the end of a sentence
            text = Regex.Replace(text, $"{Environment.NewLine}+ *$", Environment.NewLine);
            
            return text;
        }
        
        private static string RemoveEmptyLine(string text)
        {
            text = Regex.Replace(text, $"^[{Environment.NewLine}]*", string.Empty);  

            return text;
        }
    }
}