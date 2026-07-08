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
    [SerializeField] public float jumpVelocity = 4f;      // the velocity that the player's y receives

    [Header("Quality of Life")]
    [SerializeField] public float coyoteTime = 0.1f;        // grace period to jump after leaving a ledge
    [SerializeField] public float jumpBufferTime = 0.1f;    // grace period if jump pressed before landing

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.6f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    [Header("Gravity")]
    [SerializeField] private float gravityScale = 1f;

    private Rigidbody2D rb;
    private Vector2 velocity;

    public float coyoteTimer;
    public float jumpBufferTimer;
    private bool isJumping;
    public bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;

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
            isJumping = false;
        }
    }

    private void FixedUpdate()
    {   
        CheckGrounded();
        UpdateTimers();
        HandleJumpStart();
        HandleHorizontalMovement();

        rb.linearVelocity = velocity;

        if (isGrounded && velocity.y <= 0f)
        {
            isJumping = false;
        }
    }

    private bool CheckGrounded()
    {
        isGrounded = groundCheck != null &&
            Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

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
            isJumping = true;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }
    }

    private void HandleHorizontalMovement()
    {
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
            // No input: decelerate back to 0 using friction
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, fric * Time.fixedDeltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
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

}
