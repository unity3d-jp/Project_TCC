using System;
using System.Collections;
using System.Collections.Generic;
using Unity.TinyCharacterController.Check;
using Unity.VisualScripting;
using UnityEngine;

namespace Unity.TinyCharacterController.VisualScripting
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.VisualScripting.SearchCheckReceiver")] 
    public class SearchCheckReceiver : MonoBehaviour
    {
        private HighPriorityTargetSearch _check;
        
        private void OnEnable()
        {
            TryGetComponent(out _check);
            _check.OnCalculatePriority = OnSearchCheck;
        }


        private void OnDisable()
        {
            _check.OnCalculatePriority = null;
        }
 
        private void OnSearchCheck(int capacity, HighPriorityTargetSearch.SearchTargetData[] inputs)
        {
            var data = new SearchCheckEventData();
            for (var index = 0; index < capacity; index++)
            {
                var input = inputs[index];
                data.Angle = input.Angle;
                data.Collider = input.Collider;
                data.Distance = input.Distance;
                data.Index = index;
                data.ClosestPoint = input.ClosestPoint;
                data.IsInsight = input.IsInsight;
                data.IsVisible = input.IsVisible;
                EventBus.Trigger(EventNames.SearchCheckCalculateOrder, gameObject, data);
            }
        }
    }

    public struct SearchCheckEventData
    {
        public int Index;
        public Collider Collider;
        public float Angle;
        public float Distance;
        public bool IsVisible;
        public Vector3 ClosestPoint;
        public bool IsInsight;
    }
    
}
