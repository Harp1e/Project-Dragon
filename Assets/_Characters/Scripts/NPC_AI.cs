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
            if (inTalkRange && state != State.talking)
            {
                StopAllCoroutines ();
                StartCoroutine(TalkToPlayer ());
            }
        }

        protected override IEnumerator ChasePlayer ()
        {
            state = State.chasing;
            agent.speed = originalSpeed;
               
            while (inChaseRange)
            {
                character.SetDestination (player.transform.position);
                yield return new WaitForEndOfFrame ();
            }
        }

        IEnumerator TalkToPlayer ()
        {
            state = State.talking;
            agent.speed = originalSpeed;
            character.SetDestination (transform.position);
            yield return new WaitForEndOfFrame ();
        }

    }
}