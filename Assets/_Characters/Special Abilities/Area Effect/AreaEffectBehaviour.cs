using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
    public class AreaEffectBehaviour : MonoBehaviour, ISpecialAbility
    {
        AreaEffectConfig config;

        public void SetConfig (AreaEffectConfig configToSet)
        {
            this.config = configToSet;
        }

        public void Use (AbilityUseParams useParams)
        {
            DealRadialDamage (useParams);
            PlayParticleEffect ();
        }

        private void PlayParticleEffect ()
        {
            var prefab = Instantiate (config.GetParticlePrefab(), transform.position, Quaternion.identity);
            // TODO Attach to player?
            ParticleSystem myParticleSystem = prefab.GetComponent<ParticleSystem> ();
            myParticleSystem.Play ();
            Destroy (prefab, myParticleSystem.main.duration);
        }

        private void DealRadialDamage (AbilityUseParams useParams)
        {
            RaycastHit[] hits = Physics.SphereCastAll (
                transform.position,
                config.GetRadius (),
                Vector3.up,
                config.GetRadius ()
                );

            float damageToDeal = useParams.baseDamage + config.GetDamageToEachTarget ();

            foreach (RaycastHit hit in hits)
            {
                var damageable = hit.collider.gameObject.GetComponent<IDamageable> ();
                if (damageable != null)
                {
                    damageable.AdjustHealth (damageToDeal);
                }
            }
        }
    }
}