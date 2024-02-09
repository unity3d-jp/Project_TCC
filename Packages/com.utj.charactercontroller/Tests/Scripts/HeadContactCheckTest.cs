using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.TinyCharacterController.Check;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace Unity.TinyCharacterController.Test
{
    public class HeadContactCheckTest : SceneTestBase
    {
        // height, IsObjectOverhead, IsContact
        public static (float,  bool)[] RoofPositionsForIsContact = new[]
        {
            (2.0f,  true),
            (2.1f,  true),
            (2.3f,  false),
            (2.8f,  false),
        };
        
        [UnityTest]
        public IEnumerator 頭上の接触判定([ValueSource(nameof(RoofPositionsForIsContact))] (float,  bool) values)
        {
            var headContactCheck = FindComponent<HeadContactCheck>("Character_1");
            var roof = GameObject.Find("Roof");

            roof.transform.position = new Vector3(0, values.Item1, 0);
            Physics.SyncTransforms();

            yield return new WaitForSeconds(0.1f);
            
            Assert.That(headContactCheck.IsHeadContact, Is.EqualTo(values.Item2));
        }

        [UnityTest]
        public IEnumerator 頭上との距離の判定([ValueSource(nameof(RoofPositionsForIsContact))] (float, bool) values)
        {
            var headContactCheck = FindComponent<HeadContactCheck>("Character_1");
            var roof = GameObject.Find("Roof");

            roof.transform.position = new Vector3(0, values.Item1, 0);
            Physics.SyncTransforms();

            yield return new WaitForSeconds(0.1f);
            
            Assert.That(headContactCheck.DistanceFromRootPosition, Is.EqualTo(values.Item1).Using(FloatEqualityComparer.Instance));
        }
        
        public static (float,  bool)[] RoofPositionsForDetection = new[]
        {
            (2.2f,  true),
            (2.3f,  true),
            (3f,   true),
            (3.5f,  false),
            (4f,  false),
        };
        
        [UnityTest]
        public IEnumerator 頭上の有無判定([ValueSource(nameof(RoofPositionsForDetection))] (float,  bool) values)
        {
            var headContactCheck = FindComponent<HeadContactCheck>("Character_1");
            var roof = GameObject.Find("Roof");

            roof.transform.position = new Vector3(0, values.Item1, 0);
            Physics.SyncTransforms();
            
            yield return new WaitForSeconds(0.1f);

            Assert.That(headContactCheck.IsObjectOverhead, Is.EqualTo(values.Item2));
        }

        
        
        [UnityTest]
        public IEnumerator 頭上のオブジェクトが変化()
        {
            var headContactCheck = FindComponent<HeadContactCheck>("Character_1");
            var roof = GameObject.Find("Roof");

            roof.gameObject.SetActive(false);
            yield return null;

            roof.gameObject.SetActive(true);
            yield return null;

            Assert.That(headContactCheck.IsHitCollisionInThisFrame, Is.True);

            yield return null;
            Assert.That(headContactCheck.IsHitCollisionInThisFrame, Is.False);

            roof.gameObject.SetActive(false);
            yield return null;
            
            Assert.That(headContactCheck.IsHitCollisionInThisFrame, Is.False);
        }
        
        // height, IsObjectOverhead, IsContact
        public static (float,  bool)[] CharacterHeights = new[]
        {
            (1.0f,  false),
            (1.5f,  false),
            (1.9f,   true),
            (2.1f,  true)
        };
        
        [UnityTest]
        public IEnumerator 高さの変更([ValueSource(nameof(CharacterHeights))] (float,  bool) values)
        {
            var headContactCheck = FindComponent<HeadContactCheck>("Character_1");
            var roof = GameObject.Find("Roof");
            var settings = headContactCheck.GetComponent<CharacterSettings>();

            settings.Height = values.Item1;
            
            yield return null;
            
            Assert.That(headContactCheck.IsHeadContact, Is.EqualTo(values.Item2));
            
        }

        protected override string ScenePath => TestConstants.PackagePath + "Scenes/HeadContactCheckTest.unity";
    }
}
