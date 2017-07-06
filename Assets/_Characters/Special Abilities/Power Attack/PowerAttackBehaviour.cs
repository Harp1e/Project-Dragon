using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility
    {
        PowerAttackConfig config;

        public void SetConfig (PowerAttackConfig configToSet)
        {
            this.config = configToSet;
        }

        void Start ()
        {

        }

        public void Use (AbilityUseParams useParams)
        {
            DealDamage (useParams);
            PlayParticleEffect ();
        }

        private void PlayParticleEffect ()
        {
            var prefab = Instantiate (config.GetParticlePrefab (), transform.position, Quaternion.identity);
            // TODO Attach to player?
            ParticleSystem myParticleSystem = prefab.GetComponent<ParticleSystem> ();
            myParticleSystem.Play ();
            Destroy (prefab, myParticleSystem.main.duration);
        }

        private void DealDamage (AbilityUseParams useParams)
        {
            float damageToDeal = useParams.baseDamage + config.GetExtraDamage ();
            useParams.target.TakeDamage (damageToDeal);
        }
    }
}