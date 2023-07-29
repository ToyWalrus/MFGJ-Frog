using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Despawner : MonoBehaviour
{
    public Transform respawnPoint;
    public bool despawnPlatforms = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Platform") && despawnPlatforms)
        {
            Destroy(other.gameObject);
            return;
        }
        
        if (!other.CompareTag("Player")) return;

        other.transform.position = respawnPoint.position;
        var playerRb = other.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero;
        }
    }
}
