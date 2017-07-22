using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {
        AudioSource audioSource = null;

        void Start ()
        {
            audioSource = GetComponent<AudioSource> ();
        }

        public override void Use (AbilityUseParams useParams)
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

        private void DealDamage (AbilityUseParams useParams)
        {
            float damageToDeal = useParams.baseDamage + (config as PowerAttackConfig).GetExtraDamage ();
            useParams.target.TakeDamage (damageToDeal);
        }
    }
}