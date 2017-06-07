﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility
    {
        PowerAttackConfig config;

        public void SetConfig (PowerAttackConfig configToSet)
        {
            this.config = configToSet;
        }

        void Start ()
        {
            print ("Power Attack Behaviour attached to " + gameObject.name);
        }

        void Update ()
        {
            
        }

        public void Use (AbilityUseParams useParams)
        {
            print ("Power Attack used by: " + gameObject.name);
            float damageToDeal = useParams.baseDamage + config.GetExtraDamage ();
            useParams.target.TakeDamage (damageToDeal);
        }
    }
}