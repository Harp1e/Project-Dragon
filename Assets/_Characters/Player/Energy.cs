﻿using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] RawImage energyBar = null;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenPointsPerSecond = 5f;

        float currentEnergyPoints;

        void Start ()
        {
            currentEnergyPoints = maxEnergyPoints;
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
            // TODO Remove Magic Numbers...
            float xValue = -(EnergyAsPercentage () / 2f) - 0.5f;
            energyBar.uvRect = new Rect (xValue, 0f, 0.5f, 1f);
        }

        float EnergyAsPercentage ()
        {
            return currentEnergyPoints / maxEnergyPoints;
        }
    }
}