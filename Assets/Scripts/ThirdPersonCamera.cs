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
		transform.position = new Vector3(0, player.transform.position.y+2, player.transform.position.z -5);
	}
}
