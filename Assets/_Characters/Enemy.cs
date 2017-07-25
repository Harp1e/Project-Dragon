using UnityEngine;

// TODO Consider re-wire
using RPG.Core;

namespace RPG.Characters
{
    [RequireComponent (typeof (AICharacterControl))]
    public class Enemy : MonoBehaviour, IDamageable
    {

        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float chaseRadius = 5f;
        float chaseStopRadius = 5f;

        [SerializeField] float attackRadius = 4f;
        [SerializeField] float damagePerShot = 9f;
        [SerializeField] float firingPeriodInSecs = 0.5f;
        [SerializeField] float firingPeriodVariation = 0.1f;
        [SerializeField] Vector3 aimOffset = new Vector3 (0, 1f, 0);

        [SerializeField] GameObject projectileToUse;
        [SerializeField] GameObject projectileSocket;

        AICharacterControl aiCharacterControl = null;
        Player player = null;

        float currentHealthPoints;
        bool isChasing = false;
        bool isAttacking = false;

        public float healthAsPercentage
        { get { return currentHealthPoints / maxHealthPoints; } }

        public void TakeDamage (float damage)
        {
            currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);
            if (currentHealthPoints <= 0)
            {
                Destroy (gameObject);
            }
        }

        private void Start ()
        {
            player = FindObjectOfType<Player> ();
            aiCharacterControl = GetComponent<AICharacterControl> ();
            currentHealthPoints = maxHealthPoints;
            chaseStopRadius = chaseRadius;
        }

        private void Update ()
        {
            if (player.healthAsPercentage <= Mathf.Epsilon)
            {
                StopAllCoroutines ();
                Destroy (this);
            }

            float distanceToPlayer = Vector3.Distance (player.transform.position, transform.position);
            if (distanceToPlayer <= chaseRadius || (distanceToPlayer <= chaseStopRadius && isChasing))
            {
                aiCharacterControl.SetTarget (player.transform);
                isChasing = true;
            }
            else
            {
                aiCharacterControl.SetTarget (transform);
                isChasing = false;
            }
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
            // Draw chase sphere
            Gizmos.color = new Color (0f, 0f, 255f, 0.7f);
            Gizmos.DrawWireSphere (transform.position, chaseRadius);

            // Draw chaseStop sphere
            Gizmos.color = new Color (0f, 255f, 255f, 0.7f);
            Gizmos.DrawWireSphere (transform.position, chaseStopRadius);

            // Draw attack sphere
            Gizmos.color = new Color (255f, 0f, 0f, 0.7f);
            Gizmos.DrawWireSphere (transform.position, attackRadius);
        }
    }
}