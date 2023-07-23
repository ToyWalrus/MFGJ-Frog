using UnityEngine;
using UnityEngine.Events;

// Code derived from Brackey's movement tutorial https://youtu.be/dwcT-Dch0bA
public class CharacterController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 400f; // Amount of force added when the player jumps.
    [SerializeField, Range(0, .3f)] private float movementSmoothing = .05f; // How much to smooth out the movement
    [SerializeField] private LayerMask whatIsGround; // A mask determining what is ground to the character
    [SerializeField] private Transform groundCheck; // A position marking where to check if the player is grounded.
    [SerializeField] private Transform ceilingCheck; // A position marking where to check for ceilings

    public const float GroundedCheckRadius = .2f; // Radius of the overlap circle to determine if grounded

    public const float
        CeilingCheckRadius = .2f; // Radius of the overlap circle to determine if the player is hitting a ceiling

    private bool _grounded; // Whether or not the player is grounded.
    private Rigidbody2D _rb;
    private bool _facingRight = true; // For determining which way the player is currently facing.
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
    }

    public bool IsGrounded()
    {
        return _grounded;
    }

    /// <summary>
    /// The primary movement function. Call this to update the rigidbody.
    /// </summary>
    public void Move(float move)
    {
        if (_grounded)
        {
            GroundMovement(move);
        }
        else
        {
            AirMovement(move);
        }

        CheckForReversedDirection(move);
    }

    public void Jump()
    {
        // If the player should jump...
        // Add a vertical force to the player.
        _rb.AddForce(new Vector2(0f, jumpForce));
    }

    private void GroundMovement(float move)
    {
        var velocity = _rb.velocity;

        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move * 10f, velocity.y);
        // And then smoothing it out and applying it to the character
        _rb.velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref _velocity, movementSmoothing);
    }

    private void AirMovement(float move)
    {
        var velocity = _rb.velocity;

        // Half movement speed in the air
        Vector3 targetVelocity = new Vector2(move * 5f, velocity.y);
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

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        _facingRight = !_facingRight;

        // Multiply the player's x local scale by -1.
        var scale = transform.localScale;
        scale.Set(scale.x * -1, scale.y, scale.z);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, GroundedCheckRadius);
    }
}