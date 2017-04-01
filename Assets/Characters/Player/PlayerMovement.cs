using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.AI;

[RequireComponent(typeof(ThirdPersonCharacter))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AICharacterControl))]
public class PlayerMovement : MonoBehaviour
{
    // TODO Resolve fight between serialize and const
    [SerializeField] const int walkableLayerNumber = 8;
    [SerializeField] const int enemyLayerNumber = 9;

    ThirdPersonCharacter thirdPersonCharacter = null;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster cameraRaycaster = null;
    AICharacterControl aiCharacterControl = null;

    Vector3 currentDestination, clickPoint;
    GameObject walkTarget = null;

    bool isInDirectMode = false;

    void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        aiCharacterControl = GetComponent<AICharacterControl>();
        currentDestination = transform.position;
        walkTarget = new GameObject("walkTarget");

        cameraRaycaster.notifyMouseClickObservers += ProcessMouseClick; // Register for Mouse clicks
    }

    void ProcessMouseClick(RaycastHit raycastHit, int layerHit)
    {
        switch (layerHit)
        {
            case enemyLayerNumber:
                GameObject enemy = raycastHit.collider.gameObject;
                aiCharacterControl.SetTarget(enemy.transform);
                break;
            case walkableLayerNumber:
                walkTarget.transform.position = raycastHit.point;
                aiCharacterControl.SetTarget(walkTarget.transform);
                break;
            default:
                Debug.LogWarning("Can't handle mouse click for player movement");
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
