using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
       [Header("Movement")]
    [SerializeField] private float maxSpeed = 8f;          // top horizontal speed (units/s)
    [SerializeField] private float acceleration = 60f;      // units/s^2 while holding a direction
    [SerializeField] private float friction = 70f;           // units/s^2 when no input (ground)
    [SerializeField] private float airAcceleration = 40f;    // slightly less control in air
    [SerializeField] private float airFriction = 30f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 3f;          // max height in units
    [SerializeField] private float jumpTimeToPeak = 0.4f;    // seconds to reach apex if held
    [SerializeField] private float jumpTimeToDescent = 0.3f; // seconds to fall back down
    [SerializeField] private float jumpCutMultiplier = 0.5f; // velocity multiplier when jump released early

    [SerializeField] public float boostVelocity = 6f;      // boosted jump velocity

    [Header("Quality of Life")]
    [SerializeField] private float coyoteTime = 0.1f;        // grace period to jump after leaving a ledge
    [SerializeField] private float jumpBufferTime = 0.1f;    // grace period if jump pressed before landing

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.6f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 velocity;
    private Rigidbody2D cloudRB;
    private BasicCloud cloudScript; // Reference to the cloud script so we can call its public methods

    private float jumpVelocity;
    private float jumpGravity;
    private float fallGravity;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool isJumping;
    public bool isGrounded;


    private void Awake()
    {
         rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // gravity is applied manually below

        // Derive jump velocity and the two gravity values (rising vs falling)
        // from the desired height and timing, so tuning stays intuitive:
        // just set "how high" and "how long", not raw physics constants.
        jumpVelocity = (2f * jumpHeight) / jumpTimeToPeak;
        jumpGravity = (2f * jumpHeight) / (jumpTimeToPeak * jumpTimeToPeak);
        fallGravity = (2f * jumpHeight) / (jumpTimeToDescent * jumpTimeToDescent);

    }

   private void Update()
    {
        // Input reads happen in Update so button presses aren't missed
        // between fixed steps.
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer = Mathf.Max(jumpBufferTimer - Time.deltaTime, 0f);
        }

        if (isJumping && Input.GetButtonUp("Jump") && velocity.y > 0f)
        {
            // Variable height: releasing early while still rising cuts
            // upward velocity short, so gravity takes over sooner.
            velocity.y *= jumpCutMultiplier;
            isJumping = false;
        }
    }

    private void FixedUpdate()
    {   
        CheckGrounded();
        UpdateTimers();
        ApplyGravity();
        HandleJumpStart();
        HandleHorizontalMovement();
        restrictPlayerWithinBounds();
        RideCloud();

        rb.linearVelocity = velocity;

        if (isGrounded && velocity.y <= 0f)
        {
            isJumping = false;
        }
    }

    public bool CheckGrounded()
    {
        Collider2D ground = groundCheck != null
            ? Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer)
            : null;

        isGrounded = ground != null;
        cloudRB = ground != null ? ground.attachedRigidbody : null;
        cloudScript = cloudRB != null ? cloudRB.gameObject.GetComponent<BasicCloud>() : null;

        velocity = rb.linearVelocity;
        return isGrounded;
    }
    

    private void UpdateTimers()
    {
        //logic that applies coyote time
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else //in the case that the player runs off the ground, we want to decrement the coyote timer until it reaches 0
        {  
            float decremented = coyoteTimer - Time.fixedDeltaTime; //decreases coyote timer by the time between fixed updates

            if (decremented > 0f)
            {
                coyoteTimer = decremented; 
            }
            else
            {
                coyoteTimer = 0f;
            }
        }
    }

    private void HandleJumpStart()
    {
        // Start a jump if buffered and coyote time is still available
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            velocity.y = jumpVelocity;
            if (cloudScript != null && cloudScript.isWeakpointAvailable())
            {
                velocity.y = boostVelocity;
                Debug.Log("Successful boost!");
            }
            isJumping = true;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }
    }

    private void ApplyGravity()
       {
          if (velocity.y > 0f)
         {
            // Rising: use the "to peak" gravity
            velocity.y -= jumpGravity * Time.fixedDeltaTime;
         }
         else
        {
            // Falling: use the steeper "descent" gravity for a snappier feel
            velocity.y -= fallGravity * Time.fixedDeltaTime;
        }
    }
    private void HandleHorizontalMovement()
    {
       // if (cloudRB != null){
        //    velocity.x = 0;
        //    return;
       // }
        
        float inputDir = Input.GetAxisRaw("Horizontal");

        float accel = isGrounded ? acceleration : airAcceleration; //a.i told me this is a shorthand notation for if statements, so if isGrounded is true, accel = acceleration, otherwise accel = airAcceleration
        float fric = isGrounded ? friction : airFriction;

        if (Mathf.Abs(inputDir) > 0.01f)
        {
            // Accelerate toward maxSpeed rather than snapping to it
            velocity.x = Mathf.MoveTowards(velocity.x, maxSpeed * inputDir, accel * Time.fixedDeltaTime);
        }
        else
        {
           
            velocity.x = 0f;
        }
    }
    
    // Moves w/ platform if not mid jump
    private void RideCloud(){
        if (cloudRB != null && !isJumping && rb.linearVelocityY <= 0){
            velocity.y = cloudRB.linearVelocity.y;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(0f, 0f, 0f), new Vector3(GameManager.Instance.boundaryWidth, 1f, 1f));
    }

// Public API for external sources to trigger a jump directly,
// bypassing coyote time / jump buffering (e.g. double-jump powerups, bounce pads).
public void ForceJump(float velocityMultiplier = 1f)
{
    Vector2 v = rb.linearVelocity;
    v.y = jumpVelocity * velocityMultiplier;
    rb.linearVelocity = v;
    velocity = v; // keep the cached field in sync so FixedUpdate doesn't fight it
    isJumping = true;
}

private void restrictPlayerWithinBounds()
{
    float halfBoundaryWidth = GameManager.Instance.boundaryWidth / 2f;
    Vector3 position = transform.position;

    if (position.x < -halfBoundaryWidth)
    {
        position.x = -halfBoundaryWidth;
        velocity.x = 0f; // Stop horizontal movement when hitting the boundary
        transform.position = position;
    }
    else if (position.x > halfBoundaryWidth)
    {
        position.x = halfBoundaryWidth;
        velocity.x = 0f; // Stop horizontal movement when hitting the boundary
    }

    transform.position = position;

}

    
}

