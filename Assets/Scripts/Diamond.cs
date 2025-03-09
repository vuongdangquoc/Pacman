using UnityEngine;

public class Diamond : MonoBehaviour
{

    protected virtual void Eat()
    {
        FindFirstObjectByType<GameManager>().DiamondEaten(this);
    }

    //được gọi tự động khi một collider (bộ phận phát hiện va chạm) của đối tượng gắn script này "gặp" một collider khác được đánh dấu là trigger
    public void OnTriggerEnter2D(Collider2D collision)
    {
        //dòng này kiểm tra xem đối tượng mà collider này va chạm có thuộc layer "Pacman" hay không.
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            Eat();
        }
    }
}
