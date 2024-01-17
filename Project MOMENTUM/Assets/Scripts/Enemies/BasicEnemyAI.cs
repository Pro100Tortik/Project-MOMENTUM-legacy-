using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(DifficultyObjectDisabler))]
public class BasicEnemyAi : MonoBehaviour
{
    [SerializeField] private EnemyDataSO enemyData;
    [SerializeField] private EnemyState currentState = EnemyState.Idle;
    [SerializeField] private Transform head;
    [SerializeField] private Transform body;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private EnemyHealth enemyHealth;

    private EnemyFOV _enemyFOV;
    private NavMeshAgent _agent;

    private Transform _target;
    private Vector3 _lastKnowPos;

    private float _moveTimer;
    private float _reactionTimer;
    private float _attackTimer;

    private bool _isAwake = false;

    private float GetDistanceToTarget() => Vector3.Distance(transform.position, _target.position);

    private bool CanAttack() => GetDistanceToTarget() <= 5 && _enemyFOV.CanSeeTarget;

    private void Awake()
    {
        _enemyFOV = GetComponent<EnemyFOV>();
        _agent = GetComponent<NavMeshAgent>();
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;
    }

    private void OnEnable() => enemyHealth.OnDamaged += SearchAttacker;

    private void OnDisable() => enemyHealth.OnDamaged -= SearchAttacker;

    private void Update()
    {
        DecideWhatToDo();

        if (enemyHealth._isDead)
        {
            currentState = EnemyState.Dead;
            return;
        }

        rigidBody.position = transform.position + Vector3.up;

        if (_enemyFOV.CanSeeTarget)
        {
            _target = _enemyFOV.TargetPosition;
            _isAwake = true;
            currentState = EnemyState.Chase;
        }

        if (_target != null)
        {
            LookAt(_target.position);
        }
    }

    private void FixedUpdate()
    {
        //_rb.MovePosition(_agent.destination);

        _moveTimer += Time.deltaTime;
        if (_isAwake)
        {
            //_attackTimer += Time.deltaTime;
            _reactionTimer += Time.deltaTime;
        }
    }

    private void MoveAgentTo(Vector3 position, float speed)
    {
        _agent.SetDestination(position + (Random.insideUnitSphere * 5));
        _agent.speed = speed;
        _moveTimer = 0;
    }

    private void LookAt(Vector3 whatPos)
    {
        if (whatPos == Vector3.zero) return;

        head.LookAt(whatPos + (Vector3.up * 1.0f));
        var lookDir = whatPos - body.position;
        lookDir.y = 0;
        var rotation = Quaternion.LookRotation(lookDir);
        rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, rotation, 0.2f);
    }

    private void DecideWhatToDo()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                //_isAwake = false;
                break;

            case EnemyState.Chase:
                ChaseTarget();
                break;

            case EnemyState.Search:
                MoveAgentTo(_lastKnowPos, enemyData.chaseSpeed);
                break;

            case EnemyState.Attack:
                // Attack target
                _reactionTimer = 0;
                break;

            case EnemyState.Dead:
                _agent.SetDestination(transform.position);

                rigidBody.isKinematic = false;
                rigidBody.useGravity = true;
                break;
        }
    }

    private void ChaseTarget()
    {
        if (_reactionTimer > enemyData.reactionTime && CanAttack())
        {
            currentState = EnemyState.Attack;
        }
        else if (_moveTimer > enemyData.moveTime)
        {
            MoveAgentTo(_target.position, enemyData.chaseSpeed);
        }
    }

    private void SearchAttacker(GameObject attacker)
    {
        if (attacker == null)
            return;

        _isAwake = true;
        _lastKnowPos = attacker.transform.position;
        LookAt(_lastKnowPos);
        currentState = EnemyState.Search;
    }

    private void Patrol()
    {
        if (_moveTimer > Random.Range(3, 5))
        {
            _agent.SetDestination(transform.position + (Random.insideUnitSphere * 10));
            _moveTimer = 0;
        }
    }

    //private void SearchPlayer()
    //{
    //    MoveAgentTo(_lastKnowPos, 10);

    //    if (_agent.remainingDistance < _agent.stoppingDistance)
    //    {
    //        _searchTimer += Time.deltaTime;

    //        if (_moveTimer < 3)
    //        {
    //            if (playerPos != null)
    //                LookAt(playerPos.transform.position);
    //        }

    //        if (_spinTimer > 1.5f)
    //        {
    //            LookAt(transform.position + (Random.insideUnitSphere * 10));
    //            _spinTimer = 0;
    //        }

    //        if (_searchTimer > 5)
    //        {
    //            state = EnemyState.Patrol;
    //            _searchTimer = 0;
    //        }
    //    }
    //}
}
