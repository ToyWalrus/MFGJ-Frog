using UnityEngine;

public class GrapplingRope : MonoBehaviour
{
    public GrapplingGun grapplingGun;

    [Space(10)]
    [SerializeField] private int linePrecision = 40;
    [SerializeField] private float ropeTightenSpeed = 5;
    [SerializeField] private float amplitudeMultiply = 2;
    [SerializeField] private AnimationCurve ropeShape;
    private float amplitude = 0;



    public bool IsDrawing => lineRenderer.enabled;

    private float moveTime = 0;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    public void EnableRope(Vector2 startPosition)
    {
        moveTime = 0;
        lineRenderer.positionCount = linePrecision;
        amplitude = amplitudeMultiply;

        for (var i = 0; i < linePrecision; i++)
        {
            lineRenderer.SetPosition(i, startPosition);
        }

        lineRenderer.enabled = true;
    }

    public void DisableRope()
    {
        lineRenderer.enabled = false;
    }


    private void Update()
    {
        moveTime += Time.deltaTime;
    }

    public void DrawRope(Vector2 startPoint, Vector2 endPoint, bool isTaut)
    {
        if (amplitude > 0)
        {
            if (isTaut)
            {
                amplitude -= Time.deltaTime * ropeTightenSpeed;
            }
            DrawRopeWithWaves(startPoint, endPoint);
        }
        else if (isTaut)
        {

            DrawStraightLine(startPoint, endPoint);
        }
    }

    private void DrawRopeWithWaves(Vector2 startPoint, Vector2 endPoint)
    {
        for (var i = 0; i < linePrecision; i++)
        {
            var delta = i / (linePrecision - 1f);
            var offset = Vector2.Perpendicular(grapplingGun.grappleDistanceVector).normalized * ropeShape.Evaluate(delta) * amplitude;
            var targetPosition = Vector2.Lerp(startPoint, endPoint, delta) + offset;
            var currentPosition = Vector2.Lerp(startPoint, targetPosition, delta);

            lineRenderer.SetPosition(i, currentPosition);
        }
    }

    private void DrawStraightLine(Vector2 startPoint, Vector2 endPoint)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }
}
