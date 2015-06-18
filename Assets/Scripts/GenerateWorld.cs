using UnityEngine;
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

	private GameObject generatedTerrain;
	private ArrayList terrainType = new ArrayList();
	private Queue<GameObject> terrain;
	private int nextBlockLocation;
	private int countTerrainBlocks = 0;

	private const int NUMBER_OF_BLOCKS = 10;
	private const int MIN_RUN_SPACE = 6;

	void Awake()
	{
		generatedTerrain = new GameObject("Generated Terrain");
		generatedTerrain.AddComponent<ManageWorld>();
	}

	void Start ()
	{
		terrain = new Queue<GameObject>();
		nextBlockLocation = 0;
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

		return Random.Range(1,8);
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
		initialized.transform.parent = generatedTerrain.transform;
		initialized.transform.localPosition = new Vector3(0, -0.5f, nextBlockLocation*10);
		initialized.name = name+countTerrainBlocks;
		countTerrainBlocks++;
		nextBlockLocation++;
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
		nextBlockLocation = 0;
		GenerateStartingTerrain();
		PlaceStartingTerrain();
	}
	
}
