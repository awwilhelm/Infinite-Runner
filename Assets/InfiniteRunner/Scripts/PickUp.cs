using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour
{
	
	public enum PickUpType
	{
		bronze,
		silver,
		gold
	}
	
	private int points;
	private PickUpType myType;
	private Material myMaterial;
	private bool calledOnce;
	
	public Material bronzeMat;
	public Material silverMat;
	public Material goldMat;
	
	void Awake ()
	{
		calledOnce = false;
		points = 0;
		myType = PickUpType.bronze;
		myMaterial = null;
	}
	
	public void PickUpDefinition (PickUpType type)
	{
		if (calledOnce) {
			return;
		}
		calledOnce = true;
		points = 2;
		switch (type) {
		case PickUpType.bronze:
			myType = type;
			myMaterial = bronzeMat;
			points = 5;
			break;
		case PickUpType.silver:
			myType = type;
			myMaterial = silverMat;
			points = 10;
			break;
		case PickUpType.gold:
			myType = type;
			myMaterial = goldMat;
			points = 15;
			break;
		}
		
		gameObject.transform.FindChild ("Coin").GetComponent<Renderer> ().material = myMaterial;
	}
	
	public PickUpType GetMyType ()
	{
		return myType;
	}
	
	public int GetPoints ()
	{
		return points;
	}
	
	void OnDrawGizmosSelected ()
	{
		Gizmos.color = new Color (1, 0, 0, 0.5F);
		//new Vector3 (transform.position.x, transform.position.y, transform.position.z)
		Gizmos.DrawCube (new Vector3 (transform.position.x, transform.position.y, transform.position.z), new Vector3 (1, 1, 1));
	}
}