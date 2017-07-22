using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        Player player = null;
        AudioSource audioSource = null;

        public override void Use (AbilityUseParams useParams)
        {
            ApplyHealth (useParams);
            PlayParticleEffect ();
            PlayAudio ();
        }

        private void PlayAudio ()
        {
            // TODO extract to higher level procedure (Special Ability)
            audioSource.clip = config.GetAudioClip ();
            if (audioSource.clip != null)
                audioSource.Play ();
        }

        private void Start ()
        {
            player = GetComponent<Player> ();
            audioSource = GetComponent<AudioSource> ();
        }

        private void ApplyHealth (AbilityUseParams useParams)
        {
            player.Heal ((config as SelfHealConfig).GetExtraHealth ());           
        }
    }
}