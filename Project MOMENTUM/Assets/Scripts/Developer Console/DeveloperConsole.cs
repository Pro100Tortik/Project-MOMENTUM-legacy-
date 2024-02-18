using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class DeveloperConsole : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Canvas consoleCanvas;
    [SerializeField] private TMP_Text consoleLogs;
    [SerializeField] private TMP_Text consolePredict;
    [SerializeField] private TMP_InputField consoleInput;

    [Header("Inputs")]
    [SerializeField] private KeyCode consoleToggleKey = KeyCode.BackQuote;
    [SerializeField] private KeyCode consoleSubmitKey = KeyCode.Return;
    [SerializeField] private KeyCode consolePredictConfirmKey = KeyCode.Tab;
    private string _predictedCommand;
    private static List<ConsoleCommand> commands = new List<ConsoleCommand>();

    public bool IsConsoleOpened { get; private set; }

    public static void RegisterCommand(ConsoleCommand command)
    {
        if (commands.Contains(command))
            return;

        commands.Add(command);
    }

    public void ToggleConsole()
    {
        consoleCanvas.gameObject.SetActive(!consoleCanvas.gameObject.activeInHierarchy);
        IsConsoleOpened = consoleCanvas.gameObject.activeInHierarchy;

        if (consoleCanvas.gameObject.activeInHierarchy)
        {
            consoleInput.ActivateInputField();
        }
    }

    public void ClearConsole() => consoleLogs.text = string.Empty;

    public void PredictCommand(string command)
    {
        consolePredict.text = string.Empty;

        if (command == "")
        {
            return;
        }

        foreach (var cmd in commands)
        {
            if (cmd.Command.StartsWith(command))
            {
                if (cmd.Command.Contains(command))
                {
                    consolePredict.text = $"{cmd.Command} {cmd.Syntax}";
                    _predictedCommand = cmd.Command;
                    return;
                }
            }
        }
    }

    public void Submit()
    {
        AddMessage("> " + consoleInput.text);
        ProcessCommand(consoleInput.text);
        consoleInput.text = string.Empty;
        consoleInput.ActivateInputField();
    }

    private void Awake()
    {        
        Application.logMessageReceivedThreaded += HandleLog;
        consoleInput.onValueChanged.AddListener(PredictCommand);
        consoleCanvas.gameObject.SetActive(false);
        IsConsoleOpened = false;
        _predictedCommand = "help";
    }

    private void Start() => InitializeBasicCommands();

    private void InitializeBasicCommands()
    {
        RegisterCommand(new ConsoleCommand("clear", "", "Clear all logs from console window.", args =>
        {
            ClearConsole();
        }));

        RegisterCommand(new ConsoleCommand("log", "<string>","Make a log in the console.", args =>
        {
            Debug.Log(string.Join(" ", args));
        }));

        RegisterCommand(new ConsoleCommand("gravity", "<float>", "Changes gravity value.", args =>
        {
            float parameter = float.Parse(args[1]);
            Physics.gravity = new Vector3(Physics.gravity.x, -parameter, Physics.gravity.z);
        }));

        RegisterCommand(new ConsoleCommand("map", "<string>", "Changes map.", args =>
        {
            string map = args[1];
            LevelManager.ChangeLevel(map);
        }));

        RegisterCommand(new ConsoleCommand("help", "", "Show all commands and their description.", args =>
        {
            foreach (var command in commands)
            {
                AddMessage($"{command.Command} {command.Syntax} - {command.Description}");
            }
        }));

        RegisterCommand(new ConsoleCommand("quit", "", "Quit the application.", args =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }));
    }

    private void HandleLog(string logMessage, string stackTrace, LogType type)
    {
        string message = $"[{type}]: {logMessage}";
        string coloredmessage = "";
        if (type == LogType.Log)
        {
            coloredmessage = message;
        }
        else if (type == LogType.Warning)
        {
            coloredmessage = $"<color=yellow>{message}</color>";
        }
        else if (type == LogType.Error)
        {
            coloredmessage = $"<color=red>{message}</color>";
        }

        AddMessage(coloredmessage);
    }

    private void Update()
    {
        if (Input.GetKeyDown(consoleToggleKey))
        {
            ToggleConsole();
        }

        if (consoleCanvas.gameObject.activeInHierarchy)
        {
            if (Input.GetKeyDown(consoleSubmitKey))
            {
                if (consoleInput.text.Length <= 0)
                {
                    return;
                }

                Submit();
            }

            if (Input.GetKeyDown(consolePredictConfirmKey))
            {
                consoleInput.text = _predictedCommand;
                consoleInput.MoveTextEnd(false);
            }
        }
    }

    private void OnDestroy()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
        consoleInput.onValueChanged.RemoveAllListeners();
    }

    private void ProcessCommand(string command)
    {
        if (command == "")
            return;

        foreach (var Command in commands)
        {
            if (command.StartsWith(Command.Command))
            {
                string[] args = command.Substring(Command.Command.Length).Split(' ');

                Command.Callback(args);

                return;
            }
        }
    }

    private void AddMessage(string message) => consoleLogs.text += $"{message}\n";
}
