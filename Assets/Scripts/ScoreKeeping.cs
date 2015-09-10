using UnityEngine;
using UnityEngine.UI;
using System.Collections;


[RequireComponent (typeof(GenerateWorld))]
public class ScoreKeeping : MonoBehaviour
{
	
	public Text textScore;
	public Text currentEnvironment;
	public Text nextLeftEnvironment;
	public Text nextRightEnvironment;
	public int score;
	private GenerateWorld generateWorldScript;
	
	// Use this for initialization
	
	void Awake ()
	{
		generateWorldScript = GetComponent<GenerateWorld> ();
	}
	
	void Start ()
	{
		score = 0;
		ChangingScore ();
		SetSkyBoxEnvironment (ManageWorld.Environment.ocean);
	}
	
	public void AddToScore (int val)
	{
		score += val;
		ChangingScore ();
	}
	
	public void ResetScore ()
	{
		score = 0;
		ChangingScore ();
	}
	
	public int GetScore ()
	{
		return score;
	}
	
	public void SetSkyBoxEnvironment (ManageWorld.Environment skybox)
	{
		currentEnvironment.text = "Current Environment: " + SkyBoxToString (skybox);
		nextLeftEnvironment.text = "Next Left Environment: " + SkyBoxToString (generateWorldScript.GetNextLeftEnvironment ());
		nextRightEnvironment.text = "Next Right Environment: " + SkyBoxToString (generateWorldScript.GetNextRightEnvironment ());
	}
	
	private string SkyBoxToString (ManageWorld.Environment skybox)
	{
		if (skybox == ManageWorld.Environment.ocean) {
			return "Ocean";
		} else if (skybox == ManageWorld.Environment.atmosphere) {
			return "Atmosphere";
		} else if (skybox == ManageWorld.Environment.cloud) {
			return "Cloud";
		} else if (skybox == ManageWorld.Environment.rain) {
			return "Rain";
		} else if (skybox == ManageWorld.Environment.snow) {
			return "Snow";
		} else if (skybox == ManageWorld.Environment.runoff) {
			return "Runoff";
		} else if (skybox == ManageWorld.Environment.fog) {
			return "Fog";
		} else if (skybox == ManageWorld.Environment.lake) {
			return "Lake";
		} else if (skybox == ManageWorld.Environment.groundwater) {
			return "Groundwater";
		} else if (skybox == ManageWorld.Environment.plantUptake) {
			return "Plant Uptake";
		}
		return "Ocean";
	}
	
	private void ChangingScore ()
	{
		textScore.text = "Score: " + score;		
	}
}
