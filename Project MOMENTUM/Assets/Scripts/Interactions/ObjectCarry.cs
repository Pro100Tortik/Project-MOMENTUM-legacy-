using System;
using UnityEngine;

public class ObjectCarry : MonoBehaviour
{
    public event Action OnPikcup;
    public event Action OnRelease;

    [SerializeField] private float pickupRange = 5.0f;
    [SerializeField] private float carryRange = 2.5f;
    [SerializeField] private float carryMaxRange = 6.0f;
    [SerializeField] private float carryMaxWeight = 10.0f;
    [SerializeField] private float throwPower = 20.0f;
    [SerializeField] private float speed = 1000.0f;
    private float _restoreDrag;
    private Rigidbody _heldRB;
    public static bool HaveItem { get; private set; }

    private void Update()
    {
        HaveItem = _heldRB != null;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_heldRB != null)
            {
                Release(0);
            }
            else
            {
                PickUp();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (_heldRB != null)
            {
                Release(throwPower);
            }
        }
    }

    private void FixedUpdate()
    {
        if (_heldRB != null)
        {
            Vector3 target = transform.position + transform.forward * carryRange;
            Vector3 difference = target - _heldRB.position;
            _heldRB.AddForce(difference * speed);
            //_heldRB.rotation = transform.rotation;

            if (Vector3.Distance(_heldRB.position, transform.position + transform.forward * carryRange) > carryMaxRange)
            {
                Release(0);
            }
        }
    }

    private void PickUp()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, pickupRange))
        {
            hit.transform.TryGetComponent<Rigidbody>(out _heldRB);
            if (_heldRB != null)
            {
                if (_heldRB.mass > carryMaxWeight)
                {
                    return;
                }

                OnPikcup?.Invoke();
                //_heldRB.isKinematic = true;
                _heldRB.useGravity = false;
                _restoreDrag = _heldRB.drag;
                _heldRB.drag = 25;
            }
        }
    }

    private void Release(float power)
    {
        OnRelease?.Invoke();
        //_heldRB.isKinematic = false;
        _heldRB.useGravity = true;
        _heldRB.drag = _restoreDrag;
        _heldRB.AddForce(transform.forward * power, ForceMode.Impulse);
        _heldRB = null;
    }
}
