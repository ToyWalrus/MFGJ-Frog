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

    private void Update()
    {
        var dir = grapplingGun.grapplePoint - (Vector2)transform.position;
        var hookDistance = Vector2.Distance(grapplingGun.HookPosition, tongueSegmentSpriteRenderer.transform.position);
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        tongueEndSpriteRenderer.transform.position = grapplingGun.HookPosition - (dir.normalized * 1.2f);
        tongueSegmentSpriteRenderer.size = new Vector2(hookDistance, tongueSegmentSpriteRenderer.size.y);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // Controller will be enabled when the hook is fired, and disabled when the hook is released
    private void OnEnable()
    {
        tongueEndSpriteRenderer.sprite = tongueEndSprite;

        grapplingGun.onHookSet.AddListener(OnHookSet);
    }

    private void OnDisable()
    {
        grapplingGun.onHookSet.RemoveListener(OnHookSet);
    }

    private void OnHookSet()
    {
        tongueEndSpriteRenderer.sprite = tongueStuckSprite;
    }
}