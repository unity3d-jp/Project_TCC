using Unity.TinyCharacterController.Interfaces.Core;
using UnityEngine;

namespace Unity.TinyCharacterController.Core
{
    /// <summary>
    /// Base class for components that perform batch processing.
    /// Register this class with a class that inherits from <see cref="SystemBase{TComponent, TSystem}"/> for usage.
    /// </summary>
    public abstract class ComponentBase : MonoBehaviour, 
        IComponentIndex
    {
        /// <summary>
        /// Index of the component during batch processing.
        /// </summary>
        protected int Index { get; private set; } = -1;

        /// <summary>
        /// True if the component is registered.
        /// </summary>
        protected bool IsRegistered => Index != -1;
        
        int IComponentIndex.Index
        {
            get => Index;
            set => Index = value;
        }

        bool IComponentIndex.IsRegistered => IsRegistered;
    }
}