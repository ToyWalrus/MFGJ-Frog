using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FallingPlatform : MonoBehaviour
{

    [SerializeField] float _floatTimer = 2f;
    [SerializeField] float fallingSpeed;
    private float fallTime = float.PositiveInfinity;
    private bool falling = false;
    public Material fallMaterial;

    private SpriteRenderer _Sr;
    // Start is called before the first frame update
    void Awake()
    {
        _Sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

       
        if(falling == true)
        {
            if (Time.time > fallTime)
            {
                fall();
                
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && falling == false)
        {
            startTimer();
        }

    }

    private void startTimer()
    {
        Debug.Log("falling");
        falling = true;
        fallTime = Time.time + _floatTimer;
        _Sr.material = fallMaterial;
    }
    private void fall()
    {
        transform.Translate(Vector2.down * Time.deltaTime * fallingSpeed);
    }
}
