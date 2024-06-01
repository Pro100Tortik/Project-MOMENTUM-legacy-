using System.Threading.Tasks;
using BehaviorTree;
using UnityEngine;

namespace ProjectMOMENTUM
{
    public class TaskLookAtTarget : Node
    {
        public TaskLookAtTarget(Transform body)
        {
            _body = body;
        }

        private Transform _body;

        public override NodeState Evaluate()
        {
            Transform target = GetData("target") as Transform;

            if (target != null)
            {
                Rotate(target.position);
            }

            state = NodeState.Running;
            return state;
        }

        private async void Rotate(Vector3 targetPos)
        {
            var lookDir = targetPos - _body.position;
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
