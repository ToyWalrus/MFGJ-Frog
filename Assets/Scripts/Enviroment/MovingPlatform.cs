using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]  float moveRange;

    private Transform player_Transform;

    private GrapplingGun grapplingGun;

    private Vector2 moveDir;

    [SerializeField]  bool moveOnYAxis = false;
    [SerializeField]  float  moveSpeed = 8f;

    private float originalPos;
    private float currentPos;
    private float endPos;

    private bool riding;

    public bool platformGrappled;


    // Start is called before the first frame update
    void Awake()
    {
        riding = false;
        player_Transform = GameObject.Find("Frog").GetComponent<Transform>();
        grapplingGun = GameObject.Find("Grappler").GetComponent<GrapplingGun>();

        if(moveOnYAxis)
        {
            originalPos = transform.position.y;
            moveDir = Vector3.up * moveSpeed * -1;
        }
        else
        {
            originalPos = transform.position.x;
            moveDir = Vector2.right * moveSpeed * -1;
        }
       
        endPos = originalPos + moveRange;

        
    }
    void LateUpdate()
    {
        if (grapplingGun.hookIsSet)
        {

            if (grapplingGun.hitCollider == gameObject.GetComponent<Collider2D>())
            {
                Debug.Log("grappled");
                platformGrappled = true;
            }
            else
            {
                platformGrappled = false;
            }

        }
        //calculate apropiate currentPos
        if (moveOnYAxis)
        {
            currentPos = transform.position.y;
        }
        else
        {
            currentPos = transform.position.x;
        }
        if (currentPos < originalPos)//if reached original pos switch direction of movement
        {

            moveDir *= -1;
            transform.Translate(moveDir * 0.05f);
                

        }
        else if (currentPos > endPos) //if reached final pos switch direction of movement
        {
                
            moveDir *= -1;
            transform.Translate(moveDir * 0.05f);
        }

        transform.Translate(moveDir * Time.deltaTime);

        
        if (riding == true)
        {
            player_Transform.Translate(moveDir * Time.deltaTime);
        }
        
        if (platformGrappled == true)
        {
            grapplingGun.grapplePoint += (moveDir * Time.deltaTime);
        }
  
        
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


