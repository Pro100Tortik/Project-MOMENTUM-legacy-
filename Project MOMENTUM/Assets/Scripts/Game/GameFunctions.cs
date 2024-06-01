using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameFunctions
{
    public static void RestartLevel() => ChangeLevel(GetCurrentScene().buildIndex);

    public static void ChangeLevel(string level) => SceneManager.LoadSceneAsync(level);

    public static void ChangeLevel(int index) => SceneManager.LoadSceneAsync(index);

    public static Scene GetCurrentScene() => SceneManager.GetActiveScene();

    public static void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
