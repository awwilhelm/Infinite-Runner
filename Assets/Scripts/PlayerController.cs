using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private GameObject spawnPoint;
	private GameObject generatedTerrain;

	private GenerateWorld generateWorldScript;
	private ManageWorld manageWorldScript;

	private float velocity;
	private bool jumping;
	private bool ducking;
	private float timePressedDuck;
	private float crouchVal;
	private RaycastHit hit;
	private RaycastHit futureHit;
	private Vector3 futurePosition;
	
	private const float startingVelocity = 30.0f;
	private const float gravity = -90.0f;
	private const float duckingLength = 0.75f;
	private const float BUFFER = 0.000001f;

	void Start () {
		spawnPoint = GameObject.Find("SpawnPoint");
		generatedTerrain = GameObject.Find("Generated Terrain");
		generateWorldScript = GameObject.Find("World").GetComponent<GenerateWorld>();
		manageWorldScript = generatedTerrain.GetComponent<ManageWorld>();

		Spawn();
	}

	void Update ()
	{
		Vector3 currPosBasedOnState;
		Vector3 futurePosBasedOnState;
		if(Input.GetButton("Horizontal"))
		{
			transform.Translate(new Vector3(Input.GetAxis("Horizontal")*Time.deltaTime*10, 0, 0));
			//transform.position = transform.TransformPoint(Input.GetAxis("Horizontal")*Time.deltaTime, 0, 0);
			//transform.localPosition = new Vector3(transform.localPosition.x + Input.GetAxis("Horizontal")*Time.deltaTime*10, transform.localPosition.y, transform.localPosition.z);
		}

		if(Input.GetButton("Jump") && !jumping && !ducking)
		{
			velocity = startingVelocity;
			jumping = true;
		}

		if(Input.GetButton("Duck") && !ducking && !jumping)
		{
			ducking = true;
			crouchVal = 0.5f;
			timePressedDuck = Time.time;
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y-crouchVal, transform.localScale.z);
		}

		if(ducking && Time.time - timePressedDuck > duckingLength)
		{
			ducking = false;
			crouchVal = 1;
			transform.position = new Vector3(transform.position.x, spawnPoint.transform.position.y + BUFFER, transform.position.z);
			transform.localScale = new Vector3(1 ,1 ,1);
		}
		
		float futureVelocity = velocity + gravity*Time.deltaTime; 
		futurePosition = new Vector3(transform.position.x, transform.position.y + (Time.deltaTime*(futureVelocity+velocity)/2), transform.position.z);
		velocity = futureVelocity;

		currPosBasedOnState = new Vector3(transform.position.x, transform.position.y-crouchVal, transform.position.z);
		futurePosBasedOnState = new Vector3(futurePosition.x, futurePosition.y-crouchVal, futurePosition.z);

		Physics.Raycast(currPosBasedOnState, -Vector3.up, out hit);
		Physics.Raycast(futurePosBasedOnState, -Vector3.up, out futureHit);

		if(hit.transform.tag == "Ground" && futureHit.transform.tag != "Ground")
		{
			jumping = false;
			transform.position = new Vector3(transform.position.x, hit.point.y + crouchVal + BUFFER, transform.position.z);
			velocity = 0;
		}
		else
		{
			transform.position = new Vector3(futurePosition.x, futurePosition.y, futurePosition.z);
		}
	}

	public bool GetCrouch()
	{
		return ducking;
	}

	public bool GetJump()
	{
		return jumping;
	}

	void OnCollisionEnter (Collision col)
	{
		if(col.transform.tag == "Death")
		{
			Spawn();
			generateWorldScript.StartNewGame();
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.transform.tag == "TurningRight")
		{
			manageWorldScript.TurnRight();
			transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 90, transform.rotation.eulerAngles.z);
		}
		if(col.transform.tag == "TurningLeft")
		{
			manageWorldScript.TurnLeft();
			transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 90, transform.rotation.eulerAngles.z);
		}
	}

	void Spawn()
	{
		if(spawnPoint != null)
			transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y + BUFFER, spawnPoint.transform.position.z);

		generatedTerrain.transform.position = Vector3.zero;
		transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, 0, transform.rotation.eulerAngles.z);//Quaternion.identity;
		transform.localScale = new Vector3(1 ,1 ,1);
		jumping = false;
		ducking = false;
		velocity = 0;
		crouchVal = 1;
	}
}
