using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;
    public Transform diamonds;

    public MapGenerator map;
    public int ghostMultiplier { get; private set; } = 1;
    public int score {  get; private set; }
    public int lives { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NewGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(this.lives <=0 && Input.anyKeyDown)
        {
            NewGame();
        }
    }
    void NewGame()
    {
        SetScore(0);
        SetLives(3);
        NewRoud();
    }

    private void NewRoud()
    {
        //foreach (Transform pellet in pellets)
        //{
        //    pellet.gameObject.SetActive(true);
        //}
        map.GenerateAll();
        ResetState();
    }

    private void ResetState() {
        ResetGhostMultiplier();
        pacman.ResetState();
        for (int i = 0; i < ghosts.Length; i++)
        {
            this.ghosts[i].ResetState();
        }
    }

    private void ResetStateAfterPacmanEaten()
    {
        ResetGhostMultiplier();
        pacman.ResetState();
   
    }


    private void GameOver()
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(false);
        }

        pacman.gameObject.SetActive(false);
    }

    private void SetScore(int score) {
        this.score = score;
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
    }

    public void GhostEaten(Ghost ghost)
    {
        SetScore(this.score + ghost.points * this.ghostMultiplier);
        this.ghostMultiplier++;
    }

    public void PacmanEaten()
    {
        this.pacman.gameObject.SetActive(false);
        SetLives(this.lives-1);
        if(this.lives > 0)
        {
            for (int i = 0; i < ghosts.Length; i++)
            {
                this.ghosts[i].scatter.Enable();
                this.ghosts[i].chase.Disable();
                this.ghosts[i].frightened.Disable();
                this.ghosts[i].home.Disable();

            }
            Invoke(nameof(ResetStateAfterPacmanEaten),3.0f);
        }
        else
        {
            GameOver();
        }
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);
        SetScore(this.score + pellet.points);
        if (!HasRemainingPelllets())
        {
            this.pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRoud), 3.0f);
        }
    }

    public void DiamondEaten(Diamond diamond)
    {
        diamond.gameObject.SetActive(false);
        if (!HasRemainingDiamonds())
        {
            this.pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRoud), 3.0f);
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        for (int i = 0; i <this.ghosts.Length; i++)
        {
            this.ghosts[i].frightened.Enable(pellet.duration);
        }
        PelletEaten(pellet);
        CancelInvoke();
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    private bool HasRemainingDiamonds()
    {
        foreach (Transform diamond in diamonds)
        {
            if (diamond.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
    private bool HasRemainingPelllets()
    {
        foreach (Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    private void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;
    }

    public void SetPacman(Pacman newPacman)
    {
        pacman = newPacman;
    }

    public void SetGhosts(Ghost[] newGhosts)
    {
        ghosts = newGhosts;
    }
}
