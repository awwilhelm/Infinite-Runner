using UnityEngine;
using System.Collections.Generic;

public class ManagePlacesBeen : MonoBehaviour {

    public GameObject oceanGameObject;
    public GameObject atmosphereGameObject;
    public GameObject cloudGameObject;
    public GameObject rainGameObject;
    public GameObject snowGameObject;
    public GameObject runoffGameObject;
    public GameObject fogGameObject;
    public GameObject lakeGameObject;
    public GameObject groundwaterGameObject;
    public GameObject plantuptakeGameObject;
    public GameObject endGameGameObject;

    private bool[] environmentBeen = new bool[10];

    private PlayerController playerControllerScript;
    private ManageWorld manageWorldScript;
    private ScoreKeeping scoreKeepingScript;

    // Use this for initialization
    void Start () {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        manageWorldScript = GameObject.Find("Generated Terrain").GetComponent<ManageWorld>();
        scoreKeepingScript = GameObject.Find("World").GetComponent<ScoreKeeping>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ToggleEnvironment(int intType)
    {
        environmentBeen[intType] = !environmentBeen[intType];

        ManageWorld.Environment environmentType = (ManageWorld.Environment) intType;
        if(environmentType == ManageWorld.Environment.ocean)
        {
            oceanGameObject.transform.GetChild(1).gameObject.SetActive(!oceanGameObject.transform.GetChild(1).gameObject.activeSelf);
        } else if (environmentType == ManageWorld.Environment.atmosphere)
        {
            atmosphereGameObject.transform.GetChild(1).gameObject.SetActive(!atmosphereGameObject.transform.GetChild(1).gameObject.activeSelf);
        } else if (environmentType == ManageWorld.Environment.cloud)
        {
            cloudGameObject.transform.GetChild(1).gameObject.SetActive(!cloudGameObject.transform.GetChild(1).gameObject.activeSelf);
        }  else if (environmentType == ManageWorld.Environment.rain)
        {
            rainGameObject.transform.GetChild(1).gameObject.SetActive(!rainGameObject.transform.GetChild(1).gameObject.activeSelf);
        } else if (environmentType == ManageWorld.Environment.snow)
        {
            snowGameObject.transform.GetChild(1).gameObject.SetActive(!snowGameObject.transform.GetChild(1).gameObject.activeSelf);
        } else if (environmentType == ManageWorld.Environment.runoff)
        {
            runoffGameObject.transform.GetChild(1).gameObject.SetActive(!runoffGameObject.transform.GetChild(1).gameObject.activeSelf);
        } else if (environmentType == ManageWorld.Environment.fog)
        {
            fogGameObject.transform.GetChild(1).gameObject.SetActive(!fogGameObject.transform.GetChild(1).gameObject.activeSelf);
        } else if (environmentType == ManageWorld.Environment.lake)
        {
            lakeGameObject.transform.GetChild(1).gameObject.SetActive(!lakeGameObject.transform.GetChild(1).gameObject.activeSelf);
        }  else if (environmentType == ManageWorld.Environment.groundwater)
        {
            groundwaterGameObject.transform.GetChild(1).gameObject.SetActive(!groundwaterGameObject.transform.GetChild(1).gameObject.activeSelf);
        } else if (environmentType == ManageWorld.Environment.plantUptake)
        {
            plantuptakeGameObject.transform.GetChild(1).gameObject.SetActive(!plantuptakeGameObject.transform.GetChild(1).gameObject.activeSelf);
        }
        //PrintList();
    }

    public void CheckPlacesBeen()
    {
        //TODO: Compare and contrast different ways of setting up the list of booleans so we can compare if they are the same.
        List<ManageWorld.Environment> listOfPlacesBeen = manageWorldScript.GetPlacesBeen();
        listOfPlacesBeen.Sort();
        int counter = 0;
        int wrong = 0;
        for(int i = 0; i<10; i++)
        {
            if(counter < listOfPlacesBeen.Count && listOfPlacesBeen[counter] == (ManageWorld.Environment)i)
            {
                if(!environmentBeen[i])
                {
                    wrong++;
                }
                counter++;
            } else
            {
                if(environmentBeen[i])
                {
                    wrong++;
                }
            }
        }
        endGameGameObject.SetActive(false);
        scoreKeepingScript.GoToScorePage(wrong);

        //foreach(ManageWorld.Environment list in listOfPlacesBeen)
        //{
        //    print(list.ToString());
        //}
        //playerControllerScript.RestartLevel();
    }

    private void PrintList()
    {
        print(environmentBeen[0] + " " + environmentBeen[1] + " " + environmentBeen[2] + " " + environmentBeen[3] + " " + environmentBeen[4] +
         " " + environmentBeen[5] + " " + environmentBeen[6] + " " + environmentBeen[7] + " " + environmentBeen[8] + " " + environmentBeen[9]);
    }
}
