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

    protected SpriteRenderer sr;
    protected Collider2D col;
    protected Rigidbody2D rb;
    protected Vector3 startPosition;

    protected float timer;
    protected float driftClock = 0f;

    //for player
    protected bool isJumpAvailable;
    protected bool isPlayerOn;

    protected void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        startPosition = transform.position;
    }


    protected virtual void Start()
    {
        isJumpAvailable = true;
        col.enabled = true;
        sr.sprite = isJumpAvailable ? collideSprite : phaseSprite;
        timer = isJumpAvailable ? collideDuration : phaseDuration;
        
    }


    protected virtual void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            isJumpAvailable = !isJumpAvailable;
  
            // Looking to add an additional state to this cloud for an 'indicator/intermediate' phase, but below works for now
            sr.sprite = isJumpAvailable ? collideSprite : phaseSprite;
            timer = isJumpAvailable ? collideDuration : phaseDuration;
        }
    }


    protected virtual void FixedUpdate()
    {
        Drift();
    }

    /*
     * Drift() controls the movement of the cloud using a sine function.
     * It moves the cloud platform up and down in a cyclical manner. 
     * */
    protected virtual void Drift()
    {
        if (!isPlayerOn)
        {
            easeDown();
            return;
        }
        else
        {
            driftClock += Time.deltaTime;
            float dist = Mathf.Sin(driftClock * cloudMoveSpeed) * cloudMoveDistance;
            rb.MovePosition(startPosition + (new Vector3(0f, dist, 0f)));
        }
        
    }

    // to be called by player
    public bool canJump()
    {
        return isJumpAvailable;
    }


    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnCloud();
        }
    }


    protected void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOffCloud();
        }
    }


    protected virtual void playerOnCloud()
    {
        if (!isPlayerOn)
        {
            isPlayerOn = true;
            driftClock = 0f;
        }
    }


    protected virtual void playerOffCloud()
    {
        isPlayerOn = false; 
    }


    protected virtual void easeDown()
    {
        if ((Vector3)rb.position == startPosition)
            return;

        Vector3 newpos = Vector3.Lerp(rb.position, startPosition, Time.deltaTime * cloudMoveSpeed);

        if ((newpos - startPosition).sqrMagnitude < 0.0001f)
            newpos = startPosition;

        rb.MovePosition(newpos);

    }

}
