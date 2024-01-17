using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class CameraStrafeTilting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController controller;

    [Header("Settings")]
    [SerializeField] private float maxTiltAngle = 1.5f;
    [SerializeField] private float tiltSpeed = 5.0f;
    private ClientStatus _status;
    private float _roll;
    private bool _canTilt;

    private void Awake() => _status = GetComponentInParent<ClientStatus>();

    private void Update() => _canTilt = controller.PlayerVelocity.magnitude > 1.0f;

    private void LateUpdate()
    {
        CameraTilt();

        if (_status.CurrentClientState == PlayerState.Dead)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 90);
            return;
        }

        transform.localRotation = Quaternion.Euler(0, 0, _roll);
    }

    private void CameraTilt()
    {
        if (controller == null)
            return;

        float horizontalMovement = Input.GetAxisRaw("Horizontal");

        if (_status.CurrentClientState != PlayerState.Gameplay
            && _status.CurrentClientState != PlayerState.Intermission)
        {
            horizontalMovement = 0;
        }

        if (_canTilt && _status.CurrentClientState == PlayerState.Gameplay)
        {
            float targetTiltAngle = -horizontalMovement * maxTiltAngle;
            _roll = Mathf.Lerp(_roll, targetTiltAngle, Time.deltaTime * tiltSpeed);
        }
        else // if (controller.OnGround)
            _roll = Mathf.Lerp(_roll, 0f, Time.deltaTime * tiltSpeed);
    }
}
