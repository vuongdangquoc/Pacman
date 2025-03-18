using UnityEngine;

public class PowerPellet : Pellet
{
    public float duration = 8.0f;

    protected override void Eat()
    {
        int rd = Random.Range(0, 2);
        if (rd == 0)
        {
            FindFirstObjectByType<GameManager>().FrightenedMode(this);
        }
        else
        {
            FindFirstObjectByType<GameManager>().FreezeMode(this);
        }
    }
}
