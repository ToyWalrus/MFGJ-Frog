using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueController : MonoBehaviour
{
    [SerializeField] private GrapplingGun grapplingGun;

    [Space(10)]
    [SerializeField] private SpriteRenderer tongueSegmentSpriteRenderer;
    [SerializeField] private SpriteRenderer tongueEndSpriteRenderer;
    [SerializeField] private Sprite tongueEndSprite;
    [SerializeField] private Sprite tongueStuckSprite;

    private bool _isStuck = false;

    // Update is only fired when object is enabled
    private void Update()
    {
        var dir = grapplingGun.grapplePoint - (Vector2)transform.position;
        var tongueStartPosition = (Vector2)tongueSegmentSpriteRenderer.transform.position;
        var hookDistance = Vector2.Distance(grapplingGun.HookPosition, tongueStartPosition);
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (_isStuck)
        {
            // We want the tongue end to line up with the hook's grappled position
            tongueEndSpriteRenderer.transform.position = grapplingGun.HookPosition - (dir.normalized * 1.2f);
        }
        else
        {
            // We want the tongue end to follow the same trajectory as the tongue segment
            tongueEndSpriteRenderer.transform.position = tongueStartPosition + (dir.normalized * (hookDistance * 0.8f));
        }

        tongueSegmentSpriteRenderer.size = new Vector2(hookDistance, tongueSegmentSpriteRenderer.size.y);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // Controller will be enabled when the hook is fired, and disabled when the hook is released
    private void OnEnable()
    {
        tongueEndSpriteRenderer.sprite = tongueEndSprite;
        _isStuck = false;

        grapplingGun.onHookSet.AddListener(OnHookSet);
    }

    private void OnDisable()
    {
        grapplingGun.onHookSet.RemoveListener(OnHookSet);
    }

    private void OnHookSet()
    {
        tongueEndSpriteRenderer.sprite = tongueStuckSprite;
        _isStuck = true;
    }
}