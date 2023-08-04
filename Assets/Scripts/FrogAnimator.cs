using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FrogAnimator : MonoBehaviour
{
    private Animator _anim;
    private static readonly int MovingAnimProperty = Animator.StringToHash("Moving");
    private static readonly int SwingingAnimProperty = Animator.StringToHash("Swinging");
    private static readonly int GroundedAnimProperty = Animator.StringToHash("Grounded");
    private static readonly int QuickAnimProperty = Animator.StringToHash("Quick");
    private static readonly int JumpAnimProperty = Animator.StringToHash("Jump");
    private static readonly int WallHoldAnimProperty = Animator.StringToHash("WallHold");

    // It's pretty hacky to put this here this way, but it's the last day so YOLOOOO
    public Collider2D frogCollider;
    public Vector2 frogColliderRightFacing;
    public Vector2 frogColliderLeftFacing;
    
    private Vector2 _hitPoint;

    // Start is called before the first frame update
    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        var isSwinging = _anim.GetBool(SwingingAnimProperty);
        if (!isSwinging)
        {
            transform.rotation = Quaternion.identity;
            return;
        }

        var dirVector = _hitPoint - (Vector2)transform.position;
        var angle = Mathf.Atan2(dirVector.y, dirVector.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        // Always make sure sprite is facing the right direction while swinging
        transform.localScale = Vector3.one;
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
            frogCollider.offset = frogColliderLeftFacing;
        }
        else if (dir > 0)
        {
            transform.localScale = Vector3.one;
            frogCollider.offset = frogColliderRightFacing;
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
        _anim.SetBool(SwingingAnimProperty, isSwinging);
    }

    public void UpdateGrapplePoint(Vector2 point)
    {
        _hitPoint = point;
    }

    public void FlickTongue()
    {
        _anim.SetTrigger(QuickAnimProperty);
    }
    
    public void WallHold(bool isHoldingWall)
    {
        _anim.SetBool(WallHoldAnimProperty, isHoldingWall);
    }
}