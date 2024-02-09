using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.SceneManagement.Samples
{
    public class PlayerControl : MonoBehaviour
    {
        [SerializeField] private float _speed = 3;

        private PlayerInput _input;
        private CharacterController _controller;
        private InputAction _leftStick;
        private Transform _cameraTransform;

        private void Awake()
        {
            TryGetComponent(out _controller);
            TryGetComponent(out _input);
        }

        private void Start()
        {
            _leftStick = _input.actions["Move"];
            _cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            var stick = _leftStick.ReadValue<Vector2>();
            var direction = Quaternion.AngleAxis(_cameraTransform.rotation.eulerAngles.y, Vector3.up) * new Vector3(stick.x, 0, stick.y);
            var velocity = direction * _speed;
            _controller.SimpleMove(velocity * _speed);
        }
    }
}
