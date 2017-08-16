using UnityEngine;

namespace RPG.Characters
{
    public class AudioTrigger : MonoBehaviour
    {
        [SerializeField] AudioClip clip;
        [SerializeField] int layerFilter = 10;  // TODO Remove dependance on layers?
        [SerializeField] float triggerRadius = 5f;
        [SerializeField] bool isOneTimeOnly = true;

        bool hasPlayed = false;
        AudioSource audioSource;

        void Start ()
        {
            audioSource = gameObject.GetComponent<AudioSource> ();

            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider> ();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = triggerRadius;
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
                audioSource.PlayOneShot(clip);
                hasPlayed = true;
            }
        }

        void OnDrawGizmos ()
        {
            Gizmos.color = new Color (0, 255f, 0, .5f);
            Gizmos.DrawWireSphere (transform.position, triggerRadius);
        }
    }
}