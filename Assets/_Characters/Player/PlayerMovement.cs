using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;   //TODO remove 3rdPerson namespace
using UnityEngine.AI;
// Consider re-wiring
using RPG.CameraUI;

namespace RPG.Characters
{
    [RequireComponent (typeof (ThirdPersonCharacter))]
    [RequireComponent (typeof (NavMeshAgent))]
    [RequireComponent (typeof (AICharacterControl))]
    public class PlayerMovement : MonoBehaviour
    {
        // TODO Resolve fight between serialize and const
        [SerializeField] const int walkableLayerNumber = 8;
        [SerializeField] const int enemyLayerNumber = 9;

        CameraRaycaster cameraRaycaster = null;
        AICharacterControl aiCharacterControl = null;

        GameObject walkTarget = null;

        //bool isInDirectMode = false;

        void Start ()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster> ();
            aiCharacterControl = GetComponent<AICharacterControl> ();
            walkTarget = new GameObject ("walkTarget");

            cameraRaycaster.notifyMouseClickObservers += ProcessMouseClick; // Register for Mouse clicks
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable; // Register for Mouse clicks
        }

        private void OnMouseOverPotentiallyWalkable (Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                walkTarget.transform.position = destination;
                aiCharacterControl.SetTarget (walkTarget.transform);
            }           
        }

        void ProcessMouseClick (RaycastHit raycastHit, int layerHit)
        {
            switch (layerHit)
            {
                case enemyLayerNumber:
                    GameObject enemy = raycastHit.collider.gameObject;
                    aiCharacterControl.SetTarget (enemy.transform);
                    break;
                default:
                    Debug.LogWarning ("Can't handle mouse click for player movement");
                    return;
            }

        }

        // TODO Make controller input work again
        //private void ProcessDirectMovement()
        //{
        //    float h = Input.GetAxis("Horizontal");
        //    float v = Input.GetAxis("Vertical");

        //    // calculate camera relative direction to move:
        //    Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        //    Vector3 movement = v * cameraForward + h * Camera.main.transform.right;
        //    thirdPersonCharacter.Move(movement, false, false);
        //}
    }
}
