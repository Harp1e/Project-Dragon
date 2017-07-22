using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility
    {
        PowerAttackConfig config;
        AudioSource audioSource = null;

        public void SetConfig (PowerAttackConfig configToSet)
        {
            this.config = configToSet;
        }

        void Start ()
        {
            audioSource = GetComponent<AudioSource> ();
        }

        public void Use (AbilityUseParams useParams)
        {
            DealDamage (useParams);
            PlayParticleEffect ();
            PlayAudio ();
        }

        private void PlayAudio ()
        {
            audioSource.clip = config.GetAudioClip ();
            if (audioSource.clip != null)
                audioSource.Play ();
        }

        private void PlayParticleEffect ()
        {
            var particlePrefab = config.GetParticlePrefab ();
            var prefab = Instantiate (particlePrefab, transform.position, particlePrefab.transform.rotation);
            prefab.transform.parent = transform;
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