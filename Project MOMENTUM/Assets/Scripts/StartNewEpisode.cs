using UnityEngine;

public class StartNewEpisode : MonoBehaviour
{
    public void NewEpisode(string levelName)
    {
        GameFunctions.ChangeLevel(levelName);

        // Reset player inventory etc
    }
}
