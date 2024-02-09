using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;
using Unity.TinyCharacterController.Control;
using Unity.TinyCharacterController.Interfaces.Components;
using UnityEngine;
using UnityEngine.TestTools.Utils;

namespace Unity.TinyCharacterController.Test
{
    public class MoveWithPlatformTest : SceneTestBase
    {
        protected override string ScenePath => TestConstants.PackagePath + "Scenes/MoveWithPlatformTest.unity";

        private const string GroundObjectName = "Ground";
        
        /// <summary>
        /// (string) : object name
        /// (bool) : isFixedUpdate
        /// </summary>
        private static (string, bool)[] _characterNames =
        {
            ("Player-TransformBrain", false),
            ("Player-CharacterBrain", false),
            ("Player-RigidbodyBrain", true)
        };

        [UnityTest]
        public IEnumerator 地面を動かす([ValueSource(nameof(_characterNames))] (string, bool) characterData)
        {
            var ground = FindComponent<Transform>(GroundObjectName);
            var character = GetObjectAndComponentInRoot<Transform>(characterData.Item1);
            character.gameObject.SetActive(true);
            
            yield return MoveGround(ground, new Vector3(3, 0, 0), characterData.Item2);

            Assert.That(character.position, Is.EqualTo(new Vector3(3, 0, 2)).Using(Vector3EqualityComparer.Instance));            
        }
        
        [UnityTest]
        public IEnumerator 地面の移動中に地面から離れる([ValueSource(nameof(_characterNames))] (string, bool) characterData)
        {
            var ground = FindComponent<Transform>(GroundObjectName);
            var character = GetObjectAndComponentInRoot<Transform>(characterData.Item1);
            var brain = character.GetComponent<IBrain>();
            character.gameObject.SetActive(true);
            
            var startPosition = ground.position;
            var newPosition = new Vector3(5, 0, 0);
            for (var time = 0f; time < 1; time +=　characterData.Item2 ? Time.fixedDeltaTime : Time.deltaTime)
            {
                ground.position = Vector3.Lerp(startPosition, newPosition, Mathf.Clamp01(time));
                ground.gameObject.SetActive(time < 0.5f);
                
                yield return characterData.Item2 ? new WaitForFixedUpdate() : null;
            }

            ground.position = newPosition;
            
            Assert.That(brain.EffectVelocity.x , Is.GreaterThan(0));
        }
        
        [UnityTest]
        public IEnumerator 地面を回す([ValueSource(nameof(_characterNames))]  (string, bool) characterData)
        {
            var ground = FindComponent<Transform>(GroundObjectName);
            var character = GetObjectAndComponentInRoot<Transform>(characterData.Item1);
            character.gameObject.SetActive(true);

            var rotation = Quaternion.AngleAxis(90, Vector3.up);
            yield return RotateGround(ground, rotation, characterData.Item2);

            Assert.That(character.rotation, Is.EqualTo(rotation)
                .Using(QuaternionEqualityComparer.Instance));            
        }
        
        
        [UnityTest]
        public IEnumerator Controlの移動は優先されない([ValueSource(nameof(_characterNames))] (string, bool) characterData)
        {
            var ground = FindComponent<Transform>(GroundObjectName);
            var character = GetObjectAndComponentInRoot<Transform>(characterData.Item1);
            character.gameObject.SetActive(true);

            character.GetComponent<ManualControl>().MovePriority = 10;
            
            yield return MoveGround(ground, new Vector3(3, 0, 0), characterData.Item2);

            Assert.That(character.position, Is.EqualTo(new Vector3(3, 0, 2)).Using(Vector3EqualityComparer.Instance));            
        }

        [UnityTest]
        public IEnumerator Controlの回転を優先する([ValueSource(nameof(_characterNames))] (string, bool)  characterData)
        {
            var ground = FindComponent<Transform>(GroundObjectName);
            var character = GetObjectAndComponentInRoot<Transform>(characterData.Item1);
            character.gameObject.SetActive(true);

            var rotation = Quaternion.AngleAxis(-90, Vector3.up);
            character.GetComponent<ManualControl>().TurnPriority = 10;
            character.GetComponent<ManualControl>().TurnRotation = rotation;
            
            yield return RotateGround(ground, Quaternion.AngleAxis(90, Vector3.up), characterData.Item2);
            

            Assert.That(character.rotation, Is.EqualTo(rotation)
                .Using(QuaternionEqualityComparer.Instance));            
        }

        private IEnumerator RotateGround(Transform ground, Quaternion endRotation, bool isFixedUpdate)
        {
            var startRotation = ground.rotation;
            for (var time = 0f; time < 1; time += isFixedUpdate ? Time.fixedDeltaTime : Time.deltaTime)
            {
                ground.rotation = Quaternion.Lerp(startRotation, endRotation, Mathf.Clamp01(time));
                yield return isFixedUpdate ? new WaitForFixedUpdate() : null;
            }
            
            ground.rotation = endRotation;
            yield return null;
        }

        private IEnumerator MoveGround(Transform ground, Vector3 newPosition, bool isFixedUpdate)
        {
            var startPosition = ground.position;
            for (var time = 0f; time < 1; time +=　isFixedUpdate ? Time.fixedDeltaTime : Time.deltaTime)
            {
                ground.position = Vector3.Lerp(startPosition, newPosition, Mathf.Clamp01(time));
                yield return isFixedUpdate ? new WaitForFixedUpdate() : null;
            }

            ground.position = newPosition;
            yield return null;
        }
    }
}