using UnityEngine;

public class GraplingHook : MonoBehaviour
{
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private float maxHookDistance = 15.0f;
    [SerializeField] private float cooldown = 5.0f;
    [SerializeField] private LineRenderer rope;
    [SerializeField] private Transform gunpoint, cam;
    private Vector3 _grapplePoint;
    private bool _isHooked;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse3))
        {
            StartGrapple();
        }

        if (Input.GetKeyUp(KeyCode.Mouse3))
        {
            StopGrapple();
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void StartGrapple()
    {
        RaycastHit hit;
        Ray ray = new Ray(cam.position, cam.forward);
        if (Physics.Raycast(ray, out hit, maxHookDistance, grappleLayer, QueryTriggerInteraction.Ignore))
        {
            _grapplePoint = hit.point;
            _isHooked = true;
            rope.positionCount = 2;
        }
    }

    private void StopGrapple()
    {
        _isHooked = false;
        rope.positionCount = 0;
    }

    private void DrawRope()
    {
        if (!_isHooked) return;

        rope.SetPosition(0, gunpoint.position);
        rope.SetPosition(1, _grapplePoint);
    }
}
