using UnityEngine;
using System.Collections;
using RPG.CameraUI;

namespace RPG.Characters
{
    [RequireComponent (typeof (Character))]
    [RequireComponent (typeof (SpecialAbilities))]
    [RequireComponent (typeof (WeaponSystem))]
    [RequireComponent (typeof (HealthSystem))]
    public class PlayerControl : MonoBehaviour
    {
        Character character;
        SpecialAbilities abilities;
        WeaponSystem weaponSystem;
        HealthSystem healthSystem;

        void Start ()
        {
            character = GetComponent<Character> ();
            abilities = GetComponent<SpecialAbilities> ();
            weaponSystem = GetComponent<WeaponSystem> ();
            healthSystem = GetComponent<HealthSystem> ();

            RegisterForMouseEvents ();
        }

        void RegisterForMouseEvents ()
        {
            var cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
        }

        void Update ()
        {
            if (abilities != null && healthSystem.healthAsPercentage > 0f)
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

        void OnMouseOverEnemy (Enemy_AI enemy)
        {
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                weaponSystem.AttackTarget (enemy.gameObject);
            }
            else if (Input.GetMouseButton (0) && !IsTargetInRange (enemy.gameObject))
            {
                StartCoroutine (MoveAndAttack (enemy));
            }
            else if (Input.GetMouseButtonDown(1) && IsTargetInRange (enemy.gameObject))
            {
                abilities.AttemptSpecialAbility (0, enemy.gameObject);
            }
            else if (Input.GetMouseButtonDown (1) && !IsTargetInRange (enemy.gameObject))
            {
                StartCoroutine (MoveAndAttemptSpecialAbility (enemy));

            }
        }

        IEnumerator MoveToTarget(GameObject target)
        {
            character.SetDestination (target.transform.position);
            while (!IsTargetInRange(target.gameObject))
            {
                yield return new WaitForEndOfFrame ();
            }
            yield return new WaitForEndOfFrame ();
        }

        IEnumerator MoveAndAttack(Enemy_AI enemy)
        {
            yield return StartCoroutine (MoveToTarget (enemy.gameObject));
            weaponSystem.AttackTarget (enemy.gameObject);
        }

        IEnumerator MoveAndAttemptSpecialAbility (Enemy_AI enemy)
        {
            yield return StartCoroutine (MoveToTarget (enemy.gameObject));
            abilities.AttemptSpecialAbility (0, enemy.gameObject);
        }

        void OnMouseOverPotentiallyWalkable (Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                weaponSystem.StopAttacking ();
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

        public void TriggerSpecialAbility (int ability)
        {
            if (healthSystem.healthAsPercentage > 0f)
            {
                abilities.AttemptSpecialAbility (ability);
            }
        }
    }
}