using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(AICharacterControl))]
public class Enemy: MonoBehaviour {

    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float attackRadius = 4f;

    AICharacterControl aiCharacterControl = null;
    GameObject player = null;

    float currentHealthPoints = 100f;

    public float healthAsPercentage
    {
        get {
            return currentHealthPoints / maxHealthPoints;
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        aiCharacterControl = GetComponent<AICharacterControl>();
    }    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (distanceToPlayer <= attackRadius)
        {
            aiCharacterControl.SetTarget(player.transform);
        }
        else
        {
            aiCharacterControl.SetTarget(transform);
        }
    }

}
