using UnityEngine;

namespace RPG.Characters
{
    public class AudioTrigger : MonoBehaviour
    {
        AudioClip[] clips;
        float triggerVolume;
        float triggerRadius;
        bool isOneTimeOnly;

        bool hasPlayed = false;
        AudioSource audioSource;
        //Character character;
        GameObject player;

        void Start ()
        {

            if (GetComponentInParent<Character> ())
            {
                Character character = GetComponentInParent<Character> ();
                clips = character.GetAudioClips ();
                triggerVolume = character.GetTriggerVolume ();
                triggerRadius = character.GetTriggerRadius ();
                isOneTimeOnly = character.GetTriggerIsOneTimeOnly ();
            }
            else
            {
                FXTrigger fxTrigger = GetComponentInParent<FXTrigger> ();
                clips = fxTrigger.GetAudioClips ();
                triggerVolume = fxTrigger.GetTriggerVolume ();
                triggerRadius = fxTrigger.GetTriggerRadius ();
                isOneTimeOnly = fxTrigger.GetTriggerIsOneTimeOnly ();
            }
            player = GameObject.FindWithTag ("Player");

            audioSource = gameObject.GetComponentInParent<AudioSource> ();
            audioSource.volume = triggerVolume;
        }

        void Update ()
        {
            float distanceToPlayer = Vector3.Distance (transform.position, player.transform.position);
            if (distanceToPlayer <= triggerRadius)
            {
                RequestPlayAudioClip ();
            }
        }

        void RequestPlayAudioClip ()
        {
            if (isOneTimeOnly && hasPlayed)
            {
                return;
            }
            else if (audioSource.isPlaying == false)
            {
                var clip = clips[Random.Range (0, clips.Length)];
                audioSource.PlayOneShot(clip);
                hasPlayed = true;
            }
        }

        void OnDrawGizmos ()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere (transform.position, triggerRadius);
        }
    }
}