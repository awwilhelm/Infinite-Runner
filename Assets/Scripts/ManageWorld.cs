using UnityEngine;
using System.Collections;

public class ManageWorld : MonoBehaviour {

	private GenerateWorld generateWorldScript;
	
	private const float FORWARD_SPEED = 10.0f;
	private const int REGENERATE_LIMIT = -20;

	// Use this for initialization
	void Start ()
	{
		generateWorldScript = GameObject.Find("World").GetComponent<GenerateWorld>();
		transform.position = new Vector3(0, 0, REGENERATE_LIMIT);
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - Time.deltaTime * FORWARD_SPEED);

		if(transform.GetChild(0).position.z < REGENERATE_LIMIT)
		{
			generateWorldScript.RemoveBlock();
		}
	}
}
