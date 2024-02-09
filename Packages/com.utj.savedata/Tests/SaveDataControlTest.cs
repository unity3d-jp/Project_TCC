using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.SaveData.Test
{
    public class SaveDataControlTest : SceneTestBase
    {
        private const string FolderName = "SaveDataControlTest";
        private const string FileName = "SaveControl_1";
        
        protected override string ScenePath =>
            "Packages/com.utj.save.helper/Tests/SaveDataControlTest.unity";

        [TearDown]
        public void TearDown()
        {
            FileManagerBase fileManager = new FileManager();
            
            if (fileManager.IsExists(FolderName, FileName))
            {
                var json = fileManager.Read(FolderName, FileName);
                Debug.Log(json);
            }

            fileManager.Remove(FolderName, FileName);
        }
        
        [UnityTest]
        public IEnumerator データを保存する()
        {
            var saveDataControl = FindComponent<SaveDataControl>("SaveControl_1");
            saveDataControl.Save(FolderName);

            yield return null;
            
            Assert.That(saveDataControl.IsExists(FolderName), Is.True);
        }

        [Test]
        public void ファイルがない場合は初期値を使う()
        {
            var saveDataControl = FindComponent<SaveDataControl>("SaveControl_1");
            
            var nameContainer = saveDataControl.GetContainerT<StringContainer>("Name");
            var hpContainer = saveDataControl.GetContainerT<IntContainer>("HP");
            var mpContainer = saveDataControl.GetContainerT<IntContainer>("MP");
            
            Assert.That(nameContainer.Value, Is.EqualTo("TOM")); 
            Assert.That(hpContainer.Value, Is.EqualTo(100));
            Assert.That(mpContainer.Value, Is.EqualTo(15));
        }

        [Test]
        public void データをロードする()
        {
            var saveDataControl = FindComponent<SaveDataControl>("SaveControl_1");
            var nameContainer = saveDataControl.GetContainerT<StringContainer>("Name");
            var hpContainer = saveDataControl.GetContainerT<IntContainer>("HP");
            var mpContainer = saveDataControl.GetContainerT<IntContainer>("MP");

            nameContainer.Value = "TEST";
            hpContainer.Value = 20;
            mpContainer.Value = 0;
            
            saveDataControl.Save(FolderName);

            nameContainer.Value = "UPDATE VALUE";
            hpContainer.Value = 200;
            mpContainer.Value = 500;
            
            saveDataControl.Load(FolderName);
            
            Assert.That(nameContainer.Value, Is.EqualTo("TEST")); 
            Assert.That(hpContainer.Value, Is.EqualTo(20));        
            Assert.That(mpContainer.Value, Is.EqualTo(0));        
        }

        [Test]
        public void 別のオブジェクトに割り当てる()
        {
            var saveDataControl1 = FindComponent<SaveDataControl>("SaveControl_1");
            var saveDataControl2 = FindComponent<SaveDataControl>("SaveControl_2");
            
            saveDataControl1.Save(FolderName);
            
            saveDataControl2.Load(FolderName);

            var hp = saveDataControl2.GetContainerT<IntContainer>("HP");
            var mp = saveDataControl2.GetContainerT<IntContainer>("MP");
            var name = saveDataControl2.GetContainerT<StringContainer>("Name");
            
            Assert.That(hp.Value, Is.EqualTo(100));
            Assert.That(mp.Value, Is.EqualTo(15));
            Assert.That(name.Value, Is.EqualTo("TOM"));
        }

        [Test]
        public void 異なる構造のオブジェクトに割り当てる()
        {
            var saveDataControl1 = FindComponent<SaveDataControl>("SaveControl_1");
            var saveDataControl2 = FindComponent<SaveDataControl>("SaveControl_3");
            
            saveDataControl1.Save(FolderName);
            
            saveDataControl2.Load(FolderName);

            var hp = saveDataControl2.GetContainerT<IntContainer>("HP");
            var mp = saveDataControl2.GetContainerT<IntContainer>("MP");
            var name = saveDataControl2.GetContainerT<StringContainer>("Name");
            
            Assert.That(hp.Value, Is.EqualTo(100));
            Assert.That(mp.Value, Is.EqualTo(15));
            Assert.That(name.Value, Is.EqualTo("TOM"));
        }
    }
}
