using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    [RequireComponent (typeof (Character))]
    [RequireComponent (typeof (WeaponSystem))]
    [RequireComponent (typeof (HealthSystem))]
    public class Enemy_AI : Core_AI
    {
        protected override void Update ()
        {
            base.Update ();

            if (inWeaponRange && state != State.attacking)
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
                while (inChaseRange)
                {
                    character.SetDestination (player.transform.position);
                    yield return new WaitForEndOfFrame ();
                }
            }
        }
    }
}