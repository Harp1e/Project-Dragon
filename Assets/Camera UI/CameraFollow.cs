using UnityEngine;

public class CameraFollow : MonoBehaviour {

    GameObject player;

	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");	
	}
	
	void LateUpdate ()
    {
//        transform.position = Vector3.Lerp(transform.position, player.transform.position, 0.1f);
        transform.position = player.transform.position;
    }
}
