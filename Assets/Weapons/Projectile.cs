using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [SerializeField] float projectileSpeed;
    [SerializeField] GameObject shooter;        // For inspection when paused

    const float DESTROY_DELAY = 0.01f;
    float damageCaused;

    public float GetDefaultLaunchSpeed()
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
        DamageIfDamageable (collision);
    }

    private void DamageIfDamageable (Collision collision)
    {
        var layerCollidedWith = collision.gameObject.layer;
        if (layerCollidedWith != shooter.layer)
        {
            Component damageableComponent = collision.gameObject.GetComponent (typeof (IDamageable));
            if (damageableComponent)

            {
                (damageableComponent as IDamageable).TakeDamage (damageCaused);
            }
            Destroy (gameObject, DESTROY_DELAY);
        }
    }
}
