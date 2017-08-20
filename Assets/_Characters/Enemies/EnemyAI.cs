using System;
using UnityEngine;

namespace RPG.Characters
{
    [RequireComponent(typeof(WeaponSystem))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 4f;

        PlayerMovement player;

        bool isAttacking = false;   // TODO More rich state
        float currentWeaponRange;

        private void Start ()
        {
            player = FindObjectOfType<PlayerMovement> ();
        }

        private void Update ()
        {
            WeaponSystem weaponSystem = GetComponent<WeaponSystem> ();
            currentWeaponRange = weaponSystem.GetCurrentWeapon ().GetMaxAttackRange ();

            float distanceToPlayer = Vector3.Distance (player.transform.position, transform.position);
        }

        private void OnDrawGizmos ()
        {
            // Draw attack sphere
            Gizmos.color = new Color (255f, 0f, 0f, 0.7f);
            Gizmos.DrawWireSphere (transform.position, currentWeaponRange);

            // Draw chase sphere
            Gizmos.color = new Color (0f, 255f, 0f, 0.7f);
            Gizmos.DrawWireSphere (transform.position, chaseRadius);
        }
    }
}