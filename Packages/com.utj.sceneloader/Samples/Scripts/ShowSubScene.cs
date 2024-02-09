using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Unity.SceneManagement.Samples.Samples.Scripts
{
    /// <summary>
    /// Sample script for ShowSubScene.unity
    /// </summary>
    public class ShowSubScene : MonoBehaviour
    {
        /// <summary>
        /// Target Scene Loader
        /// </summary>
        [SerializeField] private SceneLoader _loader;
        
        /// <summary>
        /// Switch for loader
        /// </summary>
        [SerializeField] private Toggle _toggle;
        
        /// <summary>
        /// Objects to be displayed when the Loader is over there
        /// </summary>
        [SerializeField] private GameObject _lowModel;

        private void Awake()
        {
            // register callbacks.
            _toggle.onValueChanged.AddListener(OnChangeValue);
            _loader.OnLoaded.AddListener(OnLoad);
            _loader.OnUnloaded.AddListener(OnUnload);
        }

        private void Start()
        {
            _lowModel.SetActive(!_loader.enabled);
        }

        private void OnDestroy()
        {
            // unregister callbacks.
            _toggle.onValueChanged.RemoveListener(OnChangeValue);
            _loader.OnLoaded.RemoveListener(OnLoad);
            _loader.OnUnloaded.RemoveListener(OnUnload);
        }

        private void OnUnload() => _lowModel.SetActive(true);

        private void OnLoad(Scene arg0) => _lowModel.SetActive(false);

        private void OnChangeValue(bool isOn) => _loader.enabled = isOn;
    }
}