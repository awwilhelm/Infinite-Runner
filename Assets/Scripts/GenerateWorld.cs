using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateWorld : MonoBehaviour
{

	public GameObject player;
	public GameObject flatTerrain;
	public GameObject jumpObstacleTerrain;
	public GameObject jumpGapTerrain;
	public GameObject jumpLeftGapTerrain;
	public GameObject jumpCenterGapTerrain;
	public GameObject jumpRightGapTerrain;
	public GameObject duckObstacleTerrain;
	public GameObject turnRightTerrain;
	public GameObject turnLeftTerrain;
	public GameObject turnFork;

	private ManageWorld manageWorldScript;

	private GameObject generatedTerrain;
	private ArrayList terrainType = new ArrayList ();
	private Queue<GameObject> terrain;
	private Vector2 nextBlockLocation;
	private Vector2 leftBlockLocation;
	private Vector2 rightBlockLocation;
	private float terrainRotation;
	private int countTerrainBlocks;
	private int currentNumberOfBlocks;

	private delegate void AddCorrectDirection (Path path);
	private AddCorrectDirection addCorrectDirection;
	private bool choosingPath = false;

	private const int NUMBER_OF_BLOCKS = 20;
	private const int MIN_RUN_SPACE = 5;

	public enum Path
	{
		current,
		right,
		left
	}
	;

	private enum Theme
	{
		normal,
		other
	}
	;
	
	//Adds Generated terrain Gameobject to hold all of the terrain
	void Awake ()
	{
		generatedTerrain = new GameObject ("Generated Terrain");
		generatedTerrain.AddComponent<ManageWorld> ();

		generatedTerrain.transform.position = Vector3.zero;
	}

	//Sets everything up
	void Start ()
	{
		manageWorldScript = generatedTerrain.GetComponent<ManageWorld> ();
		manageWorldScript.SetForward ();
		terrain = new Queue<GameObject> ();
		addCorrectDirection = AddZPosition;
		nextBlockLocation = new Vector2 (0, -4);
		leftBlockLocation = Vector2.zero;
		rightBlockLocation = Vector2.zero;
		terrainRotation = 0;
		countTerrainBlocks = 0;
		currentNumberOfBlocks = 0;
		GenerateStartingTerrain ();
		PlaceStartingTerrain ();
	}

	//Generates terrain until it meets the NUMBER_OF_BLOCKS requriement
	private void GenerateStartingTerrain ()
	{
		for (int i = 0; i< MIN_RUN_SPACE; i++) {
			terrainType.Add (1);
		}
		for (int i = MIN_RUN_SPACE; i<NUMBER_OF_BLOCKS; i++) {
			terrainType.Add (GenerateRandomType ());
		}
	}

	//Places the starting terrain
	private void PlaceStartingTerrain ()
	{
		for (int i = 0; i<terrainType.Count; i++) {
			if (!choosingPath) {
				PickBlock ((int)terrainType [i]);
			}
		}
	}

	//Picks a random number (some terrain has different probabilities of being picked)
	private int GenerateRandomType ()
	{
		if (Random.Range (0, 10) < 1)
			return 1;
		else if (Random.Range (0, 30) < 1)
			return 10;
		return Random.Range (1, 10);
	}

	//Defines what each random number will add
	private void PickBlock (int type)
	{
		if (type == 1) {
			AddFlatTerrain (Theme.normal);
		} else if (type == 2) {
			AddJumpObstacleTerrain (Theme.normal);
		} else if (type == 3) {
			AddJumpGapTerrain (Theme.normal);
		} else if (type == 4) {
			AddJumpLeftGapTerrain (Theme.normal);
		} else if (type == 5) {
			AddJumpCenterGapTerrain (Theme.normal);
		} else if (type == 6) {
			AddJumpRightGapTerrain (Theme.normal);
		} else if (type == 7) {
			AddDuckObstacleTerrain (Theme.normal);
		} else if (type == 8) { //Right turn
			AddMakeRightTurn (Theme.normal);
		} else if (type == 9) { //Left turn
			AddMakeLeftTurn (Theme.normal);
		} else if (type == 10) {
			AddTPath (Theme.normal);
		}
	}

	//Adds the block with the correct rotation, position, and other ways of tracking what block you are on
	private void AddBlock (GameObject init, Path path)
	{
		Vector2 tempNextLocation;
		if (path == Path.current) {
			tempNextLocation = nextBlockLocation;
		} else if (path == Path.left) {
			tempNextLocation = leftBlockLocation;
		} else if (path == Path.right) {
			tempNextLocation = rightBlockLocation;
		} else {
			print ("ERROR: Path was not chosen");
			return;
		}

		init.transform.rotation = Quaternion.Euler (0, terrainRotation, 0);

		changeTerrainRotation ();

		init.transform.parent = generatedTerrain.transform;
		init.transform.localPosition = new Vector3 (tempNextLocation.x * 10, -0.5f, tempNextLocation.y * 10);
		init.name = name + countTerrainBlocks;
		countTerrainBlocks++;
		currentNumberOfBlocks++;
		addCorrectDirection (path);
		terrain.Enqueue (init);
	}

	//Removes oldest block
	public void RemoveBlock ()
	{
		if (terrain.Count > 0) {
			GameObject remove = terrain.Dequeue ();
			Destroy (remove);
			currentNumberOfBlocks--;
		} else {
			print ("ERROR: could not remove block");
		}
		if (!choosingPath && currentNumberOfBlocks <= NUMBER_OF_BLOCKS) {
			PickBlock (GenerateRandomType ());
		}
	}

	//Determines which path the player chose when hitting the T Turn
	public void PlayerChoseTTurn (Path path)
	{
		choosingPath = false;
		if (path == Path.left) {
			nextBlockLocation = leftBlockLocation;
		} else if (path == Path.right) {
			nextBlockLocation = rightBlockLocation;
		}
		BringTerrainUpToDate ();
	}

	//After hitting T Path, regenerates world to have correct NUMBER_OF_BLOCKS
	private void BringTerrainUpToDate ()
	{
		while (currentNumberOfBlocks < NUMBER_OF_BLOCKS) {
			PickBlock (GenerateRandomType ());
		}
	}

	//Sets terrain rotation
	void changeTerrainRotation ()
	{
		if (addCorrectDirection == AddZPosition)
			terrainRotation = 0;
		else if (addCorrectDirection == AddXPosition)
			terrainRotation = 90;
		else
			terrainRotation = -90;
	}

	//Adds counter to Z position
	private void AddZPosition (Path path)
	{
		if (path == Path.current) {
			nextBlockLocation.y++;
		} else if (path == Path.left) {
			leftBlockLocation.y++;
		} else if (path == Path.right) {
			rightBlockLocation.y++;
		}
	}

	//Adds counter to X position (to the right)
	private void AddXPosition (Path path)
	{
		if (path == Path.current) {
			nextBlockLocation.x++;
		} else if (path == Path.left) {
			leftBlockLocation.x++;
		} else if (path == Path.right) {
			rightBlockLocation.x++;
		}
	}

	//Minus counter to X position (to the left)
	private void MinusXPosition (Path path)
	{
		if (path == Path.current) {
			nextBlockLocation.x--;
		} else if (path == Path.left) {
			leftBlockLocation.x--;
		} else if (path == Path.right) {
			rightBlockLocation.x--;
		}
	}

	//Generates left turn
	private GameObject MakeLeftTurn (AddCorrectDirection XPos)
	{
		addCorrectDirection = XPos;
		return Instantiate (turnLeftTerrain, Vector3.zero, Quaternion.identity) as GameObject;
	}

	//Generates right turn
	private GameObject MakeRightTurn (AddCorrectDirection ZPos)
	{
		addCorrectDirection = ZPos;
		return Instantiate (turnRightTerrain, Vector3.zero, Quaternion.identity) as GameObject;
	}

	private void AddFlatTerrain (Theme theme)
	{
		AddBlock (Instantiate (flatTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.current);
	}

	private void AddJumpObstacleTerrain (Theme theme)
	{
		AddBlock (Instantiate (jumpObstacleTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.current);
	}

	private void AddJumpGapTerrain (Theme theme)
	{
		AddBlock (Instantiate (jumpGapTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.current);
	}

	private void AddJumpLeftGapTerrain (Theme theme)
	{
		AddBlock (Instantiate (jumpLeftGapTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.current);
	}

	private void AddJumpCenterGapTerrain (Theme theme)
	{
		AddBlock (Instantiate (jumpCenterGapTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.current);
	}

	private void AddJumpRightGapTerrain (Theme theme)
	{
		AddBlock (Instantiate (jumpRightGapTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.current);
	}

	private void AddDuckObstacleTerrain (Theme theme)
	{
		AddBlock (Instantiate (duckObstacleTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.current);
	}

	private void AddMakeRightTurn (Theme theme)
	{
		if (addCorrectDirection == MinusXPosition) {
			AddBlock (MakeRightTurn (AddZPosition), Path.current);
		} else if (addCorrectDirection == AddXPosition) {
			AddBlock (MakeLeftTurn (AddZPosition), Path.current);
		} else {
			AddBlock (MakeRightTurn (AddXPosition), Path.current);
		}
	}

	private void AddMakeLeftTurn (Theme theme)
	{
		if (addCorrectDirection == MinusXPosition) {
			AddBlock (MakeRightTurn (AddZPosition), Path.current);
		} else if (addCorrectDirection == AddXPosition) {
			AddBlock (MakeLeftTurn (AddZPosition), Path.current);
		} else {
			AddBlock (MakeLeftTurn (MinusXPosition), Path.current);
		}
	}

	private void AddTPath (Theme theme)
	{
		choosingPath = true;
		if (addCorrectDirection == MinusXPosition) {
			AddBlock (MakeRightTurn (AddZPosition), Path.current);
		} else if (addCorrectDirection == AddXPosition) {
			AddBlock (MakeLeftTurn (AddZPosition), Path.current);
		}
		
		for (int i = 0; i<4; i++) {
			AddBlock (Instantiate (flatTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.current);
		}
		
		leftBlockLocation = nextBlockLocation;
		rightBlockLocation = nextBlockLocation;
		
		AddBlock (Instantiate (turnFork, Vector3.zero, Quaternion.identity) as GameObject, Path.current);
		//split
		
		//Left
		addCorrectDirection = MinusXPosition;
		addCorrectDirection (Path.left);
		changeTerrainRotation ();
		
		for (int i = 0; i<3; i++) {
			AddBlock (Instantiate (flatTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.left);
		}
		AddBlock (MakeRightTurn (AddZPosition), Path.left);
		changeTerrainRotation ();
		AddBlock (Instantiate (flatTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.left);
		
		//Right
		addCorrectDirection = AddXPosition;
		addCorrectDirection (Path.right);
		changeTerrainRotation ();
		
		for (int i = 0; i<3; i++) {
			AddBlock (Instantiate (flatTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.right);
		}
		AddBlock (MakeLeftTurn (AddZPosition), Path.right);
		changeTerrainRotation ();
		AddBlock (Instantiate (flatTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.right);
	}



}
