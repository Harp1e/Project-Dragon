using System.Collections;
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
            print ("Power Attack Behaviour 1 attached to " + gameObject.name);
        }

        void Update ()
        {
            
        }

        public void Use ()
        {

        }
    }
}