using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour 
{
	public string tileType;
	public Vector2 location;
	public GameObject tileModel;
	
	public void TileSetup(int xVal, int yVal, string typeVal, GameObject tilePrefab)
	{
		location = new Vector2 (xVal, yVal);
		tileType = typeVal;
		tileModel = tilePrefab;
		this.name = "X" + xVal + "Y" + yVal;
	}
}
