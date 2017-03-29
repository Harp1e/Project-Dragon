using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [SerializeField] float projectileSpeed;
    [SerializeField] GameObject shooter;

    float damageCaused;

    public float GetLaunchSpeed()
    {
        return projectileSpeed;
    }

    public void SetDamage (float damage)
    {
        damageCaused = damage;
    }

    public void SetShooter(GameObject shooter)
    {
        this.shooter = shooter;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Component damageableComponent = collision.gameObject.GetComponent(typeof(IDamageable));
        if (damageableComponent) // && (shooter.GetType() != collision.gameObject.GetType()))
        {
            (damageableComponent as IDamageable).TakeDamage(damageCaused);
        }
        Destroy(gameObject, 0.1f);
    }
}
