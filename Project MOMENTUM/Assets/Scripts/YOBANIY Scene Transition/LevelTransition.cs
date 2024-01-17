using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class LevelTransition : MonoBehaviour
{
    public static LevelTransition Instance;
    public bool FadeCompleted = true;
    public bool MuteSound = false;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private RawImage image;

    private void Awake()
    {
        Instance = this;
        FadeIn();
    }

    private float FadeTime
    {
        set
        {
            image.color = new Color(0, 0, 0, value);
            audioMixer.SetFloat("Master", Mathf.Lerp(0, MuteSound ? -80 : 0, value));
        }
    }

    public void FadeIn() => StartCoroutine(Interpolate(1, 0));

    public void FadeOut() => StartCoroutine(Interpolate(0, 1));

    private IEnumerator Interpolate(float from, float to)
    {
        FadeCompleted = false;
        float current = from;
        for (float time = 0; current != to; time += Time.deltaTime * speed)
        {
            current = Mathf.Clamp01(Mathf.SmoothStep(from, to, time));
            FadeTime = current;
            yield return null;
        }
        MuteSound = false;
        FadeCompleted = true;
    }
}
