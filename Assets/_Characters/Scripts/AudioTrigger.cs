using UnityEngine;

namespace RPG.Characters
{
    public class AudioTrigger : MonoBehaviour
    {
        AudioClip[] clips;
        float triggerRadius;
        bool isOneTimeOnly;

        bool hasPlayed = false;
        AudioSource audioSource;
        Character character;
        GameObject player;

        void Start ()
        {
            character = GetComponentInParent<Character> ();
            clips = character.GetAudioClips ();
            player = GameObject.FindWithTag ("Player");
            triggerRadius = character.GetTriggerRadius ();
            isOneTimeOnly = character.GetTriggerIsOneTimeOnly ();

            audioSource = gameObject.GetComponentInParent<AudioSource> ();           
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