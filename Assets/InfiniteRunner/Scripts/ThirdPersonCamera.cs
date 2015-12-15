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
	private const float CAMERA_OFFSET_FROM_PLAYER = -5;//
	private const float CAMERA_LERP_SPEED = 10;

	private PlayerController playerControllerScript;
	private ManageWorld manageWorldScript;
	
	void Start ()
	{
		cameraYRotation = 0;
		playerControllerScript = player.GetComponent<PlayerController>();
		manageWorldScript = GameObject.Find("Generated Terrain").GetComponent<ManageWorld>();
		transform.position = new Vector3(transform.position.x, DEFAULT_CAMERA_HEIGHT, player.transform.position.z + CAMERA_OFFSET_FROM_PLAYER);
		transform.eulerAngles = new Vector3(DEFAULT_CAMERA_ANGLE, 0, 0);
		startingCameraPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
		targetCameraPosition = new Vector3(startingCameraPosition.x, startingCameraPosition.y + DEFAULT_CAMERA_HEIGHT, startingCameraPosition.z + CAMERA_OFFSET_FROM_PLAYER);
		transform.position = targetCameraPosition;
	}
	
	void Update ()
	{
		if(playerControllerScript.GetCrouch())
		{			
			targetCameraPosition = new Vector3(targetCameraPosition.x, player.transform.position.y + CROUCH_CAMERA_HEIGHT, targetCameraPosition.z);
		}
		else
		{
			if(playerControllerScript.GetJump())
			{
				targetCameraPosition = new Vector3(targetCameraPosition.x, player.transform.position.y + DEFAULT_CAMERA_HEIGHT, targetCameraPosition.z);
				transform.position = targetCameraPosition;
			}
			else
			{
//				if(manageWorldScript.GetDirection() == ManageWorld.Direction.forward)
//					targetCameraPosition = new Vector3(player.transform.position.x, startingCameraPosition.y + DEFAULT_CAMERA_HEIGHT, player.transform.position.z + CAMERA_OFFSET_FROM_PLAYER);
//				else if(manageWorldScript.GetDirection() == ManageWorld.Direction.right)
//					targetCameraPosition = new Vector3(player.transform.position.x + CAMERA_OFFSET_FROM_PLAYER, startingCameraPosition.y + DEFAULT_CAMERA_HEIGHT, player.transform.position.z);
//				else
//					targetCameraPosition = new Vector3(player.transform.position.x - CAMERA_OFFSET_FROM_PLAYER, startingCameraPosition.y + DEFAULT_CAMERA_HEIGHT, player.transform.position.z);

				targetCameraPosition = new Vector3(targetCameraPosition.x, player.transform.position.y + DEFAULT_CAMERA_HEIGHT, targetCameraPosition.z);
			}
		}

		//Rounding
		if(Mathf.Abs(transform.eulerAngles.y - cameraYRotation) > 0.1f)
		{
			transform.eulerAngles = new Vector3(DEFAULT_CAMERA_ANGLE, Mathf.LerpAngle(transform.eulerAngles.y, cameraYRotation, Time.deltaTime * CAMERA_LERP_SPEED), 0);
		}
		else
		{
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, cameraYRotation, 0);
		}

		transform.position = Vector3.Lerp(transform.position, targetCameraPosition, Time.deltaTime * CAMERA_LERP_SPEED);

		Vector2 newCirclePos = FindCircleExtension(transform.position.x, transform.position.z);
		//transform.position = new Vector3(newCirclePos.x, transform.position.y, newCirclePos.y);
	}

	public void Turning()
	{
		if(manageWorldScript.GetDirection() == ManageWorld.Direction.forward)
		{
			cameraYRotation = 0;
			targetCameraPosition = new Vector3(startingCameraPosition.x, startingCameraPosition.y + DEFAULT_CAMERA_HEIGHT, player.transform.position.z + CAMERA_OFFSET_FROM_PLAYER);
		}
		else if(manageWorldScript.GetDirection() == ManageWorld.Direction.right)
		{
			cameraYRotation = 90;
			targetCameraPosition = new Vector3(player.transform.position.x + CAMERA_OFFSET_FROM_PLAYER, startingCameraPosition.y + DEFAULT_CAMERA_HEIGHT, startingCameraPosition.z);
		}
		else
		{
			cameraYRotation = -90;
			targetCameraPosition = new Vector3(player.transform.position.x - CAMERA_OFFSET_FROM_PLAYER, startingCameraPosition.y + DEFAULT_CAMERA_HEIGHT, startingCameraPosition.z);
		}

	}

	private Vector2 FindCircleExtension(float xPos, float zPos)
	{
		float tempAngle;
		float circlePositionX;
		float circlePositionZ;
		
		tempAngle = Mathf.Acos(xPos/( Mathf.Sqrt( Mathf.Pow(xPos, 2) + Mathf.Pow(zPos, 2) )));
		
		circlePositionX = Mathf.Abs(CAMERA_OFFSET_FROM_PLAYER) * Mathf.Cos(tempAngle);
		circlePositionZ = Mathf.Abs(CAMERA_OFFSET_FROM_PLAYER) * Mathf.Sin(tempAngle);
		
		if(Mathf.Sign(xPos) == 1 && Mathf.Sign(circlePositionX) != 1)
		{
			circlePositionX *=-1;
		}

		if(Mathf.Sign(zPos) == -1 && Mathf.Sign(circlePositionZ) != -1)
		{
			circlePositionZ *= -1;
		}

		return new Vector2(circlePositionX, circlePositionZ);
	}

	public void Restart()
	{
		cameraYRotation = 0;
		playerControllerScript = player.GetComponent<PlayerController>();
		manageWorldScript = GameObject.Find("Generated Terrain").GetComponent<ManageWorld>();
		transform.position = new Vector3(transform.position.x, DEFAULT_CAMERA_HEIGHT, player.transform.position.z + CAMERA_OFFSET_FROM_PLAYER);
		startingCameraPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
		targetCameraPosition = new Vector3(startingCameraPosition.x, startingCameraPosition.y + DEFAULT_CAMERA_HEIGHT, startingCameraPosition.z + CAMERA_OFFSET_FROM_PLAYER);
	}
}
