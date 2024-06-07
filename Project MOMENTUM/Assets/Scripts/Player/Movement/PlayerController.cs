using ProjectMOMENTUM;
using System;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    private enum MoveType
    {
        Walk,
        Ladder,
        Swim,
        Noclip,
        Spectate
    }

    public event Action<AudioClip> OnStep;
    public event Action<AudioClip> OnLadderMove;
    public event Action<AudioClip> OnJump;

    [SerializeField] private Transform model;

    [SerializeField] private Player player;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform head;
    [SerializeField] private Transform orientation;
    [SerializeField] private MoveType moveType;
    [SerializeField] private LayerMask crouchCheckLayers;
    [SerializeField] private LayerMask stepMask;

    [Header("Sounds")]
    [SerializeField] private AudioClip[] stepSounds;
    [SerializeField] private AudioClip[] ladderSounds;
    [SerializeField] private AudioClip[] waterSounds;
    [SerializeField] private AudioClip[] swimSounds;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private MovementCvars cvars;

    #region Dynamic Vars
    private float _inputForward;
    private float _inputRight;

    private float _currentAccel;
    #endregion

    private float _waterDepth;
    private float _stepTimer;
    private float _ladderTimer;
    private float _waterTimer;
    private float _swimTimer;
    private float _jumpTimer;
    private float _skipFrictionTimer;
    private float _slideTimer;

    private bool _wishJump;
    private bool _isCrouching;
    private bool _autoJump;
    private bool _isInWater;
    private bool _isGrounded;
    private bool _onLadder;
    private bool _wasCrouching;
    private bool _isSliding;
    private bool _died = false;
    private bool _isOnSlope;
    //private bool _wasInWater;

    private Rigidbody _rb;
    private BoxCollider _collider;

    private Vector3 _crouchHeadPos;
    private Vector3 _standingHeadPos;
    private Vector3 _currentHeadPos;
    private Vector3 _groundNormal;
    private Vector3 _ladderNormal;
    private Vector3 _moveDir;
    private Vector3 _vel;

    private Collider _water;

    public bool OnGround => _isGrounded && !Input.GetKey(KeyCode.Space);

    public bool IsSliding => _isGrounded && _isSliding;

    public Vector3 Velocity => _rb.velocity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        TryGetComponent<BoxCollider>(out _collider);

        _standingHeadPos = head.localPosition;
        _currentHeadPos = _standingHeadPos;
        _crouchHeadPos = new Vector3(_standingHeadPos.x, _standingHeadPos.y - cvars.crouchHeight * 0.5f, _standingHeadPos.z);
    }

    private void Start()
    {
        DeveloperConsole.RegisterCommand("noclip", "", "Turns off player collision and let you fly.", args =>
        {
            moveType = moveType == MoveType.Noclip ? MoveType.Walk : MoveType.Noclip;
        });

        DeveloperConsole.RegisterCommand("bhop", "", "Allow player hold space for autojump.", args =>
        {
            _autoJump = !_autoJump;
        });
    }

    private void Update()
    {
        if (!_died)
        {
            _collider.enabled = moveType != MoveType.Noclip;
        }

        GetMoveDir();

        _inputForward = player.CanReadInputs() ? Input.GetAxisRaw("Vertical") : 0;
        _inputRight = player.CanReadInputs() ? Input.GetAxisRaw("Horizontal") : 0;

        if (!player.CanReadInputs() || player.IsDead)
            return;

        JumpQueue();

        if (!_isSliding && !_wasCrouching)
            _currentAccel = cvars.acceleration * (Input.GetKey(KeyCode.LeftShift) ? cvars.shiftMultiplier : 1);

        _isCrouching = Input.GetKey(KeyCode.LeftControl);
    }

    private void FixedUpdate()
    {
        if (player.IsDead)
        {
            if (!_died)
                Die();
        }

        Movement();
        ResetJump();
        SkipFriction();
    }

    private void Die()
    {
        _died = true;
        head.localPosition = _crouchHeadPos;
        _collider.size = new Vector3(_collider.size.x, 0.2f, _collider.size.z);
        model.localScale = new Vector3(model.localScale.x, 0.2f, model.localScale.z);
    }

    private void GetMoveDir() => _moveDir = orientation.localRotation * new Vector3(_inputRight, 0, _inputForward).normalized;

    private void Movement()
    {
        _vel = _rb.velocity;

        Crouch();

        if (moveType != MoveType.Noclip)
        {
            if (!_onLadder && moveType == MoveType.Ladder)
            {
                moveType = MoveType.Walk;
            }

            if (!_died)
            {
                PlayWaterSound();
                PlayStepSound();
                PlayLadderSound();
                PlaySwimSound();
            }
        }

        switch (moveType)
        {
            case MoveType.Walk:
                FullWalkMove();
                //DetectStep();
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
        _isOnSlope = false;
    }

    private void DetectStep()
    {
        if (!_isGrounded)
            return;

        if (_wishJump)
            return;

        if (_vel.magnitude <= 0.2f)
            return;

        var extents = _collider.bounds.extents * 1.01f;
        extents.y = 0.15f;
        var nextPos = _rb.position + _vel * Time.deltaTime;
        var center = nextPos + new Vector3(0, _collider.bounds.size.y - extents.y, 0);
        var distance = 10f;

        if (Physics.BoxCast(center: center, halfExtents: extents, direction: Vector3.down,
            orientation: orientation.localRotation, maxDistance: distance, layerMask: stepMask,
            queryTriggerInteraction: QueryTriggerInteraction.Ignore, hitInfo: out RaycastHit hit))
        {
            if (!hit.collider.enabled || hit.point == Vector3.zero || hit.normal.y <= cvars.slopeLimit * Mathf.Deg2Rad)
            {
                return;
            }

            float magicNumber = _collider.size.y * 0.5f + 0.1f;
            var stepHeight = Mathf.Abs(hit.point.y - (_rb.position.y - magicNumber));

            //if (_rb.position.y > hit.point.y)
            //{
            //    stepHeight -= 2;
            //}

            if (stepHeight <= cvars.stepHeight)
            {
                _rb.position += Vector3.up * stepHeight;
            }
        }
    }

    private void SkipFriction()
    {
        if (_skipFrictionTimer > 0)
            _skipFrictionTimer -= Time.deltaTime;
        else
            _skipFrictionTimer = 0;
    }

    private void GroundMove()
    {
        if (!player.CanReadInputs() || player.IsDead)
            return;

        if (_wishJump)
        {
            Jump();
            _skipFrictionTimer = Time.deltaTime + 0.01f;
            return;
        }

        _moveDir = GetSlopeMoveDir(_moveDir);
        _vel += BhopPhysics.GroundAccelerate(_vel, _moveDir, cvars.groundSpeed, _currentAccel);
    }

    private void AirMove()
    {
        if (!player.CanReadInputs() || player.IsDead)
            return;

        _vel += BhopPhysics.AirAccelerate(_vel, _moveDir,
            cvars.airSpeed, cvars.airAcceleration, cvars.airSpeedCap);
    }

    private void FullWalkMove()
    {
        if (_isGrounded)
        {
            if (_isSliding)
            {
                SlideMovement();
            }

            if (_skipFrictionTimer > 0)
                return;

            _rb.drag = _isSliding ? cvars.slideFriction : cvars.groundFriction;
            _rb.useGravity = _isSliding;

            GroundMove();
        }
        else 
        {
            _rb.drag = 0;
            _rb.useGravity = true;

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
            if (player.CanReadInputs() && !player.IsDead)
                _moveDir = cam.forward * Input.GetAxisRaw("Vertical") + cam.right * Input.GetAxisRaw("Horizontal");

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
                RenderSettings.fog = true;
            }
            else
            {
                RenderSettings.fog = false;
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

        if (!player.CanReadInputs() || (_inputForward == 0 && _inputRight == 0))
        {
            _vel.y -= cvars.sinkSpeed;
        }

        if (player.CanReadInputs() && !player.IsDead)
        {
            if (_wishJump)
            {
                wishvel.y = cvars.waterSpeed * 0.7f;
            }

            // Jump out, ledge check
            if (_waterDepth <= cvars.waterLevelToJumpOut && _wishJump)
            {
                var extents = _collider.bounds.extents;
                extents.y = .1f;
                extents.x *= 1.1f;
                extents.z *= 1.1f;
                if (Physics.CheckBox(transform.position, extents, Quaternion.identity, 1 << 0, QueryTriggerInteraction.Ignore))
                {
                    _vel.y = cvars.waterJumpOutPower;
                }
            }

            wishdir = wishvel;
            wishspeed = wishdir.magnitude;

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
                    if (player.CanReadInputs())
                        _vel += deltaSpeed;
                }
            }
        }
        _rb.velocity = _vel;
    }

    private void FullNoclipMove()
    {
        _rb.drag = cvars.noclipFriction;
        _rb.useGravity = false;

        _moveDir = cam.forward * _inputForward + cam.right * _inputRight;

        _vel += BhopPhysics.GroundAccelerate(_vel, _moveDir, cvars.noclipSpeed, _currentAccel);

        _rb.velocity = _vel;
    }

    private void JumpQueue()
    {
        if (!player.CanReadInputs())
        {
            _wishJump = false;
            return;
        }

        if (_autoJump)
        {
            _wishJump = Input.GetKey(KeyCode.Space);
            return;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) && !_wishJump) _wishJump = true;
            else if (Input.GetKeyUp(KeyCode.Space)) _wishJump = false;
        }
    }

    private void Jump()
    {
        if (_jumpTimer > 0)
            return;

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

        head.localPosition = Vector3.Lerp(head.localPosition, _currentHeadPos, Time.deltaTime / 0.2f);

        if (_isCrouching && !_wasCrouching)
        {
            _currentHeadPos = _crouchHeadPos;

            if (_isGrounded)
                transform.position -= Vector3.up * 0.45f;

            model.localScale = new Vector3(model.localScale.x, cvars.crouchHeight * 0.5f, model.localScale.z);
            _collider.size = new Vector3(_collider.size.x, cvars.crouchHeight, _collider.size.z);

            StartSlide();

            _wasCrouching = true;
        }
        else if (CanUncrouch() && !_isCrouching && _wasCrouching)
        {
            _currentHeadPos = _standingHeadPos;

            if (_isGrounded)
                transform.position += Vector3.up * 0.45f;

            model.localScale = new Vector3(model.localScale.x, cvars.playerHeight * 0.5f, model.localScale.z);
            _collider.size = new Vector3(_collider.size.x, cvars.playerHeight, _collider.size.z);
            StopSlide();

            _wasCrouching = false;
        }

        if (_wasCrouching)
            _currentAccel = cvars.acceleration * cvars.duckMultiplier;
    }

    private void StartSlide()
    {
        if (!CanSlide() || (!_isGrounded && Mathf.Abs(_vel.y) < cvars.velocityToStop))
            return;

        if (_isGrounded)
        {
            _vel += _vel.OnlyXZ().normalized * _currentAccel * cvars.slideMultiplier;
        }

        _isSliding = true;

        _slideTimer = cvars.slideTime;
    }

    private void StopSlide()
    {
        _isSliding = false;
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.LowerCenter;
        style.fontSize = 40;
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), _vel.magnitude.ToString("0.0"), style);
    }

    private void SlideMovement()
    {
        // Cannot gain speed if moving upwards without slope
        if (!_isOnSlope || _rb.velocity.y > -0.1f)
        {
            _slideTimer -= Time.deltaTime;
        }
        else
        {
            _vel += GetSlopeMoveDir(_vel) * _currentAccel * cvars.slideMultiplier;
            _slideTimer = cvars.slideTime;
        }

        if (_vel.OnlyXZ().magnitude < cvars.velocityToStop || _slideTimer <= 0)
            StopSlide();
    }

    private bool CanSlide() => _vel.magnitude >= cvars.velocityToStart;

    private bool CanUncrouch() => moveType == MoveType.Noclip ? true : 
        !Physics.CheckSphere(transform.position + Vector3.up * cvars.checkHeight, cvars.checkRadius, crouchCheckLayers);

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

        if (_wasCrouching)
            return;

        if (Input.GetKey(KeyCode.LeftShift))
            return;

        if (Time.time > _stepTimer)
        {
            OnStep?.Invoke(stepSounds.GetRandomElement());

            _stepTimer = Time.time + 1f / 2.5f;
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
            OnStep?.Invoke(waterSounds.GetRandomElement());
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
            OnStep?.Invoke(swimSounds.GetRandomElement());
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
            OnLadderMove?.Invoke(ladderSounds.GetRandomElement());
            _ladderTimer = Time.time + 1f / Mathf.Min(Mathf.Max(_vel.magnitude, 1.5f), 2.5f);
        }
    }

    private Vector3 GetSlopeMoveDir(Vector3 dir) => Vector3.ProjectOnPlane(dir, _groundNormal).normalized;

    private void OnTriggerEnter(Collider other)
    {
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
            float angle = Vector3.Angle(Vector3.up, cp.normal);

            if (collision.transform.CompareTag("Ladder") && angle > cvars.slopeLimit)
            {
                _ladderNormal = cp.normal;
                _onLadder = true;
                moveType = MoveType.Ladder;
                return;
            }

            _isOnSlope = angle < cvars.slopeLimit && angle > 5;

            if (angle < cvars.slopeLimit)
            {
                _isGrounded = true;
                _groundNormal = cp.normal;
                return;
            }
        }
    }
}
