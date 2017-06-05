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
        CameraRaycaster cameraRaycaster = null;
        AICharacterControl aiCharacterControl = null;

        GameObject walkTarget = null;

        //bool isInDirectMode = false;

        void Start ()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster> ();
            aiCharacterControl = GetComponent<AICharacterControl> ();
            walkTarget = new GameObject ("walkTarget");

            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable; // Register for Mouse clicks
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy; // Register for Mouse clicks
        }

        void OnMouseOverPotentiallyWalkable (Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                walkTarget.transform.position = destination;
                aiCharacterControl.SetTarget (walkTarget.transform);
            }           
        }

        void OnMouseOverEnemy (Enemy enemy)
        {
            if (Input.GetMouseButton (0) || Input.GetMouseButtonDown(1))
            {
                aiCharacterControl.SetTarget (enemy.transform);
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
