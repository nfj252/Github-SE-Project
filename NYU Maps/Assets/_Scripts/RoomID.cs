using UnityEngine;
using System.Collections;

public class RoomID : MonoBehaviour {
	public IntroController introController;
	int roomID;
	int roomCount;
	// Use this for initialization
	void Start () {
		roomID = -1;
		roomCount = 0;
	}
	
	
	public void sendRoomIDToController()
	{
		//introController.setCurrentRoomID(roomID);
		Debug.Log ("Sent to intro controller:currentRoomID" + roomID);
		introController.JoinRoom (roomID);
	}
	
	public void setRoomID(int value)
	{
		roomID = value;
	}
	public void setRoomCount(int value)
	{
		roomCount = value;
	}
}
