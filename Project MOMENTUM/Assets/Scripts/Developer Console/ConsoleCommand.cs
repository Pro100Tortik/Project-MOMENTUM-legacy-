using System;

[Serializable]
public class ConsoleCommand
{
    public string Command { get; private set; }
    public string Syntax { get; private set; }
    public string Description { get; private set; }
    public Action<string[]> Callback { get; private set; }

    public ConsoleCommand(string command, string syntax, string description, Action<string[]> callback)
    {
        Command = command;
        Syntax = syntax;
        Description = description;
        Callback = callback;
    }
}
