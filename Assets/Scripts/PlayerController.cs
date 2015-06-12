using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private GameObject spawnPoint;

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
	private const float duckingLength = 1.0f;
	private const float forwardSpeed = 10.0f;
	private const float BUFFER = 0.000001f;

	// Use this for initialization
	void Start () {
		spawnPoint = GameObject.Find("SpawnPoint");

		velocity = 0;
		jumping = false;
		ducking = false;
		crouchVal = 1;
		Spawn();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 currPosBasedOnState;
		Vector3 futurePosBasedOnState;
		if(Input.GetButton("Horizontal"))
		{
			transform.position = new Vector3(transform.position.x + Input.GetAxis("Horizontal")*Time.deltaTime*10, transform.position.y, transform.position.z);
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
			transform.position = new Vector3(transform.position.x, 1.1f, transform.position.z);
			transform.localScale = new Vector3(1 ,1 ,1);
		}
		
		float futureVelocity = velocity + gravity*Time.deltaTime; 
		futurePosition = new Vector3(transform.position.x, transform.position.y + (Time.deltaTime*(futureVelocity+velocity)/2), transform.position.z);
		velocity = futureVelocity;

		currPosBasedOnState = new Vector3(transform.position.x, transform.position.y-crouchVal, transform.position.z);
		futurePosBasedOnState = new Vector3(futurePosition.x, futurePosition.y-crouchVal, futurePosition.z);

		Physics.Raycast(currPosBasedOnState, -Vector3.up, out hit);
		Physics.Raycast(futurePosBasedOnState, -Vector3.up, out futureHit);

		Debug.DrawLine(currPosBasedOnState, hit.point, Color.green, 20, false);
		Debug.DrawLine(new Vector3(futurePosBasedOnState.x, futurePosBasedOnState.y, futurePosBasedOnState.z+0.05f), new Vector3(futureHit.point.x, futureHit.point.y, futureHit.point.z+0.05f), Color.blue, 20, false);

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

		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Time.deltaTime * forwardSpeed);
	}


	void OnCollisionEnter (Collision col)
	{
		if(col.transform.tag == "Death")
		{
			Spawn();
		}
	}

	void Spawn()
	{
		transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y + BUFFER, spawnPoint.transform.position.z);
	}
}
