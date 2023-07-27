using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FrogAnimator : MonoBehaviour
{
    private Animator _anim;
    private static readonly int MovingAnimProperty = Animator.StringToHash("Moving");
    private static readonly int JumpingAnimProperty = Animator.StringToHash("Jumping");
    private static readonly int SwingingAnimProperty = Animator.StringToHash("Swinging");
    private static readonly int QuickAnimProperty = Animator.StringToHash("Quick");

    // Start is called before the first frame update
    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    public void SetIsMoving(bool isMoving)
    {
        _anim.SetBool(MovingAnimProperty, isMoving);
    }
    
    public void SetIsJumping(bool isJumping)
    {
        _anim.SetBool(JumpingAnimProperty, isJumping);
    }
    
    public void SetIsSwinging(bool isSwinging)
    {
        _anim.SetBool(SwingingAnimProperty, isSwinging);
    }

    public void FlickTongue()
    {
        _anim.SetTrigger(QuickAnimProperty);
    }
}
