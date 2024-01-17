using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChangeManager : MonoBehaviour
{
    public bool IsLoaded = false;
    [SerializeField] private bool muteSound;

    public void RestartLevel() => StartCoroutine(FadeToSceneAsync(LevelManager.GetCurrentScene().name));

    public void ChangeLevel(string levelname) => StartCoroutine(FadeToSceneAsync(levelname));

    private IEnumerator FadeToSceneAsync(string levelname)
    {
        yield return new WaitUntil(() => LevelTransition.Instance.FadeCompleted);
        LevelTransition.Instance.MuteSound = muteSound;
        LevelTransition.Instance.FadeOut();
        yield return new WaitUntil(() => LevelTransition.Instance.FadeCompleted);

        AsyncOperation loading = SceneManager.LoadSceneAsync(levelname);
        if (!loading.isDone)
        {
            yield return null;
        }
    }
}
