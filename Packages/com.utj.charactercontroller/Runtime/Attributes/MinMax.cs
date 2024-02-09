using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.TinyCharacterController.Attributes
{
    public class MinMaxAttribute : PropertyAttribute
    {
        public float Min, Max;

        public MinMaxAttribute(float minLimit, float maxLimit)
        {
            Min = minLimit;
            Max = maxLimit;
        }
    }

}
