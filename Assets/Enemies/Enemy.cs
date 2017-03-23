using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(AICharacterControl))]
public class Enemy : MonoBehaviour, IDamageable
{

    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float attackRadius = 4f;
    [SerializeField] float chaseRadius = 5f;
    [SerializeField] float chaseStopRadius = 5f;
    [SerializeField] GameObject projectilePrefab;

    AICharacterControl aiCharacterControl = null;
    GameObject player = null;

    float currentHealthPoints = 100f;
    bool chasing = false;

    public float healthAsPercentage
    { get { return currentHealthPoints / maxHealthPoints; }}

    public void TakeDamage(float damage)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        aiCharacterControl = GetComponent<AICharacterControl>();
    }    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (distanceToPlayer <= chaseRadius || (distanceToPlayer <= chaseStopRadius && chasing))
        {
            aiCharacterControl.SetTarget(player.transform);
            chasing = true;
        }
        else
        {
            aiCharacterControl.SetTarget(transform);
            chasing = false;
        }
        if (distanceToPlayer <= attackRadius)
        {
            print(gameObject.name + " attacking player");
 //           GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        }
    }

    private void OnDrawGizmos()
    {
        // Draw chase sphere
        Gizmos.color = new Color(0f, 0f, 255f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        // Draw chaseStop sphere
        Gizmos.color = new Color(0f, 255f, 255f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, chaseStopRadius);

        // Draw attack sphere
        Gizmos.color = new Color(255f, 0f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
