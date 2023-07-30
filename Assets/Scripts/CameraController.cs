using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform player_transform;
    [SerializeField] private float deadzone = 5f;
    [SerializeField] private float moveSpeed; 
    // Start is called before the first frame update
    void Start()
    {
        player_transform = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 relativePosition =  player_transform.position - transform.position;

        // Debug.Log(relativePosition.magnitude);
        if (relativePosition.magnitude > deadzone)
        {
            transform.Translate(relativePosition  * moveSpeed * Time.deltaTime);
        }
    }
}
