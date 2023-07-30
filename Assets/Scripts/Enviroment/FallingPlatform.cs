using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] float floatTimer = 2f;
    [SerializeField] float fallingSpeed;
    private float _fallTime = float.PositiveInfinity;
    private bool _falling = false;
    public Material fallMaterial;

    private int shakeDir = 1;
    [SerializeField] private float shakeAmmount;
    [SerializeField] float shakeRate;
    private float shakeIt = -1;
    private SpriteRenderer _Sr;
    // Start is called before the first frame update
    void Awake()
    {
        _Sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

       
        if(_falling == true)
        {
            if (Time.time > _fallTime)
            {
                fall();
                _falling = false;
                
            }
            Shake();
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_falling)
        {
            startTimer();
        }
    }

    private void Shake()
    {
        if (Time.time > shakeIt)
        {
            transform.Translate(Vector2.left * shakeDir * shakeAmmount);
            shakeIt += shakeRate;
            shakeDir *= -1;
        }
    }
    private void startTimer()
    {
        Debug.Log("falling");
        _falling = true;
        _fallTime = Time.time + floatTimer;
        _Sr.material = fallMaterial;

    }

    private void fall()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;

        foreach (Transform child in transform)
        {
            child.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(0,0.4f), Random.Range(-0.1f,-0.4f)), ForceMode2D.Impulse);
            child.GetComponent<Rigidbody2D>().gravityScale = 1;
            
            child.GetComponent<PolygonCollider2D>().isTrigger = false;
            
        }
    }
}
