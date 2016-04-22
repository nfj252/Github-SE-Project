using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameflowController : MonoBehaviour 
{
	//public GameObject introSceneLeftover;  //will hold user localname from intro scene
	public GameObject playersParent;
	public GameObject playerPrefab;
	public float timerRefreshRate;
	public float playerModelMoveSpeed;

	TileController tileController;
	BuildingController buildingController;
	InGameDBController inGameDBController;
	OrientationController orientationController;
	public List<Player> players;
	int localPlayerID;
	int remainingMoves;
	bool turnHasBegun;
	bool canMovePlayer;
	float timer;

	void Start () 
	{
		tileController = FindObjectOfType<TileController>();
		tileController.Setup();
		buildingController = FindObjectOfType<BuildingController> ();
		buildingController.Setup();
		inGameDBController = FindObjectOfType<InGameDBController>();
		orientationController = FindObjectOfType<OrientationController>();

		inGameDBController.StartConnection();
		inGameDBController.FetchInitialGridData ();
		tileController.CreateGrid (inGameDBController.GetGridSize());
		inGameDBController.FetchBuildingData ();
		buildingController.CreateBuildings (inGameDBController.GetBuildingData());
		inGameDBController.FetchRoomData ();
		inGameDBController.FetchCurrentPlayerTurn ();

		players = new List<Player>();
		CreatePlayers (inGameDBController.GetNumberOfPlayers ());

		localPlayerID = 0; ///////////////////temp
		tileController.SetLocalPlayerModelRef(players[localPlayerID].playerModel); ///////////////////temp
		remainingMoves = 0;

		SetInitialCameraPosition ();
		canMovePlayer = false;
		turnHasBegun = false;


		if(GetIsLocalPlayerTurn())
			BeginTurn();
		timer = 0;
	}

	void CreatePlayers(int numberOfPlayers)
	{
		for(int i = 0; i < numberOfPlayers; i++)
		{
			GameObject tempPlayerModel = Instantiate (playerPrefab);
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
		Vector3 cameraPosition = GetLocalPlayer ().playerModel.transform.localPosition;
		cameraPosition += new Vector3 (0, Camera.main.transform.localPosition.y - GetLocalPlayer().playerModel.transform.localPosition.y, 0);
		Camera.main.transform.localPosition = cameraPosition;
	}

	public int GetLocalPlayerID()
	{
		Debug.Log ("LocalPlayerID: " + localPlayerID);
		return localPlayerID;
	}

	public bool GetCanMovePlayer()
	{
		return canMovePlayer;
	}

	public bool GetIsLocalPlayerTurn()
	{
		if (inGameDBController.GetCurrentPTurn () == localPlayerID)
			return true;
		else
			return false;
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
		return players [localPlayerID];
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

		//update GUI and tapping - enable
		orientationController.SetMovesLabel ("Roll First");
		orientationController.SetRollDiceButtonStatus (true);
		orientationController.SetEndTurnButtonStatus (true);

		tileController.SetCanLightUpTile (true);
		SetInitialCameraPosition ();
	}

	public void RollDice()
	{
		canMovePlayer = false;
		remainingMoves = (int)Random.Range (1, 7);
		orientationController.SetMovesLabel (remainingMoves.ToString());
		orientationController.SetRollDiceButtonStatus (false);
		StartCoroutine (DelayedSetCanMovePlayer (true));
	}

	public void EndTurn()
	{
		inGameDBController.IncrementPTurn ();
		turnHasBegun = false;
		//update GUI and tapping - disable
		orientationController.SetMovesLabel ("Wait Nub");
		orientationController.SetEndTurnButtonStatus (false);
		tileController.SetCanLightUpTile (false);
	}
	
	IEnumerator DelayedSetCanMovePlayer(bool val)
	{
		yield return new WaitForEndOfFrame ();
		yield return new WaitForSeconds (.1f);
		canMovePlayer = true;
		yield return null;
	}

	void Update () 
	{
		timer += Time.deltaTime;
		if(timer >= timerRefreshRate)
		{
			timer = 0f;
			inGameDBController.FetchCurrentPlayerTurn();
			inGameDBController.FetchPlayerLocations();
			for(int i = 0; i < players.Count; i++)
			{
				players[i].location = inGameDBController.GetPlayerLocation(i);
				players[i].MoveModel(inGameDBController.GetPlayerLocation(i));
			}

			if(!turnHasBegun && GetIsLocalPlayerTurn())
			{
				BeginTurn();
			}
		}
	}
}
