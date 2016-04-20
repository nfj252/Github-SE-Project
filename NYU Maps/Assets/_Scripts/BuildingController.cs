using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingController : MonoBehaviour 
{
	TileController tileController;
	public GameObject buildingParent;
	public GameObject buildingPrefab;
	public List<Building> buildings;  //temp public
	
	public void Setup()
	{
		tileController = FindObjectOfType<TileController>();
	}

	public void CreateBuildings(List<string> buildingDataList)
	{
		for(int i = 0; i < buildingDataList.Count; i++)
		{
			string[] buildingData = buildingDataList[i].Split(';');
			string name = buildingData[0];
			Vector2 entrance = new Vector2(int.Parse(buildingData[1].Split(',')[0]), int.Parse(buildingData[1].Split(',')[1]));
			string[] parsedLocations = buildingData[2].Split('-');
			Vector2 location = new Vector2();
			GameObject buildingInstance = new GameObject();
			for(int k = 0; k < parsedLocations.Length; k++)
			{
				location = new Vector2(int.Parse(parsedLocations[k].Split(',')[0]), int.Parse(parsedLocations[k].Split(',')[1]));
				tileController.GetTile((int)location.x, (int)location.y).tileType = name;
				buildingInstance = Instantiate(buildingPrefab); 
				buildingInstance.transform.parent = buildingParent.transform;
				buildingInstance.transform.localPosition = tileController.GetTile((int)location.x, (int)location.y).transform.localPosition;
				buildingInstance.GetComponent<Building>().BuildingSetup(name, entrance, location, buildingInstance);
			}
		}
	}
}
