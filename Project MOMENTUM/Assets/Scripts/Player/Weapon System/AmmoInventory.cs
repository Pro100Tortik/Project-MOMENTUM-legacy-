using System;
using System.Collections.Generic;
using UnityEngine;

public enum AmmoType
{
    None,
    Bullets,
    Shells,
    Rockets
}

[Serializable]
public struct WeaponAmmo
{
    [HideInInspector] public string ammoName;
    public int ammo;
    public int maxAmmo;
}

public class AmmoInventory : MonoBehaviour, ISaveable
{
    public event Action<AudioClip> OnAmmoPickedUp;
    public bool InfiniteAmmo { get; private set; } = false;
    [SerializeField] private List<WeaponAmmo> ammoList;
    [SerializeField] private AudioClip pickupSound;
    private ClientStatus _status;
    private bool _pickedUp;

    private void Awake() => _status = GetComponentInParent<ClientStatus>();

    private void Start()
    {
        DeveloperConsole.RegisterCommand("fullammo", "", "Give player max ammo for every weapon.", args =>
        {
            MaxAmmo();
        });

        DeveloperConsole.RegisterCommand("infiniteammo", "", "Weapon do not use any ammo.", args =>
        {
            InfiniteAmmo = !InfiniteAmmo;
        });
    }

    public int GetCurrentAmmo(AmmoType ammoType)
    {
        WeaponAmmo ammo = ammoList[(int)ammoType];
        return ammo.ammo;
    }

    public int UseAmmo(AmmoType ammoType, int ammoAmount)
    {
        WeaponAmmo ammoBefore = ammoList[(int)ammoType];
        int used = Mathf.Min(ammoAmount, ammoBefore.ammo);
        ammoBefore.ammo -= used;
        ammoList[(int)ammoType] = ammoBefore;

        return used;
    }

    public int AddAmmo(AmmoType ammoType, int ammoAmount)
    {
        WeaponAmmo ammoBefore = ammoList[(int)ammoType];
        int added = Mathf.Min(ammoAmount, ammoBefore.maxAmmo - ammoBefore.ammo);

        if (added < 1)
            return 0;

        ammoBefore.ammo += added;
        ammoList[(int)ammoType] = ammoBefore;
        _pickedUp = true;
        OnAmmoPickedUp?.Invoke(pickupSound);

        return added;
    }

    private void UpdateAmmoInventory()
    {
        string[] ammoNames = Enum.GetNames(typeof(AmmoType));
        List<WeaponAmmo> inventory = new List<WeaponAmmo>(ammoNames.Length);
        for (int i = 0; i < ammoNames.Length; i++) 
        {
            WeaponAmmo ammo = ammoList.Find(entry => entry.ammoName == ammoNames[i]);
            ammo.ammoName = ammoNames[i];
            ammo.ammo = Mathf.Min(ammo.ammo, ammo.maxAmmo);
            inventory.Add(ammo);
        }
        ammoList = inventory;
    }

    private void MaxAmmo()
    {
        for (int i = 0; i < ammoList.Count; i++)
        {
            WeaponAmmo ammo = ammoList[i];
            ammo.ammo = ammo.maxAmmo;
            ammoList[i] = ammo;
        }
    }

    private void OnValidate() => UpdateAmmoInventory();

    private void OnTriggerEnter(Collider other)
    {
        if (!_status.CanReadInputs()) return;

        AmmoPickup ammo = other.GetComponent<AmmoPickup>();
        if (ammo)
        {
            AddAmmo(ammo.AmmoType, ammo.AmmoAmount);
            if (_pickedUp)
            {
                other.gameObject.SetActive(false);
                _pickedUp = false;
            }
        }
    }

    public void LoadData(GameData data)
    {
        ammoList = data.playerData.ammo;
    }

    public void SaveData(GameData data)
    {
        data.playerData.ammo = ammoList;
    }
}
