using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(AICharacterControl))]
public class Enemy : MonoBehaviour, IDamageable {

    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float chaseRadius = 5f;
    float chaseStopRadius = 5f;

    [SerializeField] float attackRadius = 4f;
    [SerializeField] float damagePerShot = 9f;
    [SerializeField] float secondsBetweenShots = 0.5f;
    [SerializeField] Vector3 aimOffset = new Vector3 (0, 1f, 0);

    [SerializeField] GameObject projectileToUse;
    [SerializeField] GameObject projectileSocket;

    AICharacterControl aiCharacterControl = null;
    GameObject player = null;

    float currentHealthPoints;
    bool isChasing = false;
    bool isAttacking = false;

    public float healthAsPercentage
    { get { return currentHealthPoints / maxHealthPoints; }}

    public void TakeDamage(float damage)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        if (currentHealthPoints <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        aiCharacterControl = GetComponent<AICharacterControl>();
        currentHealthPoints = maxHealthPoints;
        chaseStopRadius = chaseRadius;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (distanceToPlayer <= chaseRadius || (distanceToPlayer <= chaseStopRadius && isChasing))
        {
            aiCharacterControl.SetTarget(player.transform);
            isChasing = true;
        }
        else
        {
            aiCharacterControl.SetTarget(transform);
            isChasing = false;
        }
        if (distanceToPlayer <= attackRadius && !isAttacking)
        {
            isAttacking = true;
            InvokeRepeating("FireProjectile",0f, secondsBetweenShots); // TODO Switch to CoRoutine
        }
        if (distanceToPlayer > attackRadius)
        {
            isAttacking = false;
            CancelInvoke();
        }
    }

    // TODO separate out character firing logic 
    void FireProjectile ()
    {
        GameObject newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity);
        Projectile projectileComponent = newProjectile.GetComponent<Projectile>();
        projectileComponent.SetDamage(damagePerShot);
        projectileComponent.SetShooter (gameObject);

        Vector3 unitVectorToPlayer = (player.transform.position + aimOffset - projectileSocket.transform.position).normalized;
        float projectileSpeed = projectileComponent.GetDefaultLaunchSpeed ();
        newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
    }

    private void OnDrawGizmos()
    {
        // Draw chase sphere
        Gizmos.color = new Color(0f, 0f, 255f, 0.7f);
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        // Draw chaseStop sphere
        Gizmos.color = new Color(0f, 255f, 255f, 0.7f);
        Gizmos.DrawWireSphere(transform.position, chaseStopRadius);

        // Draw attack sphere
        Gizmos.color = new Color(255f, 0f, 0f, 0.7f);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
