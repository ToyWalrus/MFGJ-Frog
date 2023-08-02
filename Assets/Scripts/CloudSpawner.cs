using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{

    public GameObject[] clouds;
    public float spawnDelay;
    public float spawnRate;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("spawnCloud", spawnDelay, spawnRate);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void spawnCloud()
    {
        Instantiate(clouds[Random.Range(0, clouds.Length)], transform.position + new Vector3(0, Random.Range(0, -150),0), transform.rotation);
    }
}
