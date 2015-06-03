using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	float startingVelocity = 70;
	float velocity;
	float floorHeight = 1.1f;
	float gravity = -20f;
	bool jumping = false;
	bool ducking = false;
	float duckingLength = 1;
	float timePressedDuck;
	RaycastHit hit;
	// Use this for initialization
	void Start () {
		
		float velocity = startingVelocity;
		transform.position = new Vector3(transform.position.x, floorHeight, transform.position.z);
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
			timePressedDuck = Time.time;
			
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y-0.5f, transform.localScale.z);
		}
		if(jumping)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime*velocity, transform.position.z);
			velocity *= 0.9f;

		}
		else if(ducking && Time.time - timePressedDuck > duckingLength)
		{
			ducking = false;
			transform.localScale = new Vector3(1 ,1 ,1);
		}
		if(Physics.Raycast(transform.position, -Vector3.up, out hit) && hit.transform.tag == "Ground"  && transform.position.y + Time.deltaTime*gravity < floorHeight)
		{
			velocity = startingVelocity;
			jumping = false;
			transform.position = new Vector3(transform.position.x, floorHeight, transform.position.z);
		}
		else
		{
			transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime*gravity, transform.position.z);
		}
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Time.deltaTime * 10);


	}


	void OnCollisionEnter (Collision col)
	{
		print (col.transform.tag);
		if(col.transform.tag == "Death")
		{
			Spawn();
		}
	}

	void Spawn()
	{
		transform.position = new Vector3(0,1,0);
	}
}
