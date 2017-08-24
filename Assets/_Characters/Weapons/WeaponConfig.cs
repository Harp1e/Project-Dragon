﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu (menuName = ("RPG/Weapon"))]
    public class WeaponConfig : ScriptableObject
    {
        public Transform gripTransform;

        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimation;
        [SerializeField] float minTimeBetweenHits = 0.5f;
        [SerializeField] float maxAttackRange = 2f;
        [SerializeField] float additionalDamage = 10f;

        public GameObject GetWeaponPrefab ()
        {
            return weaponPrefab;
        }

        public AnimationClip GetAttackAnimClip ()
        {
            RemoveAnimationEvents ();
            return attackAnimation;
        }

        public float GetMinTimeBetweenHits ()
        {
            float attackDuration = attackAnimation.length + minTimeBetweenHits;
            return attackDuration;
        }

        public float GetMaxAttackRange ()
        {
            return maxAttackRange;
        }

        public float GetAdditionalDamage ()
        {
            return additionalDamage;
        }

        private void RemoveAnimationEvents ()
            // So asset packs cannot cause crashes
        {
            attackAnimation.events = new AnimationEvent[0];
        }

    }
}