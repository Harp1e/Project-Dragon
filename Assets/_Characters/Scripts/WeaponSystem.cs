using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

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

            float characterHealth = GetComponent<HealthSystem> ().healthAsPercentage;
            bool characterIsDead = (characterHealth <= Mathf.Epsilon);

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
            GameObject dominantHand = RequestDominantHand ();
            Destroy (weaponObject);
            weaponObject = Instantiate (weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
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
                float weaponHitPeriod = currentWeaponConfig.GetMinTimeBetweenHits ();
                float timeToWait = weaponHitPeriod * character.GetAnimSpeedMultiplier ();
                bool isTimeToHitAgain = (Time.time - lastHitTime) > timeToWait;
                if (isTimeToHitAgain)
                {
                    AttackTarget ();
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
            StopAllCoroutines ();
        }

        void SetAttackAnimation ()
        {
            //animator = GetComponent<Animator> ();
            if (!character.GetOverrideController())
            {
                Debug.Break ();
                Debug.LogAssertion ("Please provide " + gameObject + " with an Animator Override Controller");
            }
            else
            {
                var animatorOverrideController = character.GetOverrideController ();
                animator.runtimeAnimatorController = animatorOverrideController;
                animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip ();
            }
        }

        private GameObject RequestDominantHand ()
        {
            var dominantHands = GetComponentsInChildren<DominantHand> ();
            int numberOfDominantHands = dominantHands.Length;
            Assert.IsFalse (numberOfDominantHands <= 0, "No DominantHand found on Player. Please add one.");
            Assert.IsFalse (numberOfDominantHands > 1, "Multiple DominantHand scripts found on Player. Please remove all but one.");
            return dominantHands[0].gameObject;
        }

        void AttackTarget ()
        {
            SetAttackAnimation ();
            transform.LookAt (target.transform);
            animator.SetTrigger (ATTACK_TRIGGER);
            float damageDelay = 1.0f;   // TODO get from the weapon...
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