
using UnityEngine;

public class BouncyPlatform : MonoBehaviour
{

    [SerializeField] private float bounceForce = 600f;
    // Start is called before the first frame update


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse) ;
        }
    }

}
