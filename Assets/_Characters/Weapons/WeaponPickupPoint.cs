using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [ExecuteInEditMode]
    public class WeaponPickupPoint : MonoBehaviour
    {
        [SerializeField] Weapon weaponConfig;
        [SerializeField] AudioClip pickupSFX;

        AudioSource audioSource;

        void Start ()
        {
            audioSource = GetComponent<AudioSource> ();
        }

        void Update ()
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
            FindObjectOfType<PlayerMovement> ().PutWeaponInHand(weaponConfig);
            audioSource.PlayOneShot (pickupSFX);
        }
    }
}