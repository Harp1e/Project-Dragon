using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXTrigger : MonoBehaviour 
{
    [SerializeField] GameObject audioTriggerPrefab;
    [SerializeField] AudioClip[] fxClips;
    [SerializeField] [Range (0f,1f)] float fxVolume = 1f;
    [SerializeField] float triggerDistance = 5f;
    [SerializeField] bool isOneTimeOnly = false;

    public AudioClip[] GetAudioClips () { return fxClips; }
    public float GetTriggerVolume () { return fxVolume; }
    public float GetTriggerRadius () { return triggerDistance; }
    public bool GetTriggerIsOneTimeOnly () { return isOneTimeOnly; }

    void Start () 
	{
        var audioSource = gameObject.AddComponent<AudioSource> ();
        audioSource.playOnAwake = false;
        Instantiate (audioTriggerPrefab, gameObject.transform);
    }
}
