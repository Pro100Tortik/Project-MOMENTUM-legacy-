using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsMultiplayer { get; private set; } = false;

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
            //Debug.Log("Yep another GameManager");
            Destroy(gameObject);
        }
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);

        IsMultiplayer = false;
    }

    private void Start()
    {
        DeveloperConsole.RegisterCommand("map", "<string>", "Load any level by name.", args =>
        {
            GameFunctions.ChangeLevel(args[1]);
        });
    }
}
