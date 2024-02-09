using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Utils;

namespace Unity.SaveData.Test
{
    public class ContainerTest : SceneTestBase
    {
        protected override string ScenePath =>
            "Packages/com.utj.save.helper/Tests/ContainerTest.unity";

        private const string FolderName = "Test";
        
        [TearDown]
        public void TearDown()
        {
            var saveDataControl = FindComponent<SaveDataControl>("SaveDataControl");
            saveDataControl.Remove(FolderName);

#if UNITY_EDITOR || UNITY_STANDALONE
            var path = saveDataControl.FileManager.GetPath("Test", "");
            var folderPath = System.IO.Path.GetDirectoryName(path);
            if( folderPath != null && System.IO.Directory.Exists(folderPath))
                System.IO.Directory.Delete(folderPath, true);
#endif
        }
        
        [Test]
        public void Boolコンテナのテスト()
        {
            var saveDataControl = FindComponent<SaveDataControl>("SaveDataControl");
            var boolContainer = saveDataControl.GetContainerT<BoolContainer>("Bool");
            var boolArrayContainer = saveDataControl.GetContainerT<BoolArrayContainer>("BoolArray");

            saveDataControl.Save(FolderName);

            boolContainer.Value = true;
            boolArrayContainer.Value = new[] { true, false, true };

            saveDataControl.Load(FolderName);

            Assert.That(boolContainer.Value, Is.False);
            Assert.That(boolArrayContainer.Value[0], Is.False);
            Assert.That(boolArrayContainer.Value[1], Is.True);
            Assert.That(boolArrayContainer.Value[2], Is.False);
        }

        [Test]
        public void Floatコンテナのテスト()
        {
            var saveDataControl = FindComponent<SaveDataControl>("SaveDataControl");
            var floatContainer = saveDataControl.GetContainerT<FloatContainer>("Float");
            var floatArrayContainer = saveDataControl.GetContainerT<FloatArrayContainer>("FloatArray");

            saveDataControl.Save(FolderName);

            floatContainer.Value = 30f;
            floatArrayContainer.Value = new[] { 0.00f, 1.11f, 2.22f };

            saveDataControl.Load(FolderName);

            Assert.That(floatContainer.Value, Is.EqualTo(123.45f).Using(FloatEqualityComparer.Instance));
            Assert.That(floatArrayContainer.Value[0], Is.EqualTo(1.23f).Using(FloatEqualityComparer.Instance));
            Assert.That(floatArrayContainer.Value[1], Is.EqualTo(4.56f).Using(FloatEqualityComparer.Instance));
            Assert.That(floatArrayContainer.Value[2], Is.EqualTo(7.89f).Using(FloatEqualityComparer.Instance));
        }

        [Test]
        public void Stringコンテナのテスト()
        {
            var saveDataControl = FindComponent<SaveDataControl>("SaveDataControl");
            var stringContainer = saveDataControl.GetContainerT<StringContainer>("String");
            var stringArrayContainer = saveDataControl.GetContainerT<StringArrayContainer>("StringArray");

            stringContainer.Value = "tset";
            stringArrayContainer.Value = new[] { "ddd", "eee", "fff" };
            
            saveDataControl.Save(FolderName);

            stringContainer.Value = "test";
            stringArrayContainer.Value = new[] { "aaa", "bbb", "ccc" };

            saveDataControl.Load(FolderName);

            Assert.That(stringContainer.Value, Is.EqualTo("tset"));
            Assert.That(stringArrayContainer.Value[0], Is.EqualTo("ddd"));
            Assert.That(stringArrayContainer.Value[1], Is.EqualTo("eee"));
            Assert.That(stringArrayContainer.Value[2], Is.EqualTo("fff"));
        }

        [Test]
        public void Vector3コンテナのテスト()
        {
            var saveDataControl = FindComponent<SaveDataControl>("SaveDataControl");
            var vector3Container = saveDataControl.GetContainerT<Vector3Container>("Vector3");
            var vector3ArrayContainer = saveDataControl.GetContainerT<Vector3ArrayContainer>("Vector3Array");

            saveDataControl.Save(FolderName);

            vector3Container.Value = new Vector3(-1, -1, -1);
            vector3ArrayContainer.Value = new[] { new Vector3(-1, 0, 0), new Vector3(-1, 0, 0) };

            saveDataControl.Load(FolderName);

            Assert.That(vector3Container.Value,
                Is.EqualTo(new Vector3(1, 1, 1)).Using(Vector3EqualityComparer.Instance));
            Assert.That(vector3ArrayContainer.Value[0],
                Is.EqualTo(new Vector3(1, 0, 0)).Using(Vector3EqualityComparer.Instance));
            Assert.That(vector3ArrayContainer.Value[1],
                Is.EqualTo(new Vector3(0, 1, 0)).Using(Vector3EqualityComparer.Instance));
            Assert.That(vector3ArrayContainer.Value[2],
                Is.EqualTo(new Vector3(0, 0, 1)).Using(Vector3EqualityComparer.Instance));
        }

        [Test]
        public void フラグコンテナのテスト()
        {
            var saveDataControl = FindComponent<SaveDataControl>("SaveDataControl");
            var flagContainer = saveDataControl.GetContainerT<FlagCollectionContainer>("Flag");

            saveDataControl.Save(FolderName);

            flagContainer.SetValue("FLAG_1", false);
            flagContainer.SetValue("FLAG_3", false);

            saveDataControl.Load(FolderName);


            Assert.That(flagContainer.GetValue("FLAG_1"), Is.False);
            Assert.That(flagContainer.GetValue("FLAG_2"), Is.True);
            Assert.That(flagContainer.GetValue("FLAG_3"), Is.False);
        }

        [Test]
        public void Intコンテナのテスト()
        {
            var saveDataControl = FindComponent<SaveDataControl>("SaveDataControl");
            var intContainer = saveDataControl.GetContainerT<IntContainer>("Int");
            var intArrayContainer = saveDataControl.GetContainerT<IntArrayContainer>("IntArray");
            var flagContainer = saveDataControl.GetContainerT<FlagCollectionContainer>("Flag");

            saveDataControl.Save(FolderName);

            intContainer.Value = 12;
            intArrayContainer.Value = new[] { 3, 2, 1, 0 };

            saveDataControl.Load(FolderName);

            Assert.That(intContainer.Value, Is.EqualTo(5));
            Assert.That(intArrayContainer.Value[0], Is.EqualTo(0));
            Assert.That(intArrayContainer.Value[1], Is.EqualTo(1));
            Assert.That(intArrayContainer.Value[2], Is.EqualTo(2));
        }
    }
}