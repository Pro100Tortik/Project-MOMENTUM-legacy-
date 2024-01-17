public static class DifficultyFunctions
{
    public static bool CanSpawnOnThisDifficulty(GameDifficulty currentdifficulty, GameDifficulty spawndifficulty)
    {
        return currentdifficulty <= spawndifficulty;
    }

    public static bool CanSpawn(GameDifficulty currentdifficulty, GameDifficultiesWithFlags spawndifficulty)
    {
        if (currentdifficulty == GameDifficulty.CanIPlayDaddy && spawndifficulty.HasFlag(GameDifficultiesWithFlags.CanIPlayDaddy))
        {
            return true;
        }
        else if (currentdifficulty == GameDifficulty.ICanDoIT && spawndifficulty.HasFlag(GameDifficultiesWithFlags.ICanDoIT))
        {
            return true;
        }
        else if (currentdifficulty == GameDifficulty.NotSoCasual && spawndifficulty.HasFlag(GameDifficultiesWithFlags.NotSoCasual))
        {
            return true;
        }
        else if (currentdifficulty == GameDifficulty.UltraViolence && spawndifficulty.HasFlag(GameDifficultiesWithFlags.UltraViolence))
        {
            return true;
        }
        else if (currentdifficulty == GameDifficulty.Nightmare && spawndifficulty.HasFlag(GameDifficultiesWithFlags.Nightmare))
        {
            return true;
        }
        return false;
    }
}
