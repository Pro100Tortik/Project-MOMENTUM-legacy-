using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PredictionPanelInfo : MonoBehaviour
{
    public event Action<string> OnButtonPresed;

    [Header("References")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private Button button;

    private RectTransform _rect;
    private string _command;

    public string Text => _command;
    public float TextWidth;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        button.onClick.AddListener(OnClick);
    }

    private void OnDestroy() => button.onClick.RemoveAllListeners();

    public void SetSize(float width)
    {
        text.rectTransform.sizeDelta = new Vector2(width, text.rectTransform.sizeDelta.y);
        _rect.sizeDelta = new Vector2(width, _rect.sizeDelta.y);
    }

    public void SetText(string command, string args)
    {
        _command = command;
        text.text = $"{command} {args}";
        TextWidth = text.GetPreferredValues().x + text.margin.x;
    }

    public void OnClick() => OnButtonPresed?.Invoke(Text);
}
