using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    // [ExecuteInEditMode]
    public class WeaponPickupPoint : MonoBehaviour
    {
        [SerializeField] WeaponConfig weaponConfig;
        [SerializeField] AudioClip pickupSFX;

        AudioSource audioSource;
        WeaponSystem weaponSystem;

        bool weaponCollected = false;

        void Start ()
        {
            audioSource = GetComponent<AudioSource> ();
        }

        //void Update ()
        public void RefreshPrefab ()
        {
            if (!Application.isPlaying)
            {
                DestroyChildren ();
                InstantiateWeapon ();
            }   
        }

        void DestroyChildren ()
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate (child.gameObject);
            }
        }

        void InstantiateWeapon ()
        {
            var weapon = weaponConfig.GetWeaponPrefab ();
            weapon.transform.position = Vector3.zero;
            Instantiate (weapon, gameObject.transform);
        }

        void OnTriggerEnter (Collider other)
        {
            weaponSystem = other.GetComponent<WeaponSystem> ();
            if (weaponSystem != null && weaponSystem.gameObject.tag == "Player" && !weaponCollected)
            // if (character != null)
            {
                weaponCollected = true;
                audioSource.PlayOneShot (pickupSFX);
                StartCoroutine (SwapWeapon ());
            }            
        }

        IEnumerator SwapWeapon ()
        {
            if (!audioSource.isPlaying)
            {
                // old weapon is lost when dropped
                weaponSystem.PutWeaponInHand (weaponConfig);
                // Disable the pickup point
                gameObject.SetActive (false);
                yield break;
            }
            yield return null;
            StartCoroutine (SwapWeapon ());
        }
    }
}