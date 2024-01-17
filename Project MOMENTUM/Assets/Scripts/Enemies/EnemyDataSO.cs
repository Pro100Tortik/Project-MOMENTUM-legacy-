using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Enemy/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Enemy Speeds")]
    public float patrolSpeed = 6.0f;
    public float chaseSpeed = 10.0f;

    [Header("Enemy FOV")]
    [Range(0, 360)] public float FOV = 100.0f;
    public float sightDistance = 40.0f;
    public int health = 40;

    [Header("Combat")]
    public float reactionTime = 0.5f;
    public float moveTime = 0.4f;

    //public int killCost;
}
