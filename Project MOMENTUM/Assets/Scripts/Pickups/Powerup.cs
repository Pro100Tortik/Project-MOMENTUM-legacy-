using UnityEngine;

[RequireComponent(typeof(DifficultyObjectDisabler))]
public class Powerup : MonoBehaviour
{
    [SerializeField] private PowerupType powerupType;
    public PowerupType GetPowerupType => powerupType;
}
