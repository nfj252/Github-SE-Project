using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameflowController : MonoBehaviour 
{
	public GameObject playersParent;
	public GameObject[] playerPrefabs;
	public float timerRefreshRate;
	public float playerModelMoveSpeed;

	TileController tileController;
	BuildingController buildingController;
	InGameDBController inGameDBController;
	OrientationController orientationController;
	TaskController taskController;
	StartGameID introHolder;

	//Player Controller Stuff
	List<Player> players; 
	int localPlayerPID;
	int localPlayerTurnID;
	int remainingMoves;
	bool turnHasBegun;

	float timer;

	void Start () 
	{
		introHolder = FindObjectOfType<StartGameID> ();
		tileController = FindObjectOfType<TileController>();
		tileController.Setup();
		buildingController = FindObjectOfType<BuildingController> ();
		buildingController.Setup();
		inGameDBController = FindObjectOfType<InGameDBController>();
		orientationController = FindObjectOfType<OrientationController>();
		taskController = FindObjectOfType<TaskController>();
		taskController.Setup();
		inGameDBController.StartConnection();
		inGameDBController.FetchInitialGridData ();
		tileController.CreateGrid (inGameDBController.GetGridSize());
		inGameDBController.FetchBuildingData ();
		buildingController.CreateBuildings (inGameDBController.GetBuildingData());
		inGameDBController.SetGameID (introHolder.GetRoomID()); 
		inGameDBController.InsertRoomData (introHolder.GetTurnID() ,introHolder.getIGNList(), introHolder.getPIDList());
		inGameDBController.FetchCurrentPlayerTurn ();
		localPlayerPID = introHolder.GetPID(); 
		localPlayerTurnID = introHolder.GetTurnID ();
		//GameObject.Destroy (introHolder);

		players = new List<Player>();
		Debug.Log (inGameDBController.GetNumberOfPlayers ());
		CreatePlayers (inGameDBController.GetNumberOfPlayers ());
		inGameDBController.FetchTaskData ();
		taskController.AssignBuildingTasks (inGameDBController.GetTaskData(), buildingController.GetBuildings());
		taskController.AssignLocalPlayerTasks (inGameDBController.GetTaskData (), GetLocalPlayer ());
		taskController.InitializeLPTaskLabels (GetLocalPlayer().tasks);

		orientationController.ScaleUI ();
		tileController.SetLocalPlayerModelRef(players[localPlayerTurnID].playerModel); 

		SetInitialCameraPosition ();
		remainingMoves = 0;
		turnHasBegun = false;
		orientationController.SetMovesLabel ("Wait Your Turn");
		orientationController.SetRollDiceButtonStatus (false);
		orientationController.SetEnterBuildingButtonStatus (false);
		orientationController.SetEndTurnButtonStatus (false);
		tileController.SetCanLightUpTile (false);

		if(GetIsLocalPlayerTurn())
			BeginTurn();

		timer = 0;
	}

	void CreatePlayers(int numberOfPlayers)
	{
		for(int i = 0; i < numberOfPlayers; i++)
		{
			GameObject tempPlayerModel = Instantiate (playerPrefabs[i]);
			Player tempPlayer = tempPlayerModel.GetComponent<Player>();
			tempPlayer.smoothTime = playerModelMoveSpeed;
			tempPlayer.playerName = inGameDBController.GetPlayerName(i);
			tempPlayer.location = inGameDBController.GetPlayerLocation(i);
			tempPlayer.playerModel = tempPlayerModel;
			tempPlayer.Setup();
			tempPlayer.playerModel.transform.parent = playersParent.transform;
			tempPlayer.playerModel.name = tempPlayer.playerName;
			Vector3 tempPlayerModelPosition = tileController.GetTile((int)tempPlayer.location.x,(int)tempPlayer.location.y).transform.localPosition;
			tempPlayer.playerModel.transform.Translate(0,.25f,0);
			players.Add(tempPlayer);

			//if(tempPlayer.playerName == introSceneLeftOver)
			//	{
			//	localPlayerID = i;
			//	tileController.SetLocalPlayerModelRef(players[localPlayerID].playerModel);
			//	}
		}	
	}

	public void SetInitialCameraPosition()
	{
		Vector3 cameraPosition = tileController.ConvertLocationToPosition(GetLocalPlayer ().destinationLocation);
		cameraPosition += new Vector3 (0, Camera.main.transform.localPosition.y, 0);
		Camera.main.transform.localPosition = cameraPosition;
	}

	public int GetLocalPlayerTurnID()
	{
		Debug.Log ("LocalPlayerTurnID: " + localPlayerTurnID);
		return localPlayerTurnID;
	}

	public int GetLocalPlayerPID()
	{
		return localPlayerPID;
	}

	public bool GetIsLocalPlayerTurn()
	{
		return inGameDBController.GetCurrentPTurn () == localPlayerTurnID;
	}

	public bool GetIsLPOnBuildingEntrance()
	{
		return tileController.GetTile ((int)GetLocalPlayer ().location.x, (int)GetLocalPlayer ().location.y).tileType == "Entrance";		
	}

	public Player GetCurrentPlayer()
	{
		return players[inGameDBController.GetCurrentPTurn()];
	}

	public int GetRemainingMoves()
	{
		return remainingMoves;
	}

	public Player GetLocalPlayer()
	{
		return players [localPlayerTurnID];
	}

	public void SetRemainingMoves(int val)
	{
		remainingMoves = val;
		orientationController.SetMovesLabel (val.ToString ());
	}

	public void BeginTurn ()
	{
		turnHasBegun = true;
		GetLocalPlayer ().ResetDestinationLocation ();

		orientationController.SetMovesLabel ("Roll First");
		orientationController.SetRollDiceButtonStatus (true);

		if (GetIsLPOnBuildingEntrance())
		{
			taskController.SetVisitedBuilding(tileController.GetTile((int)GetLocalPlayer().location.x, (int)GetLocalPlayer().location.y).building);
			orientationController.SetEnterBuildingButtonStatus (true);
		}

		tileController.SetCanLightUpTile (true);
		SetInitialCameraPosition ();
	}

	public void RollDice()
	{
		orientationController.SetCanMovePlayer(false);
		remainingMoves = (int)Random.Range (1, 7000);
		orientationController.SetMovesLabel (remainingMoves.ToString());
		orientationController.SetRollDiceButtonStatus (false);
		orientationController.SetEndTurnButtonStatus (true);
		StartCoroutine (orientationController.DelayedSetCanMovePlayer (true));
	}

	public void EndTurn()
	{
		inGameDBController.IncrementPTurn ();
		turnHasBegun = false;
		orientationController.SetMovesLabel ("Wait Your Turn");
		orientationController.SetRollDiceButtonStatus (false);
		orientationController.SetEnterBuildingButtonStatus (false);
		orientationController.SetEndTurnButtonStatus (false);
		if(orientationController.buildingTasksPanel.alpha != 0)
			orientationController.ToggleBuildingTasksPanel ();
		tileController.SetCanLightUpTile (false);
	}

	public void CheckandSetIfLPWon()
	{
		for(int i = 0; i < GetLocalPlayer().tasks.Count; i++)
		{
			if(!GetLocalPlayer().tasks[i].completed)
				break;
			else
			{
				if(i == GetLocalPlayer().tasks.Count - 1)
				{
					inGameDBController.SetWinner(localPlayerPID);
				}
			}
		}
	}

	public void ReturnToIntro()
	{
		inGameDBController.DeleteInGamePlayerData ();
		Application.LoadLevel ("Intro");
	}

	void Update () 
	{
		if(inGameDBController.GetWinnerPID().Equals(-1))
		{
			timer += Time.deltaTime;
			if(timer >= timerRefreshRate)
			{
				timer = 0f;
				StartCoroutine(inGameDBController.FetchCurrentPlayerTurnCo());
				StartCoroutine(inGameDBController.FetchPlayerLocationsCo());
				StartCoroutine(inGameDBController.FetchWinnderPID());

				for(int i = 0; i < players.Count; i++)
				{
					players[i].location = inGameDBController.GetPlayerLocation(i);
					players[i].MoveModel(inGameDBController.GetPlayerLocation(i));
				}

				if(GetIsLocalPlayerTurn())
				{
					if (!turnHasBegun) 
						BeginTurn();

					if(GetIsLPOnBuildingEntrance())
						StartCoroutine(taskController.UpdateBuildingTaskQuantityLabels(inGameDBController.GetTaskData()));
				}
			}
		}
		else
		{
			inGameDBController.FetchWinnerPlayerName();
			orientationController.SetWinnerLabel(inGameDBController.GetWinnerName());
			orientationController.ShowWinnerDisplayPanel();
		}
	}
}
