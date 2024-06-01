using BehaviorTree;
using UnityEngine;

namespace ProjectMOMENTUM
{
    public class CheckEnemyInFOV : Node
    {
        public CheckEnemyInFOV(Transform eyesPos, float viewRadius, float viewAngle,
            LayerMask targetMask, LayerMask levelMask)
        {
            _eyesPos = eyesPos;
            _viewRadius = viewRadius;
            _viewAngle = viewAngle;
            _targetMask = targetMask;
            _levelMask = levelMask;
        }

        private Transform _eyesPos;
        private float _viewRadius = 20.0f;
        private float _viewAngle = 100.0f;
        private LayerMask _targetMask;
        private LayerMask _levelMask;
        private float _timer;

        private Transform _target;
        public bool CanSeeTarget { get; private set; }

        public override NodeState Evaluate()
        {
            if (_timer >= 0.3f)
            {
                _timer = 0;
                FindVisibleTargets();
            }
            else
            {
                _timer += Time.deltaTime;
            }

            if (_target != null)
            {
                state = NodeState.Success;
                return state;
            }

            state = NodeState.Failure;
            return state;
        }

        private void FindVisibleTargets()
        {
            Collider[] targetsInViewRadius = Physics.OverlapSphere(_eyesPos.position, _viewRadius, _targetMask);

            foreach (Collider target in targetsInViewRadius)
            {
                Transform thisTarget = target.transform;
                Vector3 dirToTarget = (thisTarget.position - _eyesPos.position).normalized;
                float distanceToTarget = Vector3.Distance(_eyesPos.position, thisTarget.position);
                if (Vector3.Angle(_eyesPos.forward, dirToTarget) < _viewAngle / 2)
                {
                    if (!Physics.Raycast(_eyesPos.position, dirToTarget, distanceToTarget, _levelMask))
                    {
                        GetRootNode().SetData("target", _target);
                        _target = thisTarget;

                        CanSeeTarget = true;
                        return;
                    }
                    //else
                    //{
                    //    if (_forgetTarget)
                    //        parent.parent.ClearData("target");
                    //}
                }
            }
            CanSeeTarget = false;
        }
    }
}
