using UnityEngine;
using UnityEngine.UI;

public class MapButtonController : MonoBehaviour
{
    public Button createButton; 
    public MapGenerator mapGenerator;

    void Start()
    {
        createButton.onClick.AddListener(OnCreateButtonClick);
    }

    void OnCreateButtonClick()
    {
        mapGenerator.GenerateMap();
        mapGenerator.DrawMap();
        mapGenerator.PlaceNodes();
    }
}