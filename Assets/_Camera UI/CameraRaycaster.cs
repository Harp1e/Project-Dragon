﻿using UnityEngine;
using UnityEngine.EventSystems;
using RPG.Characters;   // So we can detect by type

namespace RPG.CameraUI
{
    public class CameraRaycaster : MonoBehaviour    
    {
        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D enemyCursor = null;
        [SerializeField] Vector2 cursorHotspot = new Vector2 (0, 0);

        const int POTENTIALLY_WALKABLE_LAYER = 8;

        Rect screenRectAtStartPlay = new Rect (0, 0, Screen.width, Screen.height);

        float maxRaycastDepth = 100f; // Hard coded value

        // Delegates
        public delegate void OnMouseOverEnemy (Enemy enemy); // declare new delegate type
        public event OnMouseOverEnemy onMouseOverEnemy; // instantiate an observer set

        public delegate void OnMouseOverTerrain (Vector3 destination); // declare new delegate type
        public event OnMouseOverTerrain onMouseOverPotentiallyWalkable; // instantiate an observer set

        void Update ()
        {
            // Check if pointer is over an interactable UI element
            if (EventSystem.current.IsPointerOverGameObject ())
            {
                // TODO Implement UI interaction
            }
            else
            {
                PerformRaycasts ();
            }
        }

        void PerformRaycasts ()
        {
            if (screenRectAtStartPlay.Contains (Input.mousePosition))
            {
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                // Specify layer priorities here...
                if (RaycastForEnemy (ray)) { return; }
                if (RaycastForPotentiallyWalkable (ray)) { return; }
            }
        }

        bool RaycastForEnemy (Ray ray)
        {
            RaycastHit hitInfo;
            Physics.Raycast (ray, out hitInfo, maxRaycastDepth);
            if (hitInfo.transform == null) { return false; }
            var gameObjectHit = hitInfo.collider.gameObject; 
            var enemyHit = gameObjectHit.GetComponent<Enemy> ();
            if (enemyHit && enemyHit.gameObject.tag != "Healer")  
            {
                Cursor.SetCursor (enemyCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverEnemy (enemyHit);
                return true;
            }
            return false;
        }

        bool RaycastForPotentiallyWalkable (Ray ray)
        {
            RaycastHit hitInfo;
            LayerMask potentiallyWalkableLayer = 1 << POTENTIALLY_WALKABLE_LAYER;
            bool potentiallyWalkableHit = Physics.Raycast (ray, out hitInfo, maxRaycastDepth, potentiallyWalkableLayer);
            if (potentiallyWalkableHit)
            {
                Cursor.SetCursor (walkCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverPotentiallyWalkable (hitInfo.point);
                return true;
            }
            return false;
        }
    }
}