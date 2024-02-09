using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.SceneManagement.Samples
{
    public class TriggerCallbacks : MonoBehaviour
    {
        [SerializeField] private string _tag;

        [SerializeField] private UnityEvent _onTriggerEnter;
        [SerializeField] private UnityEvent _onTriggerExit;
        
        private void OnTriggerEnter(Collider other)
        {
            if( other.gameObject.CompareTag(_tag))
                _onTriggerEnter?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if( other.gameObject.CompareTag(_tag))
                _onTriggerExit?.Invoke();
        }
    }
}
