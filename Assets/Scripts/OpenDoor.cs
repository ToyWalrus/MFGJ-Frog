using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{

    private Vector3 Endpoint;
    private Vector3 moveDirection;
    private Vector3 originalPos;
    public bool openDoor;


    // Start is called before the first frame update
    void Awake()
    {
        openDoor = false;
        Endpoint = transform.Find("EndPoint").transform.position;
        originalPos = transform.position;
        moveDirection = (originalPos - Endpoint).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if(openDoor && (Endpoint-transform.position).magnitude > 0.5f)
        {
            transform.Translate(moveDirection * 20 * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
    
        openDoor = true;
    }
}
