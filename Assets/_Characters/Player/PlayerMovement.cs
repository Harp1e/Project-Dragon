using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI;

// TODO Extract WeaponSystem 
namespace RPG.Characters
{
    public class PlayerMovement : MonoBehaviour
    {

        [SerializeField] Weapon currentWeaponConfig;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] ParticleSystem criticalHitParticle;

        [SerializeField] float baseDamage = 10f;
        [Range (0.1f, 1.0f)] [SerializeField] float criticalHealthChance = 0.1f;
        [SerializeField] float criticalHitMultiplier = 1.25f;

        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT_ATTACK";

        Animator animator;
        Character character;
        SpecialAbilities abilities;
        CameraRaycaster cameraRaycaster;
        Enemy enemy;
        GameObject weaponObject;

        float lastHitTime = 0f;

        void Start ()
        {
            character = GetComponent<Character> ();
            abilities = GetComponent<SpecialAbilities> ();

            RegisterForMouseEvents ();
            WeaponSetup ();         // TODO Move to WeaponSystem  
        }

        void RegisterForMouseEvents ()
        {
            cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
        }

        void Update ()
        {
            ScanForAbilityKeyDown ();
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
                AttackTarget ();
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
                return distanceToTarget <= currentWeaponConfig.GetMaxAttackRange ();
            }
        }

        // TODO Move to WeaponSystem
        void WeaponSetup ()
        {
            PutWeaponInHand (currentWeaponConfig); 
            SetAttackAnimation ();  
        }

        // TODO Move to WeaponSystem
        public void PutWeaponInHand (Weapon weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab ();
            GameObject dominantHand = RequestDominantHand ();
            Destroy (weaponObject);
            weaponObject = Instantiate (weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
        }

        // TODO Move to WeaponSystem
        void SetAttackAnimation ()
        {
            animator = GetComponent<Animator> ();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip ();    // TODO Remove constant
        }

        // TODO Move to WeaponSystem
        private GameObject RequestDominantHand ()
        {
            var dominantHands = GetComponentsInChildren<DominantHand> ();
            int numberOfDominantHands = dominantHands.Length;
            Assert.IsFalse (numberOfDominantHands <= 0, "No DominantHand found on Player. Please add one.");
            Assert.IsFalse (numberOfDominantHands > 1, "Multiple DominantHand scripts found on Player. Please remove all but one.");
            return dominantHands[0].gameObject;
        }

        // TODO use coroutines for move & attack
        void AttackTarget ()
        {
            if (Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits())
            {
                SetAttackAnimation ();
                animator.SetTrigger (ATTACK_TRIGGER);
                enemy.TakeDamage (CalculateDamage ());
                lastHitTime = Time.time;
            }
        }

        // TODO Move to WeaponSystem
        float CalculateDamage ()
        {
            bool isCriticalHit = UnityEngine.Random.Range (0f, 1f) <= criticalHealthChance;
            float damageBeforeCritical = baseDamage + currentWeaponConfig.GetAdditionalDamage ();
            if (isCriticalHit)
            {
                criticalHitParticle.Play ();
                return damageBeforeCritical * criticalHitMultiplier;
            }
            else
            {
                return damageBeforeCritical;
            }
        }
    }
}