using System;
using UnityEngine;
using UnityEngine.Events;
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


    // public GrapplingRope grapplingRope;

    [SerializeField] private LayerMask grappleLayers;
    [SerializeField] private float maxDistance = 10;

    [Space(10)]
    public SpringJoint2D joint;
    public Transform grapplePivot;
    public Transform firePoint;
    [SerializeField] private float fireSpeed = 30;

    [Space(10), Header("When firing gun with M2 exclusively")]
    [SerializeField] private float maxRopeLengthAfterHooked = 3;
    [SerializeField] private float speedToDesiredRopeLength = 1;

    [SerializeField] public string grappleModeReadout = "";
    private GrappleMode _grappleMode = GrappleMode.PullToPoint;
    [HideInInspector] public Vector2 grapplePoint;
    private float _ropeLength;

    private Vector2 _hookPosition;
    public Vector2 HookPosition => _hookPosition;
    [HideInInspector] public bool hookIsSet; // Whether the hook has reached the grapple point
    private bool _raycastDidHit;

    [Space(10)]
    public UnityEvent<Vector2, bool> onGrappleFire;
    public UnityEvent onGrappleFireForAudio;
    public UnityEvent onHookSet;
    public UnityEvent onGrappleSet;
    public UnityEvent onGrappleRelease;
    

    private Rigidbody2D _rb;
    private Camera _cam;

    [HideInInspector] public Collider2D hitCollider;

    private bool _hasFired = false; 
    public bool IsGrappled => joint.enabled;

    private void Start()
    {
        _hookPosition = firePoint.position;
        _rb = joint.GetComponent<Rigidbody2D>();
        _cam = Camera.main;

        if (joint == null)
        {
            Debug.LogError("The GrapplingGun component must have a SpringJoint2D component reference");
        }

        if (_rb == null)
        {
            Debug.LogError("The player must have a Rigidbody2D component attached to it");
        }

        // if (grapplingRope == null)
        // {
        //     Debug.LogError("The GrapplingGun component must have a GrapplingRope component reference");
        // }

        // Reset grapple variables on start
        _ropeLength = float.MinValue;
        joint.enabled = false;
        hookIsSet = false;

        onGrappleFire ??= new UnityEvent<Vector2, bool>();
        onGrappleRelease ??= new UnityEvent();
        onHookSet ??= new UnityEvent();
    }

    private void Update()
    {
        HandleInputPress();
        HandleInputHeld();
        HandleInputUp();

        if (_hasFired) return;

        // Rotate the gun to face the mouse when not grappling
        RotateGun(_cam.ScreenToWorldPoint(Input.mousePosition));
    }

    private void HandleInputPress()
    {
        var fire1JustPressed = Input.GetButtonDown("Fire1");
        var fire2JustPressed = Input.GetButtonDown("Fire2");
        var neitherInputPressed = !fire1JustPressed && !fire2JustPressed;

        // If we have already fired or neither input was just pressed, do nothing
        if (_hasFired || neitherInputPressed) return;

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

        RotateGun(grapplePoint);
        UpdateHookPosition();

        // grapplingRope.DrawRope(firePoint.position, _hookPosition, _hookIsSet);

        if (hookIsSet && _raycastDidHit)
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
        if (!_hasFired || buttonWasNotReleased) return;

        EndGrapple();
    }

    private void RotateGun(Vector3 lookPoint)
    {
        var distanceVector = lookPoint - grapplePivot.position;
        var angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;

        grapplePivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (!_hasFired)
        {
            // Update the hook position to be the firePoint if we haven't fired yet
            _hookPosition = firePoint.position;
        }
    }

    private void FireGrapple()
    {
        Vector2 grapplePosition = grapplePivot.position;
        Vector2 firePosition = firePoint.position;

        var directionVector = (Vector2)_cam.ScreenToWorldPoint(Input.mousePosition) - grapplePosition;
        var normalizedDirection = directionVector.normalized;

        // Use raycast to check if we hit something
        RaycastHit2D hit = Physics2D.Raycast(firePosition, normalizedDirection, maxDistance, grappleLayers);
        
        _raycastDidHit = hit;
        hitCollider = hit.collider;
        grapplePoint = hit ? hit.point : (normalizedDirection * maxDistance) + grapplePosition;

        // Enable the rope renderer and set the starting position
        // grapplingRope.EnableRope(firePosition, _grapplePoint - grapplePosition);
        _hasFired = true;
        _hookPosition = firePosition;
        
        onGrappleFire.Invoke(grapplePoint, hitCollider);
        onGrappleFireForAudio.Invoke();
        
    }

    private void UpdateHookPosition()
    {
        // If we haven't fired, the RotateGun() method will update the hook position
        // If the hook is already set, we don't need to update the its position
        if (!_hasFired || hookIsSet)
        {
            onHookSet.Invoke();
            _hookPosition = grapplePoint; //so that it moves with the moving platforms
            return;
        }
        // Move the hook position towards the grapple point
        _hookPosition = Vector2.MoveTowards(_hookPosition, grapplePoint, Time.deltaTime * fireSpeed);

        // If the hook is close enough to the grapple point, snap it to the grapple point
        hookIsSet = _hookPosition == grapplePoint;

        if (hookIsSet)
        {
            onHookSet.Invoke();
        }

        if (!hookIsSet || _raycastDidHit) return;

        // Do one last check that we haven't hit something
        var hitCheck = Physics2D.OverlapCircle(_hookPosition, GrappleHookCheckRadius, grappleLayers);
        if (hitCheck)
        {
            grapplePoint = hitCheck.ClosestPoint(_hookPosition);
            _raycastDidHit = true;
        }
        else
        {
            // If raycast didn't hit anything and the hook is still not set, end the grapple
            EndGrapple();
        }
    }
    private void HandleGrapple()
    {
        // Depending on the grapple mode, either pull the player towards the grapple point
        // or keep the player at a set distance from the grapple point
        switch (_grappleMode)
        {
            case GrappleMode.PullToPoint:
                grappleModeReadout = "pull";
                PullToPoint();
                break;
            case GrappleMode.KeepAtDistance:
                grappleModeReadout = "hold";
                KeepAtDistance();
                break;
            default:
                return;
        }
    }

    private void PullToPoint()
    {
        // Connect the spring joint at the point we hit the collider (grapplePoint)
        joint.connectedAnchor = grapplePoint;

        // Set the distance to the distance between the player and the fire point
        // (so the player doesn't get pulled in too close)
        joint.distance = Vector2.Distance(firePoint.position, joint.transform.position) + 2f;

        // Set the spring joint's frequency to 0 so it doesn't oscillate
        joint.frequency = 0;

        // Enable the spring joint
        joint.enabled = true;
    }

    private void KeepAtDistance()
    {
        if (MathHelpers.FloatEquals(_ropeLength, float.MinValue))
        {
            var dist = Vector2.Distance(firePoint.position, grapplePoint);
            _ropeLength = Math.Min(dist, maxRopeLengthAfterHooked);
        }

        joint.distance = _ropeLength;
        joint.frequency = speedToDesiredRopeLength;

        // If we want to automatically set the length of rope to the current distance, we can do this:
        // joint.autoConfigureDistance = true;
        // joint.frequency = 0;

        joint.connectedAnchor = grapplePoint;
        joint.enabled = true;
    }

    public void EndGrapple()
    {
        // Reset all the grapple variables
        // grapplingRope.DisableRope();
        _hasFired = false;
        _ropeLength = float.MinValue;
        joint.enabled = false;
        hookIsSet = false;
        
        onGrappleRelease.Invoke();
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
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_hookPosition, GrappleHookCheckRadius);
        }
    }
}