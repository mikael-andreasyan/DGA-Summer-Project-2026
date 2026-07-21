using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance
    {
        get;
        private set;
    }

    public enum GameState { PreStart, Playing, Paused, GameOver }
    public GameState CurrentState { get; private set; } = GameState.PreStart;

    [Header("Main Build Scene Name")]
    [SerializeField] private string mainSceneName;

    [Header("Scoring")]
    [SerializeField] private int pointsPerCloud = 100; // whatever we want
    [SerializeField] private float comboTime = 2f; // whatever we want
    private int highScore; //value that stores the player's highscore

    [Header("Bounds")]
    [Tooltip("The width of the boundaries that the player will be confined to.")]
    [SerializeField] public float boundaryWidth = 30f;

    [Header("Death")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera cam;

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject titlePanel;


    private float comboTimer;
    private bool isAlive = true;

    public int Score
    {
        get;
        private set;
    }
    public int Combo
    {
        get;
        private set;
    }

    private void Awake()
    {
        CurrentState = GameState.PreStart;
        // making sure no multiple instances
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    void Start()
    {

        print(SceneManager.GetActiveScene().name);

        highScore = PlayerPrefs.GetInt("player_HighScore");

    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentState == GameState.PreStart)
        {
            Welcome();
            if (Input.GetButtonDown("Jump"))
            {
                Time.timeScale = 1f;
                CurrentState = GameState.Playing;
                titlePanel.SetActive(false);
            }
        }

        if (!isAlive)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartLevel();
            }
            return;
        }

        TickComboTimer();
        CheckOutOfBounds();

        if (CurrentState == GameState.Playing)
        {
            CheckOutOfBounds();
        }

        if (Input.GetKeyDown(KeyCode.L))
            resetHighScore();
    }

    private void Welcome()
    {
        titlePanel.SetActive(true);
    }

    //Called by cloud when player bounces updates points
    public void RegisterCloudBounce()
    {
        Combo++;
        comboTimer = comboTime;
        Score += pointsPerCloud * Combo;
    }


    // Tick down the combo timer
    private void TickComboTimer()
    {
        if (Combo == 0)
        {
            return;
        }

        comboTimer -= Time.deltaTime;
        if (comboTimer <= 0)
        {
            Combo = 0;
        }
    }

    private void CheckOutOfBounds()
    {
        if (player == null)
        {
            return;
        }

        Vector3 viewPos = cam.WorldToViewportPoint(player.position);
        if (viewPos.y < -0.15f)
        {
            PlayerDeath();
        }
    }


    public void PlayerDeath()
    {
        isAlive = false;
        CurrentState = GameState.GameOver;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        if (Score > PlayerPrefs.GetInt("player_HighScore"))
        {
            PlayerPrefs.SetInt("player_HighScore", Score);
            highScore = Score;
        }

        print("Updated highScore: " + highScore);
    }

    // Made a function just in case anything else is needed to restart the level in the future
    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Returns player reference
    public Transform GetPlayer()
    {
        return player;
    }

    // Returns camera reference
    public Camera GetCamera()
    {
        return cam;
    }
    public void PreserveCombo()
    {
        if (Combo > 0)
        {
            comboTimer += Time.deltaTime;
        }
    }

    public void resetHighScore()
    {
        PlayerPrefs.SetInt("player_HighScore", 0);
        highScore = 0;
    }

}
