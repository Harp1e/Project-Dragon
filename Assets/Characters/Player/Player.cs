using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{

    [SerializeField] Weapon weaponInUse;

    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float damagePerHit = 10f;
    [SerializeField] float minTimeBetweenHits = 0.5f;
    [SerializeField] float maxAttackRange = 2f;

    [SerializeField] int enemyLayer = 9;

    CameraRaycaster cameraRaycaster;
    GameObject currentTarget;

    float currentHealthPoints;
    float lastHitTime = 0f;

    public float healthAsPercentage
    { get { return currentHealthPoints / maxHealthPoints; }}

    void Start()
    {
        RegisterForMouseClick ();
        currentHealthPoints = maxHealthPoints;
        PutWeaponInHand ();
    }

    private void PutWeaponInHand ()
    {
        var weaponPrefab = weaponInUse.GetWeaponPrefab ();
        weaponPrefab = Instantiate (weaponPrefab);
        // TODO move to correct position annd parent to hand
    }

    void RegisterForMouseClick ()
    {
        cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
        cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
    }


    void OnMouseClick(RaycastHit raycastHit, int layerHit)
    {
        if (layerHit == enemyLayer)
        {
            GameObject enemy = raycastHit.collider.gameObject;
            if ((enemy.transform.position - transform.position).magnitude > maxAttackRange)
            {
                return;
            }

            currentTarget = enemy;

            var enemyComponent = enemy.GetComponent<Enemy>();
            if(Time.time - lastHitTime > minTimeBetweenHits)
            {
                enemyComponent.TakeDamage(damagePerHit);
                lastHitTime = Time.time;
            }
            
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
    }
}
