using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(DifficultyObjectDisabler))]
public class BasicEnemyAi : MonoBehaviour
{
    [SerializeField] private EnemyDataSO enemyData;
    [SerializeField] private EnemyState currentState = EnemyState.Idle;
    [SerializeField] private Transform head;
    [SerializeField] private Transform body;
    [SerializeField] private EnemyHealth enemyHealth;

    private EnemyFOV _enemyFOV;
    private NavMeshAgent _agent;

    private Transform _target;
    private Vector3 _lastKnownPos;

    private int _ticks;
    private float _reactionTimer;

    private bool _isAwake = false;

    private float GetDistanceToTarget() => _target == null ? _agent.stoppingDistance + 1 
        : Vector3.Distance(transform.position, _target.position);

    private bool CanAttack() => GetDistanceToTarget() <= _agent.stoppingDistance && _enemyFOV.CanSeeTarget;

    private void Awake()
    {
        _enemyFOV = GetComponent<EnemyFOV>();
        _agent = GetComponent<NavMeshAgent>();

        _ticks += Random.Range(-7, 11);
    }

    private void OnEnable() => enemyHealth.OnDamaged += SearchAttacker;

    private void OnDisable() => enemyHealth.OnDamaged -= SearchAttacker;

    private void FixedUpdate()
    {
        //_rb.MovePosition(_agent.destination);

        _ticks += 1;
        if (_isAwake)
        {
            //_attackTimer += Time.deltaTime;
            _reactionTimer += Time.deltaTime;
        }

        DecideWhatToDo();

        if (enemyHealth.IsDead)
        {
            currentState = EnemyState.Dead;
            return;
        }

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

    private void MoveAgentTo(Vector3 position, float speed, float radius)
    {
        if (_agent == null)
            return;

        _agent.SetDestination(position + (Random.insideUnitSphere * radius));
        _agent.speed = speed;
        _ticks = 0;
    }

    private void LookAt(Vector3 whatPos)
    {
        if (whatPos == Vector3.zero) return;

        if (enemyHealth.IsDead)
            return;

        head.LookAt(whatPos + (Vector3.up * 1.0f));
        var lookDir = whatPos - body.position;
        lookDir.y = 0;
        var rotation = Quaternion.LookRotation(lookDir);
        transform.rotation = rotation;
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
                MoveAgentTo(_lastKnownPos, enemyData.chaseSpeed, 3);
                break;

            case EnemyState.Attack:
                // Attack target
                _reactionTimer = 0;
                break;

            case EnemyState.Dead:
                _agent.SetDestination(transform.position);
                break;
        }
    }

    private void ChaseTarget()
    {
        if (_reactionTimer > enemyData.reactionTime && CanAttack())
        {
            currentState = EnemyState.Attack;
            _agent.SetDestination(transform.position);
        }
        else if (_ticks > enemyData.moveTicks)
        {
            MoveAgentTo(_target.position, enemyData.chaseSpeed, 1);
        }
    }

    private void SearchAttacker(GameObject attacker)
    {
        if (attacker == null)
            return;

        _isAwake = true;
        _lastKnownPos = attacker.transform.position;
        LookAt(_lastKnownPos);
        currentState = EnemyState.Search;
    }

    private void Patrol()
    {
        if (_ticks > enemyData.moveTicks)
        {
            _agent.SetDestination(transform.position + (Random.insideUnitSphere * 10));
            _ticks = 0;
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
