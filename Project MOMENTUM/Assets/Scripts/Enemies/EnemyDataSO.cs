using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Enemy/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Enemy Speeds")]
    public float patrolSpeed = 6.0f;
    public float chaseSpeed = 10.0f;

    [Header("Enemy FOV")]
    public int health = 40;

    [Header("Combat")]
    public int reactionTime = 7;
    public int moveTicks = 15;

    //public int killCost;
}
