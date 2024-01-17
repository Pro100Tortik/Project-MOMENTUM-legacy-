using UnityEngine;

[RequireComponent(typeof(DifficultyObjectDisabler))]
public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private AmmoType ammoType;
    [SerializeField] private int ammoAmount;

    private void Start()
    {
        if (GameManager.Instance.GetDifficultyLevel() == GameDifficulty.Nightmare 
            || GameManager.Instance.GetDifficultyLevel() == GameDifficulty.CanIPlayDaddy)
        {
            ammoAmount *= 2;
        }
    }

    public AmmoType AmmoType => ammoType;

    public int AmmoAmount => ammoAmount;
}
