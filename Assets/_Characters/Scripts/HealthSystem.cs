﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.Characters
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float deathVanishSeconds = 2f;
        [SerializeField] Image healthBar;
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;

        const string DEATH_TRIGGER = "Death";

        float currentHealthPoints = 0f;

        Animator animator;
        AudioSource audioSource;
        CharacterMovement characterMovement;

        public float healthAsPercentage
            { get { return currentHealthPoints / maxHealthPoints; } }

        void Start ()
        {
            animator = GetComponent<Animator> ();
            audioSource = GetComponent<AudioSource> ();
            characterMovement = GetComponent<CharacterMovement> ();
            currentHealthPoints = maxHealthPoints;
        }

        void Update ()
        {
            UpdateHealthBar ();
        }

        void UpdateHealthBar ()
        {
            if (healthBar)  // Enemies may not always have health bars
            {
                healthBar.fillAmount = healthAsPercentage;
            }
        }

        public void TakeDamage (float damage)
        {
            bool characterDies = (currentHealthPoints - damage <= 0);
            currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);
            if (damage > 0)
            {
                var clip = damageSounds[UnityEngine.Random.Range (0, damageSounds.Length)];
                audioSource.PlayOneShot (clip);
            }
            if (characterDies)
            {
                StartCoroutine (KillCharacter ());
            }
        }

        public void Heal (float points)
        {
            currentHealthPoints = Mathf.Clamp (currentHealthPoints + points, 0f, maxHealthPoints);
        }

        IEnumerator KillCharacter ()
        {
            StopAllCoroutines ();
            characterMovement.Kill ();
            animator.SetTrigger (DEATH_TRIGGER);

            var playerComponent = GetComponent<Player> ();
            if (playerComponent && playerComponent.isActiveAndEnabled)
            {
                audioSource.clip = deathSounds[UnityEngine.Random.Range (0, deathSounds.Length)];
                audioSource.Play ();
                yield return new WaitForSecondsRealtime (audioSource.clip.length);
                SceneManager.LoadScene (0);
            }
            else    // assume is enemy for now, reconsider on other NPCs
            {
                DestroyObject (gameObject, deathVanishSeconds);
            }
        }
    }
}