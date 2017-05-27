using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;
using System;

namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] RawImage energyBar = null;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float pointsPerHit = 10f;

        CameraRaycaster cameraRaycaster = null;

        float currentEnergyPoints;

        void Start ()
        {
            currentEnergyPoints = maxEnergyPoints;
            cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
            cameraRaycaster.notifyRightClickObservers += ProcessRightClick;
        }

        private void ProcessRightClick (RaycastHit raycastHit, int layerHit)
        {
            float newEnergyPoints = currentEnergyPoints - pointsPerHit;
            currentEnergyPoints = Mathf.Clamp (newEnergyPoints, 0f, maxEnergyPoints);
            UpdateEnergyBar ();
        }

        private void UpdateEnergyBar ()
        {
            float xValue = -(EnergyAsPercentage () / 2f) - 0.5f;
            energyBar.uvRect = new Rect (xValue, 0f, 0.5f, 1f);
        }

        float EnergyAsPercentage ()
        {
            return currentEnergyPoints / maxEnergyPoints;
        }
    }
}