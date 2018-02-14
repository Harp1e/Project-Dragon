using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapFollow : MonoBehaviour 
{
    [SerializeField] Transform target;

	void LateUpdate () 
	{
        if (!target) { return; }
        Vector3 newPos = new Vector3 (target.position.x, transform.position.y, target.position.z);
        transform.position = newPos;
	}
}
