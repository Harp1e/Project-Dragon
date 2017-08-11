using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
    public class AreaEffectBehaviour : AbilityBehaviour
    {
        public override void Use (GameObject target)
        {
            DealRadialDamage ();
            PlayParticleEffect ();
            PlayAbilitySound ();
        }

        private void DealRadialDamage ()
        {
            RaycastHit[] hits = Physics.SphereCastAll (
                transform.position,
                (config as AreaEffectConfig).GetRadius (),
                Vector3.up,
                (config as AreaEffectConfig).GetRadius ()
                );

            float damageToDeal = (config as AreaEffectConfig).GetDamageToEachTarget ();

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