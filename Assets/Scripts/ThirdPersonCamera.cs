using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {
	
	public GameObject player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z-5);
	}
}
