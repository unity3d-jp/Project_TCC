using UnityEngine;

namespace Unity.SaveData
{
    /// <summary>
    /// This attribute is used for strings (String).
    /// If the string (String) value is empty, it will be displayed as a problem.
    /// This is a specification for using a PropertyDrawer.
    /// </summary>
    public class NoEmptyStringAttribute : PropertyAttribute
    {
        
    }
}