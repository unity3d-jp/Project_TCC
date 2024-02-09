using UnityEngine;

namespace Unity.TinyCharacterController.Components.Utility
{
    public static class GameObjectUtility
    {
        /// <summary>
        ///     Method to check if a GameObject is included in the list of <see cref="_hitTag" />.
        ///     Returns True even if nothing is registered in _hitTag.
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <param name="hitTags"></param>
        /// <returns>True if the object is included in the specified tags, False otherwise</returns>
        public static bool ContainTag(GameObject obj, in string[] hitTags)
        {
            // If there are no tags registered in _hitTag, always return True
            if (hitTags.Length == 0)
                return true;

            // Check if the object's tag is included in the list of _hitTag
            for (var i = 0; i < hitTags.Length; i++)
                // If the object's tag matches, return True
                if (obj.CompareTag(hitTags[i]))
                    return true;

            // If no match was found with any tag, return False
            return false;
        }
    }
}