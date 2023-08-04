
using System.Net;
using UnityEngine;
using UnityEngine.UIElements;

public class MovingPlatform : MonoBehaviour
{

    private Transform player_Transform;

    private GrapplingGun grapplingGun;

    [SerializeField]  float  moveSpeed = 8f;



    private bool riding;

    public bool platformGrappled;




    private Vector3 Endpoint;
    private Vector3 originalPos;

    private Vector3 moveDirection;

    private Vector3 relativePosition;

    private bool _switchDirection = false;

    // Start is called before the first frame update
    void Awake()
    {

        Endpoint = transform.Find("EndPoint").transform.position;
        originalPos = transform.position;
        moveDirection = (Endpoint - originalPos).normalized;

        
        riding = false;
        player_Transform = GameObject.Find("Frog").GetComponent<Transform>();
        grapplingGun = GameObject.Find("Grappler").GetComponent<GrapplingGun>();

    }
    void LateUpdate()
    {
        if (grapplingGun.hookIsSet)
        {

            if (grapplingGun.hitCollider == gameObject.GetComponent<Collider2D>())
            {
                // Debug.Log("grappled");
                platformGrappled = true;
            }
            else
            {
                platformGrappled = false;
            }

        }
        
        if (_switchDirection)
        {
            relativePosition =  transform.position - originalPos;

        }
        else
        {
            relativePosition = transform.position - Endpoint;

        }
        if (relativePosition.magnitude < 0.5)
        {
            flip();
        }

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);


        if (riding == true)
        {
            player_Transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }
        
        if (platformGrappled == true)
        {
            grapplingGun.grapplePoint += (new Vector2(moveDirection.x, moveDirection.y) * moveSpeed * Time.deltaTime);
        }
       

    }



    private void flip()
    {
        moveDirection *= -1;
        _switchDirection = !_switchDirection;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {     
        if (collision.gameObject.CompareTag("Player"))
        {
            riding = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            riding = false;
        }
    }

}


