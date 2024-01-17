using UnityEngine;

[RequireComponent(typeof(DifficultyObjectDisabler))]
public class HealthArmorPickup : MonoBehaviour
{
    [SerializeField] private bool health = true;
    [SerializeField] private ArmorType armorType;
    [SerializeField] private int value;
    public bool Health => health;
    public ArmorType GetArmorType() => armorType;
    public int GetHealValue() => value;
}
