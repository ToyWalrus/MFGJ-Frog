using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveLeft : MonoBehaviour
{
    private float moveSpeed;
    // Start is called before the first frame update
    void Awake()
    {
        moveSpeed = Random.Range(5, 10);
        var scale = transform.localScale;
        var newScale = Random.Range(10, 20);

        scale.Set(newX: newScale, newY: newScale, newZ: 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveSpeed * Vector2.left  * Time.deltaTime);
    }
}
