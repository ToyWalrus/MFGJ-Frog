
using UnityEngine;

public class KillOnContact : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 respawnPoint;

    private void Start()
    {
        respawnPoint = GameObject.Find("RespawnPoint").transform.position;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Debug.Log("respawning");
        collision.gameObject.transform.position = respawnPoint;
    }
}
