using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    //[SerializeField] private TMP_Text speed;
    //[SerializeField] private Rigidbody rb;
    [SerializeField] private LocalizedString secret;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private WeaponSwitch weaponSwitch;
    private string _secretMessage = "";

    private void Start()
    {
        secretMessage.text = string.Empty;
        UpdateAmmoText(0, AmmoType.None);
    }

    private void OnEnable()
    {
        secret.Arguments = new object[] { };
        secret.StringChanged += UpdateText;

        playerController.OnSecretFound += ShowSecretMessage;
    }

    private void OnDisable()
    {
        secret.StringChanged -= UpdateText;

        playerController.OnSecretFound -= ShowSecretMessage;
    }

    private void UpdateText(string value) => _secretMessage = value;

    private void Update()
    {
        //float vel = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude; // rb.velocity.magnitude;
        //if (rb != null && speed != null)
        //    speed.text = vel.ToString("0.0");
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
    [SerializeField] private RawImage ammoIcon;
    [SerializeField] private Texture bullets;
    [SerializeField] private Texture shells;
    [SerializeField] private Texture rockets;
    [SerializeField] private RawImage ammoBackground;

    private void UpdateAmmoText(int ammo, AmmoType ammoType)
    {
        currentAmmo.text = ammoType == AmmoType.None ? "" : ammo.ToString(); // Use ∞ if it looks normal
        ammoBackground.enabled = ammoType != AmmoType.None;

        switch (ammoType)
        {
            case AmmoType.None:
                ChangeAmmoIcon(false, null);
                break;

            case AmmoType.Bullets:
                ChangeAmmoIcon(true, bullets);
                break;

            case AmmoType.Shells:
                ChangeAmmoIcon(true, shells);   
                break;

            case AmmoType.Rockets:
                ChangeAmmoIcon(true, rockets);
                break;
        }
    }

    private void ChangeAmmoIcon(bool isEnabled, Texture ammoTexture)
    {
        ammoIcon.enabled = isEnabled;
        ammoIcon.texture = ammoTexture;
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
