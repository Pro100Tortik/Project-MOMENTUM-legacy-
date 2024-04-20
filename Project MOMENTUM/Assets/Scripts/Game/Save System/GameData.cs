using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public long lastUpdated;

    public bool godMode;
    public bool selfDamage;
    public bool infiniteAmmo;
    public PlayerData playerData;

    public GameData()
    {
        godMode = false;
        selfDamage = true;
        infiniteAmmo = false;
        playerData = new PlayerData();
    }
}               
                