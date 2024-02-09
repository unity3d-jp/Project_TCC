using System;
using Unity.TinyCharacterController.Interfaces.Utility;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace TinyCharacterControllerEditor.Process
{
    /// <summary>
    /// Inherits from <see cref="IEditorProcess"/> to collect components and execute processing during builds.
    /// This processing is also executed during gameplay, and it runs after Awake.
    /// </summary>
    public class EditorProcess : IProcessSceneWithReport
    {
        int IOrderedCallback.callbackOrder => 10;

        void IProcessSceneWithReport.OnProcessScene(Scene scene, BuildReport report)
        {
            // Allocate buffers
            var processComponents = ListPool<IEditorProcess>.Get();
            var allRootGameObjects = ListPool<GameObject>.Get();

            try
            {
                // Collect GameObjects within the scene to process
                scene.GetRootGameObjects(allRootGameObjects);

                foreach (var root in allRootGameObjects)
                {
                    // Collect a list of components with processes
                    root.GetComponentsInChildren(processComponents);

                    foreach (var process in processComponents)
                        ExecuteProcess(process);
                }
            }
            finally
            {
                // Release buffers
                ListPool<IEditorProcess>.Release(processComponents);
                ListPool<GameObject>.Release(allRootGameObjects);
            }
        }

        /// <summary>
        /// Execute processing on a component and, if necessary, destroy the component.
        /// </summary>
        /// <param name="process">The component to execute the process on.</param>
        private static void ExecuteProcess(in IEditorProcess process)
        {
            try
            {
                // Execute the process
                process.Execute();

                // If the component needs to be destroyed, destroy it
                if (process.RemoveComponentAfterBuild)
                    Object.DestroyImmediate(process as MonoBehaviour);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message, process as MonoBehaviour);
            }
        }
    }
}
