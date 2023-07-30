using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform player_transform;
    [SerializeField] private float deadzone = 2f;
    [SerializeField] private float moveSpeed = 4f; 
    // Start is called before the first frame update
    void Start()
    {
        player_transform = GameObject.Find("Frog").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 relativePosition =  (player_transform.position+(Vector3.up *2)) - transform.position;


        if (relativePosition.magnitude > deadzone)
        {
            transform.Translate(relativePosition  * moveSpeed * Time.deltaTime);
        }
    }
}
