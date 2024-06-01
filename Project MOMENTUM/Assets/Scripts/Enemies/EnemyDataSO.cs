using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Enemy/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Enemy Speeds")]
    public float patrolSpeed = 6.0f;
    public float chaseSpeed = 10.0f;

    [Header("Enemy FOV")]
    public float viewRange = 20f;
    public float viewAngle = 100f;
    public int health = 40;
}
