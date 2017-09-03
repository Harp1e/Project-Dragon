using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Characters
{
    [RequireComponent (typeof (Character))]
    [RequireComponent (typeof (WeaponSystem))]
    public abstract class Core_AI : MonoBehaviour
    {
        protected float chaseRadius;
        protected WaypointContainer patrolPath;
        protected float waypointDwellTime = 0.5f;
        protected float patrolSpeed = 0.5f;
        protected bool isCompanion = false;

        protected Character character;
        protected NavMeshAgent agent;
        protected PlayerControl player;
        protected WeaponSystem weaponSystem;

        protected enum State { idle, patrolling, attacking, chasing, talking }
        protected State state = State.idle;

        protected float currentWeaponRange;
        protected float distanceToPlayer;
        protected float originalSpeed;
        protected int nextWaypointIndex;

        protected virtual void Start ()
        {
            character = GetComponent<Character> ();
            agent = character.GetComponent<NavMeshAgent> ();
            player = FindObjectOfType<PlayerControl> ();
            originalSpeed = agent.speed;

            chaseRadius = character.GetChaseRadius ();
            patrolPath = character.GetPatrolPath ();
            waypointDwellTime = character.GetWaypointDwellTime ();
            patrolSpeed = character.GetPatrolSpeed ();
            isCompanion = character.GetIsCompanion ();
        }

        protected virtual void Update ()
        {
            distanceToPlayer = Vector3.Distance (player.transform.position, transform.position);

            weaponSystem = GetComponent<WeaponSystem> ();
            currentWeaponRange = weaponSystem.GetCurrentWeapon ().GetMaxAttackRange ();
            
            if (distanceToPlayer > chaseRadius && state != State.patrolling)
            {
                StopAllCoroutines ();
                //weaponSystem.StopAttacking ();
                StartCoroutine (Patrol ());
            }

            if (distanceToPlayer <= chaseRadius && state != State.chasing)
            {
                StopAllCoroutines ();
                StartCoroutine (ChasePlayer ());
            }
        }

        protected virtual IEnumerator ChasePlayer ()
        {
            yield return null;
        }

        IEnumerator Patrol ()
        {

            // TODO Add random patrol option - if single patrol point then select random direction to patrol

            state = State.patrolling;
            agent.speed = patrolSpeed;
            while (patrolPath != null)
            {
                Vector3 nextWaypointPos = patrolPath.transform.GetChild (nextWaypointIndex).position;
                character.SetDestination (nextWaypointPos);
                CycleWaypointWhenClose (nextWaypointPos);
                yield return new WaitForSeconds (waypointDwellTime);
            }
        }


        void CycleWaypointWhenClose (Vector3 nextWaypointPos)
        {
            if (Vector3.Distance (transform.position, nextWaypointPos) <= agent.stoppingDistance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
            }
        }

        void OnDrawGizmos ()
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