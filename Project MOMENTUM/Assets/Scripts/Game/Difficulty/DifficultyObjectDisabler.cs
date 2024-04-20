using UnityEngine;

public class DifficultyObjectDisabler : MonoBehaviour
{
    [SerializeField] private bool multiplayerOnly;
    [SerializeField] private GameDifficultiesWithFlags spawnDifficulties = GameDifficultiesWithFlags.CanIPlayDaddy;

    private void Start()
    {
        if (!DifficultyFunctions.CanSpawn(GameManager.Instance.GetDifficultyLevel(), spawnDifficulties))
        {
            gameObject.SetActive(false);
        }
    }
}
