using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event Action<AudioClip> OnStep;
    public event Action<AudioClip> OnLadderMove;
    public event Action<AudioClip> OnJump;
    public event Action OnSecretFound;

    [SerializeField] private bool AutoJump;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform head;
    [SerializeField] private Transform orientation;
    [SerializeField] private MoveType moveType;
    [SerializeField] private LayerMask crouchCheckLayers;

    [Header("Sounds")]
    [SerializeField] private AudioClip[] stepSounds;
    [SerializeField] private AudioClip[] ladderSounds;
    [SerializeField] private AudioClip[] waterSounds;
    [SerializeField] private AudioClip[] swimSounds;
    [SerializeField] private AudioClip jumpSound;

    [SerializeField] private MovementCvars cvars;

    #region Private Variables
    private float _inputForward;
    private float _inputRight;

    private float _waterDepth;
    private float _friction;
    private float _currentAccel;
    private float _stepTimer;
    private float _ladderTimer;
    private float _waterTimer;
    private float _swimTimer;
    private float _jumpTimer;
    private float _skipFrictionTimer;

    private bool _isInWater;
    private bool _wishJump;
    private bool _isGrounded;
    private bool _onLadder;
    private bool _isCrouching;
    private bool _wasCrouching;
    private bool _died = false;
    //private bool _wasInWater;

    private Rigidbody _rb;
    private BoxCollider _boxCollider;

    private Vector3 _groundNormal;
    private Vector3 _ladderNormal;
    private Vector3 _moveDir;
    private Vector3 _vel;

    private ClientStatus _status;
    private Collider _water;
    #endregion

    public bool OnGround => _isGrounded && !Input.GetKey(KeyCode.Space);

    public Vector3 PlayerVelocity => _rb.velocity;

    private bool CanAccelerate()
    {
        if (_died)
            return false;

        if (_status.CurrentClientState == PlayerState.Menus)
            return false;

        if (_status.CurrentClientState == PlayerState.Intermission)
            return false;

        return true;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        _status = GetComponentInParent<ClientStatus>();

        TryGetComponent<BoxCollider>(out _boxCollider);
    }

    private void Start()
    {
        DeveloperConsole.RegisterCommand(new ConsoleCommand("autojump", "", "Enables autohump.", args =>
        {
            AutoJump = !AutoJump;
        }));
    }

    private void Update()
    {
        if (!_died)
        {
            _boxCollider.enabled = moveType != MoveType.Noclip;
        }

        GetMoveDir();

        _inputForward = CanAccelerate() ? Input.GetAxisRaw("Vertical") : 0;
        _inputRight = CanAccelerate() ? Input.GetAxisRaw("Horizontal") : 0;

        if (!CanAccelerate())
            return;

        JumpQueue();

        _currentAccel = cvars.acceleration * (Input.GetKey(KeyCode.LeftShift) ? cvars.shiftMultiplier : 1);

        _isCrouching = Input.GetKey(KeyCode.LeftControl);
    }

    private void FixedUpdate()
    {
        Movement();
        ResetJump();
        SkipFriction();

        //if (!status.Noclip)
        //{
        //    moveType = MoveType.Walk;
        //    return;
        //}

        //if (moveType != MoveType.Noclip)
        //    moveType = MoveType.Noclip;
    }

    private void Die()
    {
        _died = true;
        head.localPosition = Vector3.up * -0.5f;
    }

    private void GetMoveDir() => _moveDir = orientation.localRotation 
        * new Vector3(_inputRight, 0, _inputForward).normalized;

    private void Movement()
    {
        if (_status.CurrentClientState == PlayerState.Dead && !_died)
        {
            Die();
        }

        Crouch();

        _vel = _rb.velocity;

        if (moveType != MoveType.Noclip)
        {
            if (!_onLadder && moveType == MoveType.Ladder)
            {
                moveType = MoveType.Walk;
            }

            if (!_died)
            {
                PlayStepSound();
                PlayLadderSound();
                PlayWaterSound();
                PlaySwimSound();
            }
        }

        switch (moveType)
        {
            case MoveType.Walk:
                FullWalkMove();
                break;

            case MoveType.Ladder:
                FullLadderMove();
                break;

            case MoveType.Noclip:
                FullNoclipMove();
                return;

            case MoveType.Swim:
                FullWaterMove();
                break;
        }
        WaterCheck();

        _groundNormal = Vector3.zero;
        _ladderNormal = Vector3.zero;
        _isGrounded = false;
        _onLadder = false;
    }

    private void SkipFriction()
    {
        if (_skipFrictionTimer > 0)
        {
            _skipFrictionTimer -= Time.deltaTime;
        }
        else
        {
            _skipFrictionTimer = 0;
        }
    }

    private void GroundMove()
    {
        if (CanAccelerate())
        {
            if (_wishJump)
            {
                Jump();
                _skipFrictionTimer = 0.03f;
                return;
            }

            if (_skipFrictionTimer > 0)
                return;
        }

        _rb.drag = _friction;

        _rb.useGravity = false;

        if (!CanAccelerate())
            return;

        _moveDir = Vector3.Cross(Vector3.Cross(_groundNormal, _moveDir), _groundNormal);
        _vel += BhopPhysics.GroundAccelerate(_vel, _moveDir, cvars.groundSpeed, _currentAccel);
    }

    private void AirMove()
    {
        _rb.drag = 0;
        _rb.useGravity = true;

        if (!CanAccelerate())
            return;

        _vel += BhopPhysics.AirAccelerate(_vel, _moveDir,
            cvars.airSpeed, cvars.airAcceleration, cvars.airSpeedCap);
    }

    private void FullWalkMove()
    {
        if (_isGrounded)
        {
            GroundMove();
        }
        else 
        { 
            AirMove(); 
        }

        _rb.velocity = _vel;
    }

    private void FullLadderMove()
    {
        _rb.useGravity = false;
        _rb.drag = 0;
        moveType = MoveType.Ladder;

        if (_died)
        {
            _vel = _ladderNormal;
            moveType = MoveType.Walk;
            _onLadder = false;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            _vel = _ladderNormal * cvars.climbingSpeed;
            moveType = MoveType.Walk;
            _onLadder = false;
        }
        else
        {
            if (CanAccelerate())
            {
                _moveDir = cam.forward * Input.GetAxisRaw("Vertical") + cam.right * Input.GetAxisRaw("Horizontal");
            }

            Vector3 speed = _moveDir.normalized * cvars.climbingSpeed;

            if (speed.magnitude != 0)
            {
                Vector3 vel = speed;
                Vector3 tmp = Vector3.zero;
                tmp.y = 1;
                Vector3 perp = Vector3.Cross(tmp, _ladderNormal).normalized;
                float normal = Vector3.Dot(vel, _ladderNormal);
                Vector3 cross = _ladderNormal * normal;
                Vector3 lateral = vel - cross;
                tmp = Vector3.Cross(_ladderNormal, perp);

                _vel = lateral + -normal * tmp;

                if (_isGrounded && normal > 0)
                    _vel += cvars.climbingSpeed * _ladderNormal;
            }
            else
                _vel = Vector3.zero;
        }

        _rb.velocity = _vel;
    }

    private void WaterCheck()
    {
        _waterDepth = _isInWater ? 0.1f : 0;
        Vector3 headPoint = head.position;
        if (_water != null)
        {
            if (_water.bounds.Contains(headPoint))
            {
                _waterDepth = 1.0f;
            }
            else
            {
                Vector3 closestPoint = _water.ClosestPoint(headPoint);
                float dist = Vector3.Distance(headPoint, closestPoint);
                _waterDepth = Mathf.Max(0.1f, head.localPosition.y + 0.5f - dist / head.localPosition.y);
            }
        }

        if (_waterDepth >= cvars.waterLevelToSwim)
            moveType = MoveType.Swim;
        else if (moveType == MoveType.Swim)
            moveType = MoveType.Walk;
    }

    private void FullWaterMove()
    {
        Vector3 wishvel, wishdir;
        float wishspeed;
        float speed, newspeed, addspeed, accelspeed;

        _rb.drag = cvars.waterFriction;

        _rb.useGravity = false;

        wishvel = _inputForward * cam.forward + _inputRight * cam.right;
        wishvel.Normalize();
        wishvel *= cvars.waterSpeed;

        if (!CanAccelerate() || (_inputForward == 0 && _inputRight == 0))
        {
            _vel.y -= cvars.sinkSpeed;
        }

        if (CanAccelerate())
        {
            if (_wishJump)
            {
                wishvel.y = cvars.waterSpeed * 0.7f;
            }
        }

        // Jump out, ledge check
        if (CanAccelerate())
        {
            if (_waterDepth <= cvars.waterLevelToJumpOut && _wishJump)
            {
                var extents = _boxCollider.bounds.extents;
                extents.y = .1f;
                extents.x *= 1.1f;
                extents.z *= 1.1f;
                if (Physics.CheckBox(transform.position, extents, Quaternion.identity, 1 << 0, QueryTriggerInteraction.Ignore))
                {
                    _vel.y = cvars.waterJumpOutPower;
                }
            }
        }

        wishdir = wishvel;
        wishspeed = wishdir.magnitude;

        //if (wishspeed > cvars.waterSpeed)
        //{
        //    wishvel *= cvars.maxSpeed / cvars.waterSpeed;
        //    wishspeed = cvars.maxSpeed;
        //}

        wishspeed *= 0.7f;

        speed = _vel.magnitude;
        if (speed > 0)
        {
            newspeed = speed - Time.deltaTime * cvars.waterSpeed * cvars.waterFriction;
            if (newspeed < 0.1f)
            {
                newspeed = 0;
            }
            //_vel *= newspeed / speed;
        }
        else
        {
            newspeed = 0;
        }

        if (wishspeed >= 0.1f)
        {
            addspeed = wishspeed - newspeed;
            if (addspeed > 0)
            {
                wishvel.Normalize();
                accelspeed = _currentAccel * 2 * wishspeed * Time.deltaTime;
                if (accelspeed > addspeed)
                {
                    accelspeed = addspeed;
                }

                Vector3 deltaSpeed = accelspeed * wishvel;
                if (CanAccelerate())
                    _vel += deltaSpeed;
            }
        }
        _rb.velocity = _vel;
    }

    private void FullNoclipMove()
    {
        _rb.drag = cvars.noclipFriction;
        _rb.useGravity = false;

        _moveDir = cam.forward * Input.GetAxisRaw("Vertical") + cam.right * Input.GetAxisRaw("Horizontal");

        if (!CanAccelerate())
            return;

        _vel += BhopPhysics.GroundAccelerate(_vel, _moveDir, cvars.noclipSpeed, _currentAccel);
        _rb.velocity = _vel;
    }

    private void JumpQueue()
    {
        if (!CanAccelerate())
        {
            _wishJump = false;
            return;
        }

        if (AutoJump)
        {
            _wishJump = Input.GetKey(KeyCode.Space);
            return;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) && !_wishJump)
            {
                _wishJump = true;
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                _wishJump = false;
            }
        }
    }

    private void Jump()
    {
        if (_jumpTimer > 0)
            return;

        if (!CanAccelerate())
            return;

        _isGrounded = false;
        _rb.drag = 0;
        _vel.y = cvars.jumpForce;
        OnJump?.Invoke(jumpSound);
        _jumpTimer = 0.15f;
        _wishJump = false;
    }

    private void Crouch()
    {
        if (_died)
            return;

        if (_isCrouching)
        {
            head.localPosition = Vector3.up * 0.25f; // crouch
            _boxCollider.size = new Vector3(_boxCollider.size.x, cvars.crouchHeight, _boxCollider.size.z);
            _wasCrouching = true;
        }
        else if (CanUncrouch() && !_isCrouching)
        {
            head.localPosition = Vector3.up * 0.75f;
            _boxCollider.size = new Vector3(_boxCollider.size.x, cvars.playerHeight, _boxCollider.size.z);
            _wasCrouching = false;
        }

        if (_wasCrouching)
            _currentAccel *= cvars.duckMultiplier;
    }

    private bool CanUncrouch()
    {
        if (moveType == MoveType.Noclip)
            return true;

        return !Physics.CheckSphere(head.position + 
            Vector3.up * cvars.checkHeight, cvars.checkRadius, crouchCheckLayers);
    }

    private void ResetJump()
    {
        if (_jumpTimer > 0)
            _jumpTimer -= Time.deltaTime;
        else
            _jumpTimer = 0;
    }

    private void PlayStepSound()
    {
        if (!_isGrounded)
            return;

        if (_vel.magnitude < 0.3f)
            return;

        if (Time.time > _stepTimer)
        {
            OnStep?.Invoke(stepSounds[UnityEngine.Random.Range(0, stepSounds.Length)]);
            _stepTimer = Time.time + 1f / (_wasCrouching ? 1.5f : Input.GetKey(KeyCode.LeftShift) ? 1.9f : 2.5f);
        }
    }

    private void PlayWaterSound()
    {
        if (!_isGrounded)
            return;

        if (!_isInWater)
            return;

        if (_vel.magnitude < 0.3f)
            return;

        if (Time.time > _waterTimer)
        {
            OnStep?.Invoke(waterSounds[UnityEngine.Random.Range(0, waterSounds.Length)]);
            _waterTimer = Time.time + 1f / (_wasCrouching ? 1.5f : Input.GetKey(KeyCode.LeftShift) ? 1.9f : 2.5f);
        }
    }

    private void PlaySwimSound()
    {
        if (!_isInWater)
            return;

        if (_waterDepth < cvars.waterLevelToSwim) 
            return;

        if (_vel.magnitude < 0.3f)
            return;

        if (Time.time > _swimTimer)
        {
            OnStep?.Invoke(swimSounds[UnityEngine.Random.Range(0, swimSounds.Length)]);
            _swimTimer = Time.time + 1f / 1.9f;
        }
    }

    private void PlayLadderSound()
    {
        if (_isGrounded)
            return;

        if (!_onLadder)
            return;

        if (_vel.magnitude < 0.3f)
            return;

        if (Time.time > _ladderTimer)
        {
            OnLadderMove?.Invoke(ladderSounds[UnityEngine.Random.Range(0, ladderSounds.Length)]);
            _ladderTimer = Time.time + 1f / Mathf.Min(Mathf.Max(_vel.magnitude, 1.5f), 2.5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SecretTrigger secret = other.GetComponent<SecretTrigger>();
        if (secret != null)
        {
            OnSecretFound?.Invoke();
            secret.SecretFound();
        }

        if (moveType == MoveType.Noclip)
            return;

        if (other.GetComponent<Water>())
        {
            if (!_water)
                _water = other;

            _isInWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Water>())
        {
            if (_water)
                _water = null;

            _isInWater = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (moveType == MoveType.Noclip)
            return;

        if (moveType == MoveType.Swim)
            return;

        foreach (ContactPoint cp in collision.contacts)
        {
            if (collision.transform.CompareTag("Ladder") &&
                cp.normal.y < Mathf.Sin(cvars.slopeLimit * Mathf.Deg2Rad))
            {
                _ladderNormal = cp.normal;
                _onLadder = true;
                moveType = MoveType.Ladder;
                return;
            }

            if (cp.normal.y > Mathf.Sin(cvars.slopeLimit * Mathf.Deg2Rad))
            {
                _friction = collision.transform.CompareTag("Ice") ? cvars.iceFriction : cvars.groundFriction;
                _isGrounded = true;
                _groundNormal = cp.normal;
                return;
            }
        }
    }
}
