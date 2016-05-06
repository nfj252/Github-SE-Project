using UnityEngine;
using System.Collections;

public class StartGameID : MonoBehaviour {

	int roomID;
	int turnID;
	int PID;
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this);
		roomID = -1;
		turnID = -1;
		PID = -1; 
	}
	public void setRoomID(int value)
	{
		roomID = value;
	}
	public void setTurnID(int value)
	{
		turnID = value;
	}
	public void setPID(int value)
	{
		PID = value;
	}

	public void resetIDs()
	{
		roomID = -1;
		turnID = -1;
		PID = -1; 
	}
}
