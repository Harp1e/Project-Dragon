using System;
using UnityEngine;

namespace RPG.Characters
{
    [RequireComponent(typeof(WeaponSystem))]
    public class NPCAI : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 4f;

        PlayerMovement player;

        private void Start ()
        {
            player = FindObjectOfType<PlayerMovement> ();
        }

        private void Update ()
        {
            float distanceToPlayer = Vector3.Distance (player.transform.position, transform.position);
        }

        private void OnDrawGizmos ()
        {
            // Draw chase sphere
            Gizmos.color = new Color (0f, 255f, 0f, 0.7f);
            Gizmos.DrawWireSphere (transform.position, chaseRadius);
        }
    }
}