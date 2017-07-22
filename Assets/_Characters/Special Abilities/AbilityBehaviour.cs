﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class AbilityBehaviour : MonoBehaviour 
    {
        protected AbilityConfig config;

        public abstract void Use (AbilityUseParams useParams);

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
                );
            particleObject.transform.parent = transform;
            particleObject.GetComponent<ParticleSystem> ().Play();
            StartCoroutine (DestroyParticleWhenFinished (particleObject));
        }

        IEnumerator DestroyParticleWhenFinished (GameObject particlePrefab)
        {
            while (particlePrefab.GetComponent<ParticleSystem>().isPlaying)
            {
                yield return new WaitForSeconds (10f);
            }
            Destroy (particlePrefab);
            yield return new WaitForEndOfFrame();
        }
    }
}