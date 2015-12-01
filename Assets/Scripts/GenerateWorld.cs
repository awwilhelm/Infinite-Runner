using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateWorld : MonoBehaviour
{
	public GameObject player;
	public GameObject flatTerrain;
	public GameObject jumpObstacleTerrain;
	public GameObject jumpGapTerrain;
	public GameObject jumpLeftObstacleTerrain;
	public GameObject jumpCenterObstacleTerrain;
	public GameObject jumpRightObstacleTerrain;
	public GameObject duckObstacleTerrain;
	public GameObject turnRightTerrain;
	public GameObject turnLeftTerrain;
	public GameObject turnFork;
	
	public Material oceanSkyBox;
	public Material atmosphereSkyBox;
	public Material cloudSkyBox;
	public Material rainSkyBox;
	public Material snowSkyBox;
	public Material runoffSkyBox;
	public Material fogSkyBox;
	public Material lakeSkyBox;
	public Material groundwaterSkyBox;
	public Material plantUptakeSkyBox;

    public Material tempSkybox1;
    public Material tempSkybox2;
    public Material tempSkybox3;

    public GameObject snowParticle;

    public Material defaultSkyBox;
	
	public GameObject coin;

	private ManageWorld manageWorldScript;
	private ScoreKeeping scoreKeepingScript;

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
	private ManageWorld.Environment currentEnvironment;
	private ManageWorld.Environment nextRightEnvironment;
	private ManageWorld.Environment nextLeftEnvironment;

    private int skyboxCounter;

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
	
	//Adds Generated terrain Gameobject to hold all of the terrain
	void Awake ()
	{
		generatedTerrain = new GameObject ("Generated Terrain");
		generatedTerrain.AddComponent<ManageWorld> ();

		generatedTerrain.transform.position = Vector3.zero;
		scoreKeepingScript = GetComponent<ScoreKeeping> ();
	}

	//Sets everything up
	void Start ()
	{
		RenderSettings.skybox = defaultSkyBox;
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
        skyboxCounter = 0;
		obstaclePercentage = STARTING_OBSTACLE_PERCENTAGE;
		currentEnvironment = ManageWorld.Environment.ocean;
        manageWorldScript.AddStartingEnvironment(currentEnvironment);
		
		GenerateStartingTerrain ();
		PlaceStartingTerrain ();
		SetEnvironmentForT ();
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
			} else if (Random.Range (0, 30) < 6) {//testing at 6 instead of 1
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
			AddFlatTerrain (ManageWorld.Environment.ocean);
		} else if (type == 2) {
			AddJumpObstacleTerrain (ManageWorld.Environment.ocean);
		} else if (type == 3) {
			AddJumpGapTerrain (ManageWorld.Environment.ocean);
		} else if (type == 4) {
			AddJumpLeftObstacleTerrain (ManageWorld.Environment.ocean);
		} else if (type == 5) {
			AddJumpCenterObstacleTerrain (ManageWorld.Environment.ocean);
		} else if (type == 6) {
			AddJumpRightObstacleTerrain (ManageWorld.Environment.ocean);
		} else if (type == 7) {
			AddDuckObstacleTerrain (ManageWorld.Environment.ocean);
		} else if (type == 8) {
			AddMakeTurn (ManageWorld.Environment.ocean);
		} else if (type == 9) {
			AddTPath (ManageWorld.Environment.ocean);
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
		
		string name = init.transform.name.Replace ("(Clone)", "");
		init.transform.parent = generatedTerrain.transform;
		init.transform.localPosition = new Vector3 (tempNextLocation.x * 10, -0.5f, tempNextLocation.y * 10);
		init.name = name + " " + countTerrainBlocks;
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
            if(remove.name == "Particles")
            {
                remove = terrain.Dequeue();
            }
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


	//INISTANTIATING ALL THE BLOCKS


	private void AddFlatTerrain (ManageWorld.Environment theme)
	{
		GameObject straightAway = Instantiate (flatTerrain, Vector3.zero, Quaternion.identity) as GameObject;
		
		if (Random.Range (0, 10) < 4) {
			GameObject coins;
			int lane = Random.Range (-1, 2);
			int type = Random.Range (0, 3);
			for (int i = -1; i < 2; i++) {
				coins = Instantiate (coin, Vector3.zero, Quaternion.identity) as GameObject;
				coins.transform.position = new Vector3 (2.5f * lane, straightAway.transform.position.y + 1.5f, 3 * i);
				PickUp pickUpScript = coins.GetComponent<PickUp> ();
				pickUpScript.PickUpDefinition ((PickUp.PickUpType)type);
				
				coins.transform.parent = straightAway.transform;
			}
		}
		
		AddBlock (straightAway, Path.current);
	}

	private void AddJumpObstacleTerrain (ManageWorld.Environment theme)
	{
		AddBlock (Instantiate (jumpObstacleTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.current);
	}

	private void AddJumpGapTerrain (ManageWorld.Environment theme)
	{
		AddBlock (Instantiate (jumpGapTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.current);
	}

	private void AddJumpLeftObstacleTerrain (ManageWorld.Environment theme)
	{
		AddBlock (Instantiate (jumpLeftObstacleTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.current);
	}

	private void AddJumpCenterObstacleTerrain (ManageWorld.Environment theme)
	{
		AddBlock (Instantiate (jumpCenterObstacleTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.current);
	}

	private void AddJumpRightObstacleTerrain (ManageWorld.Environment theme)
	{
		AddBlock (Instantiate (jumpRightObstacleTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.current);
	}

	private void AddDuckObstacleTerrain (ManageWorld.Environment theme)
	{
		AddBlock (Instantiate (duckObstacleTerrain, Vector3.zero, Quaternion.identity) as GameObject, Path.current);
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
	
	private void AddMakeTurn (ManageWorld.Environment theme)
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
	private void AddTPath (ManageWorld.Environment theme)
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
	
	public Material GetSkyBoxMaterial (ManageWorld.Environment skybox)
	{
        skyboxCounter++;
        if (skyboxCounter-1 == 0)
        {
            return tempSkybox1;
        } else if(skyboxCounter-1 == 1)
        {
            return tempSkybox2;
        } else
        {
            skyboxCounter = 0;
            return tempSkybox3;
        }


		if (skybox == ManageWorld.Environment.ocean) {
			return oceanSkyBox;
		} else if (skybox == ManageWorld.Environment.atmosphere) {
			return atmosphereSkyBox;
		} else if (skybox == ManageWorld.Environment.cloud) {
			return cloudSkyBox;
		} else if (skybox == ManageWorld.Environment.rain) {
			return rainSkyBox;
		} else if (skybox == ManageWorld.Environment.snow) {
			return snowSkyBox;
		} else if (skybox == ManageWorld.Environment.runoff) {
			return runoffSkyBox;
		} else if (skybox == ManageWorld.Environment.fog) {
			return fogSkyBox;
		} else if (skybox == ManageWorld.Environment.lake) {
			return lakeSkyBox;
		} else if (skybox == ManageWorld.Environment.groundwater) {
			return groundwaterSkyBox;
		} else if (skybox == ManageWorld.Environment.plantUptake) {
			return plantUptakeSkyBox;
		}
		return oceanSkyBox;
	}
	
	public void SetEnvironmentForT ()
	{
		int rand = -1;
		if (currentEnvironment != ManageWorld.Environment.rain && currentEnvironment != ManageWorld.Environment.runoff && currentEnvironment != ManageWorld.Environment.groundwater) {
			rand = Random.Range (0, 2);
		}
	
		switch (currentEnvironment) {
		case ManageWorld.Environment.ocean:
			nextLeftEnvironment = rand == 0 ? ManageWorld.Environment.atmosphere : ManageWorld.Environment.groundwater;
			nextRightEnvironment = rand == 1 ? ManageWorld.Environment.atmosphere : ManageWorld.Environment.groundwater;
			break;
		case ManageWorld.Environment.atmosphere:
			nextLeftEnvironment = rand == 0 ? ManageWorld.Environment.cloud : ManageWorld.Environment.fog;
			nextRightEnvironment = rand == 1 ? ManageWorld.Environment.cloud : ManageWorld.Environment.fog;
			break;
		case ManageWorld.Environment.cloud:
			nextLeftEnvironment = rand == 0 ? ManageWorld.Environment.rain : ManageWorld.Environment.snow;
			nextRightEnvironment = rand == 1 ? ManageWorld.Environment.rain : ManageWorld.Environment.snow;
			break;
		case ManageWorld.Environment.rain:
			SetNextRainEnvironment ();
			break;
		case ManageWorld.Environment.snow:
			nextLeftEnvironment = rand == 0 ? ManageWorld.Environment.atmosphere : ManageWorld.Environment.runoff;
			nextRightEnvironment = rand == 1 ? ManageWorld.Environment.atmosphere : ManageWorld.Environment.runoff;
			break;
		case ManageWorld.Environment.runoff:
			SetNextRunoffEnvironment ();
			break;
		case ManageWorld.Environment.fog:
			nextLeftEnvironment = rand == 0 ? ManageWorld.Environment.atmosphere : ManageWorld.Environment.runoff;
			nextRightEnvironment = rand == 1 ? ManageWorld.Environment.atmosphere : ManageWorld.Environment.runoff;
			break;
		case ManageWorld.Environment.lake:
			nextLeftEnvironment = rand == 0 ? ManageWorld.Environment.atmosphere : ManageWorld.Environment.groundwater;
			nextRightEnvironment = rand == 1 ? ManageWorld.Environment.atmosphere : ManageWorld.Environment.groundwater;
			break;
		case ManageWorld.Environment.groundwater:
			SetNextGroundwaterEnvironment ();
			break;
		case ManageWorld.Environment.plantUptake:
			nextLeftEnvironment = rand == 0 ? ManageWorld.Environment.atmosphere : ManageWorld.Environment.groundwater;
			nextRightEnvironment = rand == 1 ? ManageWorld.Environment.atmosphere : ManageWorld.Environment.groundwater;
			break;
		}
		scoreKeepingScript.SetSkyBoxEnvironment (currentEnvironment);

	}
	
	private void SetNextRainEnvironment ()
	{
		int rand = Random.Range (0, 3);
		if (rand == 0) {
			nextLeftEnvironment = ManageWorld.Environment.runoff;
		} else if (rand == 1) {
			nextLeftEnvironment = ManageWorld.Environment.ocean;
		} else if (rand == 2) {
			nextLeftEnvironment = ManageWorld.Environment.lake;
		}
		
		nextRightEnvironment = nextLeftEnvironment;
		while (nextRightEnvironment == nextLeftEnvironment) {
			rand = Random.Range (0, 3);
			if (rand == 0) {
				nextRightEnvironment = ManageWorld.Environment.runoff;
			} else if (rand == 1) {
				nextRightEnvironment = ManageWorld.Environment.ocean;
			} else if (rand == 2) {
				nextRightEnvironment = ManageWorld.Environment.lake;
			}
		}
	}
	
	private void SetNextRunoffEnvironment ()
	{
		int rand = Random.Range (0, 4);
		if (rand == 0) {
			nextLeftEnvironment = ManageWorld.Environment.atmosphere;
		} else if (rand == 1) {
			nextLeftEnvironment = ManageWorld.Environment.groundwater;
		} else if (rand == 2) {
			nextLeftEnvironment = ManageWorld.Environment.ocean;
		} else if (rand == 3) {
			nextLeftEnvironment = ManageWorld.Environment.lake;
		}
		
		nextRightEnvironment = nextLeftEnvironment;
		while (nextRightEnvironment == nextLeftEnvironment) {
			rand = Random.Range (0, 4);
			if (rand == 0) {
				nextLeftEnvironment = ManageWorld.Environment.atmosphere;
			} else if (rand == 1) {
				nextLeftEnvironment = ManageWorld.Environment.groundwater;
			} else if (rand == 2) {
				nextLeftEnvironment = ManageWorld.Environment.ocean;
			} else if (rand == 3) {
				nextLeftEnvironment = ManageWorld.Environment.lake;
			}
		}
	}
	
	private void SetNextGroundwaterEnvironment ()
	{
		int rand = Random.Range (0, 3);
		if (rand == 0) {
			nextLeftEnvironment = ManageWorld.Environment.plantUptake;
		} else if (rand == 1) {
			nextLeftEnvironment = ManageWorld.Environment.ocean;
		} else if (rand == 2) {
			nextLeftEnvironment = ManageWorld.Environment.runoff;
		}
		
		nextRightEnvironment = nextLeftEnvironment;
		while (nextRightEnvironment == nextLeftEnvironment) {
			rand = Random.Range (0, 3);
			if (rand == 0) {
				nextLeftEnvironment = ManageWorld.Environment.plantUptake;
			} else if (rand == 1) {
				nextLeftEnvironment = ManageWorld.Environment.ocean;
			} else if (rand == 2) {
				nextLeftEnvironment = ManageWorld.Environment.runoff;
			}
		}
	}
	
	public void SetCurrentEnvironment (ManageWorld.Environment environment)
	{
		currentEnvironment = environment;
	}
	
	public ManageWorld.Environment GetNextLeftEnvironment ()
	{
		return nextLeftEnvironment;
	}
	
	public ManageWorld.Environment GetNextRightEnvironment ()
	{
		return nextRightEnvironment;
	}

    public GameObject GetSnowParticle()
    {
        return snowParticle;
    }
	
}
