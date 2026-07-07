using UnityEngine;

/// <summary>
/// Celeste-style 2D Player Controller (Unity, legacy Input Manager)
/// - Ground movement with acceleration/deceleration toward a top speed
///   (not instant, not floaty).
/// - Variable-height jump: hold Space to reach full jump height,
///   release early to cut the jump short and fall sooner.
///
/// Setup:
/// - Attach to a GameObject with Rigidbody2D + Collider2D.
/// - Rigidbody2D: set Gravity Scale to 0 (this script drives gravity manually
///   so it can use different values for rising vs falling).
/// - Assign a "Ground" layer to your floor/platform colliders and set
///   groundLayer in the inspector, plus add a groundCheck child transform
///   near the player's feet.
/// - Uses the legacy Input Manager: Input.GetAxisRaw("Horizontal") and
///   Input.GetButton*("Jump") (Space is bound to "Jump" by default in
///   Unity's Input Manager).
/// </summary>
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

    [Header("Quality of Life")]
    [SerializeField] private float coyoteTime = 0.1f;        // grace period to jump after leaving a ledge
    [SerializeField] private float jumpBufferTime = 0.1f;    // grace period if jump pressed before landing

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.6f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 velocity;

    private float jumpVelocity;
    private float jumpGravity;
    private float fallGravity;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool isJumping;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // gravity is applied manually, so we don't need 

        // Derive jump velocity and the two gravity values (rising vs falling)
        // from the desired height and timing, so tuning stays intuitive:
        // just set "how high" and "how long", not raw physics constants.
        jumpVelocity = (2f * jumpHeight) / jumpTimeToPeak; // v = 2 * h / t, kinematics eqaution
        jumpGravity = (2f * jumpHeight) / (jumpTimeToPeak * jumpTimeToPeak); // g = 2 * h / t^2, another kinematics equation
        fallGravity = (2f * jumpHeight) / (jumpTimeToDescent * jumpTimeToDescent); // steeper gravity for falling
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

        if (isJumping && Input.GetButtonUp("Jump") && velocity.y > 0f) //if jump is released while player is ascending
        {
            
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

        rb.linearVelocity = velocity;

        if (isGrounded && velocity.y <= 0f)
        {
            isJumping = false;
        }
    }

    private void CheckGrounded()
    {
        isGrounded = groundCheck != null &&
            Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        velocity = rb.linearVelocity;
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

    private void ApplyGravity()
    {
        if (velocity.y > 0f && Input.GetButton("Jump")) //if the player is ascending and holding jump
        {
            
            velocity.y -= jumpGravity * Time.fixedDeltaTime; //decrease its velocity by the jump gravity, the gravity that applies to the player while ascending
        }
        else
        {
            // if the player is falling, however, or ascending and not holding jump, apply the fall gravity, which is a steeper gravity that makes the player fall faster
            velocity.y -= fallGravity * Time.fixedDeltaTime;
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
}
