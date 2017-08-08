using System;
using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI;

namespace RPG.Characters
{
    [RequireComponent (typeof (NavMeshAgent))]
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] float stoppingDistance = 1f;
        [SerializeField] float moveSpeedMultiplier = 0.7f;
        [SerializeField] float animatorSpeedMultiplier = 1.5f;
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField] float stationaryTurnSpeed = 180;
        [SerializeField] float moveThreshold = 1f;

        Rigidbody myRigidbody;
        NavMeshAgent agent;
        Animator animator;

        float turnAmount;
        float forwardAmount;

        void Start ()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster> ();
            myRigidbody = GetComponent<Rigidbody> ();
            myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
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
                Move (agent.desiredVelocity);
            }
            else
            {
                Move (Vector3.zero);
            }
        }

        public void Kill ()
        {
            // To allow death signaling
        }

        void Move (Vector3 movement)
        {
            SetForwardAndTurn (movement);
            ApplyExtraTurnRotation ();
            UpdateAnimator ();
        }

        void SetForwardAndTurn (Vector3 movement)
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired direction.
            if (movement.magnitude > moveThreshold)
            {
                movement.Normalize ();
            }
            var localMove = transform.InverseTransformDirection (movement);
            turnAmount = Mathf.Atan2 (localMove.x, localMove.z);
            forwardAmount = localMove.z;
        }

        void UpdateAnimator ()
        {
            animator.SetFloat ("Forward", forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat ("Turn", turnAmount, 0.1f, Time.deltaTime);
            animator.speed = animatorSpeedMultiplier;
        }

        void ApplyExtraTurnRotation ()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp (stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate (0, turnAmount * turnSpeed * Time.deltaTime, 0);
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
