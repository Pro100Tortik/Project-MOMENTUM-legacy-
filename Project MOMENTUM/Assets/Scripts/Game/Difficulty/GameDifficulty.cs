[System.Flags]
public enum GameDifficultiesWithFlags
{
    None = 0,
    CanIPlayDaddy = 1,
    ICanDoIT = 2,
    NotSoCasual = 4,
    UltraViolence = 8,
    Nightmare = 16
}

public enum GameDifficulty
{
    CanIPlayDaddy = GameDifficultiesWithFlags.CanIPlayDaddy,
    ICanDoIT = GameDifficultiesWithFlags.ICanDoIT,
    NotSoCasual = GameDifficultiesWithFlags.NotSoCasual,
    UltraViolence = GameDifficultiesWithFlags.UltraViolence,
    Nightmare = GameDifficultiesWithFlags.Nightmare,
}
