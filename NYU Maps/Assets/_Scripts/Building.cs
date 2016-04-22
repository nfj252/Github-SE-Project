using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour {

	public string buildingName;
	public Vector2 entrance;
	public List<Vector2> locations;

	public void BuildingSetup()
	{
		locations = new List<Vector2>();
	}

	public void Copy(Building other)
	{
		buildingName = other.buildingName;
		entrance = other.entrance;
		locations = other.locations;
	}
}
