using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GrapplingRope : MonoBehaviour
{
    [SerializeField, Tooltip("The higher this count, the more smooth the line will look")]
    private int linePrecision = 40;
    [SerializeField] private float ropeTightenSpeed = 5;
    [SerializeField] private float amplitudeMultiply = 2;
    [SerializeField] private AnimationCurve ropeShape;

    private float _amplitude = 0f;
    private Vector2 _distance;

    private LineRenderer _lineRenderer;
    public bool IsDrawing => _lineRenderer.enabled;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
    }

    public void EnableRope(Vector2 startPosition, Vector2 ropeDistance)
    {
        _distance = ropeDistance;
        _lineRenderer.positionCount = linePrecision;
        _amplitude = amplitudeMultiply;

        for (var i = 0; i < linePrecision; i++)
        {
            // Set all points on line renderer to start position
            _lineRenderer.SetPosition(i, startPosition);
        }

        // Start drawing the line
        _lineRenderer.enabled = true;
    }

    public void DisableRope()
    {
        // Stop drawing the line
        _lineRenderer.enabled = false;
    }

    public void DrawRope(Vector2 startPoint, Vector2 endPoint, bool isTaut)
    {
        if (_amplitude > 0)
        {
            if (isTaut)
            {
                // Decrease amplitude if the rope is taut (tightening the rope)
                _amplitude -= Time.deltaTime * ropeTightenSpeed;
            }

            // Draw the rope along the rope shape curve
            DrawRopeAlongCurve(startPoint, endPoint);
        }
        else if (isTaut)
        {
            // Draw a straight line if the rope is taut and the amplitude is 0
            DrawStraightLine(startPoint, endPoint);
        }
    }

    private void DrawRopeAlongCurve(Vector2 startPoint, Vector2 endPoint)
    {
        var perpendicularDirection = Vector2.Perpendicular(_distance).normalized;

        for (var i = 0; i < linePrecision; i++)
        {
            // Determine the point along the rope shape curve
            var delta = i / (linePrecision - 1f);

            // Evaluate the value of the curve at the current delta
            var pointValue = ropeShape.Evaluate(delta) * _amplitude;

            // Offset the point along the perpendicular direction (so the line is not always horizontal)
            var offset = perpendicularDirection * pointValue;

            // Lerp between the start and end points, then add the offset (curve)
            var targetPosition = Vector2.Lerp(startPoint, endPoint, delta) + offset;

            // Lerp between the start point and the target position to get the final position at the current delta
            var currentPosition = Vector2.Lerp(startPoint, targetPosition, delta);

            // Set the current point on the line renderer to currentPosition
            _lineRenderer.SetPosition(i, currentPosition);
        }
    }

    private void DrawStraightLine(Vector2 startPoint, Vector2 endPoint)
    {
        // Simplify the line renderer to only have 2 points, since it's a straight line
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, startPoint);
        _lineRenderer.SetPosition(1, endPoint);
    }
}