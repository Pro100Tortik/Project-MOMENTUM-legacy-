using UnityEngine;

public class WeaponMovement : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float minimumMoveSpeed = 0.3f;
    [SerializeField] private float minimumSprintSpeed = 7;

    [Header("Weapon Bob with Delay")]
    [SerializeField] private Vector3 restPosition;
    [SerializeField] private float transitionSpeed = 3;
    [SerializeField] private float normalBobAmount = 0.015f;
    [SerializeField] private float sprintBobAmount = 0.05f;

    [Header("Sway Properties")]
    [SerializeField] private float swayAmount = 0.01f;
    [SerializeField] public float maxSwayAmount = 0.1f;
    [SerializeField] public float swaySmooth = 9f;
    [SerializeField] public AnimationCurve swayCurve;

    [Range(0f, 1f)]
    [SerializeField] public float swaySmoothCounteraction = 1f;

    [Header("Rotation")]
    [SerializeField] public float rotationSwayMultiplier = 1f;

    [Header("Position")]
    [SerializeField] public float positionSwayMultiplier = -1f;

    private float _timer = Mathf.PI / 2;
    private float _bobSpeed;
    private float _bobAmount;
    private bool _isSprint;
    private bool _isMoving;

    private SettingsSaver _settings;
    private Transform _weaponTransform;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector2 sway;

    private void Start() 
    {
        _settings = SettingsSaver.Instance;

        if (!_weaponTransform)
            _weaponTransform = transform;

        initialPosition = _weaponTransform.localPosition;
        initialRotation = _weaponTransform.localRotation;
    }

    private void Update()
    {
        _isMoving = player.playerController.Velocity.magnitude > minimumMoveSpeed && player.playerController.OnGround;
        _isSprint = player.playerController.Velocity.magnitude > minimumSprintSpeed;

        _bobSpeed = Mathf.Min(player.playerController.Velocity.magnitude, 10);
        _bobAmount = _isSprint ? sprintBobAmount : normalBobAmount;

        if (!player.CanReadInputs())
        {
            return;
        }

        WeaponSway();

        if (!_settings.GameSettings.EnableWeaponBob)
            return;

        WeaponBob();
    }

    private void WeaponBob()
    {
        if (player.playerController == null)
            return;

        if (player.playerController.IsSliding)
            return;

        if (_isMoving)
        {
            _timer += _bobSpeed * Time.deltaTime;

            Vector3 newPosition = new Vector3(Mathf.Cos(_timer) * _bobAmount,
                Mathf.Sin(_timer * 2) * _bobAmount + restPosition.y, restPosition.z);
            transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, Time.deltaTime * transitionSpeed);
        }
        else
        {
            _timer = Mathf.PI / 2;

            Vector3 newPosition = new Vector3(restPosition.x, restPosition.y, restPosition.z);
            transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, Time.deltaTime * transitionSpeed);
        }

        if (_timer > Mathf.PI * 2)
            _timer = 0;
    }


    private void Reset()
    {
        Keyframe[] ks = new Keyframe[] { new Keyframe(0, 0, 0, 2), new Keyframe(1, 1) };
        swayCurve = new AnimationCurve(ks);
    }

    private void WeaponSway()
    {
        if (!_settings.GameSettings.EnableWeaponSway)
            return;

        float mouseX = Input.GetAxis("Mouse X") * swayAmount;
        float mouseY = Input.GetAxis("Mouse Y") * swayAmount;

        sway = Vector2.MoveTowards(sway, Vector2.zero, swayCurve.Evaluate(Time.deltaTime * swaySmoothCounteraction * sway.magnitude * swaySmooth));
        sway = Vector2.ClampMagnitude(new Vector2(mouseX, mouseY) + sway, maxSwayAmount);

        _weaponTransform.localPosition = Vector3.Lerp(_weaponTransform.localPosition, new Vector3(sway.x, sway.y, 0) * positionSwayMultiplier + initialPosition, swayCurve.Evaluate(Time.deltaTime * swaySmooth));
        _weaponTransform.localRotation = Quaternion.Slerp(transform.localRotation, initialRotation * Quaternion.Euler(Mathf.Rad2Deg * rotationSwayMultiplier * new Vector3(-sway.y, sway.x, 0)), swayCurve.Evaluate(Time.deltaTime * swaySmooth));
    }
}
