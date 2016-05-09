﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour {

	public string buildingName;
	public Vector2 entrance;
	public List<Vector2> locations;
	public List<Task> availableTasks;
	public Color buildingColor;

	public void SetupBuilding(string iName, Vector2 iEntrance, Color iColor)
	{
		availableTasks = new List<Task>();
		buildingName = iName;
		entrance = iEntrance;
		buildingColor = iColor;
		this.name = buildingName;
		this.GetComponent<MeshRenderer>().materials[0].color = buildingColor;
	}
}
