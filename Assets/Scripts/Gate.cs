
using UnityEngine;

public class Gate : MonoBehaviour
{
    public Transform connection;
    public new GameObject gameObject;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            Vector3 position = collision.transform.position;
            position.x = connection.position.x;
            position.y = connection.position.y;

            collision.transform.position = position;
            Destroy(gameObject);
            Destroy(connection.gameObject);
        }
    }


}
