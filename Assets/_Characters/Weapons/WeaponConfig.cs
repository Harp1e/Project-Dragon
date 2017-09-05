using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu (menuName = ("RPG/Weapon"))]
    public class WeaponConfig : ScriptableObject
    {
        public Transform gripTransform;

        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip[] attackAnimations;
        [SerializeField] float reloadTime = 0f;
        [SerializeField] float maxAttackRange = 2f;
        [SerializeField] float additionalDamage = 10f;
        [SerializeField] float damageDelay = 0.5f;
        [SerializeField] bool useDominantHand = true;

        AnimationClip attackAnimation;

        public GameObject GetWeaponPrefab ()
        {
            return weaponPrefab;
        }

        public AnimationClip GetAttackAnimClip ()
        {
            attackAnimation = attackAnimations[Random.Range (0, attackAnimations.Length)];
            RemoveAnimationEvents ();
            return attackAnimation;
        }

        public float GetMinTimeBetweenHits ()
        {
            return attackAnimation.length + reloadTime;
        }

        public float GetMaxAttackRange ()
        {
            return maxAttackRange;
        }

        public float GetAdditionalDamage ()
        {
            return additionalDamage;
        }

        public float GetDamageDelay ()
        {
            return damageDelay;
        }

        public bool GetUseDominantHand ()
        {
            return useDominantHand;
        }

        private void RemoveAnimationEvents ()
            // So asset packs cannot cause crashes
        {
            attackAnimation.events = new AnimationEvent[0];
        }

    }
}