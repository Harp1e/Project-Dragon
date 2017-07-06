﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : MonoBehaviour, ISpecialAbility
    {
        SelfHealConfig config;
        Player player;

        public void SetConfig (SelfHealConfig configToSet)
        {
            this.config = configToSet;
        }

        public void Use (AbilityUseParams useParams)
        {
            ApplyHealth (useParams);
            PlayParticleEffect ();
        }

        private void Start ()
        {
            player = GetComponent<Player> ();
        }

        private void PlayParticleEffect ()
        {
            var prefab = Instantiate (config.GetParticlePrefab (), transform.position, Quaternion.identity);
            // TODO Attach to player?
            ParticleSystem myParticleSystem = prefab.GetComponent<ParticleSystem> ();
            myParticleSystem.Play ();
            Destroy (prefab, myParticleSystem.main.duration);
        }

        private void ApplyHealth (AbilityUseParams useParams)
        {
            player.AdjustHealth (-config.GetExtraHealth ());
        }
    }
}