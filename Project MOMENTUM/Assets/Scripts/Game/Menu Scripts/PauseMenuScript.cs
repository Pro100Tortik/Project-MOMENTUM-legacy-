using UnityEngine;

public class PauseMenuScript : MonoBehaviour
{
    [SerializeField] private UIManager UIManager;
    [SerializeField] private bool pressed = false;
    private Player _player;

    //public void ContinueGame() => _status.ContinueGame();

    public void RestartLevel()
    {
        if (pressed)
            return;

        pressed = true;
        GameManager.Instance.SetTimescale(1.0f, true);
        GameFunctions.RestartLevel();
    }

    //public void Settings() => _status.GoToSettings();

    public void ExitGame()
    {
        if (pressed)
            return;

        pressed = true;
        GameFunctions.ChangeLevel(0);
    }
}
