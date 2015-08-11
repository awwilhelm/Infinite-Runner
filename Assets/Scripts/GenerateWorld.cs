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
	private int numberOfBlocksSinceLastTurn;

	private delegate void AddCorrectDirection (Path path);
	private AddCorrectDirection addCorrectDirection;
	private bool choosingPath = false;
	private int blocksSinceLastObstacle;
	private float obstaclePercentage;

	private const int NUMBER_OF_BLOCKS = 20;
	private const int MIN_RUN_SPACE = 5;
	private const int OBSTACLE_BUFFER = 1;
	private const float STARTING_OBSTACLE_PERCENTAGE = 7;

	public enum Path
	{
		current,
		right,
		left
	}

	private enum Theme
	{
		normal,
		other
	}
	
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
		blocksSinceLastObstacle = 0;
		numberOfBlocksSinceLastTurn = 0;
		obstaclePercentage = STARTING_OBSTACLE_PERCENTAGE;
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
		int nextBlock;
		if (blocksSinceLastObstacle >= OBSTACLE_BUFFER) {
			if (numberOfBlocksSinceLastTurn > 10) {
				numberOfBlocksSinceLastTurn = 0;
				blocksSinceLastObstacle = 0;
				nextBlock = 8;
			} else if (Random.Range (0, 30) < 1) {
				nextBlock = 9;
			} else {
				if (Random.Range (0, 10) < obstaclePercentage) {
					obstaclePercentage = STARTING_OBSTACLE_PERCENTAGE;
					nextBlock = Random.Range (2, 9);
					if (nextBlock == 8 || nextBlock == 7) {
						blocksSinceLastObstacle = -1;
					} else {
						blocksSinceLastObstacle = 0;
					}
				} else {
					obstaclePercentage++;
					nextBlock = 1;
				}
			}
		} else {
			blocksSinceLastObstacle++;
			nextBlock = 1;
		}

		numberOfBlocksSinceLastTurn++;
		return nextBlock;

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
			AddMakeTurn (Theme.normal);
		} else if (type == 9) {
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

	private void AddMakeTurn (Theme theme)
	{
		if (addCorrectDirection == MinusXPosition) {
			AddBlock (MakeRightTurn (AddZPosition), Path.current);
		} else if (addCorrectDirection == AddXPosition) {
			AddBlock (MakeLeftTurn (AddZPosition), Path.current);
		} else {
			int randomNumber = Random.Range (0, 2);
			if (randomNumber == 0) {
				AddBlock (MakeRightTurn (AddXPosition), Path.current);
			} else if (randomNumber == 1) {
				AddBlock (MakeLeftTurn (MinusXPosition), Path.current);
			}
		}
	}

	//Adds a T path to change theme
	private void AddTPath (Theme theme)
	{
		choosingPath = true;

		//Makes sure that the path is facing forward before the T Path
		if (addCorrectDirection == MinusXPosition) {
			AddBlock (MakeRightTurn (AddZPosition), Path.current);
		} else if (addCorrectDirection == AddXPosition) {
			AddBlock (MakeLeftTurn (AddZPosition), Path.current);
		}

		//Instantiates straight away towards the T Path
		for (int i = 0; i<4; i++) {
			AddBlock (Instantiate (flatTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.current);
		}

		//Initializes the left path end location and the right path end location
		leftBlockLocation = nextBlockLocation;
		rightBlockLocation = nextBlockLocation;

		//This is the T Block
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
