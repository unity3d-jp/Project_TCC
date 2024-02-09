using System;

namespace Unity.TinyCharacterController.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequireInterfaceAttribute : Attribute
    {
        public Type InterfaceType { get; private set; }
        
        public RequireInterfaceAttribute(Type type)
        {
            InterfaceType = type;
        }
    }
}