// using NUnit.Framework;
// using UnityEditor.TestTools.TestRunner.Api;
// using UnityEngine;
// using UnityEngine.SceneManagement;
//
// public class SceneReSetter : ITestRunSettings
// {
//     private string[] _initialScenePaths;
//
//     public void Apply()
//     {
//         for (var i = 0; i < _initialScenePaths.Length; i++)
//         {
//             _initialScenePaths[i] = SceneManager.GetSceneAt(i).path;
//         }
//
//     }
//
//     public void Dispose()
//     {
//         for (var i = 0; i < _initialScenePaths.Length; i++)
//         {
//             var scenePath = _initialScenePaths[i];
//             UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath, i == 0 ? 
//                 UnityEditor.SceneManagement.OpenSceneMode.Single : UnityEditor.SceneManagement.OpenSceneMode.Additive);
//         }
//     }
//     
//
// }