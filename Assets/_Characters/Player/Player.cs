using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
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

        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float damagePerHit = 10f;

        [SerializeField] int enemyLayer = 9;

        Animator animator;
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
        }

        public void TakeDamage (float damage)
        {
            currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);
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
            cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
        }


        void OnMouseClick (RaycastHit raycastHit, int layerHit)
        {
            if (layerHit == enemyLayer)
            {
                GameObject enemy = raycastHit.collider.gameObject;
                if (IsTargetInRange(enemy))
                {
                    AttackTarget (enemy);
                }                
            }
        }

        private void AttackTarget (GameObject target)
        {
            var enemyComponent = target.GetComponent<Enemy> ();
            if (Time.time - lastHitTime > weaponInUse.GetMinTimeBetweenHits())
            {
                animator.SetTrigger ("Attack"); // TODO make constant
                enemyComponent.TakeDamage (damagePerHit);
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