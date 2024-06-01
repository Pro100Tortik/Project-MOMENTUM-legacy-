using BehaviorTree;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectMOMENTUM
{
    public class TaskPatrol : Node
    {
        public TaskPatrol(Transform body, NavMeshAgent agent, float speed, Transform[] waypoints, float waitTime)
        {
            _body = body;
            _agent = agent;
            _speed = speed;
            _waypoints = waypoints;
            _waitTime = waitTime;
        }

        private Transform _body;
        private NavMeshAgent _agent;
        private float _speed;
        private Transform[] _waypoints;

        private int _currentWaypointIndex = 0;

        private float _waitTime = 2f;
        private float _waitCounter = 0;
        private bool _isWaiting = false;

        public override NodeState Evaluate()
        {
            if (_isWaiting)
            {
                _waitCounter += Time.deltaTime;

                if (_waitCounter >= _waitTime)
                    _isWaiting = false;
            }
            else
            {
                Transform waypoint = _waypoints[_currentWaypointIndex];
                if (Vector3.Distance(_agent.transform.position, waypoint.position.OnlyXZ()) < 0.1f)
                {
                    _agent.SetDestination(waypoint.position);
                    _agent.speed = 0;
                    _waitCounter = 0f;
                    _isWaiting = true;
                    _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
                }
                else
                {
                    _agent.SetDestination(waypoint.position);
                    _agent.speed = _speed;
                    Rotate(waypoint.position);
                }
            }

            state = NodeState.Running;
            return state;
        }

        private async void Rotate(Vector3 targetPos)
        {
            var lookDir = targetPos - _agent.transform.position;
            lookDir.y = 0;
            var rotation = Quaternion.LookRotation(lookDir);

            float time = 0;

            while (time < 1)
            {
                _body.rotation = Quaternion.Slerp(_body.rotation, rotation, time);
                time += Time.deltaTime * 4f;
                await Task.Yield();
            }
        }
    }
}
