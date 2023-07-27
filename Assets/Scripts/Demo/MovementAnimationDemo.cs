using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimationDemo : MonoBehaviour
{
    public Transform frog;
    public FrogAnimator frogAnimator;
    public float movementSpeed = 15f;

    private void Update()
    {
        var movement = Input.GetAxisRaw("Horizontal");

        frogAnimator.SetIsMoving(movement != 0);
        frogAnimator.SetIsJumping(Input.GetButton("Jump"));

        if (Input.GetButtonDown("Fire1"))
        {
            frogAnimator.FlickTongue();
        }

        frog.position += new Vector3(movement * movementSpeed * Time.deltaTime, 0, 0);

        if (movement < 0)
        {
            frog.localScale = new Vector3(-1, 1, 1);
        }
        else if (movement > 0)
        {
            frog.localScale = new Vector3(1, 1, 1);
        }
    }
}