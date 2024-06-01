using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;

namespace ProjectMOMENTUM
{
    public class TaskGoToTarget : Node
    {
        public TaskGoToTarget(NavMeshAgent agent, float speed, float timeForMove, float stoppingDistance)
        {
            _agent = agent;
            _speed = speed;
            _timeForMove = timeForMove;
            _stoppingDistance = stoppingDistance;
        }

        private NavMeshAgent _agent;
        private float _speed;
        private float _stoppingDistance;
        private float _timeForMove;
        private float _moveTimer;

        public override NodeState Evaluate()
        {
            Transform target = GetData("target") as Transform;

            if (target != null)
            {
                _moveTimer += Time.deltaTime;

                if (_moveTimer >= _timeForMove)
                {
                    _moveTimer = 0;
                    if (Vector3.Distance(_agent.transform.position, target.position.OnlyXZ()) > _stoppingDistance)
                    {
                        _agent.SetDestination(target.position);
                        _agent.speed = _speed;
                    }
                    else
                    {
                        _agent.SetDestination(_agent.transform.position);
                        _agent.speed = 0;
                    }
                }
            }

            state = NodeState.Running;
            return state;
        }
    }
}
