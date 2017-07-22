using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
    public class AreaEffectBehaviour : AbilityBehaviour
    {
        AudioSource audioSource = null;


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

        private void DealRadialDamage (AbilityUseParams useParams)
        {
            RaycastHit[] hits = Physics.SphereCastAll (
                transform.position,
                (config as AreaEffectConfig).GetRadius (),
                Vector3.up,
                (config as AreaEffectConfig).GetRadius ()
                );

            float damageToDeal = useParams.baseDamage + (config as AreaEffectConfig).GetDamageToEachTarget ();

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