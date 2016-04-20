using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour {

	public string buildingName;
	public Vector2 entrance;
	public Vector2 location;
	public GameObject buildingModel;
	//public List<int> taskIDs;
	//public List<string> taskNames;

	public void BuildingSetup(string inputName, Vector2 inputEntrance, Vector2 inputLocation, GameObject buildingPrefab)
	{
		buildingName = inputName;
		entrance = inputEntrance;
		location = inputLocation;
		buildingModel = buildingPrefab;
		this.name = inputName;
	}
}
