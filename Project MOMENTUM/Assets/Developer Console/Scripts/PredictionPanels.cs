using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PredictionPanels : MonoBehaviour
{
    public bool IsPredicting;

    [Header("References")]
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private Transform parent;
    [SerializeField] private TMP_InputField consoleInput;

    [Header("Prediction Settings")]
    [SerializeField] private int maxPanels = 5;
    private List<GameObject> _panels = new List<GameObject>();

    private void Awake()
    {
        // Object pool
        for (int i = 0; i < maxPanels; i++)
        {
            GameObject panel = Instantiate(panelPrefab, parent);
            panel.SetActive(false);
            _panels.Add(panel);
        }

        foreach (GameObject panel in _panels)
        {
            panel.GetComponent<PredictionPanelInfo>().OnButtonPresed += WriteCommand;
        }
    }

    private void OnDestroy()
    {
        foreach (GameObject panel in _panels)
        {
            panel.GetComponent<PredictionPanelInfo>().OnButtonPresed -= WriteCommand;
        }
    }

    public void AddPanel(string command, string args)
    {
        IsPredicting = true;
        foreach (GameObject panel in _panels)
        {
            if (!panel.activeInHierarchy)
            {
                panel.GetComponent<PredictionPanelInfo>().SetText(command, args);
                panel.SetActive(true);
                return;
            }
        }
    }

    public void ClearPanels()
    {
        foreach (GameObject panel in _panels)
        {
            panel.SetActive(false);
        }
        IsPredicting = false;
    }

    public void Resize()
    {
        float largestWidth = 0;

        // Get the largest text length
        foreach (GameObject panel in _panels)
        {
            if (!panel.activeInHierarchy)
                continue;

            Vector2 size = new Vector2(panel.GetComponent<PredictionPanelInfo>().TextWidth, 0);
            if (largestWidth < size.x)
            {
                largestWidth = size.x;
            }
        }

        // Resize panel and text
        foreach (GameObject panel in _panels)
        {
            var pan = panel.GetComponent<PredictionPanelInfo>();
            pan.SetSize(largestWidth);
        }
    }

    private void WriteCommand(string command)
    {
        consoleInput.text = command;
        consoleInput.ActivateInputField();
        consoleInput.MoveToStartOfLine(false, false);
    }
}
