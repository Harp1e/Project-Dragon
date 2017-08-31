using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        PlayerControl player;

        public override void Use (GameObject target)
        {
            ApplyHealth ();
            PlayParticleEffect ();
            PlayAbilitySound ();
            PlayAbilityAnimation ();
        }

        private void Start ()
        {
            player = GetComponent<PlayerControl> ();
        }

        private void ApplyHealth ()
        {
            var playerHealth = player.GetComponent<HealthSystem> ();
            playerHealth.Heal ((config as SelfHealConfig).GetExtraHealth ());           
        }
    }
}