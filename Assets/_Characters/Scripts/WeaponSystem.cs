﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

// TODO Consider allowing sword & shield operation?
// TODO do we want to trigger a Take Hit animation (based on current weapon)?
// TODO Associate attack sounds to individual weapons - remove from health system?

namespace RPG.Characters
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] WeaponConfig currentWeaponConfig;
        [SerializeField] float baseDamage = 10f;

        Animator animator;        
        Character character;
        GameObject target;
        GameObject weaponObject;

        float lastHitTime = 0f;

        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT_ATTACK";

        void Start ()
        {
            animator = GetComponent<Animator> ();
            character = GetComponent<Character> ();

            WeaponSetup ();  
        }

        void Update ()
        {
            bool targetIsDead;
            bool targetIsOutOfRange;
            if (target == null)
            {
                targetIsDead = false;
                targetIsOutOfRange = false;
            }
            else
            {
                float targetHealth = target.GetComponent<HealthSystem> ().healthAsPercentage;
                targetIsDead =  targetHealth <= Mathf.Epsilon;

                float distanceToTarget = Vector3.Distance (transform.position, target.transform.position);
                targetIsOutOfRange = distanceToTarget > currentWeaponConfig.GetMaxAttackRange();
            }
            bool characterIsDead = false;
            HealthSystem healthSystem = GetComponent<HealthSystem> ();
            if (healthSystem != null)
            {
                float characterHealth = healthSystem.healthAsPercentage;
                characterIsDead = (characterHealth <= Mathf.Epsilon);
            }

            if (characterIsDead || targetIsOutOfRange || targetIsDead)
            {
                StopAllCoroutines ();
            }
        }

        void WeaponSetup ()
        {
            PutWeaponInHand (currentWeaponConfig);
            SetAttackAnimation ();
        }

        public void PutWeaponInHand (WeaponConfig weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab ();
            if (currentWeaponConfig.GetUseDominantHand())
            {
                GameObject dominantHand = RequestDominantHand ();
                Destroy (weaponObject);
                weaponObject = Instantiate (weaponPrefab, dominantHand.transform);
                weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
                weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
            }
            else
            {
                GameObject otherHand = RequestOtherHand ();
                Destroy (weaponObject);
                weaponObject = Instantiate (weaponPrefab, otherHand.transform);
                weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
                weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
            }
           
        }

        public void AttackTarget (GameObject targetToAttack)
        {
            target = targetToAttack;
            StartCoroutine (AttackTargetRepeatedly ());
        }

        IEnumerator AttackTargetRepeatedly ()
        {
            bool attackerStillAlive = GetComponent<HealthSystem> ().healthAsPercentage > 0;               
            bool targetStillAlive = target.GetComponent<HealthSystem> ().healthAsPercentage > 0;
            while (attackerStillAlive && targetStillAlive)
            {
                var animationClip = currentWeaponConfig.GetAttackAnimClip ();
                float animationClipTime = animationClip.length / character.GetAnimSpeedMultiplier ();
                float timeToWait = animationClipTime + currentWeaponConfig.GetTimeBetweenAnimationCycles ();

                bool isTimeToHitAgain = (Time.time - lastHitTime) > timeToWait;
                if (isTimeToHitAgain)
                {
                    AttackTargetOnce ();
                }
                yield return new WaitForSeconds (timeToWait);
            }
        }

        public WeaponConfig GetCurrentWeapon ()
        {
            return currentWeaponConfig;
        }

        public void StopAttacking ()
        {
            animator.StopPlayback ();
            StopAllCoroutines ();
        }

        void SetAttackAnimation ()
        {
            if (!character.GetOverrideController())
            {
                Debug.Break ();
                Debug.LogAssertion ("Please provide " + gameObject.name + " with an Animator Override Controller");
            }
            else
            {
                var animatorOverrideController = character.GetOverrideController ();
                animator.runtimeAnimatorController = animatorOverrideController;
                animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip ();
            }
        }

        GameObject RequestDominantHand ()
        {
            var dominantHands = GetComponentsInChildren<DominantHand> ();
            int numberOfDominantHands = dominantHands.Length;
            Assert.IsFalse (numberOfDominantHands <= 0, "No DominantHand found on " + gameObject.name + ". Please add one.");
            Assert.IsFalse (numberOfDominantHands > 1, "Multiple DominantHand scripts found on " + gameObject.name + ". Please remove all but one.");
            return dominantHands[0].gameObject;
        }

        GameObject RequestOtherHand ()
        {
            var otherHands = GetComponentsInChildren<OtherHand> ();
            int numberOfOtherHands = otherHands.Length;
            Assert.IsFalse (numberOfOtherHands <= 0, "No OtherHand found on " + gameObject.name + ". Please add one.");
            Assert.IsFalse (numberOfOtherHands > 1, "Multiple OtherHand scripts found on " + gameObject.name + ". Please remove all but one.");
            return otherHands[0].gameObject;
        }

        void AttackTargetOnce ()
        {
            SetAttackAnimation ();
            transform.LookAt (target.transform);
            animator.SetTrigger (ATTACK_TRIGGER);
            float damageDelay = currentWeaponConfig.GetDamageDelay ();
            lastHitTime = Time.time;
            StartCoroutine (DamageAfterDelay (damageDelay));
        }

        IEnumerator DamageAfterDelay (float delay)
        {
            yield return new WaitForSecondsRealtime (delay);
            target.GetComponent<HealthSystem> ().TakeDamage (CalculateDamage ());
        }

        float CalculateDamage ()
        {
            return baseDamage + currentWeaponConfig.GetAdditionalDamage ();
        }
    }
}