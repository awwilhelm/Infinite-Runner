using UnityEngine;
using System.Collections.Generic;

public class ManageWorld : MonoBehaviour
{

	private GenerateWorld generateWorldScript;
	private ScoreKeeping scoreKeepingScript;
	private Environment currentSkyBox;
    private Transform player;

    private GameObject particles;
    private int snowCount;
    private Direction previousDir;
    private int depth;
    private int gameObjectID;
    private List<Environment> placesYouHaveBeen = new List<Environment>();

    private const float FORWARD_SPEED = 23.0f;
	private const int REGENERATE_LIMIT = 20;

	public enum Direction
	{
		forward,
		right,
		left
	}
	
	public enum Environment
	{
		ocean,
		atmosphere,
		cloud,
		rain,
		snow,
		runoff,
		fog,
		lake,
		groundwater,
		plantUptake
	}

	private Direction direction;

	//This determines which direction the terrain should move.  It also works in determining which way the player is facing
	void Start ()
	{
        particles = new GameObject("Particles");
        player = GameObject.Find("Player").transform;
        particles.transform.parent = transform;
        particles.transform.SetAsLastSibling();
		generateWorldScript = GameObject.Find ("World").GetComponent<GenerateWorld> ();
		scoreKeepingScript = GameObject.Find ("World").GetComponent<ScoreKeeping> ();
		transform.position = new Vector3 (0, 0, REGENERATE_LIMIT);
		direction = Direction.forward;
        previousDir = direction;
        depth = 0;
        gameObjectID = 0;

	}

	void Update ()
	{
		//Moves the terrain based on forward speed
		StepDirection ();
        particles.transform.SetAsLastSibling();
        //removes the block if it is a certain amount behind the player.
        if (transform.childCount > 0 && Mathf.Abs (transform.GetChild (0).position.z) + Mathf.Abs (transform.GetChild (0).position.x) > REGENERATE_LIMIT) {
			generateWorldScript.RemoveBlock ();

            //if(direction != previousDir)
            //{
            //    depth = 0;
            //}
            //int height=20;
            //GameObject snow;
            //if(direction == Direction.forward)
            //{
            //    for (int i = depth; i < 4; i++)
            //    {
            //        snow = Instantiate(generateWorldScript.GetSnowParticle(), new Vector3(0, height, 10*i), Quaternion.identity) as GameObject;
            //        snow.transform.name = "Particle " + gameObjectID + "a";
            //        snow.transform.parent = particles.transform;
            //        snow = Instantiate(generateWorldScript.GetSnowParticle(), new Vector3(-10, height, 10*i), Quaternion.identity) as GameObject;
            //        snow.transform.name = "Particle " + gameObjectID + "b";
            //        snow.transform.parent = particles.transform;
            //        snow = Instantiate(generateWorldScript.GetSnowParticle(), new Vector3(10, height, 10*i), Quaternion.identity) as GameObject;
            //        snow.transform.name = "Particle " + gameObjectID + "c";
            //        snow.transform.parent = particles.transform;
            //        depth++;
            //        snowCount++;
            //        gameObjectID++;
            //    }
            //    depth--;
            //} else if(direction == Direction.left)
            //{
            //    for (int i = depth; i < 4; i++)
            //    {
            //        snow = Instantiate(generateWorldScript.GetSnowParticle(), new Vector3(-10*i, height, 0), Quaternion.identity) as GameObject;
            //        snow.transform.name = "Particle " + gameObjectID + "a";
            //        snow.transform.parent = particles.transform;
            //        snow = Instantiate(generateWorldScript.GetSnowParticle(), new Vector3(-10*i, height, 10), Quaternion.identity) as GameObject;
            //        snow.transform.name = "Particle " + gameObjectID + "b";
            //        snow.transform.parent = particles.transform;
            //        snow = Instantiate(generateWorldScript.GetSnowParticle(), new Vector3(-10*i, height, -10), Quaternion.identity) as GameObject;
            //        snow.transform.name = "Particle " + gameObjectID + "c";
            //        snow.transform.parent = particles.transform;
            //        depth++;
            //        snowCount++;
            //        gameObjectID++;
            //    }
            //    depth--;
            //} else if(direction == Direction.right)
            //{
            //    for (int i = depth; i < 4; i++)
            //    {
            //        snow = Instantiate(generateWorldScript.GetSnowParticle(), new Vector3(10, height, 0), Quaternion.identity) as GameObject;
            //        snow.transform.name = "Particle " + gameObjectID + "a";
            //        snow.transform.parent = particles.transform;
            //        snow = Instantiate(generateWorldScript.GetSnowParticle(), new Vector3(10 * i, height, 10), Quaternion.identity) as GameObject;
            //        snow.transform.name = "Particle " + gameObjectID + "b";
            //        snow.transform.parent = particles.transform;
            //        snow = Instantiate(generateWorldScript.GetSnowParticle(), new Vector3(10 * i, height, -10), Quaternion.identity) as GameObject;
            //        snow.transform.name = "Particle " + gameObjectID + "c";
            //        snow.transform.parent = particles.transform;
            //        depth++;
            //        snowCount++;
            //        gameObjectID++;
            //    }
            //    depth--;
            //}
            //print(gameObjectID);
            //int number = 6;
            //if (snowCount > number)
            //{
            //    for (int i = 0; i < (snowCount - number) * 3; i++)
            //    {
            //        Destroy(particles.transform.GetChild(i).gameObject);
            //    }
            //    snowCount = number;
            //}
            //previousDir = direction;
        }
	}

    public void AddStartingEnvironment(Environment env)
    {
        placesYouHaveBeen.Add(env);
    }
    public void PrintEnvironmentsYouHaveBeenTo()
    {
        foreach(Environment environment in placesYouHaveBeen)
        {
            print(environment.ToString());
        }
    }
    //Changes which axis will start to change
    public void TurnRight ()
	{
		if (direction == Direction.left)
			direction = Direction.forward;
		else if (direction == Direction.forward)
			direction = Direction.right;
	}

	//Changes which axis will start to change
	public void TurnLeft ()
	{
		if (direction == Direction.forward)
			direction = Direction.left;
		else if (direction == Direction.right)
			direction = Direction.forward;
	}

	public void RoundTransformPosition (Collider col)
	{
		if (direction == Direction.forward) {
			transform.position = new Vector3 (transform.position.x - col.transform.position.x, transform.position.y, transform.position.z);
		}
		if (direction == Direction.left || direction == Direction.right) {
			transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z - col.transform.position.z);
		}
	}

	public void SetForward ()
	{
		direction = Direction.forward;
	}

	public Direction GetDirection ()
	{
		return direction;
	}

	private void StepDirection ()
	{
		if (direction == Direction.forward) {
			StepForward ();
		} else if (direction == Direction.right) {
			StepRight ();
		} else if (direction == Direction.left) {
			StepLeft ();
		}
	}

	private void StepForward ()
	{
		transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z - Time.deltaTime * FORWARD_SPEED);
	}

	private void StepRight ()
	{
		transform.position = new Vector3 (transform.position.x - Time.deltaTime * FORWARD_SPEED, transform.position.y, transform.position.z);
	}

	private void StepLeft ()
	{
		transform.position = new Vector3 (transform.position.x + Time.deltaTime * FORWARD_SPEED, transform.position.y, transform.position.z);
	}
	
	public void ChangeSkyBox (ManageWorld.Environment newEnvironment)
	{
		RenderSettings.skybox = generateWorldScript.GetSkyBoxMaterial (newEnvironment);
		currentSkyBox = newEnvironment;
		
		generateWorldScript.SetCurrentEnvironment (currentSkyBox);
		
		scoreKeepingScript.SetSkyBoxEnvironment (currentSkyBox);
        placesYouHaveBeen.Add(newEnvironment);

	}
}
