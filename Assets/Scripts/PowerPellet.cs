using UnityEngine;

public class PowerPellet : Pellet
{
    public float duration = 8.0f;

    protected override void Eat()
    {
        int rd = Random.Range(0, 3);
        switch (rd)
        {
            case 0:
                FindFirstObjectByType<GameManager>().FrightenedMode(this);
                break;
            case 1:
                FindFirstObjectByType<GameManager>().FreezeMode(this);
                break;
            case 2:
                FindFirstObjectByType<GameManager>().DoubleScore(this);
                break;
            default:
                break;
        }
    }
}
