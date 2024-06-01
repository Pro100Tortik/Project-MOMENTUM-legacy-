using System.Collections.Generic;
using UnityEngine.AI;
using BehaviorTree;
using UnityEngine;

namespace ProjectMOMENTUM
{
    public class BasicBT : BehaviorTree.Tree
    {
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private Transform body;
        [SerializeField] private EnemyDataSO enemyData;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private LayerMask levelMask;
        [SerializeField] private LayerMask targetMask;

        protected override Node SetupTree()
        {
            Node root = new Selector(new List<Node>
            {
                new Sequence(new List<Node>
                {
                    new CheckEnemyInFOV(body, 20, 100, targetMask, levelMask),
                    new TaskGoToTarget(agent, enemyData.chaseSpeed, 0.4f, 3f)
                }),
                //new TaskPatrol(body, agent, enemyData.patrolSpeed, waypoints, 3f),
            });
            return root;
        }
    }
}
