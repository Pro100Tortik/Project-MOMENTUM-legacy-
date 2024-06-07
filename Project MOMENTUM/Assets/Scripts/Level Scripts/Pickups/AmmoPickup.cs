using UnityEngine;

[RequireComponent(typeof(DifficultyObjectDisabler))]
public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private AmmoType ammoType;
    [SerializeField] private int ammoAmount;

    private void Start()
    {
        GameDifficulty currentDifficulty = GameManager.Instance.GetDifficultyLevel();
        if (currentDifficulty == GameDifficulty.Nightmare 
            || currentDifficulty == GameDifficulty.CanIPlayDaddy)
        {
            ammoAmount *= 2;
        }
    }

    public AmmoType AmmoType => ammoType;

    public int AmmoAmount => ammoAmount;
}
