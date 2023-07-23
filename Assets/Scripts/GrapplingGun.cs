using System;
using UnityEngine;
using Utils;

public class GrapplingGun : MonoBehaviour
{
    public const float GrappleHookCheckRadius = .15f;

    public enum GrappleMode
    {
        PullToPoint,
        PullToPlayer,
        KeepAtDistance
    }


    public GrapplingRope grapplingRope;

    [SerializeField] private LayerMask grappleLayers;
    [SerializeField] private float maxDistance = 10;

    [Space(10)]
    public GameObject player;
    public Transform gunPivot;
    public Transform firePoint;
    [SerializeField] private float fireSpeed = 30;

    [Space(10), Header("When firing gun with M2 exclusively")]
    [SerializeField] private float maxRopeLengthAfterHooked = 3;
    [SerializeField] private float speedToDesiredRopeLength = 1;
    
    private GrappleMode _grappleMode = GrappleMode.PullToPoint;
    private Vector2 _grapplePoint;
    private float _ropeLength;
    
    private Vector2 _hookPosition;
    private bool _hookIsSet; // Whether the hook has reached the grapple point
    private bool _raycastDidHit;
    
    private SpringJoint2D _springJoint;
    private Rigidbody2D _rb;
    private Camera _cam;

    private bool HasFired => grapplingRope.IsDrawing;

    private void Start()
    {
        _hookPosition = firePoint.position;
        _springJoint = player.GetComponent<SpringJoint2D>();
        _rb = player.GetComponent<Rigidbody2D>();
        _cam = Camera.main;

        if (_springJoint == null)
        {
            Debug.LogError("The player component must have a SpringJoint2D component attached to it");
        }

        if (_rb == null)
        {
            Debug.LogError("The player must have a Rigidbody2D component attached to it");
        }

        if (grapplingRope == null)
        {
            Debug.LogError("The GrapplingGun component must have a GrapplingRope component reference");
        }

        // Reset grapple variables on start
        EndGrapple();
    }

    private void Update()
    {
        HandleInputPress();
        HandleInputHeld();
        HandleInputUp();

        if (HasFired) return;

        // Rotate the gun to face the mouse when not grappling
        RotateGun(_cam.ScreenToWorldPoint(Input.mousePosition));
    }

    private void HandleInputPress()
    {
        var fire1JustPressed = Input.GetButtonDown("Fire1");
        var fire2JustPressed = Input.GetButtonDown("Fire2");
        var neitherInputPressed = !fire1JustPressed && !fire2JustPressed;

        // If we have already fired or neither input was just pressed, do nothing
        if (HasFired || neitherInputPressed) return;

        // "Fire1" will pull the player towards the grapple point
        // "Fire2" will keep the player at a set distance from the grapple point
        _grappleMode = fire1JustPressed ? GrappleMode.PullToPoint : GrappleMode.KeepAtDistance;

        // Initiate the grapple
        FireGrapple();
    }

    private void HandleInputHeld()
    {
        // If neither input is being held, do nothing
        if (!Input.GetButton("Fire1") && !Input.GetButton("Fire2")) return;

        RotateGun(_grapplePoint);
        UpdateHookPosition();

        var desiredDirection = _grapplePoint - (Vector2)gunPivot.position;
        grapplingRope.DrawRope(firePoint.position, _hookPosition, desiredDirection, _hookIsSet);

        if (_hookIsSet && _raycastDidHit)
        {
            // If the hook has reached the grapple point and we hit something, handle the grapple
            HandleGrapple();
        }
        else if (Vector2.Distance(firePoint.position, _hookPosition) > maxDistance)
        {
            // If the hook is too far away, end the grapple
            EndGrapple();
        }
    }

    private void HandleInputUp()
    {
        var buttonWasNotReleased = !Input.GetButtonUp("Fire1") && !Input.GetButtonUp("Fire2");

        // If we haven't yet fired or neither input was just released, do nothing
        if (!HasFired || buttonWasNotReleased) return;

        EndGrapple();
    }

    private void RotateGun(Vector3 lookPoint)
    {
        var distanceVector = lookPoint - gunPivot.position;
        var angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;

        gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (!HasFired)
        {
            // Update the hook position to be the firePoint if we haven't fired yet
            _hookPosition = firePoint.position;
        }
    }

    private void FireGrapple()
    {
        var directionVector = _cam.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
        var normalizedDirection = directionVector.normalized;

        // Use raycast to check if we hit something
        var hit = Physics2D.Raycast(firePoint.position, normalizedDirection, maxDistance, grappleLayers);

        _raycastDidHit = hit;
        _grapplePoint = hit ? hit.point : normalizedDirection * maxDistance; 

        // Enable the rope renderer and set the starting position
        var startingPosition = firePoint.position;
        grapplingRope.EnableRope(startingPosition);
        _hookPosition = startingPosition;
    }

    private void UpdateHookPosition()
    {
        // If we haven't fired, the RotateGun() method will update the hook position
        // If the hook is already set, we don't need to update the its position
        if (!HasFired || _hookIsSet) return;

        // Move the hook position towards the grapple point
        _hookPosition = Vector2.MoveTowards(_hookPosition, _grapplePoint, Time.deltaTime * fireSpeed);

        // If the hook is close enough to the grapple point, snap it to the grapple point
        _hookIsSet = Vector2.Distance(_hookPosition, _grapplePoint) <= GrappleHookCheckRadius;
        if (_hookIsSet)
        {
            _hookPosition = _grapplePoint;
        }

        // Not sure if we need this...?
        // if (!_hookIsSet || _raycastDidHit) return;
        //
        // // Do one last check that we haven't hit something
        // var hitCheck = Physics2D.OverlapCircle(_hookPosition, GrappleHookCheckRadius, grappleLayers);
        // if (hitCheck)
        // {
        //     grapplePoint = hitCheck.ClosestPoint(_hookPosition);
        //     _raycastDidHit = true;
        // }
        // else
        // {
        //     // If raycast didn't hit anything and the hook is still not set, end the grapple
        //     EndGrapple();
        // }
    }

    private void HandleGrapple()
    {
        // Depending on the grapple mode, either pull the player towards the grapple point
        // or keep the player at a set distance from the grapple point
        switch (_grappleMode)
        {
            case GrappleMode.PullToPoint:
                PullToPoint();
                break;
            case GrappleMode.KeepAtDistance:
                KeepAtDistance();
                break;
            default:
                return;
        }
    }

    private void PullToPoint()
    {
        // Connect the spring joint at the point we hit the collider (grapplePoint)
        _springJoint.connectedAnchor = _grapplePoint;

        // Set the distance to the distance between the player and the fire point
        // (so the player doesn't get pulled in too close)
        _springJoint.distance = Vector2.Distance(firePoint.position, player.transform.position);

        // Set the spring joint's frequency to 0 so it doesn't oscillate
        _springJoint.frequency = 0;

        // Enable the spring joint
        _springJoint.enabled = true;
    }

    private void KeepAtDistance()
    {
        if (MathHelpers.FloatEquals(_ropeLength, float.MinValue))
        {
            var dist = Vector2.Distance(firePoint.position, _grapplePoint);
            _ropeLength = Math.Min(dist, maxRopeLengthAfterHooked);
        }

        _springJoint.distance = _ropeLength;
        _springJoint.frequency = speedToDesiredRopeLength;

        // If we want to automatically set the length of rope to the current distance, we can do this:
        // _springJoint.autoConfigureDistance = true;
        // _springJoint.frequency = 0;

        _springJoint.connectedAnchor = _grapplePoint;
        _springJoint.enabled = true;
    }

    private void EndGrapple()
    {
        // Reset all the grapple variables
        grapplingRope.DisableRope();
        _ropeLength = float.MinValue;
        _springJoint.enabled = false;
        _hookIsSet = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistance);
        }

        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_hookPosition, GrappleHookCheckRadius);
        }
    }
}