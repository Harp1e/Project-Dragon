using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        Player player;

        public override void Use (AbilityUseParams useParams)
        {
            ApplyHealth (useParams);
            PlayParticleEffect ();
            PlayAbilitySound ();
        }

        private void Start ()
        {
            player = GetComponent<Player> ();
        }

        private void ApplyHealth (AbilityUseParams useParams)
        {
            var playerHealth = player.GetComponent<HealthSystem> ();
            playerHealth.Heal ((config as SelfHealConfig).GetExtraHealth ());           
        }
    }
}