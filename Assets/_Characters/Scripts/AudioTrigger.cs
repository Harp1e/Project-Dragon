using UnityEngine;

namespace RPG.Characters
{
    public class AudioTrigger : MonoBehaviour
    {
        [SerializeField] AudioClip[] clips;
        [SerializeField] int layerFilter = 10;  // TODO Remove dependance on layers?
        [SerializeField] float triggerRadius = 3f;
        [SerializeField] bool isOneTimeOnly = true;

        bool hasPlayed = false;
        AudioSource audioSource;

        void Start ()
        {
            audioSource = gameObject.GetComponentInParent<AudioSource> ();
            
            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider> ();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = triggerRadius;
            var item = GetComponent<AudioTrigger> ();
            gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");
        }

        void OnTriggerEnter (Collider other)
        {
            // TODO Remove dependance on layers?
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