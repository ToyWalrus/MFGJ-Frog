using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyPlatform : MonoBehaviour
{

    private float originalJumpForce;
    [SerializeField] private float jumpMultiplier;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            originalJumpForce = collision.gameObject.GetComponent<CharacterController>().jumpForce;
            collision.gameObject.GetComponent<CharacterController>().jumpForce *= jumpMultiplier;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<CharacterController>().jumpForce = originalJumpForce;
        }
    }
}
