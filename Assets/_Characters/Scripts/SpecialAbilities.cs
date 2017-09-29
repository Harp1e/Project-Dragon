using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class SpecialAbilities : MonoBehaviour
    {
        [SerializeField] AbilityConfig[] abilities;
        [SerializeField] Image energyBar;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenPointsPerSecond = 5f;
        [SerializeField] AudioClip outOfEnergy;

        AudioSource audioSource;

        float currentEnergyPoints;

        float energyAsPercent {  get { return currentEnergyPoints / maxEnergyPoints; } }

        void Start ()
        {
            audioSource = GetComponent<AudioSource> ();

            currentEnergyPoints = maxEnergyPoints;
            AttachInitialAbilities ();
            UpdateEnergyBar ();
        }

        void Update ()
        {
            if (currentEnergyPoints < maxEnergyPoints)
            {
                AddEnergyPoints ();
                UpdateEnergyBar ();
            }
        }

        void AttachInitialAbilities ()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachAbilityTo (gameObject);
            }
        }

        public void AttemptSpecialAbility (int abilityIndex, GameObject target = null)
        {
            var energyCost = abilities[abilityIndex].GetEnergyCost ();
            if (energyCost <= currentEnergyPoints)
            {
                ConsumeEnergy (energyCost);
                abilities[abilityIndex].Use (target);
            }
            else
            {
                audioSource.PlayOneShot (outOfEnergy);
            }
        }

        public int GetNumberOfAbilities ()
        {
            return abilities.Length;
        }

        void AddEnergyPoints ()
        {
            float pointsToAdd = regenPointsPerSecond * Time.deltaTime;
            currentEnergyPoints = Mathf.Clamp (currentEnergyPoints + pointsToAdd, 0f, maxEnergyPoints);
        }

        public void ConsumeEnergy (float amount)
        {
            float newEnergyPoints = currentEnergyPoints - amount;
            currentEnergyPoints = Mathf.Clamp (newEnergyPoints, 0f, maxEnergyPoints);
            UpdateEnergyBar ();
        }

        private void UpdateEnergyBar ()
        {
            if (energyBar)
            {
                energyBar.fillAmount = energyAsPercent;
            }
        }

    }
}