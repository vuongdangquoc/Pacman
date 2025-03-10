using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fruit management (random fruit appear sometimes)
/// </summary>
public class Fruit : Pellet
{
    public List<Sprite> fruitSprites;
    public SpriteRenderer spriteRenderer;
    protected override void Eat()
    {
        FindFirstObjectByType<GameManager>().FruitEaten(this);
    }
}
