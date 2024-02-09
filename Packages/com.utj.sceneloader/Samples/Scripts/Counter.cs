using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Unity.SceneManagement.Samples
{
    public class Counter : MonoBehaviour
    {
        [SerializeField]
        private int _count;

        [SerializeField]
        private Text _counter;

        public int Count => _count;
        
        public void Add()
        {
            _count += 1;

            Repaint();
        }

        public void Sub()
        {
            _count -= 1;
            Repaint();
        }

        private void Repaint()
        {
            _counter.text = _count.ToString("00");
        }

    }
}
