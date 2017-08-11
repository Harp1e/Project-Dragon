using UnityEngine;

// TODO Consider re-wire
using RPG.Core;

namespace RPG.Characters
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] float attackRadius = 4f;
        [SerializeField] float damagePerShot = 9f;
        [SerializeField] float firingPeriodInSecs = 0.5f;
        [SerializeField] float firingPeriodVariation = 0.1f;
        [SerializeField] Vector3 aimOffset = new Vector3 (0, 1f, 0);

        [SerializeField] GameObject projectileToUse;
        [SerializeField] GameObject projectileSocket;

        Player player = null;

        bool isAttacking = false;

        private void Start ()
        {
            player = FindObjectOfType<Player> ();
        }

        private void Update ()
        {
            float distanceToPlayer = Vector3.Distance (player.transform.position, transform.position);

            if (distanceToPlayer <= attackRadius && !isAttacking)
            {
                isAttacking = true;
                float randomisedDelay = Random.Range (firingPeriodInSecs - firingPeriodVariation,
                    firingPeriodInSecs + firingPeriodVariation);
                InvokeRepeating ("FireProjectile", 0f, randomisedDelay); // TODO Switch to CoRoutine
            }
            if (distanceToPlayer > attackRadius)
            {
                isAttacking = false;
                CancelInvoke ();
            }
        }

        public void TakeDamage (float amount)
        {
            // TODO Remove
        }

        // TODO separate out character firing logic 
        void FireProjectile ()
        {
            GameObject newProjectile = Instantiate (projectileToUse, projectileSocket.transform.position, Quaternion.identity);
            Projectile projectileComponent = newProjectile.GetComponent<Projectile> ();
            projectileComponent.SetDamage (damagePerShot);
            projectileComponent.SetShooter (gameObject);

            Vector3 unitVectorToPlayer = (player.transform.position + aimOffset - projectileSocket.transform.position).normalized;
            float projectileSpeed = projectileComponent.GetDefaultLaunchSpeed ();
            newProjectile.GetComponent<Rigidbody> ().velocity = unitVectorToPlayer * projectileSpeed;
        }

        private void OnDrawGizmos ()
        {
            // Draw attack sphere
            Gizmos.color = new Color (255f, 0f, 0f, 0.7f);
            Gizmos.DrawWireSphere (transform.position, attackRadius);
        }
    }
}