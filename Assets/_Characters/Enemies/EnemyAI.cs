using System;
using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    [RequireComponent (typeof (Character))]
    [RequireComponent (typeof(WeaponSystem))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 4f;

        Character character;
        PlayerMovement player;

        enum State { idle, patrolling, attacking, chasing }
        State state = State.idle;
            
        float currentWeaponRange;
        float distanceToPlayer = 0f;

        private void Start ()
        {
            character = GetComponent<Character> ();
            player = FindObjectOfType<PlayerMovement> ();
        }

        private void Update ()
        {
            distanceToPlayer = Vector3.Distance (player.transform.position, transform.position);

            WeaponSystem weaponSystem = GetComponent<WeaponSystem> ();
            currentWeaponRange = weaponSystem.GetCurrentWeapon ().GetMaxAttackRange ();

            if (distanceToPlayer > chaseRadius && state != State.patrolling)
            {
                StopAllCoroutines ();
                StartCoroutine (StartPatrol ());
            }
            if (distanceToPlayer <= chaseRadius && state != State.chasing)
            {
                StopAllCoroutines ();
                StartCoroutine (ChasePlayer());
            }
            if (distanceToPlayer < currentWeaponRange && state != State.attacking)
            {
                StopAllCoroutines ();
                StartCoroutine (AttackPlayer ());
            }
        }

        IEnumerator ChasePlayer ()
        {
            state = State.chasing;
            while (distanceToPlayer >= currentWeaponRange && distanceToPlayer <= chaseRadius)
            {
                character.SetDestination (player.transform.position);
                yield return new WaitForEndOfFrame ();
            }
        }

        IEnumerator StartPatrol ()
        {
            state = State.patrolling;
            Debug.Log (this.name + " Patrolling");
            while (distanceToPlayer >= chaseRadius)
            {
                yield return new WaitForEndOfFrame ();
            }
        }

        IEnumerator AttackPlayer ()
        {
            state = State.attacking;
            Debug.Log (this.name + " Attacking");
            while (distanceToPlayer <= currentWeaponRange)
            {
                yield return new WaitForEndOfFrame ();
            }
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