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
        protected float waypointDwellTime;
        protected float patrolSpeed;
        protected bool randomPatrol;
        protected float maxPatrolRadius;
        protected bool isCompanion;

        protected Character character;
        protected NavMeshAgent agent;
        protected PlayerControl player;
        protected WeaponSystem weaponSystem;

        protected enum State { idle, patrolling, attacking, chasing, talking }
        protected State state = State.idle;

        protected float currentWeaponRange;
        protected float distanceToPlayer;
        protected float originalSpeed;

        int nextWaypointIndex = 0;
        Vector3 startPosition;
        Vector3 nextRandomWaypoint;
        Vector3 previousPosition = Vector3.zero;

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
            randomPatrol = character.GetRandomPatrol ();
            maxPatrolRadius = character.GetMaxPatrolRadius ();
            isCompanion = character.GetIsCompanion ();
            startPosition = transform.position;
            nextRandomWaypoint = startPosition;
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
            state = State.patrolling;
            agent.speed = patrolSpeed;
            while (patrolPath != null || randomPatrol)
            {
                if (randomPatrol)
                {
                    character.SetDestination (nextRandomWaypoint);
                    GetNextRandomWaypoint ();
                    yield return new WaitForSeconds (waypointDwellTime);
                    character.SetDestination (nextRandomWaypoint);
                }
                else
                {
                    Vector3 nextWaypointPos = patrolPath.transform.GetChild (nextWaypointIndex).position;
                    character.SetDestination (nextWaypointPos);
                    CycleWaypointWhenClose (nextWaypointPos);
                    yield return new WaitForSeconds (waypointDwellTime);
                }
            }
        }

        void GetNextRandomWaypoint ()
        {
            if (Vector3.Distance (transform.position, nextRandomWaypoint) <= agent.stoppingDistance || 
                agent.pathStatus == NavMeshPathStatus.PathInvalid || 
                //agent.pathStatus == NavMeshPathStatus.PathPartial ||
                transform.position == previousPosition)
            {
                nextRandomWaypoint.x = Random.Range (startPosition.x - maxPatrolRadius, startPosition.x + maxPatrolRadius);
                nextRandomWaypoint.z = Random.Range (startPosition.z - maxPatrolRadius, startPosition.z + maxPatrolRadius);
                nextRandomWaypoint.y = startPosition.y;
            }
            previousPosition = transform.position;
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