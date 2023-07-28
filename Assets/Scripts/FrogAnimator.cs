using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FrogAnimator : MonoBehaviour
{
    [SerializeField] private TongueController tongueController;

    private Animator _anim;
    private static readonly int MovingAnimProperty = Animator.StringToHash("Moving");
    private static readonly int SwingingAnimProperty = Animator.StringToHash("Swinging");
    private static readonly int GroundedAnimProperty = Animator.StringToHash("Grounded");
    private static readonly int QuickAnimProperty = Animator.StringToHash("Quick");
    private static readonly int JumpAnimProperty = Animator.StringToHash("Jump");

    private float _tongueDistance;
    private Vector2 _hitPoint;

    // Start is called before the first frame update
    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!_anim.GetBool(SwingingAnimProperty))
        {
            transform.rotation = Quaternion.identity;
            return;
        }

        var dirVector = _hitPoint - (Vector2)transform.position;
        var angle = Mathf.Atan2(dirVector.y, dirVector.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetIsGrounded(bool grounded)
    {
        _anim.SetBool(GroundedAnimProperty, grounded);
    }


    public void SetIsMoving(bool isMoving, int dir)
    {
        _anim.SetBool(MovingAnimProperty, isMoving);

        if (dir < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (dir > 0)
        {
            transform.localScale = Vector3.one;
        }
    }

    public void TriggerJump()
    {
        _anim.SetTrigger(JumpAnimProperty);
    }

    public void SetIsSwinging(bool isSwinging)
    {
        SetIsSwinging(isSwinging, _hitPoint);
    }

    public void SetIsSwinging(bool isSwinging, Vector2 hitPoint)
    {
        _hitPoint = hitPoint;
        if (isSwinging)
        {
            _anim.SetBool(SwingingAnimProperty, true);
            _tongueDistance = Vector2.Distance(hitPoint, transform.position);
            transform.localScale = Vector3.one;
            Invoke("ExtendTongue", 2f / 24f); // second frame is when tongue should extend
        }
        else
        {
            if (tongueController == null)
            {
                EnableFallingAnimation();
                return;
            }

            tongueController.RetractTongue(.3f);
            Invoke("EnableFallingAnimation", .3f);
        }
    }

    public void FlickTongue()
    {
        _anim.SetTrigger(QuickAnimProperty);
    }

    private void ExtendTongue()
    {
        if (tongueController == null) return;

        tongueController.ExtendTongue(_tongueDistance, .2f, false);
    }

    private void EnableFallingAnimation()
    {
        _anim.SetBool(SwingingAnimProperty, false);
    }
}