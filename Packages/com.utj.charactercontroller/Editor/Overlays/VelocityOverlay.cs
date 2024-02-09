using System;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace TinyCharacterControllerEditor.Overlays
{
    /// <summary>
    /// Displays an Overlay that lists the parameters of IBrain.
    /// This Overlay is created by <see cref="TccEditorTool"/>.
    /// </summary>
    [Overlay(defaultDisplay = false, displayName = "Tcc Brain Velocity Debug")]
    public class VelocityOverlay : Overlay
    {
        // Configuration for the icon
        private static Texture2D _icon;
        private static bool _preIconSkinIsPro;

        // The Brain to display content for, and a class to cache and notify changes in Brain's information
        private readonly BrainCachedData _cachedData;
        private readonly IBrain _brain;

        // Paths to resources

        private const string UxmlGuid = "25fd4ae4985d94917ba440d62aa1b847"; // BrainVelocityOverlay.uxml
        private const string IconGuidL = "f109490d1e76046cebfe4c3cbad73148"; // Controller_L.png
        private const string IconGuidD = "60e336ddbf3ab471f9b366d8d2df1b6f"; // Controller_D.png

        // UI elements
        private Label _currentMoveLabel;
        private Label _currentRotationLabel;
        private Label _additionalVelocity;
        private Label _controlDirection;
        private Label _localVelocity;
        private Label _turn;
        private Label _turnSpeed;
        private Label _turnDelta;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="brain">The Brain to monitor</param>
        public VelocityOverlay(IBrain brain)
        {
            _cachedData = new BrainCachedData();
            _brain = brain;
            displayName = "TCC Velocities";
            collapsedIcon = LoadIconTexture();
        }

        /// <summary>
        /// Loads the icon texture.
        /// The texture to load changes based on the skin.
        /// </summary>
        /// <returns>The icon texture</returns>
        private static Texture2D LoadIconTexture()
        {
            // Use the cached icon if already loaded, or if the skin has not changed
            if (_icon != null || EditorGUIUtility.isProSkin == _preIconSkinIsPro)
                return _icon;

            // Load and cache the icon.
            var iconPath = AssetDatabase.GUIDToAssetPath(EditorGUIUtility.isProSkin ? IconGuidD : IconGuidL);
            _icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            Assert.IsNotNull(_icon, $"Icon file not found. ({iconPath}");
            _preIconSkinIsPro = EditorGUIUtility.isProSkin;
            return _icon;
        }

        public override VisualElement CreatePanelContent()
        {
            try
            {
                var root = new VisualElement();
                var path = AssetDatabase.GUIDToAssetPath(UxmlGuid);
                var treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
                Assert.IsNotNull(treeAsset, $"tree asset not found. ({path}");
                treeAsset.CloneTree(root);

                // Set the minimum height when stored in the bar
                if (collapsed)
                    root.style.minHeight = 180;

                // Cache UI elements
                GatherElements(root);

                // Initialize UI content
                InitialElementUI();

                // Update the UI
                UpdateUi();

                // Register a schedule to automatically update the UI
                root.schedule.Execute(UpdateUi).Every(16);

                return root;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }

            return null;
        }

        /// <summary>
        /// Gathers a list of UIElements
        /// </summary>
        /// <param name="root">The element to gather from</param>
        private void GatherElements(VisualElement root)
        {
            _currentMoveLabel = root.Q<Label>("current-move-label");
            _currentRotationLabel = root.Q<Label>("current-rotation-label");
            _additionalVelocity = root.Q<Label>("additional-velocity-label");
            _controlDirection = root.Q<Label>("control-direction-label");
            _localVelocity = root.Q<Label>("local-velocity-label");
            _turn = root.Q<Label>("turn-label");
            _turnSpeed = root.Q<Label>("turn-speed-label");
            _turnDelta = root.Q<Label>("turn-delta-label");
        }

        /// <summary>
        /// Initializes the UI elements
        /// </summary>
        private void InitialElementUI()
        {
            _currentMoveLabel.text = "none";
            _currentRotationLabel.text = "none";
            _additionalVelocity.text = "(0.0, 0.0, 0.0)";
            _controlDirection.text = "(0.0, 0.0, 0.0)";
            _localVelocity.text = "(0.0, 0.0, 0.0)";
            _turn.text = "00";
            _turnSpeed.text = "0.0";
            _turnDelta.text = "0.0";
        }

        // Updates the UI
        private void UpdateUi()
        {
            // Skip processing if the game is not playing, or occasionally when Brain is null.
            if (EditorApplication.isPlaying == false || _brain == null)
                return;

            try
            {
                // Update the values and refresh UI if there are changes.

                if (_cachedData.SetMove(_brain.CurrentMove))
                    _currentMoveLabel.text = _brain.CurrentMove == null ? "none" : _brain.CurrentMove.GetType().Name;

                if (_cachedData.SetTurn(_brain.CurrentTurn))
                    _currentRotationLabel.text =
                        _brain.CurrentTurn == null ? "none" : _brain.CurrentTurn.GetType().Name;

                if (_cachedData.SetEffect(_brain.EffectVelocity))
                    _additionalVelocity.text = _brain.EffectVelocity.ToString("F1");

                if (_cachedData.SetControlVelocity(_brain.ControlVelocity))
                    _controlDirection.text = _brain.ControlVelocity.ToString("F1");

                if (_cachedData.SetLocalVelocity(_brain.LocalVelocity))
                    _localVelocity.text = _brain.LocalVelocity.ToString("F1");

                if (_cachedData.SetYawAngle(_brain.YawAngle))
                    _turn.text = _brain.YawAngle.ToString("00");

                if (_cachedData.SetTurnSpeed(_brain.TurnSpeed))
                    _turnSpeed.text = _brain.TurnSpeed.ToString("0.0");

                if (_cachedData.SetDeltaTurnAngle(_brain.DeltaTurnAngle))
                    _turnDelta.text = _brain.DeltaTurnAngle.ToString("0.0");
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }
    }

    /// <summary>
    /// Caches Brain data to reduce the frequency of UI updates, and detects changes in values.
    /// </summary>
    internal class BrainCachedData
    {
        // Values to cache

        private IMove _move;
        private ITurn _turn;
        private Vector3 _effect;
        private Vector3 _controlVelocity;
        private Vector3 _localVelocity;
        private float _yawAngle;
        private float _turnSpeed;
        private float _deltaTurn;

        // Methods to update the cache

        public bool SetMove(in IMove value) => IsChangeValueClass(ref _move, value);
        public bool SetTurn(in ITurn value) => IsChangeValueClass(ref _turn, value);
        public bool SetEffect(in Vector3 value) => IsChangeValue(ref _effect, value);
        public bool SetControlVelocity(in Vector3 value) => IsChangeValue(ref _controlVelocity, value);
        public bool SetLocalVelocity(in Vector3 value) => IsChangeValue(ref _localVelocity, value);
        public bool SetYawAngle(in float value) => IsChangeValue(ref _yawAngle, value);
        public bool SetTurnSpeed(in float value) => IsChangeValue(ref _turnSpeed, value);
        public bool SetDeltaTurnAngle(in float value) => IsChangeValue(ref _deltaTurn, value);


        /// <summary>
        /// Updates the value of a component. Returns true if the value has changed.
        /// </summary>
        /// <param name="obj">The variable to operate on</param>
        /// <param name="newValue">The new value</param>
        /// <typeparam name="T">Class inheriting IPriority</typeparam>
        /// <returns>True if the value has changed</returns>
        private static bool IsChangeValue<T>(ref T obj, in T newValue) where T : struct
        {
            var result = !newValue.Equals(obj);
            obj = newValue;
            return result;
        }

        /// <summary>
        /// Updates the control. Returns true if the value has changed.
        /// </summary>
        /// <param name="obj">The control to operate on</param>
        /// <param name="newValue">The new value</param>
        /// <typeparam name="T">Class inheriting IPriority</typeparam>
        /// <returns>True if the value has changed</returns>
        private static bool IsChangeValueClass<T>(ref T obj, in T newValue) where T : class
        {
            var result = obj == newValue;
            obj = newValue;
            return result;
        }
    }
}
