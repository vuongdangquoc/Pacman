using Assets.Scripts;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Sprite mainMenuSprite;
    public Sprite instructionsSprite;
    public Image background;
    public List<Button> buttons;
    public List<Button> saveSlots;
    public Button mainMenuButton;
    public Button continueButton;
    private void Start()
    {
        mainMenuButton.gameObject.SetActive(false);
        continueButton.interactable = false;

        //Check if there is save data
        bool hasSaveData = SaveData.CheckAllSlots();
        continueButton.interactable = hasSaveData;
    }
    public void LoadScene(int saveIndex)
    {
        if (saveIndex != 0)
        {
            PacmanData pd = SaveData.LoadGame(saveIndex);
            SaveData.currentData = pd;
        }
        SceneManager.LoadScene("New Scene");
    }

    public void ShowMenu(bool showInstructions)
    {
        buttons.ForEach(b => b.gameObject.SetActive(!showInstructions));
        saveSlots.ForEach(s => s.gameObject.SetActive(false));
        background.sprite = showInstructions ? instructionsSprite : mainMenuSprite;
        mainMenuButton.gameObject.SetActive(showInstructions);
    }

    public void ShowSaveSlots()
    {
        buttons.ForEach(b => b.gameObject.SetActive(false));
        mainMenuButton.gameObject.SetActive(true);
        foreach (var saveSlot in saveSlots)
        {
            saveSlot.gameObject.SetActive(true);
            int saveIndex = saveSlots.IndexOf(saveSlot) + 1;
            if (SaveData.CheckSlot(saveIndex))
            {
                saveSlot.interactable = true;
                PacmanData pd = SaveData.LoadGame(saveIndex);
                saveSlot.transform.Find("Info").GetComponent<TextMeshProUGUI>().text = pd.ToString();
                saveSlot.transform.Find("Continue").GetComponent<TextMeshProUGUI>().text = "CONTINUE";
            }
            else
            {
                saveSlot.transform.Find("Continue").GetComponent<TextMeshProUGUI>().text = "";
                saveSlot.transform.Find("Info").GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }
    public void RemoveSlot(int slotNumber)
    {
        SaveData.RemoveSlot(slotNumber);
        saveSlots[slotNumber - 1].interactable = false;
        saveSlots[slotNumber - 1].transform.Find("Continue").GetComponent<TextMeshProUGUI>().text = "";
        saveSlots[slotNumber - 1].transform.Find("Info").GetComponent<TextMeshProUGUI>().text = "";

        //Check if there is save data
        bool hasSaveData = SaveData.CheckAllSlots();
        continueButton.interactable = hasSaveData;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
