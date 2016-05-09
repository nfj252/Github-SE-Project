using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class StartGameID : MonoBehaviour 
{
	public int roomID;
	public int turnID;
	public int PID;
	public List<string> igns;
	public List<int> pids;

	void Start () 
	{
		DontDestroyOnLoad (this);
		igns = new List<string>();
		pids = new List<int>();
		roomID = -1;
		turnID = -1;
		PID = -1; 
	}
	public void addIGN (string ign)
	{
		igns.Add (ign);
	}
	public void addPID (int value)
	{
		pids.Add (value);
	}
	public List<string> getIGNList ()
	{
		return igns;
	}
	public List<int> getPIDList ()
	{
		return pids;
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

	public int GetRoomID()
	{
		return roomID;
	}

	public int GetTurnID()
	{
		return turnID;
	}

	public int GetPID()
	{
		return PID;
	}
}
