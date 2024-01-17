using UnityEngine;
using UnityEngine.Events;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private UnityEvent OnStartPressed;
    [SerializeField] private MainMenuState menuState;

    [Header("Main Menu Elements")]
    [SerializeField] private GameObject gameInfo;
    [SerializeField] private GameObject titleMenu;
    [SerializeField] private GameObject chooseMenu;
    [SerializeField] private GameObject difficultySelectMenu;
    [SerializeField] private GameObject episodeSelectMenu;
    [SerializeField] private GameObject levelSelectMenu;
    [SerializeField] private GameObject newGameMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject credits;
    private bool _startNewEpisode;
    private GameManager _gm;

    private void Awake()
    {
        Time.timeScale = 1.0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start() => _gm = GameManager.Instance;

    #region Button Functions
    public void GoToMainMenu() => menuState = MainMenuState.Title;

    public void GoToSelectMenu() => menuState = MainMenuState.AnotherMenuYep; // Choose to start new game or play unlocked levels

    public void StartNewGame() => _startNewEpisode = true;

    public void SelectLevel() => _startNewEpisode = false;

    public void ChangeMenu()
    {
        if (_startNewEpisode)
            menuState = MainMenuState.NewGame;
        else
            menuState = MainMenuState.EpisodeSelect;
    }

    public void GoToEpisodeSelection() => menuState = MainMenuState.EpisodeSelect;

    public void GoToLevelSelection() => menuState = MainMenuState.LevelSelect;

    public void GoToDifficultySelect() => menuState = MainMenuState.DifficultySelect;

    public void GoToSettings() => menuState = MainMenuState.Settings;

    public void GoToCredits() => menuState = MainMenuState.Credits;

    // Difficulties
    public void SetDifficulty1() => _gm.SetDifficultyNoob();
    public void SetDifficulty2() => _gm.SetDifficultyEasy();
    public void SetDifficulty3() => _gm.SetDifficultyNormal();
    public void SetDifficulty4() => _gm.SetDifficultyTrue();
    public void SetDifficulty5() => _gm.SetDifficultyNightmare();

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
            Application.Quit();
    }
    #endregion

    private void Update()
    {
        if (menuState == MainMenuState.PressStart)
        {
            if (Input.anyKeyDown)
            {
                OnStartPressed?.Invoke();
                menuState = MainMenuState.Title;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuState == MainMenuState.LevelSelect)
            {
                menuState = MainMenuState.EpisodeSelect;
            }
            else if (menuState == MainMenuState.EpisodeSelect || menuState == MainMenuState.NewGame)
            {
                menuState = MainMenuState.DifficultySelect;
            }
            else if (menuState == MainMenuState.DifficultySelect)
            {
                menuState = MainMenuState.AnotherMenuYep;
            }
            else if (menuState != MainMenuState.Title)
            {
                menuState = MainMenuState.Title;
            }
            else if (menuState == MainMenuState.Title)
            {
                menuState = MainMenuState.PressStart;
            }
        }

        gameInfo.SetActive(menuState != MainMenuState.LevelSelect 
            && menuState != MainMenuState.PressStart);
        titleMenu.SetActive(menuState == MainMenuState.Title);
        chooseMenu.SetActive(menuState == MainMenuState.AnotherMenuYep);
        difficultySelectMenu.SetActive(menuState == MainMenuState.DifficultySelect);
        episodeSelectMenu.SetActive(menuState == MainMenuState.EpisodeSelect);
        levelSelectMenu.SetActive(menuState == MainMenuState.LevelSelect);
        newGameMenu.SetActive(menuState == MainMenuState.NewGame);
        settingsMenu.SetActive(menuState == MainMenuState.Settings);
        credits.SetActive(menuState == MainMenuState.Credits);
    }
}
