using UnityEngine;

public class CameraFollow : MonoBehaviour {

    GameObject player;

	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");	
	}
	
	void LateUpdate () {
        transform.position = player.transform.position;
	}
}
