using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.SaveData.Test
{
    public class InventoryContainerTest : SceneTestBase
    {
        private const string GameObjectName = "SaveDataControl";
        private const string FolderName = "Test";

        [TearDown]
        public void TearDown()
        {
            var saveDataControl = FindComponent<SaveDataControl>(GameObjectName);
            saveDataControl.Remove(FolderName);
            
#if UNITY_EDITOR || UNITY_STANDALONE
            var path = saveDataControl.FileManager.GetPath("Test", "");
            var folderPath = System.IO.Path.GetDirectoryName(path);
            if( folderPath != null && System.IO.Directory.Exists(folderPath))
                System.IO.Directory.Delete(folderPath, true);
#endif
        }
        
        
        [Test]
        public void インスタンスの取得の確認()
        {
            var inventory = FindComponent<InventoryContainer>(GameObjectName);
            Assert.That(inventory, Is.Not.Null);
        }

        [Test]
        public void 現在のアイテム一覧を確認する()
        {
            var inventory = FindComponent<InventoryContainer>(GameObjectName);
            var itemNames = new List<string>(inventory.ItemNames);
            Assert.That(itemNames[0], Is.EqualTo("Item1"));
            Assert.That(itemNames[1], Is.EqualTo("Item2"));
            Assert.That(itemNames[2], Is.EqualTo("Item3"));
        }

        [Test]
        public void 現在のアイテムの個数を確認する()
        {
            var inventory = FindComponent<InventoryContainer>(GameObjectName);
            Assert.That(inventory.GetItemCount("Item1"), Is.EqualTo(0));
            Assert.That(inventory.GetItemCount("Item2"), Is.EqualTo(2));
            Assert.That(inventory.GetItemCount("Item3"), Is.EqualTo(4));
        }

        [Test]
        public void 既存のアイテムを増やす()
        {
            var inventory = FindComponent<InventoryContainer>(GameObjectName);
            inventory.AddItem("Item2", 102);
            
            Assert.That(inventory.GetItemCount("Item1"), Is.EqualTo(0));
            Assert.That(inventory.GetItemCount("Item2"), Is.EqualTo(104));
            Assert.That(inventory.GetItemCount("Item3"), Is.EqualTo(4));
        }

        [Test]
        public void 既存のアイテムを減らす()
        {
            var inventory = FindComponent<InventoryContainer>(GameObjectName);
            inventory.SubtractItem("Item3", 1);
            
            Assert.That(inventory.GetItemCount("Item1"), Is.EqualTo(0));
            Assert.That(inventory.GetItemCount("Item2"), Is.EqualTo(2));
            Assert.That(inventory.GetItemCount("Item3"), Is.EqualTo(3));
        }
        
        

        [Test]
        public void アイテムを取得する()
        {
            var inventory = FindComponent<InventoryContainer>(GameObjectName);
            inventory.SubtractItem("NewItem", 15);
            Assert.That(inventory.GetItemCount("NewItem"), Is.EqualTo(0));
        }

        [Test]
        public void アイテムを失う()
        {
            var inventory = FindComponent<InventoryContainer>(GameObjectName);
            inventory.SubtractItem("Item3", 4);
            
            Assert.That(inventory.ExistsItem("Item3"), Is.False); 
            Assert.That(inventory.GetItemCount("Item3"), Is.EqualTo(0));
        }

        [Test]
        public void 最大個数より多くアイテムを失う()
        {
            var inventory = FindComponent<InventoryContainer>(GameObjectName);
            inventory.SubtractItem("Item3", 12);
            
            Assert.That(inventory.ExistsItem("Item3"), Is.False); 
            Assert.That(inventory.GetItemCount("Item3"), Is.EqualTo(0));
        }

        [Test]
        public void アイテムがない状態でアイテムを失う()
        {
            var inventory = FindComponent<InventoryContainer>(GameObjectName);
            inventory.SubtractItem("NewItem", 12);
        }

        [Test]
        public void アイテムの個数を直接指定する()
        {
            var inventory = FindComponent<InventoryContainer>(GameObjectName);
            inventory.SetItemCount("Item1", 30);
            inventory.SetItemCount("Item2", -5);
            
            Assert.That(inventory.GetItemCount("Item1"), Is.EqualTo(30));
            Assert.That(inventory.ExistsItem("Item2"), Is.False);
            Assert.That(inventory.GetItemCount("Item2"), Is.EqualTo(0));
        }

        [Test]
        public void 保存()
        {
            var inventory = FindComponent<InventoryContainer>(GameObjectName);
            var saveDataControl = inventory.GetComponent<SaveDataControl>();
            
            inventory.AddItem("Item1", 8);
            inventory.AddItem("Item2", 6);
            inventory.AddItem("Item3", 4);

            saveDataControl.Save(FolderName);

            Assert.That(saveDataControl.IsExists("Test"), Is.True);
        }

        [Test]
        public void 保存と展開()
        {
            var inventory = FindComponent<InventoryContainer>(GameObjectName);
            var saveDataControl = inventory.GetComponent<SaveDataControl>();

            saveDataControl.Save(FolderName);
            
            inventory.AddItem("Item1", 15);
            inventory.AddItem("Item2", 10);
            inventory.AddItem("Item3", 5);
            
            saveDataControl.Load(FolderName);
            
            Assert.That(inventory.GetItemCount("Item1"), Is.EqualTo(0));
            Assert.That(inventory.GetItemCount("Item2"), Is.EqualTo(2));
            Assert.That(inventory.GetItemCount("Item3"), Is.EqualTo(4));
        }
        
        
        protected override string ScenePath => "Packages/com.utj.save.helper/Tests/InventoryContainerTest.unity";
    }
}
