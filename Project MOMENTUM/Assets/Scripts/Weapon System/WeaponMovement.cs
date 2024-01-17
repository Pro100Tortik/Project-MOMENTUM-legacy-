using UnityEngine;

public class WeaponMovement : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private float minimumMoveSpeed = 0.3f;
    [SerializeField] private float minimumSprintSpeed = 7;

    [Header("Weapon Bob with Delay")]
    [SerializeField] private Vector3 restPosition;
    [SerializeField] private float transitionSpeed = 3;
    [SerializeField] private float normalBobAmount = 0.015f;
    [SerializeField] private float sprintBobAmount = 0.05f;
    private float _timer = Mathf.PI / 2;
    private float _bobSpeed;
    private float _bobAmount;
    private bool _isSprint;
    private bool _isMoving;
    private ClientStatus _status;

    private void Awake()
    {
        _status = GetComponentInParent<ClientStatus>();
    }

    private void Update()
    {
        _isMoving = controller.PlayerVelocity.magnitude > minimumMoveSpeed && controller.OnGround;
        _isSprint = controller.PlayerVelocity.magnitude > minimumSprintSpeed;

        _bobSpeed = Mathf.Min(controller.PlayerVelocity.magnitude, 10);
        _bobAmount = _isSprint ? sprintBobAmount : normalBobAmount;

        if (_status.CurrentClientState != PlayerState.Gameplay)
        {
            return;
        }

        WeaponBob();
    }

    private void WeaponBob()
    {
        if (controller == null)
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
}
