using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private WeaponSwitch weaponSwitch;
    private string _secretMessage = "You found secret zone";

    private void Start()
    {
        secretMessage.text = string.Empty;
        UpdateAmmoText(0, AmmoType.None);
    }

    private void OnEnable()
    {
        playerController.OnSecretFound += ShowSecretMessage;
    }

    private void OnDisable()
    {
        playerController.OnSecretFound -= ShowSecretMessage;
    }

    private void Update()
    {
        if (weaponSwitch.GetCurrentWeaponData() != null)
        {
            AmmoType ammoType = weaponSwitch.GetCurrentWeaponData().ammoType;
            int currentAmmo = ammoInventory.GetCurrentAmmo(ammoType);
            UpdateAmmoText(currentAmmo, ammoType);
        }
        else
        {
            UpdateAmmoText(0, AmmoType.None);
        }

        UpdateHealthArmor();
    }

    private void OnValidate()
    {
        UpdateCrosshair();
    }

    #region Ammo
    [Header("Ammo")]
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private TMP_Text currentAmmo;

    private void UpdateAmmoText(int ammo, AmmoType ammoType)
    {
        currentAmmo.text = (ammoType == AmmoType.None || ammoInventory.InfiniteAmmo) ? "-" : ammo.ToString(); // Use ∞ if it looks normal
        
        switch (ammoType)
        {
            case AmmoType.None:
                break;

            case AmmoType.Bullets:
                break;

            case AmmoType.Shells:
                break;

            case AmmoType.Rockets:
                break;
        }
    }
    #endregion

    #region Crosshair
    [Header("Crosshair")]
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private RectTransform topLine;
    [SerializeField] private RectTransform bottomLine;
    [SerializeField] private RectTransform leftLine;
    [SerializeField] private RectTransform rightLine;

    [Header("Crosshair Settings")]
    [SerializeField, Min(0)] private float width;
    [SerializeField, Min(0)] private float height;
    [SerializeField, Min(0)] private float size;

    private void UpdateCrosshair()
    {
        crosshair.sizeDelta = new Vector2(size, size);

        topLine.sizeDelta = new Vector2(width, height);
        bottomLine.sizeDelta = new Vector2(width, height);
        leftLine.sizeDelta = new Vector2(height, width);
        rightLine.sizeDelta = new Vector2(height, width);
    }
    #endregion

    #region Health & Armor
    [Header("Health & Armor")]
    [SerializeField] private TMP_Text health;
    [SerializeField] private TMP_Text armor;
    [SerializeField] private RawImage armorSprite;

    private void UpdateHealthArmor()
    {
        health.text = playerHealth.Health.ToString();
        armor.text = playerHealth.Armor.ToString();
        armorSprite.color = GetArmorColor(playerHealth.GetCurrentArmorType());
    }

    private Color GetArmorColor(ArmorType armorType)
    {
        switch (armorType)
        {
            case ArmorType.Green:
                return Color.green;

            case ArmorType.Blue:
                return Color.blue;

            case ArmorType.Red:
                return Color.red;

            default: 
                return Color.white;
        }
    }
    #endregion

    #region Messages
    [SerializeField] private TMP_Text secretMessage;
    private Coroutine _currentSecretMessage;

    private void ShowSecretMessage() => SecretMessage();

    private Coroutine SecretMessage()
    {
        if (_currentSecretMessage != null)
            StopCoroutine(_currentSecretMessage);

        return _currentSecretMessage = StartCoroutine(SecretShowMessage());
    }

    private IEnumerator SecretShowMessage()
    {
        secretMessage.text = _secretMessage;
        yield return new WaitForSeconds(2f);
        secretMessage.text = string.Empty;
    }
    #endregion
}
