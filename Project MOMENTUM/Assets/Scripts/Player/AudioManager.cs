using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource playerSource;
    [SerializeField] private AudioSource stepSource;

    private void OnEnable()
    {
        player.weaponInventory.OnWeaponAdded += PlaySound;
        player.ammoInventory.OnAmmoPickedUp += PlaySound;
        player.playerController.OnStep += PlayStepSound;
        player.playerController.OnLadderMove += PlayStepSound;
        player.playerController.OnJump += PlaySound;
        player.playerHealth.OnDamage += PlaySound;
        player.playerHealth.OnPickup += PlaySound;
    }

    private void OnDisable()
    {
        player.weaponInventory.OnWeaponAdded -= PlaySound;
        player.ammoInventory.OnAmmoPickedUp -= PlaySound;
        player.playerController.OnStep -= PlayStepSound;
        player.playerController.OnLadderMove -= PlayStepSound;
        player.playerController.OnJump -= PlaySound;
        player.playerHealth.OnDamage -= PlaySound;
        player.playerHealth.OnPickup -= PlaySound;
    }

    private void PlaySound(AudioClip sound)
    {
        if (sound != null)
            playerSource.PlayOneShot(sound);
    }

    private void PlayStepSound(AudioClip sound)
    {
        if (sound != null)
            stepSource.PlayOneShot(sound);
    }
}
