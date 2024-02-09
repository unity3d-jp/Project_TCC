using System;
using System.Collections;
using System.Collections.Generic;
using Unity.SaveData;
using Unity.SaveData.Core;
using UnityEngine;
using UnityEngine.UI;

namespace GameDataSave.Sample
{
    public class CountUpSample : MonoBehaviour
    {
        [Header("save/load settings")]
        [SerializeField]
        private string _path = "SaveLoadSample";

        [Header("ui settings")]
        [SerializeField]
        private Canvas _canvas;
        
        private IntContainer _countData;
        private SaveDataControl _saveControl;
        private Button _addButton;
        private Button _subtractButton;
        private Button _saveButton;
        private Text _countLabel;
        private Animation _animation;
        private InventoryContainer _inventory;

        private void Awake()
        {
            TryGetComponent(out _countData);
            TryGetComponent(out _saveControl);
        }

        private void Start()
        {
            BindComponents();
            LoadGameData();
            SetUIEvents();
            UpdateUI();

            _inventory.SubtractItem("Item1", 1);
        }

        private void UpdateUI()
        {
            _countLabel.text = _countData.Value.ToString("000");
        }

        private void SetUIEvents()
        {
            _addButton.onClick.AddListener(OnAddButtonDown);
            _subtractButton.onClick.AddListener(OnSubtractButtonDown);
            _saveButton.onClick.AddListener(SaveGameData);
        }


        private void BindComponents()
        {
            TryGetComponent(out _inventory);
            _canvas.transform.Find("Footer/Subtract Button")?.TryGetComponent(out _subtractButton);
            _canvas.transform.Find("Footer/Add Button")?.TryGetComponent(out _addButton);
            _canvas.transform.Find("Count Label")?.TryGetComponent(out _countLabel);
            _canvas.transform.Find("Footer/Save Button")?.TryGetComponent(out _saveButton);
            _canvas.transform.Find("Dialog")?.TryGetComponent(out _animation);
        }

        private void LoadGameData()
        {
            _saveControl.Load(_path);
        }

        private void SaveGameData()
        {
            _saveControl.Save(_path);
            _animation.Play();
        }

        private void OnAddButtonDown()
        {
            _countData.Value++;
            UpdateUI();
        }

        private void OnSubtractButtonDown()
        {
            _countData.Value = Mathf.Max(0, --_countData.Value);
            UpdateUI();
        }
    }
}
