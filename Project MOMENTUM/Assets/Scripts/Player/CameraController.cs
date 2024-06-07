using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    [Header("Settings")]
    [SerializeField] private float sensitivity = 1.0f;
    [SerializeField] private Vector2 yRotationClamp = new Vector2(-90.0f, 90.0f);

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Camera cam;
    [SerializeField] private Camera weaponCam;
    [SerializeField] private Transform head;
    [SerializeField] private Transform orientation;
    private SettingsSaver _setting;

    private float _yaw, _pitch;

    private void Start()
    {
        _setting = SettingsSaver.Instance;
        sensitivity = _setting.GameSettings.Sensitivity;
        _yaw = transform.rotation.eulerAngles.y;
        _pitch = transform.rotation.eulerAngles.x;

        DeveloperConsole.RegisterCommand("fov", "<float>", "Changes field of view.", args =>
        {
            cam.fieldOfView = float.Parse(args[1]);
        });

        DeveloperConsole.RegisterCommand("view_model_fov", "<float>", "Changes view of weapon.", args =>
        {
            weaponCam.fieldOfView = float.Parse(args[1]);
        });
    }

    private void LateUpdate()
    {
        transform.position = head.position;

        if (player.CanReadInputs())
        {
            sensitivity = _setting.GameSettings.Sensitivity;

            MoveCamera();
        }

        transform.localRotation = Quaternion.Euler(_pitch, _yaw, 0);
    }

    private void MoveCamera()
    {
        _yaw += Input.GetAxisRaw("Mouse X") * sensitivity;
        _pitch += -Input.GetAxisRaw("Mouse Y") * sensitivity;

        _pitch = Mathf.Clamp(_pitch, yRotationClamp.x, yRotationClamp.y);
        //_yaw = _yaw < -180.0f ? _yaw = 180.0f : _yaw > 180.0f ? _yaw = -180.0f : _yaw;
        _yaw = _yaw.Wrap(-180f, 180f);

        orientation.localRotation = Quaternion.Euler(0, _yaw, 0);
    }

    public void DeadCamera()
    {
        transform.localRotation = Quaternion.Euler(_pitch, _yaw, 90);
    }

    public void RotateCamera(float x) => _yaw = x;

    public void RotateCamera(float x, float y)
    {
        _yaw = x;
        _pitch = y;
    }
}
