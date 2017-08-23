using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Characters
{
    [RequireComponent (typeof (Character))]
    [RequireComponent (typeof (WeaponSystem))]
    public class NPCAI : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 4f;
        [SerializeField] WaypointContainer patrolPath;
        [SerializeField] float waypointWaitTime = 0.5f;
        [SerializeField] float patrolSpeed = 0.5f;
        [SerializeField] bool isCompanion = false;

        Character character;
        NavMeshAgent agent;
        PlayerMovement player;

        enum State { idle, patrolling, talking, chasing }
        State state = State.idle;

        float currentWeaponRange;
        float distanceToPlayer;
        float originalSpeed;
        int nextWaypointIndex;


        private void Start ()
        {
            character = GetComponent<Character> ();
            agent = character.GetComponent<NavMeshAgent> ();
            player = FindObjectOfType<PlayerMovement> ();
            originalSpeed = agent.speed;
        }

        private void Update ()
        {
            distanceToPlayer = Vector3.Distance (player.transform.position, transform.position);

            WeaponSystem weaponSystem = GetComponent<WeaponSystem> ();
            currentWeaponRange = weaponSystem.GetCurrentWeapon ().GetMaxAttackRange ();

            if (distanceToPlayer > chaseRadius && state != State.patrolling)
            {
                StopAllCoroutines ();
                StartCoroutine (Patrol ());
            }
            if (distanceToPlayer <= chaseRadius && state != State.chasing && isCompanion)
            {
                StopAllCoroutines ();
                StartCoroutine (ChasePlayer ());
            }

            if (distanceToPlayer < currentWeaponRange && state != State.talking)
            {
                StopAllCoroutines ();
                StartCoroutine (TalkToPlayer ());
            }
        }

        IEnumerator ChasePlayer ()
        {
            state = State.chasing;
            agent.speed = originalSpeed;
            while (distanceToPlayer >= currentWeaponRange && distanceToPlayer <= chaseRadius)
            {
                character.SetDestination (player.transform.position);
                yield return new WaitForEndOfFrame ();
            }
        }

        IEnumerator Patrol ()
        {
            state = State.patrolling;
            agent.speed = patrolSpeed;
            while (true)
            {
                if (patrolPath == null) { yield break; }
                Vector3 nextWaypointPos = patrolPath.transform.GetChild (nextWaypointIndex).position;
                character.SetDestination (nextWaypointPos);
                CycleWaypointWhenClose (nextWaypointPos);
                yield return new WaitForSeconds (waypointWaitTime);
            }
        }

        void CycleWaypointWhenClose (Vector3 nextWaypointPos)
        {
            if (Vector3.Distance (transform.position, nextWaypointPos) <= agent.stoppingDistance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
            }
        }

        IEnumerator TalkToPlayer ()
        {
            state = State.talking;
            agent.speed = originalSpeed;
            character.SetDestination (player.transform.position);
            yield break;
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