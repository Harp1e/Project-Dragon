using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

// TODO Consider re-wiring
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamageable
    {

        [SerializeField] Weapon weaponInUse;
        [SerializeField] AnimatorOverrideController animatorOverrideController;

        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;

        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float baseDamage = 10f;

        // Temporarily serialize for debugging
        [SerializeField] SpecialAbility[] abilities;

        const string ATTACK_TRIGGER = "Attack";
        const string DEATH_TRIGGER = "Death";

        Animator animator;
        AudioSource audioSource;
        CameraRaycaster cameraRaycaster;

        float currentHealthPoints;
        float lastHitTime = 0f;

        public float healthAsPercentage
        { get { return currentHealthPoints / maxHealthPoints; } }

        void Start ()
        {
            RegisterForMouseClick ();
            SetCurrentMaxHealth ();
            PutWeaponInHand ();
            SetupRuntimeAnimator ();
            abilities[0].AttachComponentTo (gameObject);
            audioSource = GetComponent<AudioSource> ();
        }

        public void TakeDamage (float damage)
        {
            ReduceHealth (damage);
            bool playerDies = (currentHealthPoints <= 0f);
            if (playerDies)
            {
                StartCoroutine (KillPlayer ());
            }
        }

        IEnumerator KillPlayer ()
        {
            animator.SetTrigger (DEATH_TRIGGER);

            audioSource.clip = deathSounds[UnityEngine.Random.Range (0, deathSounds.Length)];
            audioSource.Play ();
            yield return new WaitForSecondsRealtime (audioSource.clip.length);

            SceneManager.LoadScene (0);
        }

        void ReduceHealth(float damage)
        {
            currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);
            if (damage > 0)
            {
                audioSource.clip = damageSounds[UnityEngine.Random.Range (0, damageSounds.Length)];
                audioSource.Play ();
            } 
        }

        private void SetCurrentMaxHealth ()
        {
            currentHealthPoints = maxHealthPoints;
        }

        private void SetupRuntimeAnimator ()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController["DEFAULT_ATTACK"] = weaponInUse.GetAttackAnimClip ();    // TODO Remove constant
        }

        private void PutWeaponInHand ()
        {
            var weaponPrefab = weaponInUse.GetWeaponPrefab ();
            GameObject dominantHand = RequestDominantHand ();
            var weapon = Instantiate (weaponPrefab, dominantHand.transform);
            weapon.transform.localPosition = weaponInUse.gripTransform.localPosition;
            weapon.transform.localRotation = weaponInUse.gripTransform.localRotation;
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

        void OnMouseOverEnemy (Enemy enemy)
        {
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                AttackTarget (enemy);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                AttemptSpecialAbility (0, enemy);
            }
        }

        void AttemptSpecialAbility (int abilityIndex, Enemy enemy)
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

        private void AttackTarget (Enemy enemy)
        {
            if (Time.time - lastHitTime > weaponInUse.GetMinTimeBetweenHits())
            {
                animator.SetTrigger (ATTACK_TRIGGER);
                enemy.TakeDamage (baseDamage);
                lastHitTime = Time.time;
            }
        }

        private bool IsTargetInRange (GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            {
                return distanceToTarget <= weaponInUse.GetMaxAttackRange();
            }
        }

    }
}