using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private SettingsSaver config;
    [SerializeField] private float sensitivity = 1.0f;

    [Header("References")]
    [SerializeField] private Transform head;
    [SerializeField] private Transform orientation;
    private ClientStatus _status;

    private float _yaw, _pitch;

    private void Awake()
    {
        _status = GetComponentInParent<ClientStatus>();
    }

    private void Start()
    {
        sensitivity = config.GameSettings.sensitivity;
        _yaw = transform.rotation.eulerAngles.y;
        _pitch = transform.rotation.eulerAngles.x;
    }

    private void LateUpdate()
    {
        transform.position = head.position;

        if (_status.CurrentClientState != PlayerState.Menus 
            && _status.CurrentClientState != PlayerState.Intermission)
        {
            MoveCamera();
        }

        transform.localRotation = Quaternion.Euler(_pitch, _yaw, 0);
    }

    private void MoveCamera()
    {
        _yaw += Input.GetAxisRaw("Mouse X") * sensitivity;
        _pitch += -Input.GetAxisRaw("Mouse Y") * sensitivity;

        _pitch = Mathf.Clamp(_pitch, -90.0f, 90.0f);
        _yaw = _yaw < -180.0f ? _yaw = 180.0f : _yaw > 180.0f ? _yaw = -180.0f : _yaw;

        orientation.localRotation = Quaternion.Euler(0, _yaw, 0);
    }

    public void DeadCamera()
    {
        transform.localRotation = Quaternion.Euler(_pitch, _yaw, 90);
    }

    public void RotateCamera(float x, float y)
    {
        _yaw = x;
        _pitch = y;
    }
}
