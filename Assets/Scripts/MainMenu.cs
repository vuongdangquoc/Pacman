using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Sprite mainMenuSprite;
    public Sprite instructionsSprite;
    public Image background;
    public List<Button> buttons;
    public Button mainMenuButton;
    private void Start()
    {
        print("start");
        mainMenuButton.gameObject.SetActive(false);
    }
    public void LoadScene()
    {
        SceneManager.LoadScene("New Scene");
    }

    public void ShowMenu(bool showInstructions)
    {
        buttons.ForEach(b => b.gameObject.SetActive(!showInstructions));
        background.sprite = showInstructions ? instructionsSprite : mainMenuSprite;
        mainMenuButton.gameObject.SetActive(showInstructions);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
