using UnityEngine;
using UnityEngine.UI;

public class MapButtonController : MonoBehaviour
{
    public Button createButton; // Tham chiếu đến button
    public MapGenerator mapGenerator; // Tham chiếu đến script MapGenerator

    void Start()
    {
        createButton.onClick.AddListener(OnCreateButtonClick);
    }

    void OnCreateButtonClick()
    {
        mapGenerator.GenerateMap();
        mapGenerator.DrawMap();
        mapGenerator.PlacePacman();
    }
}