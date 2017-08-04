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
        [SerializeField] float moveSpeedMultiplier = 0.7f;
        // TODO Consider animation speed multiplier

        ThirdPersonCharacter character;
        Rigidbody myRigidbody;
        NavMeshAgent agent;
        Animator animator;

        void Start ()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster> ();
            character = GetComponent<ThirdPersonCharacter> ();
            myRigidbody = GetComponent<Rigidbody> ();
            animator = GetComponent<Animator> ();

            agent = GetComponent<NavMeshAgent> ();
            agent.updatePosition = true;
            agent.updateRotation = false;
            agent.stoppingDistance = stoppingDistance;

            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        void Update ()
        {
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                character.Move (agent.desiredVelocity);
            }
            else
            {
                character.Move (Vector3.zero);
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

        // We implement this function to override the default root motion
        // This allows us to modify the positional speed before it's applied
        void OnAnimatorMove ()
        {
            if (Time.deltaTime > 0)
            {
                Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

                // preserve y component of current velocity
                velocity.y = myRigidbody.velocity.y;
                myRigidbody.velocity = velocity;
            }
        }

    }
}
