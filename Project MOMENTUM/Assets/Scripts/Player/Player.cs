using UnityEngine;

public class Player : MonoBehaviour
{
    public bool IsSpectator { get; private set; } = false;
    public bool IsDead { get; private set; } = false;
    public bool IsPaused { get; private set; } = false;
    public bool CanRespawn { get; private set; } = true;

    [Header("References")]
    public PlayerController playerController;
    public PlayerHealth playerHealth;
    public WeaponInventory weaponInventory;
    public AmmoInventory ammoInventory;
    public CameraController cameraController;
    public GameObject origin;

    private DeveloperConsole _console;
    private Timer _gameOverTimer;
    private GameManager _gm;

    public bool Crutch()
    {
        if (_console.IsConsoleOpened) return false;

        return true;
    }

    public bool CanReadInputs()
    {
        if (IsPaused) return false;

        if (_console != null)
            if (_console.IsConsoleOpened) return false;

        return true;
    }

    public void Pause(bool value) => IsPaused = value;

    public void Pause() => IsPaused = !IsPaused;

    private void Start()
    {
        playerHealth.OnDeath += Die;
        _gm = GameManager.Instance;
        _gameOverTimer = new Timer(_gm.RespawnTime);
        _console = DeveloperConsole.Instance;
        UIManager.Instance.SetPlayer(this);
    }

    private void OnDestroy() => playerHealth.OnDeath -= Die;

    private void Die() => IsDead = true;

    private void GameOverScreen()
    {
        if (!IsDead)
            return;

        _gameOverTimer.Tick(Time.deltaTime);

        if (IsPaused)
            return;

        if (_gameOverTimer.RemainigSeconds > 0)
            return;

        if (_gm.IsAutoRespawn)
        {
            _gm.Respawn();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _gm.Respawn();
        }
    }
}
