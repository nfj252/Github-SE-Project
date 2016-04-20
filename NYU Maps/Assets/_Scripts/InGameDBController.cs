using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using System.Data;
using Npgsql;

public class InGameDBController : MonoBehaviour 
{
	int gameID;
	int numberOfPlayers; 
	List<string> playerNames;
	List<Vector2> playerLocations;
	int currentPlayerTurn;
	
	Vector2 gridSize;
	List<string> buildingData;

	NpgsqlConnection dbcon;
	NpgsqlCommand dbcmd;
	NpgsqlDataReader reader;
	
	void Start()
	{
		playerNames = new List<string>();
		playerLocations = new List<Vector2>();
		buildingData = new List<string>();
	}

	public void StartConnection () 
	{
		string connectionString = "Server= pdc-amd01.poly.edu;" +
								  "Database=kl1983;" +
								  "User ID=kl1983;" +
								  "Password=z86udpx%;";
		
		dbcon = new NpgsqlConnection(connectionString);
		dbcon.Open();
		Debug.Log ("Connected");
	}

	public void FetchRoomData()
	{
		gameID = 1;  //temp

		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText =  string.Format ("SELECT ign, xcoord, ycoord FROM ingameplayer WHERE roomid = '{0}' ORDER BY turnid ASC;", gameID);
		reader = dbcmd.ExecuteReader();
		while(reader.Read()) 
		{
			playerNames.Add(reader.GetString (0));
			playerLocations.Add (new Vector2 ((reader.GetInt32(1)), reader.GetInt32(2)));  
		}
		CleanUpSQLVariables ();

		numberOfPlayers = playerNames.Count; 
	}

	public void FetchInitialGridData()
	{
		//QUERY GRID DATA
		int gridX = 34;  //will be fetched from database later on
		int gridY = 14;  //will be fetched from database later on
		gridSize = new Vector2 (gridX, gridY);
	}

	public void FetchBuildingData()
	{
		//QUERY BUILDING ENTRANCE DATA
		//in each while loop iteration of reader.Read()
		string tempReader0 = "Library"; //temp for reader.GetString(0) = name
		string tempReader1 = "5,4"; //temp for reader.GetString(1) = entrance
		string tempReader2 = "5,5-6,5-5,6-6,6"; //temp for reader.GetString(2) = locations
		string tempReader3 = "90,0,45"; //temp for reader.GetString(3) = color
		string tempReader4 = "2"; //temp for reader.GetString(4) = height
		buildingData.Add (tempReader0 + ";" + tempReader1 + ";" + tempReader2 + ";" + tempReader3 + ";" + tempReader4);
	}

	public void FetchPlayerLocations()
	{
		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText =  string.Format ("SELECT xcoord, ycoord FROM ingameplayer WHERE roomid = '{0}' ORDER BY turnid ASC;", gameID);
		reader = dbcmd.ExecuteReader();
		int counter = 0;
		while(reader.Read()) 
		{
			playerLocations[counter] = (new Vector2 ((reader.GetInt32(0)), reader.GetInt32(1))); 
			counter++;
		}
		CleanUpSQLVariables ();
	}

	public void FetchCurrentPlayerTurn()
	{
		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText =  string.Format ("SELECT currentturn FROM ingameroom WHERE roomid = '{0}';", (int)gameID);
		reader = dbcmd.ExecuteReader();
		while(reader.Read()) 
			currentPlayerTurn = reader.GetInt32(0);
		CleanUpSQLVariables ();
	}

	public void IncrementPTurn()
	{
		currentPlayerTurn++;
		if (currentPlayerTurn == numberOfPlayers)
			currentPlayerTurn = 0;

		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText =  string.Format ("UPDATE ingameroom SET currentturn = '{0}' WHERE roomid = '{1}';", 
		                                    (int)currentPlayerTurn, (int)gameID);
		reader = dbcmd.ExecuteReader();
		CleanUpSQLVariables ();
	}

	public void MovePlayer(int playerCountID, Vector2 destination)
	{
		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText =  string.Format ("UPDATE ingameplayer SET xcoord = '{0}', ycoord = '{1}' WHERE turnid = '{2}';", 
		                                    (int)destination.x, (int)destination.y, playerCountID);
		reader = dbcmd.ExecuteReader();
		int counter = 0;
		while(reader.Read()) 
		{
			playerLocations[counter] = (new Vector2 ((reader.GetInt32(0)), reader.GetInt32(1))); 
			counter++;
		}
		CleanUpSQLVariables ();
	}

	public int GetCurrentPTurn()
	{
		return currentPlayerTurn;
	}

	public string GetPlayerName(int playerCountID)
	{
		return playerNames [playerCountID];
	}

	public Vector2 GetPlayerLocation(int playerCountID)
	{
		return playerLocations [playerCountID];
	}
	
	public Vector2 GetGridSize()
	{
		return gridSize;
	}

	public int GetNumberOfPlayers()
	{
		return numberOfPlayers;
	}

	public List<string> GetBuildingData()
	{
		return buildingData;
	}

	void CleanUpSQLVariables()
	{
		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
	}

	void OnApplicationQuit()
	{
		dbcon.Close();
		dbcon = null;
		Debug.Log ("Connection Closed");
	}
}
