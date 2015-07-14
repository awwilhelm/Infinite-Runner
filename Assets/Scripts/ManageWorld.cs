using UnityEngine;
using System.Collections;

public class ManageWorld : MonoBehaviour {

	private GenerateWorld generateWorldScript;
	
	private const float FORWARD_SPEED = 12.0f;
	private const int REGENERATE_LIMIT = 20;

	public enum Direction
	{
		forward,
		right,
		left
	};

	private Direction direction;

	void Start ()
	{
		generateWorldScript = GameObject.Find("World").GetComponent<GenerateWorld>();
		transform.position = new Vector3(0, 0, REGENERATE_LIMIT);
		direction = Direction.forward;
	}

	void Update ()
	{
		StepDirection();

		if(transform.childCount > 0 && Mathf.Abs(transform.GetChild(0).position.z) + Mathf.Abs(transform.GetChild(0).position.x) > REGENERATE_LIMIT)
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

	public void RoundTransformPosition(Collider col)
	{
		if(direction == Direction.forward)
		{
			transform.position = new Vector3(transform.position.x - col.transform.position.x, transform.position.y, transform.position.z);
		}
		if(direction == Direction.left || direction == Direction.right)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - col.transform.position.z);
		}
	}

	public void SetForward()
	{
		direction = Direction.forward;
	}

	public Direction GetDirection()
	{
		return direction;
	}

	private void StepDirection()
	{
		if(direction == Direction.forward)
		{
			StepForward();
		}
		else if(direction == Direction.right)
		{
			StepRight();
		}
		else if(direction == Direction.left)
		{
			StepLeft();
		}
	}

	private void StepForward()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - Time.deltaTime * FORWARD_SPEED);
	}

	private void StepRight()
	{
		transform.position = new Vector3(transform.position.x - Time.deltaTime * FORWARD_SPEED, transform.position.y, transform.position.z);
	}

	private void StepLeft()
	{
		transform.position = new Vector3(transform.position.x + Time.deltaTime * FORWARD_SPEED, transform.position.y, transform.position.z);
	}
}
