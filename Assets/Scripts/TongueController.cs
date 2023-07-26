using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueController : MonoBehaviour
{
    [SerializeField] private Animator tongueAnimator;
    [SerializeField] private SpriteRenderer tongueSpriteRenderer;

    private static readonly int QuickTongueAnimProperty = Animator.StringToHash("Quick");
    private static readonly int ButtonHeldAnimProperty = Animator.StringToHash("ButtonHeld");
    private static readonly int StuckAnimProperty = Animator.StringToHash("Stuck");

    private const float TongueExtendAnimDuration = .375f;
    private const float TongueRetractAnimDuration = .375f;
    private const float FlickAnimDuration = .375f;
    private const float Framerate = 1f / 24f;

    private float _baseTongueSizeX;

    private void Start()
    {
        _baseTongueSizeX = tongueSpriteRenderer.size.x;
    }

    public void FlickTongue()
    {
        float duration = .458f;
        StopAllCoroutines();

        tongueAnimator.speed = duration / FlickAnimDuration;
        tongueAnimator.SetTrigger(QuickTongueAnimProperty);
        StartCoroutine(ExtendTongueDuringFlick(duration * FlickAnimDuration));
    }

    public void ExtendTongue(float distance, float duration = .5f, bool retractAfterDuration = true)
    {
        StopAllCoroutines();

        tongueAnimator.speed = duration / TongueExtendAnimDuration;
        tongueAnimator.SetBool(ButtonHeldAnimProperty, true);
        StartCoroutine(ExtendTongueOverDuration(distance, duration * TongueExtendAnimDuration, retractAfterDuration));
    }

    public void SetTongueStuck(bool isStuck)
    {
        tongueAnimator.SetBool(StuckAnimProperty, isStuck);
    }

    public void RetractTongue(float duration = .3f)
    {
        StopAllCoroutines();

        tongueAnimator.speed = duration / TongueRetractAnimDuration;
        tongueAnimator.SetBool(ButtonHeldAnimProperty, false);
        StartCoroutine(RetractTongueOverDuration(duration * TongueRetractAnimDuration));
    }

    private IEnumerator ExtendTongueDuringFlick(float duration)
    {
        var startTime = Time.time;
        var sizeY = tongueSpriteRenderer.size.y;

        ResetTongueSize();

        while (Time.time - startTime < duration)
        {
            var stepSize = GetStepSize(1.17f, duration);
            tongueSpriteRenderer.size = new Vector2(tongueSpriteRenderer.size.x + stepSize, sizeY);
            yield return new WaitForSeconds(Framerate);
        }

        ResetTongueSize();
    }

    // This is separate from the animation -- this is stretching the tongue sprite
    private IEnumerator ExtendTongueOverDuration(float distance, float duration, bool retractAfterDuration)
    {
        var startTime = Time.time;
        var sizeY = tongueSpriteRenderer.size.y;

        ResetTongueSize();

        while (Time.time - startTime < duration)
        {
            var stepSize = GetStepSize(distance, duration);
            tongueSpriteRenderer.size = new Vector2(tongueSpriteRenderer.size.x + stepSize, sizeY);
            yield return new WaitForSeconds(Framerate);
        }

        if (retractAfterDuration)
        {
            RetractTongue();
        }
    }

    private IEnumerator RetractTongueOverDuration(float duration)
    {
        var startTime = Time.time;
        var size = tongueSpriteRenderer.size;
        var distance = size.x - _baseTongueSizeX;
        var sizeY = size.y;


        while (Time.time - startTime < duration)
        {
            var stepSize = GetStepSize(distance, duration);
            tongueSpriteRenderer.size =
                new Vector2(Mathf.Max(tongueSpriteRenderer.size.x - stepSize, _baseTongueSizeX), sizeY);
            yield return new WaitForSeconds(Framerate);
        }

        ResetTongueSize();
    }

    private float GetStepSize(float distance, float duration)
    {
        return Framerate * distance / duration;
    }

    private void ResetTongueSize()
    {
        tongueSpriteRenderer.size = new Vector2(_baseTongueSizeX, tongueSpriteRenderer.size.y);
    }
}