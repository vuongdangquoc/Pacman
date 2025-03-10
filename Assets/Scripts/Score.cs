using UnityEngine;

/// <summary>
/// Score display
/// </summary>
public class Score : MonoBehaviour
{
    public SpriteRenderer[] scorePanel { get; private set; }
    public Sprite[] sprites;
    /// <summary>
    /// Display the score in the board.
    /// </summary>

    private void Awake()
    {
        scorePanel = GetComponentsInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// Set score panel.
    /// </summary>
    public void DisplayScore(int score)
    {
        int displayIndex = scorePanel.Length - 1;
        while (score > 0 && displayIndex >= 0)
        {
            scorePanel[displayIndex].sprite = sprites[score % 10];
            score /= 10;
            displayIndex--;
        }
        while (displayIndex >= 0)
        {
            scorePanel[displayIndex].sprite = sprites[0];
            displayIndex--;
        }
    }
}
