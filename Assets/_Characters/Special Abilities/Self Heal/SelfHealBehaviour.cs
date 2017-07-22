using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        Player player = null;

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
            player.Heal ((config as SelfHealConfig).GetExtraHealth ());           
        }
    }
}