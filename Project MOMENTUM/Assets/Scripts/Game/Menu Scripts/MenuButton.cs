using UnityEngine;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private AudioSource buttonSource;
    [SerializeField] private AudioClip buttonSound;

    public void PlayButtonSound()
    {
        buttonSource.PlayOneShot(buttonSound);
    }
}
