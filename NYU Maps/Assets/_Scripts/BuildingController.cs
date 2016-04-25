using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingController : MonoBehaviour 
{
	TileController tileController;
	public GameObject buildingParent;
	public GameObject buildingPrefab;
	public Material entranceTileMat;
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
			GameObject buildingInstance = Instantiate(buildingPrefab); 
			Building buildingComponent = buildingInstance.GetComponent<Building>();
			buildingInstance.transform.parent = buildingParent.transform;

			//attributes
			string buildingName = buildingData[0];
			Vector2 entrance = new Vector2(int.Parse(buildingData[1].Split(',')[0]), int.Parse(buildingData[1].Split(',')[1]));
			Vector2 lowerLeftLoc = new Vector2(int.Parse(buildingData[2].Split(',')[0]), int.Parse(buildingData[2].Split(',')[1]));
			Color buildingColor = new Color(int.Parse(buildingData[3].Split(',')[0])/255f, 
			                                int.Parse(buildingData[3].Split(',')[1])/255f, 
			                                int.Parse(buildingData[3].Split(',')[2])/255f, .5f);
			int length = int.Parse(buildingData[4]);
			int width = int.Parse(buildingData[5]);
			int height = int.Parse(buildingData[6]);

			for(int j = 0; j < length; j++)
			{
				for(int k = 0; k < width; k++)
				{
					buildingComponent.locations.Add(tileController.GetTile((int)lowerLeftLoc.x + j, (int)lowerLeftLoc.y + k).location);
					tileController.GetTile((int)lowerLeftLoc.x + j, (int)lowerLeftLoc.y + k).tileType = "Building";
				}
			}
			buildingComponent.SetupBuilding(buildingName, entrance, buildingColor);

			buildingInstance.transform.localPosition = tileController.GetTile((int)lowerLeftLoc.x, (int)lowerLeftLoc.y).transform.localPosition + 
					new Vector3((length-1)/2f, height/2f, (width-1)/2f); 
			buildingInstance.transform.localScale = new Vector3(length, height, width);

			tileController.GetTile((int)entrance.x, (int)entrance.y).tileType = "Entrance";
			tileController.GetTile((int)entrance.x, (int)entrance.y).GetComponent<MeshRenderer>().material = entranceTileMat;
			tileController.GetTile((int)entrance.x, (int)entrance.y).GetComponent<MeshRenderer>().material.color = buildingColor;

			tileController.GetTile((int)entrance.x, (int)entrance.y).building = buildingComponent;
			buildings.Add (buildingComponent);
		}
	}

	public List<Building> GetBuildings()
	{
		return buildings;
	}

	Vector2 GetLowerLeftLocation(List<Vector2> tileLocations)
	{
		Vector2 mostLowerLeftLoc = tileLocations[0];
		for(int i = 1; i < tileLocations.Count; i++)
		{
			if(tileLocations[i].x < mostLowerLeftLoc.x && tileLocations[i].y < mostLowerLeftLoc.y)
				mostLowerLeftLoc = tileLocations[i];
		}
		return mostLowerLeftLoc;
	}

	Vector2 GetDimensions(List<Vector2> tileLocations)
	{
		return new Vector2(1, 1);
	}
}
