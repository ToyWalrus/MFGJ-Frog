using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]  float moveRange;
    [Header(" -1/1 positive means positive movement in the selected axis")]
    [SerializeField]  int rangeDir = 1;

    private Transform player_Transform;

    private GrapplingGun grapplingGun;

    private Vector2 moveDir;

    [SerializeField]  bool moveOnYAxis = false;
    [SerializeField]  float  moveSpeed = 8f;

    private float originalPos;
    private float currentPos;
    private float endPos;

    private bool riding;

    private float unstuckOffset = 10f;

    public bool platformGrappled;

    // Start is called before the first frame update
    void Awake()
    {
        riding = false;
        player_Transform = GameObject.Find("Player").GetComponent<Transform>();
        grapplingGun = GameObject.Find("Grappler").GetComponent<GrapplingGun>();
        if(moveOnYAxis)
        {
            originalPos = transform.position.y;
            moveDir = Vector3.up * moveSpeed *-1;
        }
        else
        {
            originalPos = transform.position.x;
            moveDir = Vector2.right * moveSpeed * -1;
        }
       
        endPos = originalPos + moveRange * rangeDir;
    }

    // Update is called once per frame

    private void Update()
    {
        if (grapplingGun._hookIsSet && grapplingGun.hitACollider)
        {
            try
            {
                if (grapplingGun.hitCollider == gameObject.GetComponent<Collider2D>())
                {
                    platformGrappled = true;
                }
                else
                {
                    platformGrappled = false;
                }
            }catch(NullReferenceException)
            { 
            Debug.Log("noCollider");
                platformGrappled = false;
            }
            
        }
    }
    void LateUpdate()
    {
        //calculate apropiate currentPos
        if (moveOnYAxis)
        {
            currentPos = transform.position.y;
        }
        else
        {
            currentPos = transform.position.x;
        }


        if (rangeDir > 0)
        {
            if (currentPos < originalPos)//if reached original pos switch direction of movement
            {
                transform.position.Set(transform.position.x + unstuckOffset, originalPos, 0);
                moveDir *= -1;

            }
            else if (currentPos > endPos) //if reached final pos switch direction of movement
            {
                transform.position.Set(transform.position.x - unstuckOffset, endPos, 0);
                moveDir *= -1;
            }
        }
        else if (rangeDir < 0)
        {
            if (currentPos > originalPos)//if reached original pos switch direction of movement
            {
                moveDir *= -1;
                transform.position.Set(originalPos,transform.position.y - unstuckOffset , 0);
            }
            else if (currentPos < endPos) //if reached final pos switch direction of movement
            {
                moveDir *= -1;
                transform.position.Set(endPos, transform.position.y + unstuckOffset, 0);
            }        
         }

        transform.Translate(moveDir * Time.deltaTime);

        if(moveOnYAxis == false)
        {
            if (riding == true)
            {
                player_Transform.Translate(moveDir * Time.deltaTime);
            }
        }
        if (platformGrappled == true)
        {
            grapplingGun._grapplePoint += (moveDir * Time.deltaTime);
        }
  
        
     }
    private void OnTriggerEnter2D(Collider2D collision)
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


