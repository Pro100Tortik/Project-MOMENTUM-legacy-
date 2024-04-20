using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] private bool disableRendererOnStart = true;
    [Space]
    [Header("Custom Trigger Events")]
    [SerializeField] private UnityEvent OnEnter;
    [SerializeField] private UnityEvent OnExit;
    [SerializeField] private bool isOneUse = true;

    [Header("Damaging Trigger Settings")]
    [SerializeField] private bool isDamaging = false;
    [SerializeField] private int damage = 5;
    [SerializeField] private float damageSpeed = 1.0f;

    [Header("Teleporter Trigger Settings")]
    [SerializeField] private bool isTeleporter = false;
    [SerializeField] private Transform teleportPos = null;

    [Header("Change Level Trigger")]
    [SerializeField] private bool isChangingLevel = false;
    [SerializeField] private bool changeByIndex = false;
    [SerializeField] private int levelIndex = 0;
    [SerializeField] private string level = "MainMenu";

    [Header("Play Sound Trigger")]
    [SerializeField] private AudioSource source = null;
    [SerializeField] private AudioClip audioClip = null;
    [SerializeField] private bool startNewClipInstantly = false;
    [SerializeField] private Vector2 pitchMinMax = Vector2.one;

    private float _timer;
    private MeshRenderer _renderer;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;

        _renderer = GetComponent<MeshRenderer>();

        if (_renderer != null && disableRendererOnStart)
            _renderer.enabled = false;
    }

    private void Update()
    {
        if (!isDamaging)
            return;

        if (_timer < 0)
        {
            _timer = 0;
        }
        else
        {
            _timer -= Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        if (!isTeleporter)
            return;

        if (teleportPos == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(teleportPos.position, Vector3.one);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (isChangingLevel)
        {
            if (changeByIndex)
            {
                GameFunctions.ChangeLevel(levelIndex);
            }
            else
            {
                GameFunctions.ChangeLevel(level);
            }
            return;
        }

        if (source != null)
        {
            if (audioClip != null)
            {
                if (!startNewClipInstantly)
                {
                    if (!source.isPlaying)
                    {
                        source.pitch = UnityEngine.Random.Range(pitchMinMax.x, pitchMinMax.y);
                        source.PlayOneShot(audioClip);
                    }
                }
                else
                {
                    source.Stop();
                    source.pitch = UnityEngine.Random.Range(pitchMinMax.x, pitchMinMax.y);
                    source.PlayOneShot(audioClip);
                }
            }
        }

        OnEnter?.Invoke();

        if (isTeleporter)
        {
            other.transform.position = teleportPos.position;
            if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.velocity = Vector3.zero;
            }
        }

        if (isDamaging)
            return;

        if (isOneUse)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isDamaging)
            return;

        if (other.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            if (_timer <= 0)
            {
                damagable.Damage(null, damage);
                _timer = damageSpeed;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        OnExit?.Invoke();
    }
}
