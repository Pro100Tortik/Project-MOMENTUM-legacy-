using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    [SerializeField] private ObjectCarry carry;
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private int startWeapon;
    [SerializeField] private List<GameObject> weapons;
    [SerializeField] private List<GameObject> secondWeapons;
    private ClientStatus _status;

    internal int _currentWeapon = -1;
    private int _lastWeapon;

    private void Awake()
    {
        _status = GetComponentInParent<ClientStatus>();
    }

    private void Start()
    {
        DisableWeapons();
        SwitchWeapon(startWeapon);
    }

    private void OnEnable()
    {
        weaponInventory.OnWeaponPickedUp += ChangeOnPickup;
        weaponInventory.OnSecondWeaponPickedUp += ChangeOnSecondPickup;
        carry.OnPikcup += DisableWeapons;
        carry.OnRelease += EnableCurrentWeapon;
    }

    private void OnDisable()
    {
        weaponInventory.OnWeaponPickedUp -= ChangeOnPickup;
        weaponInventory.OnSecondWeaponPickedUp -= ChangeOnSecondPickup;
        carry.OnPikcup -= DisableWeapons;
        carry.OnRelease -= EnableCurrentWeapon;
    }

    private void Update()
    {
        if (_status.CurrentClientState == PlayerState.Dead)
        {
            DisableWeapons();
            return;
        }

        if (_status.CurrentClientState == PlayerState.Menus)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchWeapon(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            SwitchWeapon(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            SwitchWeapon(4);
        else if (Input.GetKeyDown(KeyCode.Q))
            SwitchWeapon(_lastWeapon);

        if (Input.GetKeyDown(KeyCode.R))
            DualWield(_currentWeapon);
    }

    private void SwitchWeapon(int index)
    {
        if (index > weapons.Count - 1)
            return;

        if (!weaponInventory.HaveWeapon(GetWeaponData(index)))
            return;

        if (index != _currentWeapon)
        {
            _lastWeapon = _currentWeapon;
        }

        _currentWeapon = index;

        DisableWeapons();

        weapons[index].SetActive(true);

        if (weapons[index].GetComponent<WeaponAbstract>().WasDualWielded())
            DualWield(index);
    }

    private void DualWield(int index)
    {
        if (!weaponInventory.HaveSecondWeapon(GetWeaponData(index)))
            return;

        if (!GetWeaponData(index).canBeDualWielded)
            return;

        secondWeapons[index].SetActive(!secondWeapons[index].activeSelf);
    }

    private void DisableWeapons()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].SetActive(false);
            }

            if (i < secondWeapons.Count)
            {
                if (secondWeapons[i] != null)
                    secondWeapons[i].SetActive(false);
            }
        }
    }

    private void EnableCurrentWeapon()
    {
        if (_currentWeapon >= 0)
        {
            weapons[_currentWeapon].SetActive(true);
        }
    }

    private void ChangeOnPickup(WeaponDataSO weapon)
    {
        if (carry.HaveItem)
            return;

        for (int i = 0;i < weapons.Count;i++)
        {
            if (GetWeaponData(i) == weapon)
                SwitchWeapon(i);
        }
    }

    private void ChangeOnSecondPickup(WeaponDataSO weapon)
    {
        if (carry.HaveItem)
            return;

        ChangeOnPickup(weapon);
        for (int i = 0; i < weapons.Count; i++)
        {
            if (GetWeaponData(i) == weapon)
                DualWield(i);
        }
    }

    public WeaponDataSO GetCurrentWeaponData()
    {
        if (_currentWeapon < 0)
            return null;
        return weapons[_currentWeapon].GetComponent<WeaponAbstract>().WeaponData();
    }

    private WeaponDataSO GetWeaponData(int index)
    {
        WeaponAbstract weapon = weapons[index].GetComponent<WeaponAbstract>();
        if (weapon == null)
            return null;
        else
            return weapon.WeaponData();
    }
}
