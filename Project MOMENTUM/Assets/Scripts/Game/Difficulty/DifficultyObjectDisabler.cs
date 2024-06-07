using UnityEngine;

public class DifficultyObjectDisabler : MonoBehaviour
{
    [SerializeField] private bool multiplayerOnly;
    [SerializeField] private GameDifficultiesWithFlags spawnDifficulties = GameDifficultiesWithFlags.CanIPlayDaddy;

    private void Start()
    {
        if (!spawnDifficulties.HasFlag((GameDifficultiesWithFlags)GameManager.Instance.GetDifficultyLevel()))
        {
            gameObject.SetActive(false);
        }
    }
}
