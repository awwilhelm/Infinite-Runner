using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {
	
	public GameObject player;
	private Camera thirdPersonCamera;
	
	private float cameraYRotation;

	private Vector3 startingCameraPosition;
	private Vector3 targetCameraPosition;
	
	private const float DEFAULT_CAMERA_ANGLE = 25f;
	private const float DEFAULT_CAMERA_HEIGHT = 4;
	private const float CROUCH_CAMERA_HEIGHT = 1;
	private const float CAMERA_OFFSET_FROM_PLAYER = -5;
	private const float CAMERA_LERP_SPEED = 10;

	private PlayerController playerControllerScript;
	private ManageWorld manageWorldScript;
	
	void Start ()
	{
		cameraYRotation = 0;
		playerControllerScript = player.GetComponent<PlayerController>();
		manageWorldScript = GameObject.Find("Generated Terrain").GetComponent<ManageWorld>();
		transform.position = new Vector3(transform.position.x, DEFAULT_CAMERA_HEIGHT, player.transform.position.z + CAMERA_OFFSET_FROM_PLAYER);
		startingCameraPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
		targetCameraPosition = new Vector3(startingCameraPosition.x, startingCameraPosition.y + DEFAULT_CAMERA_HEIGHT, startingCameraPosition.z + CAMERA_OFFSET_FROM_PLAYER);
	}
	
	void Update ()
	{
		if(playerControllerScript.GetCrouch())
		{			
			transform.position = Vector3.Lerp(transform.position,
			                                  new Vector3(transform.position.x, player.transform.position.y + CROUCH_CAMERA_HEIGHT, player.transform.position.z + CAMERA_OFFSET_FROM_PLAYER),
			                                  Time.deltaTime * CAMERA_LERP_SPEED);
			transform.eulerAngles = new Vector3(DEFAULT_CAMERA_ANGLE, cameraYRotation, transform.eulerAngles.z);
		}
		else
		{
			if(playerControllerScript.GetJump())
			{
				transform.position = new Vector3(transform.position.x, player.transform.position.y + DEFAULT_CAMERA_HEIGHT, player.transform.position.z + CAMERA_OFFSET_FROM_PLAYER);
			}
			else
			{
				transform.position = targetCameraPosition;//Vector3.Lerp(transform.position, newPositon, Time.deltaTime * CAMERA_LERP_SPEED);
			}
			transform.eulerAngles = new Vector3(DEFAULT_CAMERA_ANGLE, cameraYRotation, transform.eulerAngles.z);
		}
	}

	public void Turning()
	{

		if(manageWorldScript.GetDirection() == 0)
		{
			cameraYRotation = 0;
			targetCameraPosition = new Vector3(startingCameraPosition.x, startingCameraPosition.y + DEFAULT_CAMERA_HEIGHT, player.transform.position.z + CAMERA_OFFSET_FROM_PLAYER);
		}
		else if(manageWorldScript.GetDirection() == 1)
		{
			cameraYRotation = 90;
			targetCameraPosition = new Vector3(player.transform.position.x + CAMERA_OFFSET_FROM_PLAYER, startingCameraPosition.y + DEFAULT_CAMERA_HEIGHT, startingCameraPosition.z);
		}
		else
		{
			cameraYRotation = -90;
			targetCameraPosition = new Vector3(player.transform.position.x - CAMERA_OFFSET_FROM_PLAYER, startingCameraPosition.y + DEFAULT_CAMERA_HEIGHT, startingCameraPosition.z);
		}

		player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);

	}
}
