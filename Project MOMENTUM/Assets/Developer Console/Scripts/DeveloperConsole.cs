using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DeveloperConsole : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Canvas consoleCanvas;
    [SerializeField] private TMP_Text consoleLogs;
    [SerializeField] private TMP_Text consolePredict;
    [SerializeField] private TMP_InputField consoleInput;

    [Header("Console Settings")]
    [SerializeField] private KeyCode consoleToggleKey = KeyCode.BackQuote;
    [SerializeField] private KeyCode consoleSubmitKey = KeyCode.Return;
    [SerializeField] private KeyCode consolePredictConfirmKey = KeyCode.Tab;
    [SerializeField] private int maxLogs = 128;

    private static List<ConsoleCommand> _commands = new List<ConsoleCommand>();
    private static Queue<string> _logs = new Queue<string>();
    private string _predictedCommand;

    public bool IsConsoleOpened { get; private set; }

    public static void RegisterCommand(string command, string syntax, string description, Action<string[]> callback)
    {
        ConsoleCommand com = new ConsoleCommand(command, syntax, description, callback);

        if (_commands.Contains(com))
            return;

        _commands.Add(com);
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

    public void ClearConsole()
    {
        _logs.Clear();
        consoleLogs.text = string.Empty;
    }

    public void PredictCommand(string command)
    {
        consolePredict.text = string.Empty;

        if (command == "")
        {
            return;
        }

        foreach (var cmd in _commands)
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

    private void Submit()
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

        UpdateLogs();

        _commands.Clear();
    }

    private void Start() => InitializeBasicCommands();

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

    private void InitializeBasicCommands()
    {
        RegisterCommand("clear", "", "Clear all logs from console window.", args =>
        {
            ClearConsole();
        });

        RegisterCommand("help", "", "Show all commands and their description.", args =>
        {
            _commands.Sort((x, y) => string.Compare(x.Command, y.Command));
            foreach (var command in _commands)
            {
                AddMessage($"{command.Command} {command.Syntax} - {command.Description}");
            }
        });
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

    private void ProcessCommand(string command)
    {
        if (command == "")
            return;

        foreach (var Command in _commands)
        {
            if (command.StartsWith(Command.Command))
            {
                string[] args = command.Substring(Command.Command.Length).Split(' ');

                Command.Callback(args);

                return;
            }
        }
    }

    private void EnqueueLog(string message)
    {
        if (_logs.Count >= maxLogs)
        {
            _logs.Dequeue();
        }
        _logs.Enqueue(message);
    }

    private void AddMessage(string message)
    {
        EnqueueLog($"{message}\n");
        UpdateLogs();
    }

    private void UpdateLogs()
    {
        if (_logs == null)
            return;

        consoleLogs.text = string.Empty;

        foreach (var log in _logs)
        {
            consoleLogs.text += log;
        }
    }
}
