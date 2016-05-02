using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileController : MonoBehaviour 
{
	public GameObject tilePrefab;
	public GameObject tilesParent;
	public GameObject cameraParent;
	public GameObject mapBackdrop;
	public Color defaultColor;
	public Color redColor;
	public Color blueColor;
	public Color greenColor;
	public List<Tile> tilesContainer;  //temp public

	Vector2 gridSize;
	bool canLightUp;
	GameObject localPlayerModelRef;
	List<Tile> litContainer;

	public void Setup()
	{
		canLightUp = false;
		litContainer = new List<Tile>();
	}

	void Update () 
	{
		if(canLightUp)
		{
			LightUpTile(GetTile (Mathf.RoundToInt(localPlayerModelRef.transform.localPosition.x), 
			                     Mathf.RoundToInt(localPlayerModelRef.transform.localPosition.z)));
		}
	}

	void LightUpTile(Tile inputTile)
	{
		for(int i = 0; i < litContainer.Count; i++)
			ResetTileColor(litContainer[i]);
		litContainer.Clear();
		if(inputTile.tileType == "None")
		{
			inputTile.GetComponent<MeshRenderer>().material.color = greenColor;
			litContainer.Add (inputTile);
		}
	}
	
	void ResetTileColor(Tile inputTile)
	{
		if(inputTile.tileType == "None")
			inputTile.gameObject.GetComponent<MeshRenderer>().material.color = defaultColor;
	}

	public void SetCanLightUpTile(bool val)
	{
		canLightUp = val;

		if(!val)
		{
			for(int i = 0; i < tilesContainer.Count; i++)
				ResetTileColor(tilesContainer[i]);
		}
	}

	public Vector3 ConvertLocationToPosition(Vector2 inputLoc, float height = 0f)
	{
		Vector3 pos = GetTile ((int)inputLoc.x, (int)inputLoc.y).gameObject.transform.localPosition;
		return new Vector3 (pos.x,height,pos.z);
	}

	public void SetLocalPlayerModelRef(GameObject playerModelReference)
	{
		localPlayerModelRef = playerModelReference;
	}

	public void CreateGrid(Vector2 inputGridSize)
	{
		gridSize = inputGridSize;
		for(int i = 0; i < gridSize.y; i++)
		{
			for(int j = 0; j < gridSize.x; j++)
			{
				GameObject tileInstance = Instantiate (tilePrefab);
				tileInstance.transform.parent = tilesParent.transform;
				tileInstance.transform.localPosition = new Vector3(j, 0, i);
				tileInstance.GetComponent<Tile>().TileSetup(j, i, "None", tileInstance);
				tilesContainer.Add(tileInstance.GetComponent<Tile>());
			}
		}
		mapBackdrop.transform.localScale = new Vector3 (gridSize.x, gridSize.y, 1);
		mapBackdrop.transform.localPosition = new Vector3 (gridSize.x/2 - .5f, mapBackdrop.transform.localPosition.y, gridSize.y/2 - .5f);
		cameraParent.transform.localPosition = mapBackdrop.transform.localPosition;
		Camera.main.transform.SetParent (cameraParent.transform);
	}

	public Tile GetTile(int x, int y)
	{
		for(int i = 0; i < tilesContainer.Count; i++)
		{
			if(tilesContainer[i].location.x == x && tilesContainer[i].location.y == y)
				return tilesContainer[i];
		}
		return null;
	}
}
