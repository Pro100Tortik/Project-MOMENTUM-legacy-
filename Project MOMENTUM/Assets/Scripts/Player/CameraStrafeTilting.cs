using UnityEngine;

public class CameraStrafeTilting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;

    [Header("Settings")]
    [SerializeField] private float maxTiltAngle = 1.5f;
    [SerializeField] private float tiltSpeed = 5.0f;
    private float _roll;
    private bool _canTilt;

    private void Update() => _canTilt = player.playerController.Velocity.magnitude > 1.0f;

    private void LateUpdate()
    {
        CameraTilt();

        if (player.IsDead)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 90);
            return;
        }

        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, _roll);
    }

    private void CameraTilt()
    {
        if (player.playerController == null)
            return;

        float horizontalMovement = Input.GetAxisRaw("Horizontal");

        if (!player.CanReadInputs())
        {
            horizontalMovement = 0;
        }

        if (_canTilt && player.CanReadInputs())
        {
            float targetTiltAngle = -horizontalMovement * maxTiltAngle;
            _roll = Mathf.Lerp(_roll, targetTiltAngle, Time.deltaTime * tiltSpeed);
        }
        else // if (controller.OnGround)
            _roll = Mathf.Lerp(_roll, 0f, Time.deltaTime * tiltSpeed);
    }
}
