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

    }

    // called by player when getting pointS
    private void UpdateScore()
    {

    }

    //Called by cloud when player bounces
    private void RegisterCloudBounce()
    {

    }


    // Tick down the combo timer
    private void TickComboTimer()
    {

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
