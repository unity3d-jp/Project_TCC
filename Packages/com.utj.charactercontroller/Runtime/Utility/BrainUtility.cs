using System.Collections;
using System.Collections.Generic;
using Unity.TinyCharacterController.Interfaces;
using Unity.Mathematics;
using UnityEngine;
using Unity.TinyCharacterController.Interfaces.Core;

namespace Unity.TinyCharacterController.Utility
{
    public static class BrainUtility
    {
        public static Vector3 LimitAxis(in Vector3 currentPosition, Vector3 newPosition, in bool3 freezeAxis)
        {
            // Correct for position offset due to component pushing.
            // Calculate this only if one of the axes is locked.
            if (freezeAxis.x  || freezeAxis.y  || freezeAxis.z )
            {
                // Reset the position to the initial value before calculation.
                if (freezeAxis.x)
                    newPosition.x = currentPosition.x;
                if (freezeAxis.y)
                    newPosition.y = currentPosition.y;
                if (freezeAxis.z)
                    newPosition.z = currentPosition.z;
            }

            return newPosition;
        }
        
        public static bool ClosestHit(RaycastHit[] hits, CharacterSettings settings, out RaycastHit closestHit, int count, float maxDistance)
        {
            var min = maxDistance;
            closestHit = new RaycastHit();
            var isHit = false;

            for (var i = 0; i < count; i++)
            {
                var hit = hits[i];
                if (hit.distance > min || hit.collider == null || settings.IsOwnCollider(hit.collider))
                    continue;

                min = hit.distance;
                closestHit = hit;
                isHit = true;
            }

            return isHit;
        }


    }
}
