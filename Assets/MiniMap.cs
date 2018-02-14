using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour 
{
    [SerializeField] Transform target;
    [SerializeField] float followSpeed = 5f;

    float heightAbovePlayer = 30f;

	void LateUpdate () 
	{
        if (!target) { return; }

        Vector3 newPos = new Vector3 (
            target.position.x,
            target.position.y + heightAbovePlayer,
            target.position.z
        );
        transform.position = newPos;
	}
}
