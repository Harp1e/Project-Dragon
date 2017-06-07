using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public class AreaEffectBehaviour : MonoBehaviour, ISpecialAbility
    {
        AreaEffectConfig config;

        public void SetConfig (AreaEffectConfig configToSet)
        {
            this.config = configToSet;
        }

        void Start ()
        {
            print ("Area Effect Behaviour attached to " + gameObject.name);
        }

        public void Use (AbilityUseParams useParams)
        {
            print ("Area Effect used by: " + gameObject.name);
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
                if (damageable != null)
                {
                    damageable.TakeDamage (damageToDeal);
                }
            }
        }
    }
}