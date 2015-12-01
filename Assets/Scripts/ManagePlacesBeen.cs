using UnityEngine;
using System.Collections;

public class ManagePlacesBeen : MonoBehaviour {

    public struct EnviromentBeen
    {
        public bool ocean;
        public bool atmoshere;
        public bool cloud;
        public bool rain;
        public bool snow;
        public bool runoff;
        public bool fog;
        public bool lake;
        public bool groundwater;
        public bool plantuptake;
    }
    EnviromentBeen environmentBeen;

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

    private PlayerController playerControllerScript;

    // Use this for initialization
    void Start () {
        playerControllerScript.GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ToggleEnvironment(int intType)
    {
        ManageWorld.Environment environmentType = (ManageWorld.Environment) intType;
        if(environmentType == ManageWorld.Environment.ocean)
        {
            environmentBeen.ocean = !environmentBeen.ocean;
            oceanGameObject.transform.GetChild(1).gameObject.SetActive(!oceanGameObject.transform.GetChild(1).gameObject.activeSelf);
        } else if (environmentType == ManageWorld.Environment.atmosphere)
        {
            environmentBeen.atmoshere = !environmentBeen.atmoshere;
            atmosphereGameObject.transform.GetChild(1).gameObject.SetActive(!atmosphereGameObject.transform.GetChild(1).gameObject.activeSelf);
        } else if (environmentType == ManageWorld.Environment.cloud)
        {
            environmentBeen.cloud = !environmentBeen.cloud;
            cloudGameObject.transform.GetChild(1).gameObject.SetActive(!cloudGameObject.transform.GetChild(1).gameObject.activeSelf);
        }  else if (environmentType == ManageWorld.Environment.rain)
        {
            environmentBeen.rain = !environmentBeen.rain;
            rainGameObject.transform.GetChild(1).gameObject.SetActive(!rainGameObject.transform.GetChild(1).gameObject.activeSelf);
        } else if (environmentType == ManageWorld.Environment.snow)
        {
            environmentBeen.snow = !environmentBeen.snow;
            snowGameObject.transform.GetChild(1).gameObject.SetActive(!snowGameObject.transform.GetChild(1).gameObject.activeSelf);
        } else if (environmentType == ManageWorld.Environment.runoff)
        {
            environmentBeen.runoff = !environmentBeen.runoff;
            runoffGameObject.transform.GetChild(1).gameObject.SetActive(!runoffGameObject.transform.GetChild(1).gameObject.activeSelf);
        } else if (environmentType == ManageWorld.Environment.fog)
        {
            environmentBeen.fog = !environmentBeen.fog;
            fogGameObject.transform.GetChild(1).gameObject.SetActive(!fogGameObject.transform.GetChild(1).gameObject.activeSelf);
        } else if (environmentType == ManageWorld.Environment.lake)
        {
            environmentBeen.lake = !environmentBeen.lake;
            lakeGameObject.transform.GetChild(1).gameObject.SetActive(!lakeGameObject.transform.GetChild(1).gameObject.activeSelf);
        }  else if (environmentType == ManageWorld.Environment.groundwater)
        {
            environmentBeen.groundwater = !environmentBeen.groundwater;
            groundwaterGameObject.transform.GetChild(1).gameObject.SetActive(!groundwaterGameObject.transform.GetChild(1).gameObject.activeSelf);
        } else if (environmentType == ManageWorld.Environment.plantUptake)
        {
            environmentBeen.plantuptake = !environmentBeen.plantuptake;
            plantuptakeGameObject.transform.GetChild(1).gameObject.SetActive(!plantuptakeGameObject.transform.GetChild(1).gameObject.activeSelf);
        }
        PrintList();
    }

    public void CheckPlacesBeen()
    {
        //TODO: Compare and contrast different ways of setting up the list of booleans so we can compare if they are the same.

        //playerControllerScript.RestartLevel();
    }

    private void PrintList()
    {
        print(environmentBeen.ocean + " " + environmentBeen.atmoshere + " " + environmentBeen.cloud + " " + environmentBeen.rain + " " + environmentBeen.snow +
            " " + environmentBeen.runoff + " " + environmentBeen.fog + " " + environmentBeen.lake + " " + environmentBeen.groundwater + " " + environmentBeen.plantuptake);
    }
}
