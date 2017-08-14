using System;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Characters
{
    [SelectionBase]
    public class Character : MonoBehaviour
    {
        [Header ("Animator")]
        [SerializeField] RuntimeAnimatorController animatorController;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] Avatar characterAvatar;

        [Header ("AudioSource")]
        [Range (0, 1)][SerializeField] float audioVolume = 1f;
        [Range(0,1)][SerializeField] float audioSpatialBlend = 0.5f;

        [Header ("Capsule Collider")]
        [SerializeField] Vector3 colliderCenter = new Vector3 (0f, 1.03f, 0f);
        [SerializeField] float colliderRadius = 0.2f;
        [SerializeField] float colliderHeight = 2.03f;

        [Header ("Movement")]
        [SerializeField] float moveSpeedMultiplier = 0.7f;
        [SerializeField] float animatorSpeedMultiplier = 1.5f;
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField] float stationaryTurnSpeed = 180;
        [SerializeField] float moveThreshold = 1f;

        [Header ("NavMesh Agent")]
        [SerializeField] float navMeshSteeringSpeed = 1f;
        [SerializeField] float navMeshStoppingDistance = 1f;

        Rigidbody myRigidbody;
        NavMeshAgent navMeshAgent;
        Animator animator;

        bool isAlive = true;
        float turnAmount;
        float forwardAmount;

        public AnimatorOverrideController GetAnimatorOverrideController ()
        {
            return animatorOverrideController;
        }

        void Awake ()
        {
            AddRequiredComponents ();
        }

        private void AddRequiredComponents ()
        {
            animator = gameObject.AddComponent<Animator> ();
            animator.runtimeAnimatorController = animatorController;
            animator.avatar = characterAvatar;

            var audioSource = gameObject.AddComponent<AudioSource> ();
            audioSource.volume = audioVolume;
            audioSource.spatialBlend = audioSpatialBlend;
            audioSource.playOnAwake = false;

            var capsuleCollider = gameObject.AddComponent<CapsuleCollider> ();
            capsuleCollider.center = colliderCenter;
            capsuleCollider.radius = colliderRadius;
            capsuleCollider.height = colliderHeight;

            navMeshAgent = gameObject.AddComponent<NavMeshAgent> ();
            navMeshAgent.speed = navMeshSteeringSpeed;
            navMeshAgent.stoppingDistance = navMeshStoppingDistance;
            navMeshAgent.autoBraking = false;
            navMeshAgent.updatePosition = true;
            navMeshAgent.updateRotation = false;

            myRigidbody = gameObject.AddComponent<Rigidbody> ();
            myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        }

        void Update ()
        {
            if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance && isAlive)
            {
                Move (navMeshAgent.desiredVelocity);
            }
            else
            {
                Move (Vector3.zero);
            }
        }

        public void Kill ()
        {
            isAlive = false;
        }

        public void SetDestination (Vector3 worldPos)
        {
            navMeshAgent.destination = worldPos;
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
