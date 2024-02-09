using Unity.TinyCharacterController.Interfaces.Utility;
using Unity.TinyCharacterController.Utility;
using UnityEngine;

namespace Unity.TinyCharacterController.Modifier
{
    /// <summary>
    /// Tags used to determine if it is a moving floor.
    /// <see cref="TinyCharacterController.Effect.MoveWithPlatform"/>
    /// </summary>
    [AddComponentMenu(MenuList.Gimmick + "MovePlatform")]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-200)]
    public class MovePlatform : MonoBehaviour, IEditorProcess
    {
        /// <summary>
        /// Processing before game playback or game build
        /// </summary>
        void IEditorProcess.Execute()
        {
            // If there is an Animator in the parent object, add AnimatorUpdate to change the timing of Animator updates.
            var animator = GetComponentInParent<Animator>();

            if (animator != null && animator.TryGetComponent(out AnimatorUpdate _) == false)
            {
                animator.gameObject.AddComponent<AnimatorUpdate>();
            }
        }

        /// <summary>
        /// Setting to determine whether to remove the component during object build.
        /// </summary>
        bool IEditorProcess.RemoveComponentAfterBuild => false;

    }
}
