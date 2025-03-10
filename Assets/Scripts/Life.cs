using UnityEngine;

/// <summary>
/// Life display
/// </summary>
public class Life : MonoBehaviour
{
    public SpriteRenderer lifePanel { get; private set; }
    public Sprite[] sprites;
    private void Awake()
    {
        lifePanel = GetComponentInChildren<SpriteRenderer>();
    }
    public void DisplayLife(int life)
    {
        lifePanel.sprite = sprites[life];
    }
}
