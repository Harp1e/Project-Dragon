using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class WaypointContainer : MonoBehaviour
    {
        void OnDrawGizmos ()
        {
            Vector3 firstPosition = transform.GetChild(0).position;
            Vector3 previousPosition = firstPosition;
            foreach (Transform waypoint in transform)
            {
                Gizmos.color = new Color (0.5f, 0.5f, 0.5f, 0.7f);
                Gizmos.DrawSphere (waypoint.position, 0.2f);
                Gizmos.color = Color.gray;
                Gizmos.DrawLine (previousPosition, waypoint.position);
                previousPosition = waypoint.position;
            }
            Gizmos.DrawLine (previousPosition, firstPosition);
        }

    }
}