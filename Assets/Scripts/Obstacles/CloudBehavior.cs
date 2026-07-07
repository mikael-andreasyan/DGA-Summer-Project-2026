using UnityEngine;

public class CloudBehavior : MonoBehaviour
{
    public Sprite collideSprite;
    // public Sprite intermediateSprite; - need to add a type of indicator visual to show a cloud is about to not be jumpable
    public Sprite phaseSprite;

    public float collideDuration = 3f;
    public float phaseDuration = 2f;

    public float cloudMoveSpeed = 1f;
    public float cloudMoveDistance = 1f;

    private SpriteRenderer sr;
    private Collider2D col;
    private Vector3 startPosition;
    private bool isEnabled;
    private float timer; 


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        startPosition = transform.position;
    }


    void Start()
    {
        isEnabled = true;
        col.enabled = isEnabled;
        sr.sprite = isEnabled ? collideSprite : phaseSprite;
        timer = isEnabled ? collideDuration : phaseDuration;
        
    }


    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            isEnabled = !isEnabled;
            col.enabled = isEnabled;
            // Looking to add an additional state to this cloud for an 'indicator/intermediate' phase, but below works for now
            sr.sprite = isEnabled ? collideSprite : phaseSprite;
            timer = isEnabled ? collideDuration : phaseDuration;
        }
        Drift();
    }

    /*
     * Drift() controls the movement of the cloud using a sine function.
     * It moves the cloud platform up and down in a cyclical manner. 
     * */
    private void Drift()
    {
        float dist = Mathf.Sin( Time.time * cloudMoveSpeed ) * cloudMoveDistance;
        transform.position = startPosition + (new Vector3(0f, dist, 0f));
    }
}
