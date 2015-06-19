using UnityEngine;
using System.Collections;

public class ManageWorld : MonoBehaviour {

	private GenerateWorld generateWorldScript;
	
	private const float FORWARD_SPEED = 10.0f;
	private const int REGENERATE_LIMIT = -20;

	private enum Direction
	{
		forward,
		right,
		left
	};

	Direction direction;

	void Start ()
	{
		generateWorldScript = GameObject.Find("World").GetComponent<GenerateWorld>();
		transform.position = new Vector3(0, 0, REGENERATE_LIMIT);
		direction = Direction.forward;
	}

	void Update ()
	{
		if(direction == Direction.forward)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - Time.deltaTime * FORWARD_SPEED);
		}
		else if(direction == Direction.right)
		{
			transform.position = new Vector3(transform.position.x - Time.deltaTime * FORWARD_SPEED, transform.position.y, transform.position.z);
		}
		else if(direction == Direction.left)
		{
			transform.position = new Vector3(transform.position.x + Time.deltaTime * FORWARD_SPEED, transform.position.y, transform.position.z);
		}

		if(transform.GetChild(0).position.z < REGENERATE_LIMIT)
		{
			generateWorldScript.RemoveBlock();
		}
	}

	public void TurnRight()
	{
		if(direction == Direction.left)
			direction = Direction.forward;
		else if(direction == Direction.forward)
			direction = Direction.right;
	}

	public void TurnLeft()
	{
		if(direction == Direction.forward)
			direction = Direction.left;
		else if(direction == Direction.right)
			direction = Direction.forward;
	}

	public void SetForward()
	{
		direction = Direction.forward;
	}

	public int GetDirection()
	{
		return (int)direction;
	}
}
