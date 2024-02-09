using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody _rigidbody;
    Vector3 _forward;

    [SerializeField] float speed = 4;

    void Awake()
    {
        TryGetComponent(out _rigidbody);
    }

    void OnEnable()
    {
        _forward = transform.forward;
        Destroy(gameObject, 5);
    }

    void FixedUpdate()
    {
        var deltaTime = Time.fixedDeltaTime;
        _rigidbody.MovePosition(_rigidbody.position + _forward * (speed * deltaTime));
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
