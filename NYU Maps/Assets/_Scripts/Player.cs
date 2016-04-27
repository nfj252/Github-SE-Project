using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour 
{
	public string playerName;
	public Vector2 location;
	public List<Task> tasks;
	public GameObject playerModel;
	public Vector2 destinationLocation;
	public Vector3 destinationPosition;
	public float smoothTime;

	Vector3 velocity = Vector3.zero;
	TileController tileController;


	public void Setup()
	{
		tileController = FindObjectOfType<TileController> ();
		ResetDestinationLocation ();
	}
	
	void Update()
	{
		playerModel.transform.localPosition = Vector3.MoveTowards (playerModel.transform.localPosition, destinationPosition, Time.deltaTime * smoothTime);
		//playerModel.transform.localPosition = Vector3.SmoothDamp(playerModel.transform.localPosition, destinationPosition, ref velocity, smoothTime);
	}

	public void ResetDestinationLocation()
	{
		destinationLocation = location;
		destinationPosition = tileController.ConvertLocationToPosition(location, playerModel.transform.localPosition.y);
	}

	public void MoveModel(Vector2 destination)
	{
		destinationLocation = destination;
		destinationPosition = tileController.ConvertLocationToPosition(destination, playerModel.transform.localPosition.y);
	}
}
