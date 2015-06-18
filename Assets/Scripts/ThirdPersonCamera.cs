using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {
	
	public GameObject player;
	private Camera thirdPersonCamera;

	private const float DEFAULT_CAMERA_ANGLE = 25f;
	private const float DEFAULT_CAMERA_HEIGHT = 4;
	private const float CROUCH_CAMERA_HEIGHT = 1;
	private const float CAMERA_OFFSET_FROM_PLAYER = -5;
	private const float CAMERA_LERP_SPEED = 10;

	private PlayerController playerControllerScript;

	void Start ()
	{
		playerControllerScript = player.GetComponent<PlayerController>();
		transform.position = new Vector3(transform.position.x, DEFAULT_CAMERA_HEIGHT, player.transform.position.z + CAMERA_OFFSET_FROM_PLAYER);
	}

	void Update ()
	{
		if(playerControllerScript.GetCrouch())
		{
			Vector3 tempPlayer = new Vector3(0, player.transform.position.y, player.transform.position.z);
			Vector3 tempCamera = new Vector3(0, transform.position.y, transform.position.z);

			transform.position = Vector3.Lerp(transform.position,
			                                  new Vector3(transform.position.x, player.transform.position.y + CROUCH_CAMERA_HEIGHT, player.transform.position.z + CAMERA_OFFSET_FROM_PLAYER),
			                                  Time.deltaTime * CAMERA_LERP_SPEED);
			transform.rotation = Quaternion.LookRotation(tempPlayer - tempCamera);
		}
		else
		{
			if(playerControllerScript.GetJump())
				transform.position = new Vector3(transform.position.x, player.transform.position.y + DEFAULT_CAMERA_HEIGHT, player.transform.position.z + CAMERA_OFFSET_FROM_PLAYER);
			else
				transform.position = Vector3.Lerp(transform.position,
				                                  new Vector3(transform.position.x, player.transform.position.y + DEFAULT_CAMERA_HEIGHT, player.transform.position.z + CAMERA_OFFSET_FROM_PLAYER),
				                                  Time.deltaTime * CAMERA_LERP_SPEED);

			transform.eulerAngles = Vector3.Lerp(transform.eulerAngles,
			                                  new Vector3(DEFAULT_CAMERA_ANGLE, transform.eulerAngles.y, transform.eulerAngles.z), Time.deltaTime * CAMERA_LERP_SPEED);
		}
	}
}
