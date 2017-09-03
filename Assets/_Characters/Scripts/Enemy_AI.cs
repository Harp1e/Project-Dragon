using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Characters
{
    [RequireComponent (typeof (HealthSystem))]
    public class Enemy_AI : Core_AI
    {

        protected override void Update ()
        {
            base.Update ();

            if (distanceToPlayer < currentWeaponRange && state != State.attacking)
            {
                StopAllCoroutines ();
                state = State.attacking;
                agent.speed = originalSpeed;

                weaponSystem.AttackTarget (player.gameObject);
            }
        }

        protected override IEnumerator ChasePlayer ()
        {
            {
                state = State.chasing;
                agent.speed = originalSpeed;
                while (distanceToPlayer >= currentWeaponRange && distanceToPlayer <= chaseRadius)
                {
                    character.SetDestination (player.transform.position);
                    yield return new WaitForEndOfFrame ();
                }
            }
        }
    }
}