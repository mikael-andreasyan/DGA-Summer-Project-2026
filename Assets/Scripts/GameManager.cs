using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
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
    

    [Header("Bounds")]
    [Tooltip("The width of the boundaries that the player will be confined to.")]
    [SerializeField] public float boundaryWidth = 30f;

    [Header("Death")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject newRecordText;

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject countdownText;
    [SerializeField] private GameObject escapeText;

    [Header("Start Platform")]
    [SerializeField] private GameObject startPlatform;
    [SerializeField] private Vector2 platformOffset;


    private float comboTimer;
    private bool isAlive = true;
    private bool isPaused = false;
    private bool isUnPausing = false;

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

    public int highScore
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

        if (SceneManager.GetActiveScene().name.Equals(mainSceneName))
        {

            GameObject.Instantiate(startPlatform, (Vector2)player.position - platformOffset, player.rotation);
        }

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
        
        handlePause();
    }

    private void Welcome()
    {
        titlePanel.SetActive(true);
    }

    // Called by cloud when the player lands on a fresh cloud
    public void RegisterCloudBounce()
    {
        Score += pointsPerCloud;
    }

    // Increases combo when landing on weakpoint of cloud
    public void RegisterBoost()
    {
        Combo++;
        comboTimer = comboTime;
        Score += pointsPerCloud * Combo;
    }


    public void LoseCombo()
    {
        Combo = 0;
        comboTimer = 0f;
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
        newRecordText.SetActive(false);

        Time.timeScale = 0f;

        if (Score > PlayerPrefs.GetInt("player_HighScore"))
        {
            PlayerPrefs.SetInt("player_HighScore", Score);
            highScore = Score;
            newRecordText.SetActive(true);
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

    public void handlePause()
    {

        if (CurrentState == GameState.Playing && Input.GetKeyDown(KeyCode.Escape))
        {
            print("Paused!");
            pausePanel.SetActive(true);
            countdownText.SetActive(false);
            escapeText.SetActive(true);


            Time.timeScale = 0f;
            isPaused = true;
            CurrentState = GameState.Paused;
        }

        else if (CurrentState == GameState.Paused && Input.GetKeyDown(KeyCode.Q))
        {
            print("quit game");
            Application.Quit();
        }

        else if (CurrentState == GameState.Paused && Input.GetKeyDown(KeyCode.Escape))
        {
           if (!isUnPausing)
            StartCoroutine(ResumeGame());
        }
    }

    IEnumerator ResumeGame()
    {
        print("Unpausin!)");
        
        isUnPausing = true;
        escapeText.SetActive(false);
        countdownText.SetActive(true);

        TMPro.TextMeshProUGUI textComponent = countdownText.GetComponent<TMPro.TextMeshProUGUI>();

        textComponent.text = "3";
        yield return new WaitForSecondsRealtime(1f);

        textComponent.text = "2";
        yield return new WaitForSecondsRealtime(1f);

        textComponent.text = "1";
        yield return new WaitForSecondsRealtime(1f);


        countdownText.SetActive(false);
        escapeText.SetActive(true);
        pausePanel.SetActive(false);

        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        isPaused = false;
        isUnPausing = false;


    }




}
