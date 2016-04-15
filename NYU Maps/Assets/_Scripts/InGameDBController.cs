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
	NpgsqlConnection dbcon;
	NpgsqlCommand dbcmd;
	NpgsqlDataReader reader;

	void Start()
	{
		playerNames = new List<string>();
		playerLocations = new List<Vector2>();
		currentPlayerTurn = 0;
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
		//QUERY PLAYER TURN
		//currentPlayerTurn = result
	}

	public void IncrementPTurn()
	{
		currentPlayerTurn++;
		if (currentPlayerTurn >= numberOfPlayers)
			currentPlayerTurn = 0;
		Debug.Log ("CurrentPlayerTurn: " + currentPlayerTurn);
		//QUERY INSERT CURRENT PLAYER TURN
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
