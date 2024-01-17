using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeLevelButton : MonoBehaviour
{
    [SerializeField] private AudioSource buttonSource;
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private LevelChangeManager levelChangeManager;
    [SerializeField] private RawImage levelImage;
    [SerializeField] private TMP_Text levelNameText;
    [SerializeField] private Texture2D levelImageTexture;
    [SerializeField] private string levelNameUI;
    [SerializeField] private string levelName;
    private Button levelChangeButton;

    private void Awake()
    {
        if (levelImage != null)
        {
            if (levelImageTexture != null)
                levelImage.texture = levelImageTexture;
            else
                levelImage.enabled = false;
        }

        if (levelNameText != null)
            levelNameText.text = levelNameUI;

        levelChangeButton = GetComponent<Button>();
    }

    public void StartLevel()
    {
        levelChangeButton.interactable = false;
        levelChangeManager.ChangeLevel(levelName);
        buttonSource.PlayOneShot(buttonSound);
    }
}
