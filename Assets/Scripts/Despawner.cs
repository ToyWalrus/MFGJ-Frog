using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Despawner : MonoBehaviour
{
    public Transform playerRespawnPoint;
    public LayerMask layersToDespawnOnEnter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the other object is on a layer that is in the layersToDespawnOnEnter mask, destroy it.
        var objectLayerMask = 1 << other.gameObject.layer;
        if ((layersToDespawnOnEnter & objectLayerMask) != 0)
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
    
    // Bit Logic Explanation:
    
    // "<<" is the bitwise left shift operator, which is used to shift the bits of the integer on the left
    // side of the operator to the left by the number on the right side of the operator. For example, 1 << 3
    // would shift 00000001 three places to the left, which would result in 00001000.
    
    // In the case of Unity layers, each layer is assigned a number that is a power of 2. Meaning
    // that each layer has it's own "bit" in the integer.
    
    // When using the "&" bitwise operator, the result is an integer that will have a 1 in each bit
    // that is 1 in both of the integers being compared. For example...
    
    // 00001000 (8)  & 00001000 (8) = 00001000 (8)
    // 00001000 (8)  & 00000100 (4) = 00000000 (0)
    // 00001100 (12) & 00001000 (8) = 00001000 (8)
    
    // So if the layer mask is 00001100 (12), and the object's layer is 00001000 (8), then the
    // result of the "&" operator will be 00001000 (8), which is not 0, and therefore on a matching
    // layer.
}