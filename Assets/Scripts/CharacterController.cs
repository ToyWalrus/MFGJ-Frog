using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;

// Code derived from Brackey's movement tutorial https://youtu.be/dwcT-Dch0bA
public class CharacterController : MonoBehaviour
{
    [SerializeField] public float jumpForce = 450f; // Amount of force added when the player jumps.
    [SerializeField, Range(0, .3f)] private float movementSmoothing = .05f; // How much to smooth out the movement
    [SerializeField] private LayerMask whatIsGround; // A mask determining what is ground to the character
    [SerializeField] private Transform groundCheck; // A position marking where to check if the player is grounded.
    [SerializeField] private Transform WallCheck; // A position marking where to check if the player is sliding on a wall.
    [SerializeField] private float groundLinearDrag = 3; // Linear drag when grounded
    
    public const float GroundedCheckRadius = .2f; // Radius of the overlap circle to determine if grounded
    
    private bool _grounded; // Whether or not the player is grounded.
    private bool _sliding;
    private Rigidbody2D _rb;
    private bool _facingRight = true; // For determining which way the player is currently facing.
    private int _facingRightInt = 1; //For calculating movement direction on jump more easily
    private Vector3 _velocity = Vector3.zero; // Velocity of the player. Managed by the SmoothDamp function.

    [Space(20)]
    public UnityEvent onLandEvent; // Event triggered when the player lands after being in the air.

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        onLandEvent ??= new UnityEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = _grounded;
        _grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        var colliders = Physics2D.OverlapCircleAll(groundCheck.position, GroundedCheckRadius, whatIsGround);
        foreach (var collider in colliders)
        {
            // Ignore self
            if (collider.gameObject == gameObject) continue;

            _grounded = true;
            if (!wasGrounded)
            {
                onLandEvent.Invoke();
            }
        }

        // When on the ground, increase drag so that the player doesn't slide after releasing movement keys
        _rb.drag = _grounded ? groundLinearDrag : 0.05f;


        bool wasSliding = _sliding;
        _sliding = false;

        var wallColliders = Physics2D.OverlapCircleAll(WallCheck.position, GroundedCheckRadius, whatIsGround);
        foreach (var collider in wallColliders)
        {
            // Ignore self
            if (collider.gameObject == gameObject) continue;

            _sliding = true;
            if (!wasSliding && !IsGrounded())
            {
                onLandEvent.Invoke();
            }
        }

        if (isWallSliding())
        {
            gameObject.transform.Find("FrogAnimator").GetComponent<FrogAnimator>().WallHold(true);
        }
        else
        {
            gameObject.transform.Find("FrogAnimator").GetComponent<FrogAnimator>().WallHold(false);
        }
    }


    public bool IsGrounded()
    {
        return _grounded;
    }

    public bool isWallSliding()
    {
        return _sliding;
    }

    /// <summary>
    /// The primary movement function. Call this to update the rigidbody.
    /// </summary>
    public void Move(float move)
    {
        if (_grounded)
        {
            GroundMovement(move);
            gameObject.GetComponent<AudioSource>().Play();
        }
        else
        {
            AirMovement(move);
        }

        CheckForReversedDirection(move);
    }

    public void Jump( bool grappling, string grappleMode)
    {
        // If the player should jump...
        // Add a vertical force to the player.
        
        _rb.velocity = Vector3.zero;
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void GroundMovement(float move)
    {
        var velocity = _rb.velocity;

        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move * 15f, velocity.y);
        // And then smoothing it out and applying it to the character
        _rb.velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref _velocity, movementSmoothing);
    }

    private void AirMovement(float move)
    {
        var velocity = _rb.velocity;

        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move * 10f, velocity.y);
        // And then smoothing it out and applying it to the character
        _rb.velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref _velocity, movementSmoothing);

    }

    private void CheckForReversedDirection(float move)
    {
        var hasReversedDirection = move > 0 && !_facingRight || move < 0 && _facingRight;

        // If the input is moving the player in the opposite direction that it's facing...
        if (hasReversedDirection)
        {
            // ... flip the player.
            Flip();
        }
    }

    public void Flip()
    {
        // Switch the way the player is labelled as facing.
        _facingRight = !_facingRight;
        _facingRightInt *= -1;

        // Multiply the player's x local scale by -1.
        var scale = transform.localScale;
        scale.Set(scale.x * -1, scale.y, scale.z);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, GroundedCheckRadius);
        Gizmos.DrawWireSphere(WallCheck.position, GroundedCheckRadius);
    }
}