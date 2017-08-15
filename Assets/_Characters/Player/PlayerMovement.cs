using UnityEngine;
using RPG.CameraUI;

namespace RPG.Characters
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] ParticleSystem criticalHitParticle;
        [Range (0.1f, 1.0f)] [SerializeField] float criticalHealthChance = 0.1f;
        [SerializeField] float criticalHitMultiplier = 1.25f;

        Character character;
        SpecialAbilities abilities;
        CameraRaycaster cameraRaycaster;
        Enemy enemy;
        WeaponSystem weaponSystem;


        void Start ()
        {
            character = GetComponent<Character> ();
            abilities = GetComponent<SpecialAbilities> ();
            weaponSystem = GetComponent<WeaponSystem> ();

            RegisterForMouseEvents ();
           
        }

        void RegisterForMouseEvents ()
        {
            cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
        }

        void Update ()
        {
            if (abilities != null)
            {
                ScanForAbilityKeyDown ();
            }
        }

        void ScanForAbilityKeyDown ()
        {
            for (int keyIndex = 1; keyIndex < abilities.GetNumberOfAbilities(); keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                {
                    abilities.AttemptSpecialAbility (keyIndex);
                }
            }
        }

        void OnMouseOverEnemy (Enemy enemyToSet)
        {
            this.enemy = enemyToSet;
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                weaponSystem.AttackTarget (enemy.gameObject);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                abilities.AttemptSpecialAbility (0);
            }
        }

        void OnMouseOverPotentiallyWalkable (Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                character.SetDestination (destination);
            }
        }

        bool IsTargetInRange (GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            {
                return distanceToTarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange ();
            }
        }

    }
}