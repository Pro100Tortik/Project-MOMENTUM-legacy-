using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    #region GameStates
    [SerializeField] private GameState currentGameState;
    public GameState GetCurrentGameState() => currentGameState;
    public void StartLevel() => currentGameState = GameState.Level;
    public void StartIntermission() => currentGameState = GameState.Intermission;
    #endregion

    #region Difficulties
    [SerializeField] private GameDifficulty difficulty = GameDifficulty.NotSoCasual;
    public GameDifficulty GetDifficultyLevel() => difficulty;
    public void SetDifficultyNoob() => difficulty = GameDifficulty.CanIPlayDaddy;
    public void SetDifficultyEasy() => difficulty = GameDifficulty.ICanDoIT;
    public void SetDifficultyNormal() => difficulty = GameDifficulty.NotSoCasual;
    public void SetDifficultyTrue() => difficulty = GameDifficulty.UltraViolence;
    public void SetDifficultyNightmare() => difficulty = GameDifficulty.Nightmare;
    #endregion

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Yep another GameManager");
            Destroy(gameObject);
        }
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }
}
