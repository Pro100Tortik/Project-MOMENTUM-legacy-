using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource weaponSource;
    [SerializeField] private AudioSource pickupSource;
    [SerializeField] private AudioSource playerSteps;
    [SerializeField] private AudioSource playerLadder;
    [SerializeField] private AudioSource playerJump;
    [SerializeField] private AudioSource playerPain;

    private void OnEnable()
    {
        player.weaponInventory.OnWeaponAdded += PlayPickupSound;
        player.ammoInventory.OnAmmoPickedUp += PlayPickupSound;
        player.playerController.OnStep += PlayStepSound;
        player.playerController.OnLadderMove += PlayLadderSound;
        player.playerController.OnJump += PlayJumpSound;
        player.playerHealth.OnDamage += PlayPainSound;
        player.playerHealth.OnPickup += PlayPickupSound;
    }

    private void OnDisable()
    {
        player.weaponInventory.OnWeaponAdded -= PlayPickupSound;
        player.ammoInventory.OnAmmoPickedUp -= PlayPickupSound;
        player.playerController.OnStep -= PlayStepSound;
        player.playerController.OnLadderMove -= PlayLadderSound;
        player.playerController.OnJump -= PlayJumpSound;
        player.playerHealth.OnDamage -= PlayPainSound;
        player.playerHealth.OnPickup -= PlayPickupSound;
    }

    private void PlayWeaponSound(AudioClip sound)
    {
        if (sound != null)
            weaponSource.PlayOneShot(sound);
    }

    private void PlayPickupSound(AudioClip sound)
    {
        if (sound != null)
            pickupSource.PlayOneShot(sound);
    }

    private void PlayStepSound(AudioClip sound)
    {
        if (sound != null)
            playerSteps.PlayOneShot(sound);
    }
    private void PlayLadderSound(AudioClip sound)
    {
        if (sound != null)
            playerLadder.PlayOneShot(sound);
    }

    private void PlayJumpSound(AudioClip sound)
    {
        if (sound != null)
            playerJump.PlayOneShot(sound);
    }

    private void PlayPainSound(AudioClip sound)
    {
        if (sound != null)
            playerPain.PlayOneShot(sound);
    }
}
