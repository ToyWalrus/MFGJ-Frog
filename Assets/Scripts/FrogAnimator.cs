using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FrogAnimator : MonoBehaviour
{
    [SerializeField] private TongueController tongueController;

    private Animator _anim;
    private static readonly int MovingAnimProperty = Animator.StringToHash("Moving");
    private static readonly int JumpingAnimProperty = Animator.StringToHash("Jumping");
    private static readonly int SwingingAnimProperty = Animator.StringToHash("Swinging");
    private static readonly int GroundedAnimProperty = Animator.StringToHash("Grounded");
    private static readonly int QuickAnimProperty = Animator.StringToHash("Quick");

    private float _tongueDistance;

    // Start is called before the first frame update
    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    public void SetIsGrounded(bool grounded)
    {
        _anim.SetBool(GroundedAnimProperty, grounded);
    }

    public void SetIsMoving(bool isMoving)
    {
        _anim.SetBool(MovingAnimProperty, isMoving);
    }

    public void SetIsJumping(bool isJumping)
    {
        _anim.SetBool(JumpingAnimProperty, isJumping);
    }

    public void SetIsSwinging(bool isSwinging, Vector2 hitPoint)
    {
        if (isSwinging)
        {
            _anim.SetBool(SwingingAnimProperty, true);
            _tongueDistance = Vector2.Distance(hitPoint, transform.position);
            Invoke("ExtendTongue", 2f / 24f); // second frame is when tongue should extend
        }
        else
        {
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
        tongueController.ExtendTongue(_tongueDistance, .2f, false);
    }

    private void EnableFallingAnimation()
    {
        _anim.SetBool(SwingingAnimProperty, false);
    }
}