using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] float floatTimer = 2f;
    [SerializeField] float fallingSpeed;
    private float _fallTime = float.PositiveInfinity;
    private bool _falling = false;
    public Material fallMaterial;

    private SpriteRenderer _sr;

    // Start is called before the first frame update
    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_falling != true) return;

        if (Time.time > _fallTime)
        {
            Fall();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_falling)
        {
            StartTimer();
        }
    }

    private void StartTimer()
    {
        _falling = true;
        _fallTime = Time.time + floatTimer;
        _sr.material = fallMaterial;
    }

    private void Fall()
    {
        transform.Translate(Vector2.down * Time.deltaTime * fallingSpeed);
    }
}