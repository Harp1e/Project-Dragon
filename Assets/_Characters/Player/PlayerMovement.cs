using UnityEngine;
using RPG.CameraUI;

namespace RPG.Characters
{
    public class PlayerMovement : MonoBehaviour
    {
        Character character;
        CameraRaycaster cameraRaycaster;
        EnemyAI enemy;
        SpecialAbilities abilities;
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

        void OnMouseOverEnemy (EnemyAI enemyToSet)
        {
            this.enemy = enemyToSet;
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                weaponSystem.AttackTarget (enemy.gameObject);
            }
            else if (Input.GetMouseButtonDown(1) && IsTargetInRange (enemy.gameObject))
            {
                abilities.AttemptSpecialAbility (0, enemy.gameObject);
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