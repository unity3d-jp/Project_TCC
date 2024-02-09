using System;
using NUnit.Framework;
using UnityEngine;

namespace Unity.ScenarioImporter.Test
{
    public class ScenarioConverterTest : SceneTestBase
    {
        protected override string ScenePath =>
            "Packages/com.utj.scenario.importer/Tests/Scenes/Test.unity";

        [Test]
        public void シナリオの抽出()
        {
            var dummy = FindComponent<ScenarioDummyComponent>("Text_1");
            var converter = new ScenarioToDialogueConverter(dummy.TextData);
            
            Assert.That(converter.Dialogue[0].Text, Is.EqualTo("AAA" + Environment.NewLine + "CCC" ));
            Assert.That(converter.Dialogue[1].Text, Is.EqualTo("BBB"));
            Assert.That(converter.Dialogue[2].Text, Is.EqualTo("CCC"));
        }
        
        [Test]
        public void タグの抽出()
        {
            var dummy = FindComponent<ScenarioDummyComponent>("Text_2");
            var converter = new ScenarioToDialogueConverter(dummy.TextData);
            
            Assert.That(converter.Dialogue[0].Tag, Is.EqualTo("TAG_1")); 
            Assert.That(converter.Dialogue[1].Tag, Is.EqualTo("TAG_2")); 
            Assert.That(converter.Dialogue[2].Tag, Is.Null); 
        }

        [Test]
        public void タグからシナリオ抽出()
        {
            var dummy = FindComponent<ScenarioDummyComponent>("Text_2");
            var converter = new ScenarioToDialogueConverter(dummy.TextData);

            var dialog = ScriptableObject.CreateInstance<DialogueScript>();
            dialog.DialogueEntries = converter.Dialogue;

            var dialogueEntries = dialog.GetDialogueEntriesWithTag("TAG_2");
            
            Assert.That(dialogueEntries.Count, Is.EqualTo(2));
            Assert.That(dialogueEntries[0].Text, Is.EqualTo("BBB"));
            Assert.That(dialogueEntries[1].Text, Is.EqualTo("CCC"));
        }

        [Test]
        public void イベントの抽出()
        {
            var dummy = FindComponent<ScenarioDummyComponent>("Text_3");
            var converter = new ScenarioToDialogueConverter(dummy.TextData);

            
            // Assert.That(converter.Dialogue[0].Text, Is.EqualTo("AAAA"));
            Assert.That(converter.Dialogue[0].Events.Count, Is.EqualTo(1));
            Assert.That(converter.Dialogue[0].Events[0].EventName, Is.EqualTo("event"));
            Assert.That(converter.Dialogue[0].Events[0].EventArgs[0], Is.EqualTo("AAA"));
            Assert.That(converter.Dialogue[0].Events[0].EventArgs[1], Is.EqualTo("BBB"));
            Assert.That(converter.Dialogue[1].Events.Count, Is.EqualTo(2));
            var eventIndex = converter.Dialogue[1].Events.FindIndex(c => c.EventName == "event");
            var callIndex = converter.Dialogue[1].Events.FindIndex(c => c.EventName == "call");
            Assert.That(converter.Dialogue[1].Events[eventIndex].EventName, Is.EqualTo("event"));
            Assert.That(converter.Dialogue[1].Events[eventIndex].EventArgs[0], Is.EqualTo("CCC"));
            Assert.That(converter.Dialogue[1].Events[eventIndex].EventArgs[1], Is.EqualTo("DDD"));
            Assert.That(converter.Dialogue[1].Events[callIndex].EventName, Is.EqualTo("call"));
            Assert.That(converter.Dialogue[1].Events[callIndex].EventArgs[0], Is.EqualTo("EEE"));
            Assert.That(converter.Dialogue[1].Events[callIndex].EventArgs[1], Is.EqualTo("FFF"));
        }
    }
}