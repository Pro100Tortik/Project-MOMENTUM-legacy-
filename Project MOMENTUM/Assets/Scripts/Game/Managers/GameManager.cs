using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool IsPaused { get; private set; } = false;
    public bool IsMultiplayer { get; private set; } = false;
    public bool IsAutoRespawn { get; private set; } = false;
    public float RespawnTime { get; private set; } = 1.0f;

    private float _lastTimeScale;

    [SerializeField] private GameDifficulty difficulty = GameDifficulty.NotSoCasual;

    public GameDifficulty GetDifficultyLevel() => difficulty;

    public void SetDifficultyLevel(GameDifficulty difficulty) => this.difficulty = difficulty;

    public void SetTimescale(float value, bool isHost)
    {
        if (IsMultiplayer && !isHost)
            return;

        _lastTimeScale = Time.timeScale;
        Time.timeScale = value;
        IsPaused = Time.timeScale == 0;
    }

    public void Respawn()
    {
        if (IsMultiplayer)
        {
            // Handle respawn
        }
        else
        {
            // if no checkpoints
            // Well there is no :D
            GameFunctions.RestartLevel();
        }
    }

    private void Awake()
    {
        GameFunctions.DisableCursor();
    }

    private void Start()
    {
        DeveloperConsole.RegisterCommand("map", "<string>", "Load any level by name.", args =>
        {
            GameFunctions.ChangeLevel(args[1]);
        });
    }

    public void Pause(bool isHost)
    {
        if (IsMultiplayer && !isHost)
            return;

        SetTimescale(_lastTimeScale, isHost);
    }
}
