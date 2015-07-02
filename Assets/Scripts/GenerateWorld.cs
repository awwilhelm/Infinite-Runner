﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateWorld : MonoBehaviour {

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

	private ManageWorld manageWorldScript;

	private GameObject generatedTerrain;
	private ArrayList terrainType = new ArrayList();
	private Queue<GameObject> terrain;
	private int nextBlockLocation;
	private float terrainRotation;
	private int nextBlockLocationX;
	private int countTerrainBlocks = 0;

	private delegate void AddCorrectDirection();
	private AddCorrectDirection addCorrectDirection;

	private const int NUMBER_OF_BLOCKS = 20;
	private const int MIN_RUN_SPACE = 6;

	void Awake()
	{
		generatedTerrain = new GameObject("Generated Terrain");
		generatedTerrain.AddComponent<ManageWorld>();
	}

	void Start ()
	{
		manageWorldScript = generatedTerrain.GetComponent<ManageWorld>();
		manageWorldScript.SetForward();
		terrain = new Queue<GameObject>();
		addCorrectDirection = AddZPosition;
		nextBlockLocation = 0;
		nextBlockLocationX = 0;
		terrainRotation = 0;
		GenerateStartingTerrain();
		PlaceStartingTerrain();

	}

	private void GenerateStartingTerrain()
	{
		for(int i = 0; i< MIN_RUN_SPACE; i++)
		{
			terrainType.Add(1);
		}
		for(int i = MIN_RUN_SPACE; i<NUMBER_OF_BLOCKS; i++)
		{
			terrainType.Add(GenerateRandomType());
		}
	}

	private void PlaceStartingTerrain()
	{
		for(int i = 0; i<terrainType.Count; i++)
		{
			AddBlock((int)terrainType[i]);
		}
	}

	private int GenerateRandomType()
	{
		if(Random.Range(0,10) < 4)
			return 1;
		return Random.Range(8,10);
	}

	private void AddBlock(int type)
	{
		GameObject initialized = null;

		if(type == 1)
		{
			initialized = Instantiate(flatTerrain, Vector3.zero, Quaternion.identity) as GameObject;
		}
		else if(type == 2)
		{
			initialized = Instantiate(jumpObstacleTerrain, Vector3.zero, Quaternion.identity) as GameObject;
		}
		else if(type == 3)
		{
			initialized = Instantiate(jumpGapTerrain, Vector3.zero, Quaternion.identity) as GameObject;
		}
		else if(type == 4)
		{
			initialized = Instantiate(jumpLeftGapTerrain, Vector3.zero, Quaternion.identity) as GameObject;
		}
		else if(type == 5)
		{
			initialized = Instantiate(jumpCenterGapTerrain, Vector3.zero, Quaternion.identity) as GameObject;
		}
		else if(type == 6)
		{
			initialized = Instantiate(jumpRightGapTerrain, Vector3.zero, Quaternion.identity) as GameObject;
		}
		else if(type == 7)
		{
			initialized = Instantiate(duckObstacleTerrain, Vector3.zero, Quaternion.identity) as GameObject;
		}
		else if(type == 8) //Right turn
		{
			if(addCorrectDirection == MinusXPosition)
			{
				initialized = MakeRightTurn(AddZPosition);
			}
			else if(addCorrectDirection == AddXPosition)
			{
				initialized = MakeLeftTurn(AddZPosition);
			}
			else
			{
				initialized = MakeRightTurn(AddXPosition);
			}
		}
		else if(type == 9) //Left turn
		{
			if(addCorrectDirection == MinusXPosition)
			{
				initialized = MakeRightTurn(AddZPosition);
			}
			else if(addCorrectDirection == AddXPosition)
			{
				initialized = MakeLeftTurn(AddZPosition);
			}
			else
			{
				initialized = MakeLeftTurn(MinusXPosition);
			}
		}

		initialized.transform.rotation = Quaternion.Euler(0, terrainRotation, 0);

		if(addCorrectDirection == AddZPosition)
			terrainRotation = 0;
		else if(addCorrectDirection == AddXPosition)
			terrainRotation = 90;
		else
			terrainRotation = -90;

		initialized.transform.parent = generatedTerrain.transform;
		initialized.transform.localPosition = new Vector3(nextBlockLocationX*10, -0.5f, nextBlockLocation*10);
		initialized.name = name+countTerrainBlocks;
		countTerrainBlocks++;
		addCorrectDirection();
		terrain.Enqueue(initialized);
	}

	public void RemoveBlock()
	{
		GameObject remove = terrain.Dequeue();
		Destroy(remove);
		AddBlock(GenerateRandomType());
	}

	private void RemoveAllBlocks()
	{
		for(int i = 0; i<NUMBER_OF_BLOCKS; i++)
		{
			Destroy(generatedTerrain.transform.GetChild(i).gameObject);
		}
	}

	public void StartNewGame()
	{
		RemoveAllBlocks();
		terrainType.Clear();
		terrain.Clear();
		manageWorldScript.SetForward();
		addCorrectDirection = AddZPosition;
		nextBlockLocation = 0;
		nextBlockLocationX = 0;
		terrainRotation = 0;
		GenerateStartingTerrain();
		PlaceStartingTerrain();
	}

	private void AddZPosition()
	{
		nextBlockLocation++;
	}

	private void AddXPosition()
	{
		nextBlockLocationX++;
	}

	private void MinusXPosition()
	{
		nextBlockLocationX--;
	}

	private GameObject MakeLeftTurn(AddCorrectDirection XPos)
	{
		addCorrectDirection = XPos;
		return Instantiate(turnLeftTerrain, Vector3.zero, Quaternion.identity) as GameObject;
	}

	private GameObject MakeRightTurn(AddCorrectDirection ZPos)
	{
		addCorrectDirection = ZPos;
		return Instantiate(turnRightTerrain, Vector3.zero, Quaternion.identity) as GameObject;
	}
	
}
