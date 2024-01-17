using System.Collections.Generic;
using UnityEngine;

public class SaveSlotsMenu : MonoBehaviour
{
    private SaveSlot[] saveSlots;

    private bool isLoading = false;

    private void Awake()
    {
        saveSlots = GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClicked(SaveSlot slot)
    {
        SaveManager.Instance.ChangeSelectedProfileID(slot.GetProfileID());

        if (!isLoading)
        {
            SaveManager.Instance.NewGame();
        }

        SaveManager.Instance.SaveGame();
        LevelManager.ChangeLevel("tutorial");
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        gameObject.SetActive(true);

        isLoading = isLoadingGame;

        Dictionary<string, GameData> profilesGameData
            = SaveManager.Instance.GetAllProfilesData();

        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileID(), out profileData);
            saveSlot.SetData(profileData);

            if (profileData == null && isLoadingGame)
            {
                // make it not interactable
            }
        }
    }

    public void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }
}
