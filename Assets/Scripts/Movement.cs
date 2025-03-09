using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public float speed = 8.0f;
    public float speedMultiplier = 1.0f;
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;
    public new Rigidbody2D rigidbody { get; private set; }
    public Vector2 direction { get; private set; }
    public Vector2 nextDirection { get; private set; }
    public Vector3 startingPosition { get; private set; }
    private Vector2 lastPosition; // Lưu vị trí cũ
    private readonly Vector2[] possibleDirections = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    public bool isGhost = false; // Xác định xem có phải ghost không

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        startingPosition = transform.position;
    }

    void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        speedMultiplier = 1.0f;
        direction = initialDirection;
        nextDirection = Vector2.zero;
        transform.position = startingPosition;
        lastPosition = rigidbody.position; // Lưu vị trí ban đầu
        enabled = true;
    }

    void Update()
    {
        if (this.nextDirection != Vector2.zero)
        {
            SetDirection(nextDirection);
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = rigidbody.position;
        Vector2 translation = direction * speed * speedMultiplier;

        rigidbody.MovePosition(position + translation);

        // Chỉ áp dụng cho Ghost: nếu vị trí không thay đổi, thử quay đầu hoặc chọn hướng khác
        if (isGhost && (Vector2)transform.position == lastPosition)
        {
            TryChangeDirection();
        }

        lastPosition = transform.position; // Cập nhật vị trí cuối
    }

    public void SetDirection(Vector2 direction, bool force = false)
    {
        if (force || !Occupied(direction))
        {
            this.direction = direction;
            this.nextDirection = Vector2.zero;
        }
        else
        {
            nextDirection = direction;
        }
    }

    public bool Occupied(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0.0f, direction, 1f, obstacleLayer);
        return hit.collider != null;
    }

    private void TryChangeDirection()
    {
        Vector2 reverseDirection = -direction; // Quay đầu lại

        // Nếu quay đầu không bị chặn thì quay đầu
        if (!Occupied(reverseDirection))
        {
            SetDirection(reverseDirection, true);
            return;
        }

        // Nếu quay đầu bị chặn, thử hướng khác
        foreach (var dir in possibleDirections)
        {
            if (dir != direction && dir != reverseDirection && !Occupied(dir)) // Tránh đi tiếp hướng cũ hoặc quay đầu bị chặn
            {
                SetDirection(dir, true);
                break;
            }
        }
    }
}
