using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI;

namespace RPG.Characters
{
    [RequireComponent (typeof (ThirdPersonCharacter))]
    [RequireComponent (typeof (NavMeshAgent))]
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] float stoppingDistance = 1f;

        ThirdPersonCharacter character;
        NavMeshAgent agent;
//        GameObject walkTarget;

        void Start ()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster> ();
            character = GetComponent<ThirdPersonCharacter> ();
//            walkTarget = new GameObject ("walkTarget");

            agent = GetComponent<NavMeshAgent> ();
            agent.updatePosition = false;
            agent.updateRotation = true;
            agent.stoppingDistance = stoppingDistance;

            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        void Update ()
        {
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                character.Move (agent.desiredVelocity, false, false);
            }
            else
            {
                character.Move (Vector3.zero, false, false);

            }
        }

        void OnMouseOverPotentiallyWalkable (Vector3 destination)
        {
            if (Input.GetMouseButton (0))
            {
                agent.SetDestination (destination);
            }
        }

        void OnMouseOverEnemy (Enemy enemy)
        {
            if (Input.GetMouseButton (0) || Input.GetMouseButtonDown (1))
            {
                agent.SetDestination (enemy.transform.position);
            }
        }
    }
}
