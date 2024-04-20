using UnityEngine;

public class PauseMenuScript : MonoBehaviour
{
    [SerializeField] private bool pressed = false;
    private ClientStatus _status;

    private void Awake() => _status = GetComponentInParent<ClientStatus>();

    //public void ContinueGame() => _status.ContinueGame();

    public void RestartLevel()
    {
        if (pressed)
            return;

        pressed = true;
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
