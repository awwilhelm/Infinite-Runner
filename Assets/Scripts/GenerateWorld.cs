using UnityEngine;
using System.Collections;

public class GenerateWorld : MonoBehaviour {

	public GameObject player;
	public GameObject flatTerrain;
	public GameObject jumpTerrain;
	private GameObject generatedTerrain;
	private ArrayList terrain = new ArrayList();

	void Start ()
	{
		GeneratePlayer();
		generatedTerrain = new GameObject("Generated Terrain");
		GenerateTerrain();
		PlaceTerrain();
	}
	
	private void GeneratePlayer()
	{
		Instantiate(player, new Vector3(0, 1, 1), Quaternion.identity);
	}

	private void GenerateTerrain()
	{
		for(int i = 0; i<10; i++)
		{
			terrain.Add(Random.Range(1,3));
		}
	}

	private void PlaceTerrain()
	{
		for(int i = 0; i<terrain.Count; i++)
		{
			GameObject initialized = null;
			if((int)terrain[i] == 1)
			{
				initialized = Instantiate(flatTerrain, new Vector3(0, -0.5f, i*10), Quaternion.identity) as GameObject;
			}
			else if((int)terrain[i] == 2)
			{
				initialized = Instantiate(jumpTerrain, new Vector3(0, -0.5f, i*10), Quaternion.identity) as GameObject;
			}
			initialized.transform.parent = generatedTerrain.transform;
		}
	}
}
