using UnityEngine.SceneManagement;

public static class LevelManager
{
    public static void RestartLevel() => SceneManager.LoadSceneAsync(GetCurrentScene().buildIndex);

    public static void ChangeLevel(string level) => SceneManager.LoadSceneAsync(level);

    public static void ChangeLevelWithIndex(int index) => SceneManager.LoadSceneAsync(index);

    public static Scene GetCurrentScene() => SceneManager.GetActiveScene();
}
