using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("References")]
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject gameName;

    [Header("Ammo")]
    [SerializeField] private TMP_Text currentAmmo;

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

    [Header("Health & Armor")]
    [SerializeField] private TMP_Text health;
    [SerializeField] private TMP_Text armor;
    [SerializeField] private RawImage armorSprite;

    [Header("Damage Effect")]
    public RawImage damageEffect;
    [SerializeField] private float fadeSpeed = 2.5f;

    //[Header("Messages")]
    //[SerializeField] private TMP_Text secretMessage;
    //private Coroutine _currentSecretMessage;
    private Player _player;

    public void SetPlayer(Player player) => _player = player;

    private void Awake() => damageEffect.color = new Color(1, 0, 0, 0);

    private void Start()
    {
        //secretMessage.text = string.Empty;
        UpdateAmmoText(0, AmmoType.None);
    }

    private void Update()
    {
        if (_player.weaponInventory.GetCurrentWeaponData() != null)
        {
            AmmoType ammoType = _player.weaponInventory.GetCurrentWeaponData().primaryAmmoType;
            int currentAmmo = _player.ammoInventory.GetCurrentAmmo(ammoType);
            UpdateAmmoText(currentAmmo, ammoType);
        }
        else
        {
            UpdateAmmoText(0, AmmoType.None);
        }

        UpdateHealthArmor();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        GameManager.Instance.Pause(true);
        _player.Pause();
        DisableAllMenus();

        if (_player.IsPaused)
        {
            GameFunctions.EnableCursor();
        }
        else
        {
            GameFunctions.DisableCursor();
        }

        pauseMenu.SetActive(_player.IsPaused);
        gameName.SetActive(_player.IsPaused);
    }

    private void FixedUpdate() => damageEffect.color = Color.Lerp(damageEffect.color, new Color(1, 0, 0, 0), Time.deltaTime * fadeSpeed);

    private void OnValidate() => UpdateCrosshair();

    private void UpdateAmmoText(int ammo, AmmoType ammoType) => currentAmmo.text = (ammoType == AmmoType.None || _player.ammoInventory.InfiniteAmmo) ? "-" : ammo.ToString();

    private void UpdateCrosshair()
    {
        crosshair.sizeDelta = new Vector2(size, size);

        topLine.sizeDelta = new Vector2(width, height);
        bottomLine.sizeDelta = new Vector2(width, height);
        leftLine.sizeDelta = new Vector2(height, width);
        rightLine.sizeDelta = new Vector2(height, width);
    }

    private void UpdateHealthArmor()
    {
        health.text = _player.playerHealth.Health.ToString();
        armor.text = _player.playerHealth.Armor.ToString();
        armorSprite.color = GetArmorColor(_player.playerHealth.GetCurrentArmorType());
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

    private void DisableAllMenus()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        gameName.SetActive(false);
    }

    //private Coroutine SecretMessage()
    //{
    //    if (_currentSecretMessage != null)
    //        StopCoroutine(_currentSecretMessage);
    //
    //    return _currentSecretMessage = StartCoroutine(SecretShowMessage());
    //}

    //private IEnumerator SecretShowMessage()
    //{
    //    secretMessage.text = _secretMessage;
    //    yield return new WaitForSeconds(2f);
    //    secretMessage.text = string.Empty;
    //}
}
