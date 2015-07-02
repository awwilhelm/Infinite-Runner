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
		StepDirection();

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
		
		RoundTransformPosition();
	}

	public void TurnLeft()
	{
		if(direction == Direction.forward)
			direction = Direction.left;
		else if(direction == Direction.right)
			direction = Direction.forward;
		
		RoundTransformPosition();
	}

	private void RoundTransformPosition()
	{
		float x = transform.position.x;
		float z = transform.position.z;

		if(direction == Direction.forward)
		{
			if(Mathf.Abs(transform.position.x % 10) >= 5)
			{
				if(Mathf.Sign(transform.position.x%10) == 1) 
					x = Mathf.RoundToInt(transform.position.x + (10 - transform.position.x % 10));
				else
				{
					x = Mathf.RoundToInt(transform.position.x - (10 + transform.position.x % 10));
				}
			}
			else
			{
				x = Mathf.Round(transform.position.x - transform.position.x % 10);
				StepDirection();
			}
			
			transform.position = new Vector3(x, transform.position.y, transform.position.z);
		}
		if(direction == Direction.left || direction == Direction.right)
		{
			if(Mathf.Abs(transform.position.z % 10) >= 5)
			{
				z = Mathf.RoundToInt(transform.position.z - (10 + transform.position.z % 10));
			}
			else
			{
				z = Mathf.RoundToInt(transform.position.z - (transform.position.z % 10));
				StepDirection();
			}
			transform.position = new Vector3(transform.position.x, transform.position.y, z);
		}
	}

	public void SetForward()
	{
		direction = Direction.forward;
	}

	public int GetDirection()
	{
		return (int)direction;
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
