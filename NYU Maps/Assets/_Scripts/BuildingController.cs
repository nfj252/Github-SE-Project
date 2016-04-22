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
			Color buildingColor = new Color(int.Parse(buildingData[3].Split(',')[0])/255, 
			                                int.Parse(buildingData[3].Split(',')[1])/255, 
			                                int.Parse(buildingData[3].Split(',')[2])/255,
			                                .5f);
			Vector2 entrance = new Vector2(int.Parse(buildingData[1].Split(',')[0]), 
			                               int.Parse(buildingData[1].Split(',')[1]));
			tileController.GetTile((int)entrance.x, (int)entrance.y).tileType = "Entrance-" + name;
			tileController.GetTile((int)entrance.x, (int)entrance.y).GetComponent<MeshRenderer>().materials[0].color = buildingColor;

			Debug.Log (tileController.GetTile((int)entrance.x, (int)entrance.y));
			string[] parsedLocations = buildingData[2].Split('-');
			Vector2 location = new Vector2();
			GameObject buildingInstance = new GameObject();

			for(int k = 0; k < parsedLocations.Length; k++)
			{
				location = new Vector2(int.Parse(parsedLocations[k].Split(',')[0]), 
				                       int.Parse(parsedLocations[k].Split(',')[1]));
				tileController.GetTile((int)location.x, (int)location.y).tileType = name;
				buildingInstance = Instantiate(buildingPrefab); 
				buildingInstance.GetComponent<MeshRenderer>().materials[0].color = buildingColor;

				buildingInstance.transform.parent = buildingParent.transform;
				buildingInstance.transform.localPosition = tileController.GetTile((int)location.x, (int)location.y).transform.localPosition;
				buildingInstance.transform.localScale = new Vector3(1,int.Parse(buildingData[4]),1);
				buildingInstance.transform.localPosition += new Vector3(0,buildingInstance.transform.localScale.y/2,0);
				buildingInstance.GetComponent<Building>().BuildingSetup(name, entrance, location, buildingInstance);
			}
		}
	}
}
