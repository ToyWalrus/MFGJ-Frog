using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueDemo : MonoBehaviour
{
    public TongueController tongueController;
    public float extendDuration = .3f;

    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.rotation = Quaternion.identity;
            tongueController.transform.rotation = transform.rotation;
            tongueController.FlickTongue();
        }

        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var dir = (Vector2)_cam.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            tongueController.transform.rotation = transform.rotation;

            tongueController.ExtendTongue(dir.magnitude, extendDuration, false);
            Invoke("SetTongueStuck", extendDuration * .375f);
        }

        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            CancelInvoke("SetTongueStuck");
            tongueController.SetTongueStuck(false);
            tongueController.RetractTongue(extendDuration / 2f);
        }
    }

    void SetTongueStuck()
    {
        tongueController.SetTongueStuck(true);
    }
}