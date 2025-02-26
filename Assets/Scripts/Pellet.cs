using UnityEngine;

public class Pellet : MonoBehaviour
{
    public int points = 10;

    protected virtual void Eat()
    {
        FindFirstObjectByType<GameManager>().PelletEaten(this);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            Eat();
        }
    }
}
