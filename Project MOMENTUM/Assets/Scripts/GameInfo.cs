using TMPro;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    [SerializeField] private ClientStatus status;
    [SerializeField] private TMP_Text FPSText;
    private float _fpsRefreshTimer;

    private void Awake()
    {
        FPSText.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            FPSText.enabled = !FPSText.enabled;
        }

        if (!FPSText.enabled)
            return;

        if (_fpsRefreshTimer > 0)
            return;

        FPSText.text = GetFPS();
    }

    private void FixedUpdate()
    {
        if (_fpsRefreshTimer > 0)
        {
            _fpsRefreshTimer -= Time.deltaTime;
        }
        else
        {
            _fpsRefreshTimer = 0;
        }
    }

    private string GetFPS()
    {
        _fpsRefreshTimer = 0.2f;
        int fps = Mathf.RoundToInt(1f / Time.deltaTime);
        return fps.ToString();
    }
}
