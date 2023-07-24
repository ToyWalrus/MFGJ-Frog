using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed = 15f;
    [SerializeField] public GrapplingGun grappler;

    private float _movement = 0f;
    private bool _jumpPressed = false;

    [SerializeField, Tooltip("How long after jumping can the player jump again?")]
    private float jumpCooldown = 0.3f;
    private float _jumpCooldownTimer = 0f;

    // Make sure that this is always less than the jump cooldown, otherwise you can get into
    // unintended multi-jump situations
    [SerializeField, Tooltip("How long after leaving the ground can the player still jump?")]
    private float coyoteTime = 0.1f;
    private float _coyoteTimeTimer = 0f;

    [SerializeField, Tooltip("How many jumps can the player do before touching the ground again?")]
    private int maxJumps = 1;
    private int _jumpsSinceLastGrounded = 0;

    private void OnEnable()
    {
        controller.onLandEvent.AddListener(ResetJumpCount);
    }

    private void OnDisable()
    {
        controller.onLandEvent.RemoveListener(ResetJumpCount);
    }

    // Get input
    private void Update()
    {
        _movement = Input.GetAxisRaw("Horizontal");

        if (!CheckCanJump(Time.deltaTime)) return;

        _jumpPressed = Input.GetButton("Jump");

    }

    // Move player
    private void FixedUpdate()
    {
        // Don't accept movement if we're grappling
        if (_jumpPressed)
        {
            
            
            controller.Jump(grappler.IsGrappled, grappler.grappleModeReadout);
            _jumpsSinceLastGrounded++;

            if (grappler.IsGrappled)
            {
                grappler.EndGrapple();
            }
        }

        if (grappler != null && grappler.IsGrappled) return;
        
        if (_movement != 0f)
        {
            controller.Move(_movement * speed * Time.fixedDeltaTime);
        }

        

        ResetJumpPressed();
    }

    private bool CheckCanJump(float deltaTime)
    {
        UpdateCoyoteTime(deltaTime);
        UpdateJumpCooldown(deltaTime);
        return _jumpCooldownTimer <= 0f && !_jumpPressed && (_jumpsSinceLastGrounded < maxJumps || _coyoteTimeTimer >= 0f);
    }

    private void UpdateCoyoteTime(float deltaTime)
    {
        if (controller.IsGrounded())
        {
            _coyoteTimeTimer = coyoteTime;
        }
        else
        {
            _coyoteTimeTimer -= deltaTime;
        }
    }

    private void UpdateJumpCooldown(float deltaTime)
    {
        _jumpCooldownTimer -= deltaTime;
    }
    
    private void ResetJumpPressed()
    {
        if (_jumpPressed)
        {
            _jumpCooldownTimer = jumpCooldown;
        }

        _jumpPressed = false;
    }

    private void ResetJumpCount()
    {
        _jumpsSinceLastGrounded = 0;
    }
}