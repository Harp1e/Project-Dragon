using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class AbilityBehaviour : MonoBehaviour 
    {
        protected AbilityConfig config;

        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT_ATTACK";

        public abstract void Use (GameObject target = null);

        public void SetConfig (AbilityConfig configToSet)
        {
            config = configToSet;
        }

        protected void PlayParticleEffect ()
        {
            var particlePrefab = config.GetParticlePrefab ();
            var particleObject = Instantiate (
                particlePrefab, 
                transform.position, 
                particlePrefab.transform.rotation
                ).GetComponent<ParticleSystem>();
            particleObject.transform.parent = transform;
            particleObject.Play();
            StartCoroutine (DestroyParticleWhenFinished (particleObject));
        }

        IEnumerator DestroyParticleWhenFinished (ParticleSystem particleObject)
        {
            yield return new WaitWhile (() => particleObject.IsAlive ());
            Destroy (particleObject.gameObject);
        }

        protected void PlayAbilityAnimation ()
        {
            Animator animator = GetComponent<Animator> ();
            AnimatorOverrideController animatorOverrideController = GetComponent<Character> ().GetOverrideController ();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = config.GetAbilityAnimation ();
            animator.SetTrigger (ATTACK_TRIGGER);
        }

        protected void PlayAbilitySound ()
        {
            var abilitySound = config.GetRandomAbilitySound ();
            var audioSource = GetComponent<AudioSource> ();
            audioSource.PlayOneShot (abilitySound);
        }
    }
}