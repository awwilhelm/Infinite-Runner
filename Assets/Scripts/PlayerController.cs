using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

	public LayerMask playerMask;
	private GameObject spawnPoint;
	private GameObject generatedTerrain;

	private GenerateWorld generateWorldScript;
	private ManageWorld manageWorldScript;
	private ThirdPersonCamera thirdPersonCameraScript;

	private float velocity;
	private bool jumping;
	private bool ducking;
	private float timePressedDuck;
	private float crouchVal;
	private RaycastHit hit;
	private RaycastHit futureHit;
	private Vector3 futurePosition;

	private bool collidedOnce;
	
	private const float startingVelocity = 30.0f;
	private const float gravity = -90.0f;
	private const float duckingLength = 0.5f;
	private const float BUFFER = 0.000001f;
	private const float RUNWAY_EDGE = 4.3f;
	private const float SKIN_WIDTH = 0.2f;
	Vector3 currPosBasedOnState;
	Vector3 temp;

	void Start ()
	{
		spawnPoint = GameObject.Find ("SpawnPoint");
		generatedTerrain = GameObject.Find ("Generated Terrain");
		generateWorldScript = GameObject.Find ("World").GetComponent<GenerateWorld> ();
		thirdPersonCameraScript = Camera.main.GetComponent<ThirdPersonCamera> ();
		manageWorldScript = generatedTerrain.GetComponent<ManageWorld> ();
		collidedOnce = false;

		Spawn ();
	}

	void Update ()
	{
		Vector3 futurePosBasedOnState;
		if (Input.GetButton ("Horizontal")) {
			transform.Translate (Vector3.right * Input.GetAxis ("Horizontal") * Time.deltaTime * 10);

			if (transform.position.x >= RUNWAY_EDGE) {
				transform.position = new Vector3 (RUNWAY_EDGE, transform.position.y, transform.position.z);
			} else if (transform.position.x <= -RUNWAY_EDGE) {
				transform.position = new Vector3 (-RUNWAY_EDGE, transform.position.y, transform.position.z);
			}
			
			if (transform.position.z >= RUNWAY_EDGE) {
				transform.position = new Vector3 (transform.position.x, transform.position.y, RUNWAY_EDGE);
			} else if (transform.position.z <= -RUNWAY_EDGE) {
				transform.position = new Vector3 (transform.position.x, transform.position.y, -RUNWAY_EDGE);
			}
		}

		if (Input.GetButton ("Jump") && !jumping && !ducking) {
			velocity = startingVelocity;
			jumping = true;
		}

		if (Input.GetButton ("Duck") && !ducking && !jumping) {
			ducking = true;
			crouchVal = 0.5f;
			timePressedDuck = Time.time;
			transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y - crouchVal, transform.localScale.z);
		}

		if (ducking && Time.time - timePressedDuck > duckingLength) {
			ducking = false;
			crouchVal = 1;
			transform.position = new Vector3 (transform.position.x, spawnPoint.transform.position.y + BUFFER, transform.position.z);
			transform.localScale = new Vector3 (1, 1, 1);
		}

		HandleGravity ();

		collidedOnce = false;
	}

	public bool GetCrouch ()
	{
		return ducking;
	}

	public bool GetJump ()
	{
		return jumping;
	}

	private void HandleGravity ()
	{
		float futureVelocity = velocity + gravity * Time.deltaTime;
		float futureLength = Time.deltaTime * (futureVelocity + velocity) / 2;
		futureLength = Mathf.Abs (futureLength);
		float dir = velocity > 0 ? 1 : -1;
		velocity = futureVelocity;
		currPosBasedOnState = new Vector3 (transform.position.x, transform.position.y - transform.localScale.y + SKIN_WIDTH, transform.position.z);
		
		Physics.Raycast (currPosBasedOnState, Vector3.up * dir, out hit, futureLength + SKIN_WIDTH);
		
		if (dir < 0) {
			if (hit.transform && hit.transform.tag == "Ground") {
				//print ("hit  "+ (transform.position.y-transform.localScale.y));
				transform.position = new Vector3 (transform.position.x, hit.point.y + transform.localScale.y, transform.position.z);
				velocity = 0;
				jumping = false;
			} else {
				transform.position = new Vector3 (transform.position.x, transform.position.y + (futureLength * dir), transform.position.z);
			}
		} else {
			transform.position = new Vector3 (transform.position.x, transform.position.y + futureLength, transform.position.z);
		}
	}

	void OnTriggerEnter (Collider col)
	{
		print ("collision " + col.transform.name);
		if (col.transform.tag == "Death") {
			Application.LoadLevel (0);
		}
		if (!collidedOnce) {
			if (col.transform.tag == "TurningRight") {
				if (col.transform.parent.tag == "TurningFork") {
					generateWorldScript.PlayerChoseTTurn (GenerateWorld.Path.right);
				}

				manageWorldScript.TurnRight ();
				manageWorldScript.RoundTransformPosition (col);
				transform.rotation = Quaternion.Euler (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 90, transform.rotation.eulerAngles.z);
				if (manageWorldScript.GetDirection () == ManageWorld.Direction.forward) {
					transform.position = new Vector3 (transform.position.z, transform.position.y, transform.position.z);
				} else if (manageWorldScript.GetDirection () == ManageWorld.Direction.right) {
					transform.position = new Vector3 (transform.position.x, transform.position.y, -(transform.position.x));
				}
				thirdPersonCameraScript.Turning ();
				collidedOnce = true;
				Destroy (col.transform.parent.gameObject);
			} else if (col.transform.tag == "TurningLeft") {
				if (col.transform.parent.tag == "TurningFork") {
					generateWorldScript.PlayerChoseTTurn (GenerateWorld.Path.left);
				}

				manageWorldScript.TurnLeft ();
				manageWorldScript.RoundTransformPosition (col);
				transform.rotation = Quaternion.Euler (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 90, transform.rotation.eulerAngles.z);
				if (manageWorldScript.GetDirection () == ManageWorld.Direction.forward) {
					transform.position = new Vector3 (-(transform.position.z), transform.position.y, transform.position.z);
				} else if (manageWorldScript.GetDirection () == ManageWorld.Direction.left) {
					transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.x);
				}
				thirdPersonCameraScript.Turning ();
				collidedOnce = true;
				Destroy (col.transform.parent.gameObject);
			}

		}
	}

	void Spawn ()
	{
		if (spawnPoint != null) {
			transform.position = new Vector3 (spawnPoint.transform.position.x, spawnPoint.transform.position.y + BUFFER, spawnPoint.transform.position.z);
		}

		thirdPersonCameraScript.Restart ();
		generatedTerrain.transform.position = Vector3.zero;
		transform.localRotation = Quaternion.Euler (transform.localRotation.eulerAngles.x, 0, transform.rotation.eulerAngles.z);
		transform.localScale = new Vector3 (1, 1, 1);
		jumping = false;
		ducking = false;
		velocity = 0;
		crouchVal = 1;
	}
}
