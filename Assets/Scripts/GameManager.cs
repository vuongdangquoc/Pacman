using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;
    public Transform diamonds;
    public Fruit fruit;
    public TextMeshProUGUI txtPoint;
    public TextMeshProUGUI txtLife;
    public TextMeshProUGUI txtDiamond;
    public TextMeshProUGUI txtPowerup;
    public MapGenerator map;
    public GameObject pauseMenu;
    public GameObject gameOverScreen;
    public GameObject levelUpScreen;
    public LightSystem lightSystem;
    public float currentTimeScale = 1;
    public int ghostMultiplier { get; private set; } = 1;
    public int score { get; private set; }
    public int initialLevelScore { get; private set; }
    public int lives { get; private set; }
    public int remainingDiamonds { get; private set; }
    public int level { get; private set; }

    public Tilemap walls;
    private Color initialWallColor;
    private List<Color> initialGhostColors = new List<Color>();

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
        initialWallColor = walls.GetColor(new Vector3Int(0, 0, 0));
        foreach (var ghost in ghosts)
        {
            Color color = ghost.transform.Find("Body").GetComponent<SpriteRenderer>().color;
            initialGhostColors.Add(color);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!pauseMenu.gameObject.activeSelf && !gameOverScreen.gameObject.activeSelf && !levelUpScreen.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause(true);
        }
    }
    public void NewGame()
    {
        level = SaveData.currentData.Level;
        foreach (var ghost in ghosts)
        {
            ghost.frightened.duration = 8;
        }
        gameOverScreen.gameObject.SetActive(false);
        currentTimeScale = 1 + (SaveData.currentData.Level - 1) * 0.05f;
        SetScore(SaveData.currentData.Score);
        SetLives(SaveData.currentData.Lives);
        NewRound();
    }

    private void NewRound()
    {
        LightSystem.Instance.ResetLight();
        levelUpScreen.gameObject.SetActive(false);
        Time.timeScale = currentTimeScale;
        map.GenerateAll();
        ResetState();
    }

    private void ResetState()
    {
        initialLevelScore = score;
        SetDiamonds(diamonds.childCount);
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
        gameOverScreen.gameObject.SetActive(true);
    }

    private void SetScore(int score)
    {
        int previousScore = this.score;
        this.score = score;
        txtPoint.text = "POINT: " + score.ToString();
        if (score / 10000 > previousScore / 10000)
        {
            SetLives(++lives);
        }
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        txtLife.text = "X" + lives.ToString();
    }

    private void SetDiamonds(int diamonds)
    {
        this.remainingDiamonds = diamonds;
        txtDiamond.text = "X" + diamonds.ToString();
    }

    public void GhostEaten(Ghost ghost)
    {
        SetScore(this.score + ghost.points * this.ghostMultiplier);
        this.ghostMultiplier++;
    }

    public void PacmanEaten()
    {
        this.pacman.gameObject.SetActive(false);
        SetLives(this.lives - 1);
        if (this.lives > 0)
        {
            for (int i = 0; i < ghosts.Length; i++)
            {
                this.ghosts[i].scatter.Enable();
                this.ghosts[i].chase.Disable();
                this.ghosts[i].frightened.Disable();
                this.ghosts[i].home.Disable();

            }
            Invoke(nameof(ResetStateAfterPacmanEaten), 3.0f);
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
    }

    public void FruitEaten(Fruit fruit)
    {
        PelletEaten(fruit);
        lightSystem.IncreaseLight();
    }
    public void DiamondEaten(Diamond diamond)
    {
        diamond.gameObject.SetActive(false);
        SetDiamonds(remainingDiamonds - 1);
        if (!HasRemainingDiamonds())
        {
            level++;
            levelUpScreen.gameObject.SetActive(true);
            pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 3.0f);
            currentTimeScale += 0.05f;

            //decrease ghost frighted time (min. 2 seconds)
            foreach (var ghost in ghosts)
            {
                if (ghost.frightened.duration >= 2)
                {
                    ghost.frightened.duration -= 0.5f;
                }
            }
        }
    }

    public void FrightenedMode(PowerPellet pellet)
    {
        Thawing();
        txtPowerup.text = "FRIGHTENED";
        foreach (var ghost in this.ghosts)
        {
            ghost.frightened.Enable(pellet.duration);
        }
        PelletEaten(pellet);
        CancelInvoke();
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    public void FreezeMode(PowerPellet pellet)
    {
        txtPowerup.text = "FREEZE";
        Color freezeColor = new Color(135f / 255f, 206f / 255f, 235f / 255f);
        BoundsInt bounds = walls.cellBounds;
        foreach (var position in bounds.allPositionsWithin)
        {
            TileBase tile = walls.GetTile(position);
            if (tile != null)
            {
                walls.SetColor(position, freezeColor);
            }
        }
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].transform.Find("Body").GetComponent<SpriteRenderer>().color = freezeColor;
            ghosts[i].movement.speed = 0;
            ghosts[i].GetComponent<CircleCollider2D>().isTrigger = true;
            if (ghosts[i].home.enabled)
            {
                float remainingDuration = ghosts[i].home.duration;
                ghosts[i].home.Enable(pellet.duration);
            }else if (ghosts[i].frightened.enabled)
            {
                ghosts[i].frightened.Disable();
            }
        }
        PelletEaten(pellet);
        CancelInvoke();
        Invoke(nameof(Thawing), pellet.duration);
    }

    public void Thawing()
    {
        txtPowerup.text = "";
        BoundsInt bounds = walls.cellBounds;
        foreach (var position in bounds.allPositionsWithin)
        {
            TileBase tile = walls.GetTile(position);
            if (tile != null)
            {
                walls.SetColor(position, initialWallColor);
            }
        }
        for (int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].transform.Find("Body").GetComponent<SpriteRenderer>().color = initialGhostColors[i];
            ghosts[i].movement.speed = 0.1f;
            ghosts[i].GetComponent<CircleCollider2D>().isTrigger = false;
        }
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

    private void ResetGhostMultiplier()
    {
        txtPowerup.text = "";
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

    public void SetFruit(Fruit newFruit)
    {
        fruit = newFruit;
        fruit.GenerateRandomSprite();
    }

    public void TogglePause(bool paused)
    {
        Time.timeScale = paused ? 0 : currentTimeScale;
        pacman.gameObject.SetActive(!paused);
        pauseMenu.gameObject.SetActive(paused);
        backButton.gameObject.SetActive(false);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public List<Button> saveButtons;
    public List<Button> saveSlots;
    public Button backButton;
    public void ShowSaveSlots()
    {
        saveButtons.ForEach(b => b.gameObject.SetActive(false));
        backButton.gameObject.SetActive(true);
        foreach (var saveSlot in saveSlots)
        {
            saveSlot.gameObject.SetActive(true);
            int saveIndex = saveSlots.IndexOf(saveSlot) + 1;
            TextMeshProUGUI tmp = saveSlot.transform.Find("Status").GetComponent<TextMeshProUGUI>();
            if (SaveData.CheckSlot(saveIndex))
            {
                tmp.text = "OVERRIDE";
                tmp.color = Color.red;
            }
            else
            {
                tmp.text = "EMPTY";
                tmp.color = Color.white;
            }
        }
    }
    public void Back()
    {
        saveButtons.ForEach(b => b.gameObject.SetActive(true));
        saveSlots.ForEach(b => b.gameObject.SetActive(false));
        backButton.gameObject.SetActive(false);
    }
    public void SaveGame(int slot)
    {
        PacmanData pd = new PacmanData()
        {
            Level = level,
            Lives = lives,
            Score = initialLevelScore
        };
        SaveData.SaveGame(slot, pd);

        TextMeshProUGUI tmp = saveSlots[slot - 1].transform.Find("Status").GetComponent<TextMeshProUGUI>();
        tmp.text = "OVERRIDE";
        tmp.color = Color.red;
    }
}
