using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TitleScreen : MonoBehaviour
{
    public UnityEvent OnEcsPressed;

    [Header("Main Menu Elements")]
    [SerializeField] private List<GameObject> menus;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject credits;
    private int _menuIndex = 0;
    private GameManager _gm;

    private void Awake()
    {
        Time.timeScale = 1.0f;

        EnableMenu(_menuIndex);
    }

    private void Start()
    {
        _gm = GameManager.Instance;
        GameFunctions.EnableCursor();
    }

    #region Button Functions
    public void SetDifficulty1() => _gm.SetDifficultyLevel(GameDifficulty.CanIPlayDaddy);
    public void SetDifficulty2() => _gm.SetDifficultyLevel(GameDifficulty.ICanDoIT);
    public void SetDifficulty3() => _gm.SetDifficultyLevel(GameDifficulty.NotSoCasual);
    public void SetDifficulty4() => _gm.SetDifficultyLevel(GameDifficulty.UltraViolence);
    public void SetDifficulty5() => _gm.SetDifficultyLevel(GameDifficulty.Nightmare);

    public void SinglePlayer() => NextMenu();

    public void NewGame() => NextMenu();

    public void MainMenu() => EnableMenu(0);

    public void Settings()
    {
        DisableMenus();
        settings.SetActive(true);
        _menuIndex = -1;
    }

    public void Credits()
    {
        DisableMenus();
        credits.SetActive(true);
        _menuIndex = -1;
    }

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEcsPressed?.Invoke();
            PreviousMenu();
        }
    }

    private void EnableMenu(int menuIndex)
    {
        DisableMenus();
        menus[menuIndex].SetActive(true);
    }

    private void DisableMenus()
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }
        settings.SetActive(false);
        credits.SetActive(false);
    }

    private void NextMenu()
    {
        _menuIndex++;
        EnableMenu(_menuIndex);
    }

    public void PreviousMenu()
    {
        if (_menuIndex < 0)
        {
            _menuIndex = 0;
        }
        else if (_menuIndex > 0)
        {
            _menuIndex--;
        }
        EnableMenu(_menuIndex);
    }
}
