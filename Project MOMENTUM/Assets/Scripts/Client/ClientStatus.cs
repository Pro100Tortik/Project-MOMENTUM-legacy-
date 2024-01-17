using UnityEngine;

public class ClientStatus : MonoBehaviour
{
    [SerializeField] private PlayerState playerState = PlayerState.Gameplay;
    [SerializeField] private MenuState menuState = MenuState.Pause;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject gameName;
    [SerializeField] private GameObject intermissionScreen;
    [SerializeField] private float gameOverRestartDelay = 1.0f;
    private float _gameOverTimer;
    private PlayerHealth playerHealth;

    #region Readonly Variables
    public PlayerState CurrentClientState => playerState;
    #endregion

    #region Changing Variables
    public void Die() => playerState = PlayerState.Dead;

    public void ContinueGame() => playerState = PlayerState.Gameplay;

    public void GoToPause() => menuState = MenuState.Pause;

    public void GoToSettings() => menuState = MenuState.Settings;
    #endregion

    private void Awake()
    {
        _gameOverTimer = gameOverRestartDelay;
        playerHealth = GetComponentInChildren<PlayerHealth>();
    }

    private void Start() => playerHealth.OnDeath += Die;

    private void OnDestroy() => playerHealth.OnDeath -= Die;

    private void Update()
    {
        GameStates();
        MenuStates();
        GameOverScreen();
        Intermission();

        gameOverMenu.SetActive(playerState == PlayerState.Dead);
        intermissionScreen.SetActive(playerState == PlayerState.Intermission);

        // Time.timeScale = gameState == GameState.Menus ? 0.0f : 1.0f;

        //if (gameState == PlayerState.Dead)
        //    return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (playerState == PlayerState.Intermission)
                return;

            if (playerState == PlayerState.Dead)
                return;

            if (playerState != PlayerState.Menus)
            {
                playerState = PlayerState.Menus;
                GoToPause();
            }
            else
            {
                if (menuState != MenuState.Pause)
                {
                    GoToPause();
                }
                else
                {
                    ContinueGame();
                }
            }
        }
    }

    private void GameStates()
    {
        switch (playerState)
        {
            case PlayerState.Menus:
                EnableCursor();
                break;

            case PlayerState.Gameplay:
            case PlayerState.Intermission:
            case PlayerState.Dead:
                DisableCursor();
                break;
        }
    }

    private void MenuStates()
    {
        gameName.SetActive(playerState == PlayerState.Menus);
        pauseMenu.SetActive(playerState == PlayerState.Menus && menuState == MenuState.Pause);
        settingsMenu.SetActive(playerState == PlayerState.Menus && menuState == MenuState.Settings);
    }

    private void GameOverScreen()
    {
        if (playerState != PlayerState.Dead)
            return;

        if (_gameOverTimer > 0)
            _gameOverTimer -= Time.fixedDeltaTime;

        if (Input.GetMouseButtonDown(0) && _gameOverTimer <= 0)
            LevelManager.RestartLevel();
    }

    private void Intermission()
    {
        if (playerState != PlayerState.Intermission)
        {
            return;
        }
    }

    private void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
