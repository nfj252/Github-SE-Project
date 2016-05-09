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
	public List<string> playerNames;
	public List<Vector2> playerLocations;
	public List<int> pids;
	int currentPlayerTurn;
	int winnerPID;
	String winnerName;
	
	Vector2 gridSize;
	List<string> buildingInitializationData;
	List<string> taskInitializationData;

	NpgsqlConnection dbcon;
	NpgsqlCommand dbcmd;
	NpgsqlDataReader reader;
	
	void Start()
	{
		playerNames = new List<string>();
		playerLocations = new List<Vector2>();
		buildingInitializationData = new List<string>();
		taskInitializationData = new List<string>();
		pids = new List<int> ();
		winnerPID = -1;
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

	public void InsertRoomData(int turnID, List<string> insertNames, List<int> insertIDs)
	{
		numberOfPlayers = insertNames.Count; 
		playerNames = insertNames;
		pids = insertIDs;

		for(int i = 0; i < numberOfPlayers; i++)
		{
			playerLocations.Add(new Vector2(0,0));
		}



			dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText =  string.Format ("INSERT INTO ingameplayer (pid, ign, roomid, xcoord, ycoord, turnid) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');", 
			                                    pids[turnID], playerNames[turnID], gameID, playerLocations[turnID].x, playerLocations[turnID].y, turnID);
			reader = dbcmd.ExecuteReader();
			CleanUpSQLVariables ();

	}

	public void FetchInitialGridData()
	{
		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText =  string.Format ("Select * From gridData");
		reader = dbcmd.ExecuteReader();
		reader.Read ();
		gridSize = new Vector2 (reader.GetInt32(0), reader.GetInt32(1));
		CleanUpSQLVariables ();
	}

	public void FetchBuildingData()
	{
		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText =  string.Format ("SELECT name,entrance,lllocation,color,length,width,height FROM ingamebuilding;");
		reader = dbcmd.ExecuteReader();
		while(reader.Read()) 
		{
			buildingInitializationData.Add(reader.GetString(0) + ";" + reader.GetString(1) + ";" + reader.GetString(2) + ";" + 
			                               reader.GetString(3) + ";" + reader.GetString(4) + ";" + reader.GetString(5) + ";" + reader.GetString(6));
		}
		CleanUpSQLVariables ();
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

	public IEnumerator FetchPlayerLocationsCo()
	{
		FetchPlayerLocations ();
		yield return null;
	}

	public IEnumerator FetchCurrentPlayerTurnCo()
	{
		FetchCurrentPlayerTurn ();
		yield return null;
	}

	public IEnumerator FetchWinnderPID()
	{
		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText =  string.Format ("SELECT winner FROM ingameroom");
		reader = dbcmd.ExecuteReader();
		reader.Read (); 
		winnerPID = reader.GetInt32(0);
		CleanUpSQLVariables ();
		yield return null;
	}

	public void FetchTaskData()
	{
		taskInitializationData.Clear ();
		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText =  string.Format ("SELECT taskname, quantity, buildingname from ingametask");
		reader = dbcmd.ExecuteReader();
		while(reader.Read()) 
			taskInitializationData.Add(reader.GetString(0) + ";" + reader.GetInt32(1) + ";" + reader.GetString(2));
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

	public void DecrementBuildingTaskQuantity(string buildingTaskName)
	{
		dbcmd = dbcon.CreateCommand(); 
		dbcmd.CommandText = string.Format ("UPDATE ingametask SET quantity = quantity - 1 WHERE taskname = '{0}'", buildingTaskName);
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

	public void SetWinner(int localPlayerID)
	{
		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText =  string.Format ("UPDATE ingameroom SET winner = '{0}';", localPlayerID);
		reader = dbcmd.ExecuteReader();
		CleanUpSQLVariables ();
	}

	public int GetWinnerPID()
	{
		return winnerPID;
	}

	public string GetWinnerName()
	{
		return winnerName;
	}

	public void FetchWinnerPlayerName()
	{
		dbcmd = dbcon.CreateCommand();
		dbcmd.CommandText =  string.Format ("SELECT ign FROM users WHERE pid = '{0}';", winnerPID);
		reader = dbcmd.ExecuteReader();
		reader.Read ();
		winnerName = reader.GetString (0);
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
		return buildingInitializationData;
	}

	public List<string> GetTaskData()
	{
		return taskInitializationData;
	}

	public void SetGameID(int id)
	{
		gameID = id;
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
