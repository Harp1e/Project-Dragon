using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] Image energyOrb = null;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenPointsPerSecond = 5f;

        float currentEnergyPoints;

        void Start ()
        {
            currentEnergyPoints = maxEnergyPoints;
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

        private void AddEnergyPoints ()
        {
            float pointsToAdd = regenPointsPerSecond * Time.deltaTime;
            currentEnergyPoints = Mathf.Clamp (currentEnergyPoints + pointsToAdd, 0f, maxEnergyPoints);
        }

        public bool IsEnergyAvailable(float amount)
        {
            return amount <= currentEnergyPoints;
        }

        public void ConsumeEnergy (float amount)
        {
            float newEnergyPoints = currentEnergyPoints - amount;
            currentEnergyPoints = Mathf.Clamp (newEnergyPoints, 0f, maxEnergyPoints);
            UpdateEnergyBar ();
        }

        private void UpdateEnergyBar ()
        {
            energyOrb.fillAmount = EnergyAsPercentage();
        }

        float EnergyAsPercentage ()
        {
            return currentEnergyPoints / maxEnergyPoints;
        }
    }
}