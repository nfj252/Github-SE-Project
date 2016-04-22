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
		buildings = new List<Building>();
	}

	public void CreateBuildings(List<string> buildingDataList)
	{
		for(int i = 0; i < buildingDataList.Count; i++)
		{
			string[] buildingData = buildingDataList[i].Split(';');
			Color buildingColor = new Color(int.Parse(buildingData[3].Split(',')[0])/255, 
			                                int.Parse(buildingData[3].Split(',')[1])/255, 
			                                int.Parse(buildingData[3].Split(',')[2])/255,
			                                .5f);
			Building tempBuilding = new Building();
			tempBuilding.BuildingSetup();
			tempBuilding.buildingName = buildingData[0];
			tempBuilding.entrance = new Vector2(int.Parse(buildingData[1].Split(',')[0]), 
			                                	int.Parse(buildingData[1].Split(',')[1]));
			tileController.GetTile((int)tempBuilding.entrance.x, (int)tempBuilding.entrance.y).tileType = "Entrance";
			tileController.GetTile((int)tempBuilding.entrance.x, (int)tempBuilding.entrance.y).GetComponent<MeshRenderer>().materials[0].color = buildingColor;
			tileController.GetTile((int)tempBuilding.entrance.x, (int)tempBuilding.entrance.y).building = tempBuilding;

			string[] parsedLocations = buildingData[2].Split('-');
			for(int k = 0; k < parsedLocations.Length; k++)
			{
				tempBuilding.locations.Add(new Vector2(int.Parse(parsedLocations[k].Split(',')[0]), int.Parse(parsedLocations[k].Split(',')[1])));
				tileController.GetTile((int)tempBuilding.locations[k].x, (int)tempBuilding.locations[k].y).tileType = "Building";
				GameObject buildingInstance = Instantiate(buildingPrefab); 
				buildingInstance.name = tempBuilding.buildingName;
				buildingInstance.GetComponent<MeshRenderer>().materials[0].color = buildingColor;
				buildingInstance.transform.parent = buildingParent.transform;
				buildingInstance.transform.localPosition = tileController.GetTile((int)tempBuilding.locations[k].x, (int)tempBuilding.locations[k].y).transform.localPosition;
				buildingInstance.transform.localScale = new Vector3(1,int.Parse(buildingData[4]), 1);
				buildingInstance.transform.localPosition += new Vector3(0, buildingInstance.transform.localScale.y/2, 0);
				if(k == 0)
				{
					buildingInstance.AddComponent<Building>();
					buildingInstance.GetComponent<Building>().Copy(tempBuilding);
					buildings.Add (buildingInstance.GetComponent<Building>());
				}
			}
		}
	}
}
