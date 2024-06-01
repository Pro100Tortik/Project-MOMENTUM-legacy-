using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class DeveloperConsole : Singleton<DeveloperConsole>
{
    [Header("References")]
    [SerializeField] private PredictionPanels panels;

    [Header("UI Elements")]
    [SerializeField] private Canvas consoleCanvas;
    [SerializeField] private TMP_Text consoleLogs;
    [SerializeField] private TMP_InputField consoleInput;

    [Header("Console Settings")]
    [SerializeField] private int maxLogs = 128;
    [SerializeField] private int maxInputCommandMemory = 32;

    [Header("Keybinds")]
    [SerializeField] private KeyCode consoleToggleKey = KeyCode.BackQuote;
    [SerializeField] private KeyCode consoleSubmitKey = KeyCode.Return;
    [SerializeField] private KeyCode consoleTabKey = KeyCode.Tab;
    [SerializeField] private KeyCode consoleLeftKey = KeyCode.LeftArrow;
    [SerializeField] private KeyCode consoleRightKey = KeyCode.RightArrow;
    [SerializeField] private KeyCode consoleUpKey = KeyCode.UpArrow;
    [SerializeField] private KeyCode consoleDownKey = KeyCode.DownArrow;

    private static List<ConsoleCommand> _commands = new List<ConsoleCommand>();
    private static Queue<string> _logs = new Queue<string>();
    private static Queue<string> _lastCommands = new Queue<string>();
    private List<string> _predictedCommands = new List<string>();
    private int _currentCommand = -1;
    private int _predictedCommandIndex = -1;
    private bool _isSearching;

    public bool IsConsoleOpened { get; private set; }

    public static void RegisterCommand(string command, string syntax, string description, Action<string[]> callback)
    {
        ConsoleCommand com = new ConsoleCommand(command, syntax, description, callback);

        if (_commands.Contains(com))
            return;

        _commands.Add(com);
    }

    public void AllowPrediction(string da) => _isSearching = false;

    public void ToggleConsole()
    {
        consoleCanvas.gameObject.SetActive(!consoleCanvas.gameObject.activeInHierarchy);
        IsConsoleOpened = consoleCanvas.gameObject.activeInHierarchy;

        if (consoleCanvas.gameObject.activeInHierarchy)
        {
            UpdateLogs();
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
        if (consoleInput.text.Contains(' '))
        {
            _isSearching = false;
        }

        if (_isSearching)
            return;

        panels.ClearPanels();
        _predictedCommands.Clear();

        if (command == "")
        {
            return;
        }

        List<string> added = new List<string>();

        foreach (var cmd in _commands)
        {
            // Find all commands that starts with command text
            if (!cmd.Command.StartsWith(command))
                continue;

            if (added.Contains(cmd.Command))
                continue;

            added.Add(cmd.Command);
            _predictedCommands.Add(cmd.Command);
            panels.AddPanel(cmd.Command, cmd.Syntax);
        }

        panels.Resize();
    }

    private void Awake()
    {        
        consoleInput.onValueChanged.AddListener(PredictCommand);
        consoleCanvas.gameObject.SetActive(false);
        IsConsoleOpened = false;

        UpdateLogs();

        _commands.Clear();
    }

    private void Start()
    {
        InitializeBasicCommands();
        Application.logMessageReceivedThreaded += HandleLog;
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

                Enqueue(_lastCommands, maxInputCommandMemory, consoleInput.text);

                _currentCommand = -1;
                _predictedCommandIndex = -1;
                _isSearching = false;

                Submit();
            }

            SearchInPredicted();
            SearchInLastCommands();
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

    private void SearchInPredicted()
    {
        if (_predictedCommands.Count == 0)
            return;

        if (Input.GetKeyDown(consoleDownKey) || Input.GetKeyDown(consoleTabKey))
        {
            _isSearching = true;
            if (_predictedCommandIndex < _predictedCommands.Count - 1)
            {
                _predictedCommandIndex++;
            }
            else
            {
                _predictedCommandIndex = 0;
            }
            consoleInput.text = _predictedCommands[_predictedCommandIndex];
            consoleInput.MoveToEndOfLine(false, false);
        }

        if (Input.GetKeyUp(consoleUpKey))
        {
            _isSearching = true;
            if (_predictedCommandIndex > 0)
            {
                _predictedCommandIndex--;
            }
            else
            {
                _predictedCommandIndex = _predictedCommands.Count - 1;
            }
            consoleInput.text = _predictedCommands[_predictedCommandIndex];
            consoleInput.MoveToEndOfLine(false, false);
        }
    }

    private void SearchInLastCommands()
    {
        if (_lastCommands.Count == 0)
            return;

        if (panels.IsPredicting)
            return;

        if (Input.GetKeyDown(consoleRightKey) || Input.GetKeyDown(consoleTabKey))
        {
            _isSearching = true;
            var cmmnds = _lastCommands.ToArray();
            if (_currentCommand > 0)
            {
                _currentCommand--;
            }
            else
            {
                _currentCommand = _lastCommands.Count - 1;
            }
            consoleInput.text = cmmnds[_currentCommand];
            consoleInput.MoveToEndOfLine(false, false);
        }

        if (Input.GetKeyDown(consoleLeftKey))
        {
            _isSearching = true;
            var cmmnds = _lastCommands.ToArray();
            if (_currentCommand < _lastCommands.Count - 1)
            {
                _currentCommand++;
            }
            else
            {
                _currentCommand = 0;
            }
            consoleInput.text = cmmnds[_currentCommand];
            consoleInput.MoveToEndOfLine(false, false);
        }
    }

    private void Submit()
    {
        AddMessage("> " + consoleInput.text);
        ProcessCommand(consoleInput.text);
        consoleInput.text = string.Empty;
        consoleInput.ActivateInputField();
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
                try
                {
                    Command.Callback(args);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                }

                return;
            }
        }
    }

    private void Enqueue(Queue<string> queue, int maxValue, string message)
    {
        if (queue.Count >= maxValue)
        {
            queue.Dequeue();
        }
        queue.Enqueue(message);
    }

    private void AddMessage(string message)
    {
        Enqueue(_logs, maxLogs, $"{message}\n");
        UpdateLogs();
    }

    private void UpdateLogs()
    {
        if (_logs == null)
            return;

        if (!IsConsoleOpened)
            return;

        consoleLogs.text = string.Empty;

        foreach (var log in _logs)
        {
            consoleLogs.text += log;
        }
    }
}
