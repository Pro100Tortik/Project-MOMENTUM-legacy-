using UnityEngine;

public class ClientStatus : MonoBehaviour
{
    public bool IsDead { get; private set; } = false;
    public bool IsPaused { get; private set; } = false;

    [SerializeField] private DeveloperConsole console;
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject gameName;
    [SerializeField] private float gameOverRestartDelay = 1.0f;
    private float _gameOverTimer;
    private PlayerHealth playerHealth;
    private GameManager _gm;

    public bool Crutch()
    {
        if (console.IsConsoleOpened) return false;

        return true;
    }

    public bool CanReadInputs()
    {
        //if (IsDead) return false;

        if (IsPaused) return false;

        if (console.IsConsoleOpened) return false;

        return true;
    }

    public void Pause()
    {
        IsPaused = !IsPaused;

        DisableAllMenus();

        //if (!_gm.IsMultiplayer)
        //{
            Time.timeScale = IsPaused ? 0.0f : 1.0f;
        //}

        if (IsPaused)
        {
            GameFunctions.EnableCursor();
            pauseMenu.SetActive(true);
        }
        else
        {
            GameFunctions.DisableCursor();
        }
    }

    public void Settings()
    {
        DisableAllMenus();
        settingsMenu.SetActive(true);
    }

    public void GoBack()
    {
        if (console.IsConsoleOpened)
        {
            console.ToggleConsole();
            return;
        }

        if (settingsMenu.activeInHierarchy)
        {
            EnablePauseMenu();
        }
        else
        {
            Pause();
        }
    }

    private void Awake()
    {
        _gameOverTimer = gameOverRestartDelay;
        playerHealth = GetComponentInChildren<PlayerHealth>();
        _gm = GameManager.Instance;

        IsPaused = false;

        Time.timeScale = IsPaused ? 0.0f : 1.0f;
        GameFunctions.DisableCursor();
        DisableAllMenus();
    }

    private void Start() => playerHealth.OnDeath += Die;
    
    private void OnDestroy() => playerHealth.OnDeath -= Die;

    private void Die() => IsDead = true;

    private void Update()
    {
        GameOverScreen();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBack();
        }

        if (!IsPaused)
        {
            if (console.IsConsoleOpened)
            {
                GameFunctions.EnableCursor();
            }
            else
            {
                GameFunctions.DisableCursor();
            }
        }
    }

    private void EnablePauseMenu()
    {
        DisableAllMenus();
        pauseMenu.SetActive(true);
    }

    private void DisableAllMenus()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        gameName.SetActive(IsPaused);
    }

    private void GameOverScreen()
    {
        if (!IsDead)
            return;

        gameOverMenu.SetActive(true);
        UI.SetActive(false);

        if (_gameOverTimer > 0)
            _gameOverTimer -= Time.fixedDeltaTime;

        if (IsPaused)
            return;

        if (Input.GetMouseButtonDown(0) && _gameOverTimer <= 0)
        {
            if (!_gm.IsMultiplayer)
                GameFunctions.RestartLevel();
        }
    }
}
