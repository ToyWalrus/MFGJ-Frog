using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Despawner : MonoBehaviour
{
    public Transform playerRespawnPoint;

    public LayerMask layersToDespawnOnEnter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((layersToDespawnOnEnter & (1 << other.gameObject.layer)) != 0)
        {
            Destroy(other.gameObject);
            return;
        }

        if (!other.CompareTag("Player")) return;

        other.transform.position = playerRespawnPoint.position;
        var playerRb = other.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero;
        }
    }
}