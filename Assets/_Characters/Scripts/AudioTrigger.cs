using UnityEngine;

namespace RPG.Characters
{
    public class AudioTrigger : MonoBehaviour
    {
        AudioClip[] clips;
        int layerFilter;
        float triggerRadius;
        bool isOneTimeOnly;

        bool hasPlayed = false;
        AudioSource audioSource;
        Character character;

        void Start ()
        {
            character = GetComponentInParent<Character> ();
            clips = character.GetAudioClips ();
            layerFilter = character.GetTriggerLayerFilter ();
            triggerRadius = character.GetTriggerRadius ();
            isOneTimeOnly = character.GetTriggerIsOneTimeOnly ();

            audioSource = gameObject.GetComponentInParent<AudioSource> ();
            
            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider> ();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = triggerRadius;
            gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");
        }

        void OnTriggerEnter (Collider other)
        {
            if (other.gameObject.layer == layerFilter)
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