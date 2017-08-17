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
        WeaponSystem character;

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
            character = other.GetComponent<WeaponSystem> ();
            if (character != null && character.gameObject.tag == "Player" && !weaponCollected)
            // if (character != null)
            {
                weaponCollected = true;
                // weapon is lost when dropped
                character.PutWeaponInHand (weaponConfig);
                audioSource.PlayOneShot (pickupSFX);
                // Disable the pickup point
                StartCoroutine (DisablePickup ());
            }            
        }

        IEnumerator DisablePickup ()
        {
            if (!audioSource.isPlaying)
            {
                gameObject.SetActive (false);
                yield break;
            }
            yield return null;
            StartCoroutine (DisablePickup ());
        }
    }
}