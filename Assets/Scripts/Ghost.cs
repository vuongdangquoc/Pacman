using UnityEngine;

public class Ghost : MonoBehaviour
{
    public int points = 200;
    public Movement movement { get; private set; }

    public GhostHome home { get; private set; }
    public GhostScatter scatter { get; private set; }
    public GhostChase chase { get; private set; }

    public GhostFrightened frightened { get; private set; }

    public GhostBehavior initialBehavior;

    //target muon duoi theo
    public Transform target;


    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        movement = GetComponent<Movement>();
        home = GetComponent<GhostHome>();
        scatter = GetComponent<GhostScatter>();
        chase = GetComponent<GhostChase>();
        frightened = GetComponent<GhostFrightened>();

        
    }

    public void ResetState()
    {
        this.gameObject.SetActive(true);
        this.movement.ResetState();
        this.frightened.Disable();
        this.chase.Disable();
        this.scatter.Enable();
        if (this.home != this.initialBehavior)
        {
            this.home.Disable();
        }

        if(this.initialBehavior != null)
        {
            this.initialBehavior.Enable();
        }
    }

    private void Start()
    {
        ResetState();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            if (this.frightened.enabled)
            {
                FindFirstObjectByType<GameManager>().GhostEaten(this);
                ResetState();
            }
            else
            {
                FindFirstObjectByType<GameManager>().PacmanEaten();
            }
        }
    }

}
