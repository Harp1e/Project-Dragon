using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : MonoBehaviour, ISpecialAbility
    {
        SelfHealConfig config = null;
        Player player = null;
        AudioSource audioSource = null;

        public void SetConfig (SelfHealConfig configToSet)
        {
            this.config = configToSet;
        }

        public void Use (AbilityUseParams useParams)
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

        private void PlayParticleEffect ()
        {
            var prefab = Instantiate (config.GetParticlePrefab (), transform.position, Quaternion.identity);
            prefab.transform.parent = transform;
            ParticleSystem myParticleSystem = prefab.GetComponent<ParticleSystem> ();
            myParticleSystem.Play ();
            Destroy (prefab, myParticleSystem.main.duration);
        }

        private void ApplyHealth (AbilityUseParams useParams)
        {
            player.Heal (config.GetExtraHealth ());           
        }
    }
}