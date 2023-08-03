using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GrapplingGun grappler;
    [SerializeField] private CharacterController controller;

    private AudioSource audioSource;

    [SerializeField] private AudioClip Jump;
    [SerializeField] private AudioClip Land;
    [SerializeField] private AudioClip Hop;
    [SerializeField] private AudioClip TongueContact;
    [SerializeField] private AudioClip TongueSwing;
    [SerializeField] private AudioClip tongueShoot;

    private void OnEnable()
    {
        controller.onLandEvent.AddListener(PlayLandAudio);
        controller.onJumpEvent.AddListener(PlayJumpAudio);
        grappler.onGrappleFireForAudio.AddListener(PlayGrappleAudio);
    }

    private void OnDisable()
    {
        controller.onLandEvent.RemoveListener(PlayLandAudio);
        controller.onJumpEvent.RemoveListener(PlayJumpAudio);
        grappler.onGrappleFireForAudio.RemoveListener(PlayGrappleAudio);
  
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();  
    }

    private void PlayLandAudio()
    {

    }

    private void PlayJumpAudio()
    {
        
    }
    private void PlayGrappleAudio()
    {

    }


}
