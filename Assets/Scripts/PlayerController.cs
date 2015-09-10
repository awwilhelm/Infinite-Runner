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
	private PickUpManager pickUpManagerScript;

	private float velocity;
	private bool jumping;
	private bool ducking;
	private float timePressedDuck;
	private float crouchVal;
	private RaycastHit hit;
	private RaycastHit futureHit;
	private Vector3 futurePosition;

	private bool collidedOnce;
	private bool grounded;
	private bool canTurn;
	private bool canTurnLeft;
	private bool canTurnRight;
	private int numberOfTurnTrigIn;
	
	private const float startingVelocity = 50.0f;
	private const float gravity = -180.0f;
	private const float duckingLength = 0.5f;
	private const float BUFFER = 0.000001f;
	private const float RUNWAY_EDGE = 4.3f;
	private const float SKIN_WIDTH = 0.2f;
	private const float PLAYER_SPEED = 5f;
	private const float PLAYER_SPEED_FIRST = 7.5f;
	private const float PLAYER_SPEED_SECOND = 10f;
	Vector3 bottomOfPlayerPosition;
	Vector3 temp;

	void Start ()
	{
		spawnPoint = GameObject.Find ("SpawnPoint");
		generatedTerrain = GameObject.Find ("Generated Terrain");
		generateWorldScript = GameObject.Find ("World").GetComponent<GenerateWorld> ();
		thirdPersonCameraScript = Camera.main.GetComponent<ThirdPersonCamera> ();
		manageWorldScript = generatedTerrain.GetComponent<ManageWorld> ();
		pickUpManagerScript = GameObject.Find ("World").GetComponent<PickUpManager> ();
		collidedOnce = false;
		grounded = false;
		numberOfTurnTrigIn = 0;
		ResetCanTurnVariables ();

		Spawn ();
	}

	void Update ()
	{
		MovePlayer ();

		TurnPlayer ();

		if (Input.GetButton ("Jump") && !jumping && !ducking && grounded) {
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

	//Moves player left to right using the mouse
	private void MovePlayer ()
	{
		Vector3 MouseToWorldPos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 6.0f));
		Vector3 targetPosition = transform.position;
		
		if (manageWorldScript.GetDirection () == ManageWorld.Direction.forward) {
			targetPosition = new Vector3 (MouseToWorldPos.x, transform.position.y, transform.position.z);
		} else if (manageWorldScript.GetDirection () == ManageWorld.Direction.left) {
			targetPosition = new Vector3 (transform.position.x, transform.position.y, MouseToWorldPos.z);
		} else if (manageWorldScript.GetDirection () == ManageWorld.Direction.right) {
			targetPosition = new Vector3 (transform.position.x, transform.position.y, MouseToWorldPos.z);
		}

		//Makes sure that the player cannot exceed the edges 
		if (targetPosition.x >= RUNWAY_EDGE) {
			targetPosition = new Vector3 (RUNWAY_EDGE, transform.position.y, transform.position.z);
		} else if (targetPosition.x <= -RUNWAY_EDGE) {
			targetPosition = new Vector3 (-RUNWAY_EDGE, transform.position.y, transform.position.z);
		}
		
		//Makes sure that the player cannot exceed the edges s
		if (targetPosition.z >= RUNWAY_EDGE) {
			targetPosition = new Vector3 (transform.position.x, transform.position.y, RUNWAY_EDGE);
		} else if (targetPosition.z <= -RUNWAY_EDGE) {
			targetPosition = new Vector3 (transform.position.x, transform.position.y, -RUNWAY_EDGE);
		}

		//If the player is close to target, the lerp will speed up giving a cleaner and faster feel
		if (Vector3.Distance (transform.position, targetPosition) > 0.2f) {
			transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * PLAYER_SPEED);
		} else if (Vector3.Distance (transform.position, targetPosition) > 0.1f) {
			transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * PLAYER_SPEED_FIRST);
		} else {
			transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * PLAYER_SPEED_SECOND);
		}

		HandleGravity ();
	}

	private void TurnPlayer ()
	{
		float axis = Input.GetAxisRaw ("Horizontal");
		if (canTurnLeft || canTurnRight) {
			return;
		} else if (canTurn && Input.GetButtonDown ("Horizontal")) {
			if (axis > 0) {
				canTurnRight = true;
			} else if (axis < 0) {
				canTurnLeft = true;
			}
		}
	}
	
	private void HandleGravity ()
	{
		//Kinematic Equation to calculate the future velocity/length
		float futureVelocity = velocity + gravity * Time.deltaTime;
		float futureLength = Time.deltaTime * (futureVelocity + velocity) / 2;
		futureLength = Mathf.Abs (futureLength);
		float dir = velocity > 0 ? 1 : -1;
		velocity = futureVelocity;
		bottomOfPlayerPosition = new Vector3 (transform.position.x, transform.position.y - transform.localScale.y + SKIN_WIDTH, transform.position.z);
		
		Physics.Raycast (bottomOfPlayerPosition, Vector3.up * dir, out hit, futureLength + SKIN_WIDTH);

		//If dir < 0 Then it is going down else, it is going up
		if (dir < 0) {
			if (hit.transform && hit.transform.tag == "Ground") {
				transform.position = new Vector3 (transform.position.x, hit.point.y + transform.localScale.y, transform.position.z);
				velocity = 0;
				jumping = false;
				grounded = true;
			} else {
				transform.position = new Vector3 (transform.position.x, transform.position.y + (futureLength * dir), transform.position.z);
				grounded = false;
			}
		} else {
			transform.position = new Vector3 (transform.position.x, transform.position.y + futureLength, transform.position.z);
			grounded = false;
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.transform.tag == "TurningLeftPress" || col.transform.tag == "TurningRightPress" || col.transform.tag == "TurningTPress") {
			canTurn = true;
			numberOfTurnTrigIn++;
		}
		
		if (col.transform.tag == "Coin") {
			GameObject.Find ("World").GetComponent<ScoreKeeping> ().AddToScore (col.transform.parent.GetComponent<PickUp> ().GetPoints ());
			Destroy (col.transform.parent.gameObject);
		}

		//If the placer enters an obstacle it will restart the level
		if (col.transform.tag == "Death") {
			Death ();
		}
		//If it hits a left or right turn it changes the players and cameras rotation and position then deletes the trigger
		if (!collidedOnce) {
			if (col.transform.tag == "TurningRight") {

				if (canTurnRight) {
					if (col.transform.parent.tag == "TurningFork") {
						generateWorldScript.PlayerChoseTTurn (GenerateWorld.Path.right);
						manageWorldScript.ChangeSkyBox (generateWorldScript.GetNextRightEnvironment ());
						generateWorldScript.SetEnvironmentForT ();
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
					ResetCanTurnVariables ();
					Destroy (col.transform.parent.gameObject);
				} else if (canTurnLeft) {
					Destroy (col.transform.gameObject);
				} else {
					Death ();
				}
			} else if (col.transform.tag == "TurningLeft") {

				if (canTurnLeft) {
					if (col.transform.parent.tag == "TurningFork") {
						generateWorldScript.PlayerChoseTTurn (GenerateWorld.Path.left);
						manageWorldScript.ChangeSkyBox (generateWorldScript.GetNextLeftEnvironment ());
						generateWorldScript.SetEnvironmentForT ();
						
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
					ResetCanTurnVariables ();
					Destroy (col.transform.parent.gameObject);
				} else if (canTurnRight) {
					Destroy (col.transform.gameObject);
				} else {
					Death ();
				}
			}

		}
	}

	void OnTriggerExit (Collider col)
	{
		if (col.transform.tag == "TurningLeftPress" || col.transform.tag == "TurningRightPress" || col.transform.tag == "TurningTPress") {
			numberOfTurnTrigIn--;
			if (numberOfTurnTrigIn < 1) {
				canTurn = false;
			}
		}
	}

	private void ResetCanTurnVariables ()
	{
		canTurn = false;
		canTurnLeft = false;
		canTurnRight = false;
	}

	void Death ()
	{
		Application.LoadLevel (0);
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
