using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// TODO Refactor to provide a core AI functionality shared by enemy & NPC

namespace RPG.Characters
{
    [RequireComponent (typeof (Character))]
    [RequireComponent (typeof (WeaponSystem))]
    [RequireComponent (typeof (HealthSystem))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 4f;
        [SerializeField] WaypointContainer patrolPath;
        [SerializeField] float waypointDwellTime = 0.5f;
        [SerializeField] float patrolSpeed = 0.5f;

        Character character;
        NavMeshAgent agent;
        PlayerControl player;

        enum State { idle, patrolling, attacking, chasing }
        State state = State.idle;

        float currentWeaponRange;
        float distanceToPlayer;
        float originalSpeed;
        int nextWaypointIndex;


        private void Start ()
        {
            character = GetComponent<Character> ();
            agent = character.GetComponent<NavMeshAgent> ();
            player = FindObjectOfType<PlayerControl> ();
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
                weaponSystem.StopAttacking ();
                StartCoroutine (Patrol ());
            }
            if (distanceToPlayer <= chaseRadius && state != State.chasing)
            {
                StopAllCoroutines ();
                weaponSystem.StopAttacking ();
                StartCoroutine (ChasePlayer());
            }
            if (distanceToPlayer < currentWeaponRange && state != State.attacking)
            {
                StopAllCoroutines ();
                state = State.attacking;
                agent.speed = originalSpeed;

                weaponSystem.AttackTarget (player.gameObject);
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
            while (patrolPath != null)
            {
                Vector3 nextWaypointPos = patrolPath.transform.GetChild (nextWaypointIndex).position;
                character.SetDestination (nextWaypointPos);
                CycleWaypointWhenClose (nextWaypointPos);
                yield return new WaitForSeconds(waypointDwellTime);
            }
        }

        void CycleWaypointWhenClose (Vector3 nextWaypointPos)
        {
            if (Vector3.Distance (transform.position, nextWaypointPos) <= agent.stoppingDistance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
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