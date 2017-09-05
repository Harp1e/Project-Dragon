using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    [RequireComponent (typeof (Character))]
    [RequireComponent (typeof (WeaponSystem))]
    public class NPC_AI : Core_AI
    {

        protected override void Start ()
        {
            base.Start ();
        }

        protected override void Update ()
        {
            base.Update ();
            if (distanceToPlayer < character.GetTriggerRadius() && state != State.talking)
            {
                StopAllCoroutines ();
                TalkToPlayer ();
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

        void TalkToPlayer ()
        {
            state = State.talking;
            agent.speed = originalSpeed;
            character.SetDestination (transform.position);
        }

    }
}