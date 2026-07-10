using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance
    {
        get;
        private set;
    }

    public enum GameState {PreStart, Playing, Paused, GameOver }
    public GameState CurrentState { get; private set; } = GameState.PreStart;

    [Header("Scoring")]
    [SerializeField] private int pointsPerCloud = 100; // whatever we want
    [SerializeField] private float comboTime = 2f; // whatever we want

    [Header("Bounds")]
    [Tooltip("The width of the boundaries that the player will be confined to.")]
    [SerializeField] public float boundaryWidth = 30f;

    [Header("Death")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera cam;

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;

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

    // Update is called once per frame
    void Update()
    {
        if(CurrentState == GameState.PreStart && Input.GetButtonDown("Jump"))
            CurrentState = GameState.Playing;



        if (!isAlive)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartLevel();
            }
            return;
        }

        TickComboTimer();
    }

    //Called by cloud when player bounces updates points
    public void RegisterCloudBounce()
    {
        Combo++;
        comboTimer = comboTime;
        Score += pointsPerCloud * Combo;
        print("Combo: " + Combo + " Score: " + Score);
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


    public void PlayerDeath()
    {
        isAlive = false;
        gameOverPanel.SetActive(true);
    }

    // Made a function just in case anything else is needed to restart the level in the future
    private void RestartLevel()
    {
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

}
