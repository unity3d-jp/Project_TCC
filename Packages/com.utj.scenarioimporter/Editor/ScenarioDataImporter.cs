using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Unity.ScenarioImporter
{
    /// <summary>
    /// This importer extracts scenarios and events from text data and makes them available as usable data.
    /// It automatically processes files with the ".scenario" extension and converts them into ScriptableObjects.
    /// </summary>
    [ScriptedImporter(1, ScenarioImporterConstants.Extensions)]
    public class ScenarioDataImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var scenario = GetScenarioData(ctx.assetPath);
            var textData = LoadScenario(ctx.assetPath);

            var toDialogueConverter = new ScenarioToDialogueConverter(textData);
            scenario.DialogueEntries = toDialogueConverter.Dialogue;

            ctx.AddObjectToAsset(scenario.name, scenario);
            ctx.SetMainObject(scenario);
        }
        
        /// <summary>
        /// Loads the data for a scenario.
        /// If the scenario data does not exist, it generates an asset.
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>scenario data</returns>
        private static DialogueScript GetScenarioData(string path)
        {
            var scenario = AssetDatabase.LoadAssetAtPath<DialogueScript>(path);

            if (scenario == null)
            {
                scenario = ScriptableObject.CreateInstance<DialogueScript>();
                scenario.name = Path.GetFileName(path);
            }
            return scenario;
        }
        
        /// <summary>
        /// Retrieves the body of the scenario
        /// </summary>
        /// <param name="path">File Path</param>
        /// <returns>scenario text.</returns>
        private static string LoadScenario(string path)
        {
            return File.ReadAllText(path);
        }
    }
}