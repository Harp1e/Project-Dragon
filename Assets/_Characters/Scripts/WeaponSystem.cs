using System.Collections;
using System.Collections.Generic;
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
            print ("Attacking: " + target);
            // TODO use a repeating attack coroutine
        }

        public WeaponConfig GetCurrentWeapon ()
        {
            return currentWeaponConfig;
        }

        void SetAttackAnimation ()
        {
            animator = GetComponent<Animator> ();
            var animatorOverrideController = character.GetOverrideController ();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip ();    // TODO Remove constant
        }

        private GameObject RequestDominantHand ()
        {
            var dominantHands = GetComponentsInChildren<DominantHand> ();
            int numberOfDominantHands = dominantHands.Length;
            Assert.IsFalse (numberOfDominantHands <= 0, "No DominantHand found on Player. Please add one.");
            Assert.IsFalse (numberOfDominantHands > 1, "Multiple DominantHand scripts found on Player. Please remove all but one.");
            return dominantHands[0].gameObject;
        }

        // TODO use coroutines for move & attack
        void AttackTarget ()
        {
            if (Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits ())
            {
                SetAttackAnimation ();
                animator.SetTrigger (ATTACK_TRIGGER);
                lastHitTime = Time.time;
            }
        }

        float CalculateDamage ()
        {
            return baseDamage + currentWeaponConfig.GetAdditionalDamage ();
        }
    }
}