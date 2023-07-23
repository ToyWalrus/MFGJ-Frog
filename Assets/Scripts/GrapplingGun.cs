using UnityEngine;
using Utils;

// TODO: create "cooldown" for gun, animate rope back to gun

public class GrapplingGun : MonoBehaviour {
    public GrapplingRope grapplingRope;

    [SerializeField]
    private LayerMask grappleLayers;
    [SerializeField]
    private float maxDistance = 10;

    [Space(10)]
    public GameObject player;
    public Transform gunPivot;
    public Transform firePoint;

    [Space(10)]
    [SerializeField]
    private bool usePhysics = true;
    [SerializeField]
    private float launchSpeed = 2;
    [SerializeField]
    private float fireSpeed = 30;

    [Header("When firing gun with M2 exclusively")]
    [SerializeField]
    private float desiredRopeLengthAfterHooked = 3;
    [SerializeField]
    private float speedToDesiredRopeLength = 1;

    [HideInInspector]
    public Vector2 grapplePoint;
    [HideInInspector]
    public Vector2 grappleDistanceVector;

    public bool IsGrappled => springJoint.enabled;

    private float ropeLength;
    private bool useDesiredLength = false;
    private bool pullToPoint = true;
    private bool raycastDidHit = false;
    private bool hookIsSet = false;
    private bool canFire = true;
    private Vector2 hookPosition;
    private SpringJoint2D springJoint;
    private Rigidbody2D rb;
    private Camera cam;
    private float prevGravScale;

    private bool HasFired => grapplingRope.IsDrawing;

    private void Start() {
        hookPosition = firePoint.position;
        springJoint = player.GetComponent<SpringJoint2D>();
        rb = player.GetComponent<Rigidbody2D>();
        cam = Camera.main;
        prevGravScale = rb.gravityScale;

        if (springJoint == null) {
            Debug.LogError("The player component must have a SpringJoint2D component attached to it");
        }

        if (rb == null) {
            Debug.LogError("The player must have a Rigidbody2D component attached to it");
        }

        ropeLength = float.MinValue;
        springJoint.autoConfigureDistance = false;
        springJoint.enabled = false;
    }

    private void Update() {
        var screenMousePos = Input.mousePosition;
        if (!HandleMousePressThisFrame()) {
            HandleMouseHeld();
        }

        if (!HandleMouseUpThisFrame() && !HasFired) {
            Vector2 mousePos = cam.ScreenToWorldPoint(screenMousePos);
            RotateGun(mousePos);
        }
    }

    private bool HandleMousePressThisFrame() {
        if (HasFired || !canFire) return false;

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            pullToPoint = true;
            useDesiredLength = false;
            FireGrapple();
            return true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            pullToPoint = false;
            useDesiredLength = true;
            FireGrapple();
            return true;
        }

        return false;
    }

    private void HandleMouseHeld() {
        if (!Input.GetKey(KeyCode.Mouse0) && !Input.GetKey(KeyCode.Mouse1)) return;

        RotateGun(grapplePoint);
        UpdateHookPosition();
        grapplingRope.DrawRope(firePoint.position, hookPosition, hookIsSet);

        if (hookIsSet && raycastDidHit) {
            Grapple();
        }
        else if (Vector2.Distance(firePoint.position, hookPosition) > maxDistance) {
            EndGrapple();
        }
    }

    private bool HandleMouseUpThisFrame() {
        if (HasFired && (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1))) {
            EndGrapple();
            return true;
        }

        return false;
    }

    private void RotateGun(Vector3 lookPoint) {
        var distanceVector = lookPoint - gunPivot.position;
        var angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (!HasFired) {
            hookPosition = firePoint.position;
        }
    }

    private void FireGrapple() {
        Vector2 distanceVector = cam.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
        var hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized, maxDistance, grappleLayers);
        if (hit) {
            grapplePoint = hit.point;
            raycastDidHit = true;
        }
        else {
            grapplePoint = distanceVector.normalized * maxDistance;
            raycastDidHit = false;
        }

        grappleDistanceVector = grapplePoint - (Vector2) gunPivot.position;
        var position = firePoint.position;
        grapplingRope.EnableRope(position);
        hookPosition = position;
    }

    private void UpdateHookPosition() {
        if (HasFired && !hookIsSet) {
            hookPosition = Vector2.MoveTowards(hookPosition, grapplePoint, Time.deltaTime * fireSpeed);
            hookIsSet = hookPosition == grapplePoint;

            if (hookIsSet && !raycastDidHit) {
                if (Physics2D.OverlapCircle(hookPosition, .25f, grappleLayers)) {
                    // Do one last check that we haven't hit something
                    raycastDidHit = true;
                }
                else {
                    EndGrapple();
                }
            }
        }
    }

    public void Grapple() {
        if (pullToPoint) {
            if (usePhysics) {
                Vector2 distanceVector = firePoint.position - player.transform.position;

                springJoint.connectedAnchor = grapplePoint;
                springJoint.distance = distanceVector.magnitude;
                springJoint.frequency = 0;
                springJoint.enabled = true;
            }
            else {
                prevGravScale = rb.gravityScale;
                rb.gravityScale = 0;
                rb.velocity = Vector2.zero;

                Vector2 firePointDistance = firePoint.position - player.transform.localPosition;
                var targetPos = grapplePoint - firePointDistance;
                player.transform.position = Vector2.MoveTowards(player.transform.position, targetPos,
                    Time.deltaTime * launchSpeed * 10);
            }
        }
        else {
            if (useDesiredLength) {
                if (MathHelpers.FloatEquals(ropeLength, float.MinValue)) {
                    var dist = Vector2.Distance(firePoint.position, grapplePoint);
                    ropeLength = dist < desiredRopeLengthAfterHooked ? dist : desiredRopeLengthAfterHooked;
                }

                springJoint.distance = ropeLength;
                springJoint.frequency = speedToDesiredRopeLength;
            }
            else {
                springJoint.autoConfigureDistance = true;
                springJoint.frequency = 0;
            }

            springJoint.connectedAnchor = grapplePoint;
            springJoint.enabled = true;
        }
    }

    private void EndGrapple() {
        grapplingRope.DisableRope();
        ropeLength = float.MinValue;
        springJoint.enabled = false;
        rb.gravityScale = prevGravScale;
        hookIsSet = false;
    }

    private void OnDrawGizmosSelected() {
        if (firePoint != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistance);
        }

        if (Application.isPlaying) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hookPosition, .5f);
        }
    }

}
