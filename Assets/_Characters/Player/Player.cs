using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// TODO Consider re-wiring
using RPG.CameraUI;
using RPG.Core;

namespace RPG.Characters
{
    public class Player : MonoBehaviour
    {

        [SerializeField] Weapon currentWeaponConfig;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] ParticleSystem criticalHitParticle = null;

        [SerializeField] float baseDamage = 10f;
        [Range (0.1f, 1.0f)] [SerializeField] float criticalHealthChance = 0.1f;
        [SerializeField] float criticalHitMultiplier = 1.25f;

        // Temporarily serialize for debugging
        [SerializeField] AbilityConfig[] abilities;

        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT_ATTACK";

        Animator animator;
        CameraRaycaster cameraRaycaster;
        Enemy enemy;
        GameObject weaponObject;

        float lastHitTime = 0f;

        void Start ()
        {
            RegisterForMouseClick ();
            PutWeaponInHand (currentWeaponConfig);
            SetAttackAnimation ();
            AttachInitialAbilities ();
        }

        public void PutWeaponInHand (Weapon weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab ();
            GameObject dominantHand = RequestDominantHand ();
            Destroy (weaponObject);
            weaponObject = Instantiate (weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
        }

        private void AttachInitialAbilities ()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachAbilityTo (gameObject);
            }
        }

        private void Update ()
        {
            var healthPercentage = GetComponent<HealthSystem> ().healthAsPercentage;
            if (healthPercentage > Mathf.Epsilon)
            {
                ScanForAbilityKeyDown ();
            }
        }

        private void ScanForAbilityKeyDown ()
        {
            for (int keyIndex = 1; keyIndex < abilities.Length; keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                {
                    AttemptSpecialAbility (keyIndex);
                }
            }
        }


        private void SetAttackAnimation ()
        {
            animator = GetComponent<Animator>();
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

        void RegisterForMouseClick ()
        {
            cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        void OnMouseOverEnemy (Enemy enemyToSet)
        {
            this.enemy = enemyToSet;
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                AttackTarget ();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                AttemptSpecialAbility (0);
            }
        }

        void AttemptSpecialAbility (int abilityIndex)
        {
            var energyComponent = GetComponent<Energy> ();
            var energyCost = abilities[abilityIndex].GetEnergyCost ();
            if (energyComponent.IsEnergyAvailable(energyCost))
            {
                energyComponent.ConsumeEnergy (energyCost);    
                var abilityParams = new AbilityUseParams (enemy, baseDamage);
                abilities[abilityIndex].Use (abilityParams);
            }
        }

        private void AttackTarget ()
        {
            if (Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits())
            {
                SetAttackAnimation ();
                animator.SetTrigger (ATTACK_TRIGGER);
                enemy.TakeDamage (CalculateDamage ());
                lastHitTime = Time.time;
            }
        }

        private float CalculateDamage ()
        {
            bool isCriticalHit = UnityEngine.Random.Range (0f, 1f) <= criticalHealthChance;
            float damageBeforeCritical = baseDamage + currentWeaponConfig.GetAdditionalDamage ();
            if (isCriticalHit)
            {
                criticalHitParticle.Play ();
                return damageBeforeCritical * criticalHitMultiplier;
            }
            else
            {
                return damageBeforeCritical;
            }
        }

        private bool IsTargetInRange (GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            {
                return distanceToTarget <= currentWeaponConfig.GetMaxAttackRange();
            }
        }

    }
}