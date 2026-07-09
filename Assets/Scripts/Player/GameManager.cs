using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance
    {
        get;
        private set;
    }

    [Header("Scoring")]
    [SerializeField] private int pointsPerCloud = 100; // whatever we want
    [SerializeField] private float comboTime = 2f; // whatever we want

    [Header("Death")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera cam;

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
        // do some death stuff
        if (!isAlive)
        {
            return;
        }

        TickComboTimer();
        // CheckOutOfBounds();
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

    // check player tranform to see if outside camera bounds
    private void CheckOutOfBounds()
    {

    }

    private void PlayerDeath()
    {

    }

    // Returns player reference
    public Transform GetPlayer() {
        return player;
    }

}
