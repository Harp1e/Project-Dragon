﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
    public class AreaEffectBehaviour : AbilityBehaviour
    {
        AreaEffectConfig config;
        AudioSource audioSource = null;

        public void SetConfig (AreaEffectConfig configToSet)
        {
            this.config = configToSet;
        }

        private void Start ()
        {
            audioSource = GetComponent<AudioSource> ();
        }

        public override void Use (AbilityUseParams useParams)
        {
            DealRadialDamage (useParams);
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
                bool hitPlayer = hit.collider.gameObject.GetComponent<Player> ();
                if (damageable != null && !hitPlayer)
                {
                    damageable.TakeDamage (damageToDeal);
                }
            }
        }
    }
}