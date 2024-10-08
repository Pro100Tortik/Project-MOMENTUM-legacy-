using System.Collections.Generic;

namespace BehaviorTree
{
    public enum NodeState
    {
        Running,
        Success,
        Failure
    }

    public class Node
    {
        protected NodeState state;

        public Node parent;
        protected List<Node> children = new List<Node>();

        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        public Node() => parent = null;

        public Node(List<Node> children)
        {
            foreach (Node child in children)
                Attach(child);
        }

        protected void Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public virtual NodeState Evaluate() => NodeState.Failure;

        public Node GetRootNode()
        {
            Node node = parent;
            while (node != null)
            {
                if (node.parent == null)
                    break;

                node = node.parent;
            }
            return node;
        }

        public void SetData(string key, object value) => _dataContext[key] = value;

        public object GetData(string key)
        {
            if (_dataContext.TryGetValue(key, out object value))
                return value;

            Node node = parent;

            while (node != null)
            {
                value = node.GetData(key);

                if (value != null)
                    return value;

                node = node.parent;
            }

            return null;
        }

        public bool ClearData(string key)
        {
            if (_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }

            Node node = parent;

            while (node != null)
            {
                bool cleared = ClearData(key);

                if (cleared)
                    return true;

                node = node.parent;
            }

            return false;
        }
    }
}