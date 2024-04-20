using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class BasicCommands : MonoBehaviour
{
    private AmbientMode _ambientMode;
    private Color _temp;

    private void Start()
    {
        InitializeBasicCommands();
    }

    private void InitializeBasicCommands()
    {
        DeveloperConsole.RegisterCommand("fps_max", "<int>", "Set the maximum FPS (frames per second) value.", args =>
        {
            Application.targetFrameRate = int.Parse(args[1]);
        });

        DeveloperConsole.RegisterCommand("fullbright", "", "Make everything pitch white.", args =>
        {
            if (RenderSettings.ambientLight != Color.white || RenderSettings.ambientMode != AmbientMode.Flat)
            {
                _temp = RenderSettings.ambientLight;
                RenderSettings.ambientLight = Color.white;

                _ambientMode = RenderSettings.ambientMode;
                RenderSettings.ambientMode = AmbientMode.Flat;
            }
            else
            {
                RenderSettings.ambientMode = _ambientMode;
                RenderSettings.ambientLight = _temp;
            }
        });

        DeveloperConsole.RegisterCommand("log", "<string>", "Make a log in the console.", args =>
        {
            Debug.Log(string.Join(" ", args));
        });

        DeveloperConsole.RegisterCommand("gravity", "<float>", "Changes gravity value.", args =>
        {
            float parameter = float.Parse(args[1]);
            Physics.gravity = new Vector3(Physics.gravity.x, -parameter, Physics.gravity.z);
        });

        DeveloperConsole.RegisterCommand("map", "<string>", "Loads map.", args =>
        {
            SceneManager.LoadSceneAsync(args[1]);
        });

        DeveloperConsole.RegisterCommand("quit", "", "Quit the application.", args =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        });
    }
}
