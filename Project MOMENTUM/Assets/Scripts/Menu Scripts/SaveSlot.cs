using TMPro;
using UnityEngine;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileID = "";

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TMP_Text saveName;

    public void SetData(GameData data)
    {
        noDataContent.SetActive(data == null);
        hasDataContent.SetActive(data != null);

        saveName.text = profileID;
    }

    public string GetProfileID()
    {
        return profileID;
    }
}
