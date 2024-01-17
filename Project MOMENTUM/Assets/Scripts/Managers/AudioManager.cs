using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private WeaponInventory weaponInventory;
    [SerializeField] private AmmoInventory ammoInventory;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource weaponSource;
    [SerializeField] private AudioSource pickupSource;
    [SerializeField] private AudioSource playerSteps;
    [SerializeField] private AudioSource playerLadder;
    [SerializeField] private AudioSource playerJump;
    [SerializeField] private AudioSource playerPain;

    private void OnEnable()
    {
        weaponInventory.OnWeaponAdded += PlayPickupSound;
        ammoInventory.OnAmmoPickedUp += PlayPickupSound;
        playerController.OnStep += PlayStepSound;
        playerController.OnLadderMove += PlayLadderSound;
        playerController.OnJump += PlayJumpSound;
        playerHealth.OnDamage += PlayPainSound;
        playerHealth.OnPickup += PlayPickupSound;
    }

    private void OnDisable()
    {
        weaponInventory.OnWeaponAdded -= PlayPickupSound;
        ammoInventory.OnAmmoPickedUp -= PlayPickupSound;
        playerController.OnStep -= PlayStepSound;
        playerController.OnLadderMove -= PlayLadderSound;
        playerController.OnJump -= PlayJumpSound;
        playerHealth.OnDamage -= PlayPainSound;
        playerHealth.OnPickup -= PlayPickupSound;
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
