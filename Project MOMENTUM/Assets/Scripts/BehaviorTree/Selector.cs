using System.Collections.Generic;

namespace BehaviorTree
{
    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;

            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.Failure:
                        continue;

                    case NodeState.Success:
                        state = NodeState.Success;
                        return state;

                    case NodeState.Running:
                        state = NodeState.Running;
                        return state;

                    default:
                        continue;
                }
            }

            state = anyChildIsRunning ? NodeState.Running : NodeState.Success;
            return state;
        }
    }
}