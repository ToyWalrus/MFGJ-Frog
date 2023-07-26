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
            tongueController.FlickTongue();
        }

        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var length = Mathf.Abs(_cam.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x);
            tongueController.ExtendTongue(length, extendDuration, false);
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