using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	float startingVelocity = 70;
	float velocity;
	float floorHeight = 1f;
	float gravity = -20f;
	bool jumping = false;
	bool ducking = false;
	float duckingLength = 1;
	float timePressedDuck;
	float speed = 10;
	float crouchVal = 0;
	RaycastHit hit;
	RaycastHit futureHit;
	Vector3 futurePosition;
	// Use this for initialization
	void Start () {
		
		float velocity = startingVelocity;
		Spawn();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetButton("Horizontal"))
		{
			transform.position = new Vector3(transform.position.x + Input.GetAxis("Horizontal")*Time.deltaTime*10, transform.position.y, transform.position.z);
		}

		if(Input.GetButton("Jump") && !jumping)
		{
			velocity = Mathf.Abs(velocity);
			jumping = true;
		}

		if(Input.GetButton("Duck") && !ducking)
		{
			ducking = true;
			crouchVal = 0;
			timePressedDuck = Time.time;
			transform.position = new Vector3(transform.position.x, 0, transform.position.z);
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y-crouchVal, transform.localScale.z);
		}

		if(jumping)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime*velocity, transform.position.z);
			velocity *= 0.9f;

		}
		else if(ducking && Time.time - timePressedDuck > duckingLength)
		{
			ducking = false;
			crouchVal = 0;
			//transform.localScale = new Vector3(1 ,1 ,1);
		}

		futurePosition = new Vector3(transform.position.x, transform.position.y + Time.deltaTime*gravity, transform.position.z);

		Vector3 temp = new Vector3(transform.position.x, transform.position.y-1, transform.position.z);
		Vector3 temp2 = new Vector3(futurePosition.x, futurePosition.y-1, futurePosition.z);
		Physics.Raycast(temp, -Vector3.up, out hit);
		Physics.Raycast(temp2, -Vector3.up, out futureHit);

		Debug.DrawLine(temp, hit.point, Color.green, 20, false);
		Debug.DrawLine(new Vector3(temp2.x, temp2.y, temp2.z+0.05f), new Vector3(futureHit.point.x, futureHit.point.y, futureHit.point.z+0.05f), Color.blue, 20, false);

		if(Physics.Raycast(temp, -Vector3.up, out hit) && hit.transform.tag == "Ground" && Physics.Raycast(temp2, -Vector3.up, out futureHit) && futureHit.transform.tag != "Ground")
		{
			velocity = startingVelocity;
			jumping = false;
		}
		else
		{
			transform.position = new Vector3(futurePosition.x, futurePosition.y, futurePosition.z);
		}

		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Time.deltaTime * speed);
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
		transform.position = new Vector3(0,1.1f,0);
	}
}
