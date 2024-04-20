using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerStepSounds : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private CapsuleCollider playerCollider;
    [SerializeField] private List<TextureSound> textureSounds;
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        StartCoroutine(CheckGround());
    }

    private IEnumerator CheckGround()
    {
        while (true)
        {
            Ray ray = new Ray(transform.position, Vector3.down * (playerCollider.height + 0.2f));

            if (controller.OnGround && controller.PlayerVelocity != Vector3.zero && Physics.Raycast(ray, out RaycastHit hit, 1.0f))
            {
                if (hit.collider.TryGetComponent<Terrain>(out Terrain terrain))
                {
                    yield return StartCoroutine(PlayFootstepSoundFromTerrain(terrain));
                }
                else if (hit.collider.TryGetComponent<Renderer>(out Renderer renderer))
                {
                    yield return StartCoroutine(PlayFootstepSoundFromRenderer(renderer));
                }
            }

            yield return null;
        }
    }

    private IEnumerator PlayFootstepSoundFromTerrain(Terrain terrain)
    {
        yield return null;
    }

    private IEnumerator PlayFootstepSoundFromRenderer(Renderer renderer)
    {
        foreach (TextureSound ts in textureSounds)
        {
            if (ts.albedo == renderer.material.GetTexture("_MainTex"))
            {
                AudioClip clip = GetClipFromTextureSound(ts);

                audioSource.PlayOneShot(clip);
                yield return new WaitForSeconds(clip.length);
                break;
            }
        }
    }

    private AudioClip GetClipFromTextureSound(TextureSound ts)
    {
        int clipIndex = UnityEngine.Random.Range(0, textureSounds.Count - 1);
        return ts.sounds[clipIndex];
    }

    #region Texture Sound
    [Serializable]
    private class TextureSound
    {
        public Texture albedo;
        public List<AudioClip> sounds;
    }
    #endregion
}
