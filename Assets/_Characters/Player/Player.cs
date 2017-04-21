﻿using System;
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
        [SerializeField] float minTimeBetweenHits = 0.5f;
        [SerializeField] float maxAttackRange = 2f;

        [SerializeField] int enemyLayer = 9;

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
            OverrideAnimatorController ();
        }

        private void SetCurrentMaxHealth ()
        {
            currentHealthPoints = maxHealthPoints;
        }

        private void OverrideAnimatorController ()
        {
            Animator animator = GetComponent<Animator>();
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
        // TODO refactor to simplify and reduce length & complexity
        {
            if (layerHit == enemyLayer)
            {
                GameObject enemy = raycastHit.collider.gameObject;
                if (enemy == null || (enemy.transform.position - transform.position).magnitude > maxAttackRange)
                {
                    return;
                }

                var enemyComponent = enemy.GetComponent<Enemy> ();
                if (Time.time - lastHitTime > minTimeBetweenHits)
                {
                    enemyComponent.TakeDamage (damagePerHit);
                    lastHitTime = Time.time;
                }
            }
        }

        public void TakeDamage (float damage)
        {
            currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);
        }
    }
}