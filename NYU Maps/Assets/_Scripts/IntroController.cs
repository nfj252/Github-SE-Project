using UnityEngine;
using System.Collections;
using System;
using System.Data;
using Npgsql;
using UnityEngine.EventSystems;



public class IntroController : MonoBehaviour {
	public GameObject loginContainer;
	public GameObject lobbyContainer;
	public GameObject registerContainer;
	public GameObject prelobbyContainer;
	public GameObject createRoomContainer;
	public GameObject currentRoomContainer;
	public GameObject startGameID;

	public UITexture loginTexture;
	public UITexture lobbyTexture;
	public UITexture registerTexture;
	public UITexture prelobbyTexture;
	public UITexture createRoomTexture;


	public UIPanel loginPanel;
	public UIInput loginUserInput;
	public UIInput loginPasswordInput;

	public UIPanel registerPanel;
	public UIInput registerUserInput;
	public UIInput registerPasswordInput;
	public UIInput registerPasswordReconfirmInput;

	public UIPanel prelobbyPanel;
	public UIInput preLobbyIGNInput;
	public UILabel preLobbyIGNLabel;
	public UIButton joinRoomButton;
	public UIButton createRoomButton;

	public UIPanel lobbyPanel;
	public GameObject[] rooms;
	public UILabel[] nameLabels;
	public UILabel[] numPlayersLabels;
	public UILabel[] roomNumLabels;

	public UIPanel createRoomPanel;
	public UIInput createRoomNameInput;
	public UIButton createRoomCreateButton;
	public UIButton createRoomCancelButton;

	public UIPanel currentRoomPanel;
	public UITexture currentRoomTexture;
	public UIButton startGameButton;
	public UIButton leaveRoomButton;
	public UILabel[] currentRoomPlayerNameLabels;

	public NpgsqlConnection dbcon;
	public StartGameID startGameController;
	private string username;
	private string password;
	private string ign;
	private int pid;
	private int currentRoomID;
	private float timer;
	private bool isHost;
	private bool keepRefreshing;
	private int turnNum;
	private int numPlayers;
	public float timeTilRefresh;
	
	// Use this for initialization
	void Start () 
	{
		timer = 0;
		isHost = false;	
		pid = -1;
		currentRoomID = -1;
		turnNum = 0;
		numPlayers = -1;
		keepRefreshing = true;

		float heightContainerScaler = (float)(Screen.height+4)/(float)loginTexture.height;	
		loginContainer.transform.localScale = new Vector3(heightContainerScaler, heightContainerScaler, 1f);
		lobbyContainer.transform.localScale = new Vector3(heightContainerScaler, heightContainerScaler, 1f);
		registerContainer.transform.localScale = new Vector3(heightContainerScaler, heightContainerScaler, 1f);
		prelobbyContainer.transform.localScale = new Vector3(heightContainerScaler, heightContainerScaler, 1f);
		currentRoomContainer.transform.localScale = new Vector3(heightContainerScaler, heightContainerScaler, 1f);
		createRoomContainer.transform.localScale = new Vector3(heightContainerScaler, heightContainerScaler, 1f);
		
		joinRoomButton.isEnabled = false;
		joinRoomButton.UpdateColor(true);
		createRoomButton.isEnabled = false;
		createRoomButton.UpdateColor(true);
		createRoomCreateButton.isEnabled = false;
		createRoomButton.UpdateColor(true);
//Database Connection===================================================================================================================================
		string connectionString =
			"Server= pdc-amd01.poly.edu;" +
				"Database=kl1983;" +
				"User ID=kl1983;" +
				"Password=z86udpx%;";

		dbcon = new NpgsqlConnection(connectionString);
		dbcon.Open();
		Debug.Log ("Connected");
		NpgsqlCommand dbcmd = dbcon.CreateCommand();


	}
	void OnDestroy() {
		Debug.Log("Killing intro connection");
		if (dbcon != null) {
			if (dbcon.State.ToString() != "Closed") {
				dbcon.Close();
				Debug.Log ("Successfully closed db connection");
			}
			dbcon.Dispose();
		}
	}
	void OnApplicationQuit()
	{
		if (currentRoomID > 0) {
			LeaveRoom();
		}
	}
	void Update () {
		timer += Time.deltaTime;
		/*if(timer >= timeTilRefresh)
		{
			timer = 0f;
			//StartCoroutine(ConsistentRefresh());
			Refresh();
		}*/
		if(keepRefreshing)
			Refresh();
		//StartCoroutine(consistentRefresh());
	}

	public bool gameHasStarted()
	{
		//Check if user exists
		string selectSQL = string.Format ("SELECT game_is_ready,roomid FROM  playerroom WHERE game_is_ready = TRUE AND roomid = '{0}' ", currentRoomID);
		NpgsqlCommand dbcmd = new NpgsqlCommand (selectSQL, dbcon);
		NpgsqlDataReader reader = dbcmd.ExecuteReader();
		if (reader.HasRows) {
			if(reader.Read()){
				reader.Close();
				reader = null;
				dbcmd.Dispose();
				return true;
			}
		}
		reader.Close();
		reader = null;
		dbcmd.Dispose();
		return false;
	}
	public void GetSubmittedUsername()
	{
		username = loginUserInput.value; 
		Debug.Log (username);
	}
//Login with email/password==============================================================================================================================
	public void LoginUser ()
	{
		username = loginUserInput.value; 
		Debug.Log (username);
		password = loginPasswordInput.value; 
		Debug.Log (password);

		//Check if user exists
		string selectSQL = string.Format ("SELECT pid, ign FROM users WHERE email = '{0}' AND password = '{1}' ", username, password);
		NpgsqlCommand dbcmd = new NpgsqlCommand (selectSQL, dbcon);
		NpgsqlDataReader reader = dbcmd.ExecuteReader();
		//Login success
		if (reader.HasRows) {

			if (reader.Read()){
				pid = (int)reader["pid"];
				if(reader["ign"].ToString().Length > 0)
					ign = (string)reader["ign"];
				else{
					ign = string.Empty;
				}
				Debug.Log("THEIGN"+ ign);
			}
			Debug.Log ("Successfully logged in");
			HideLoginPanel();
			ShowPrelobbyPanel();
		} else {
			Debug.Log ("Login Failed");
		}
		reader.Close ();
		reader = null;
		dbcmd.Dispose ();
	}
//Register with username/password=======================================================================================================================
	public void RegisterUser ()
	{
		string username, password, passwordReconfirm;
		string insertSQL, selectSQL;
		NpgsqlCommand dbcmd;
		username = registerUserInput.value;
		password = registerPasswordInput.value;
		passwordReconfirm = registerPasswordReconfirmInput.value;

	//Check if username is already taken
		selectSQL = string.Format ("SELECT email FROM users WHERE email = '{0}' ", username);
		dbcmd = new NpgsqlCommand (selectSQL, dbcon);
		NpgsqlDataReader reader = dbcmd.ExecuteReader();
		if (reader.HasRows) {
			Debug.Log ("This username is already taken");
			return;
		}
		reader.Close ();
		reader = null;
		dbcmd.Dispose ();
	//Check all fields are valid & Insert values to database
		if (password != passwordReconfirm) {
			Debug.Log ("Please reconfirm your password");
		} else if (username != String.Empty && (password == passwordReconfirm) ) {

		//Insert username password to database
			insertSQL = string.Format("INSERT INTO users(email, password) " +
									  "VALUES('{0}', '{1}') ;", 
			                          username, password);
			dbcmd = new NpgsqlCommand (insertSQL, dbcon);
			dbcmd.ExecuteNonQuery();
			dbcmd.Dispose();
		//Show Login Panel
			HideRegisterPanel();
			ShowLoginPanel();
			Debug.Log ("Register success");
		}
		Debug.Log ("Username:" + username);
		Debug.Log ("Password:" + password);
		Debug.Log ("Password Reconfirm:" + passwordReconfirm);
	}
	public void Refresh()
	{
		string deleteSQL;
		NpgsqlCommand dbcmd;
		//Debug.Log (currentRoomID);
		if (currentRoomID <= 0) {
			ShowRooms ();
			//StartCoroutine(ShowRoomsCo);
		}
		else {
			DisplayCurrentRoomPlayers ();
			//StartCoroutine(DisplayCurrentRoomPlayersCo);
			if(isHost)
				startButtonUpdate ();
			//StartCoroutine(StartButtonUpdateCo);
			/*else{
				if(currentRoomPlayerNameLabels[0].text == string.Empty){
					Debug.Log ("Host left");
					currentRoomID = -1;
					HideCurrentGameRoomPanel();
					ShowLobbyPanel();
				}
			}*/

			if(gameHasStarted())
			{
				fillLists();
				if(isHost){
					deleteSQL = string.Format("DELETE FROM inroomstatus WHERE roomid='{0}'", currentRoomID);
					dbcmd = new NpgsqlCommand (deleteSQL, dbcon);
					dbcmd.ExecuteNonQuery();
					dbcmd.Dispose();
				}
				Debug.Log("Killing intro connection");
				if (dbcon != null) {
					if (dbcon.State.ToString() != "Closed") {
						dbcon.Close();
						Debug.Log ("Successfully closed db connection");
					}
					dbcon.Dispose();
				}
				keepRefreshing = false;
				Application.LoadLevel("Main");
			}
		}
	}
	IEnumerator ConsistentRefresh()
	{
		yield return new WaitForEndOfFrame();
		Debug.Log (currentRoomID);
		if (currentRoomID <= 0) {
			ShowRooms ();
			//StartCoroutine(ShowRoomsCo);
		}
		else {
			DisplayCurrentRoomPlayers ();
			//StartCoroutine(DisplayCurrentRoomPlayersCo);
			if(isHost)
				startButtonUpdate ();
				//StartCoroutine(StartButtonUpdateCo);
			/*else{
				if(currentRoomPlayerNameLabels[0].text == string.Empty){
					Debug.Log ("Host left");
					currentRoomID = -1;
					HideCurrentGameRoomPanel();
					ShowLobbyPanel();
				}
			}*/
		}
		yield return null;
	}
	IEnumerator DisplayCurrentRoomPlayersCo()
	{
		yield return new WaitForEndOfFrame();
		Debug.Log ("DisplayCurrentRoomPlayers() w/ roomid" + currentRoomID);
		int rowsAffected = 0;
		NpgsqlCommand dbcmd;
		string selectSQL = string.Format ("SELECT ign FROM inroomstatus WHERE roomid = '{0}' LIMIT 4", currentRoomID);
		dbcmd = new NpgsqlCommand (selectSQL, dbcon);
		NpgsqlDataReader reader = dbcmd.ExecuteReader();
		for (int i=0; i<currentRoomPlayerNameLabels.Length; i++) {
			currentRoomPlayerNameLabels[i].text = string.Empty;
		}
		if (reader.HasRows) {
			while(reader.Read())
			{
				currentRoomPlayerNameLabels[rowsAffected].text = (string)reader["ign"];
				rowsAffected++;
			}
		}
		reader.Close ();
		reader = null;
		dbcmd.Dispose ();
		yield return null;
	}
	public void DisplayCurrentRoomPlayers()
	{
		//Debug.Log ("DisplayCurrentRoomPlayers() w/ roomid" + currentRoomID);
		int rowsAffected = 0;
		NpgsqlCommand dbcmd;
		string selectSQL = string.Format ("SELECT ign FROM inroomstatus WHERE roomid = '{0}' ORDER BY turnid ASC LIMIT 4", currentRoomID);
		dbcmd = new NpgsqlCommand (selectSQL, dbcon);
		NpgsqlDataReader reader = dbcmd.ExecuteReader();
		for (int i=0; i<currentRoomPlayerNameLabels.Length; i++) {
			currentRoomPlayerNameLabels[i].text = string.Empty;
		}
		if (reader.HasRows) {
			while(reader.Read())
			{
				currentRoomPlayerNameLabels[rowsAffected].text = (string)reader["ign"];
				rowsAffected++;
			}
		}
		reader.Close ();
		reader = null;
		dbcmd.Dispose ();
	}

	public void ShowRooms()
	{
		string selectSQL = string.Empty;
		string roomName = string.Empty;
		int roomID = -1;
		int rowsAffected = 0;
		int roomCount = 0;
		NpgsqlCommand dbcmd;
		selectSQL = string.Format ("SELECT roomname,roomcount, roomid FROM playerroom WHERE game_is_ready = FALSE ORDER BY roomid DESC LIMIT 6");
		dbcmd = new NpgsqlCommand (selectSQL, dbcon);
		NpgsqlDataReader reader = dbcmd.ExecuteReader();
		for(int i =0; i<rooms.Length; i++)
		{
			rooms[i].GetComponent<UIButton>().isEnabled = false;
			rooms[i].GetComponent<UIButton>().UpdateColor(true);
			nameLabels[i].text = "Room name";
			numPlayersLabels[i].text = "0/4";
			roomNumLabels[i].text = i + string.Empty;
		}
		if (reader.HasRows) {
			while (reader.Read()) {
				//Debug.Log ((string)reader["roomname"]);
				roomName = (string)reader ["roomname"];
				roomCount = (int)reader ["roomcount"];
				roomID = (int)reader ["roomid"];
				//Debug.Log (roomName);
				nameLabels [rowsAffected].text = roomName;
				numPlayersLabels [rowsAffected].text = roomCount + "/4";
				roomNumLabels [rowsAffected].text = roomID.ToString ();
				rooms [rowsAffected].GetComponent<RoomID> ().setRoomID (roomID);
				rooms [rowsAffected].GetComponent<RoomID> ().setRoomCount (roomCount);
				if (roomCount > 0 && roomCount < 4) {
					rooms [rowsAffected].GetComponent<UIButton> ().isEnabled = true;
					rooms [rowsAffected].GetComponent<UIButton> ().UpdateColor (true);

				}
				rowsAffected++;
				roomCount = 0;
			}
		}
		reader.Close ();
		reader = null;
		dbcmd.Dispose ();
	}
	public void PrelobbyUpdatePlayerName()
	{
		Debug.Log ("Value of ign at PRELOBBYUPDATE" + ign);
		if (ign != string.Empty && ign.Length > 4) {
			preLobbyIGNLabel.text = ign;
			joinRoomButton.isEnabled = true;
			joinRoomButton.UpdateColor(true);
			createRoomButton.isEnabled = true;
			createRoomButton.UpdateColor(true);
		}
	}
//Ask Nick:
	//How does name change work 
	//How to join room: double click the room? Or select and then press join button. How does it recognize the click of the room
	//My guess: make the tiles buttons. On click retrieve the value of room number

//Join an open room
//Room ID, IGNs, 
	public void JoinRoom (int roomid)
	{
		startGameButton.isEnabled = false;
		startGameButton.UpdateColor (true);
		setCurrentRoomID (roomid);
		NpgsqlCommand dbcmd;
		string selectSQL = string.Format("SELECT roomcount FROM playerroom WHERE roomid = '{0}';", currentRoomID);
		dbcmd = new NpgsqlCommand (selectSQL, dbcon);
		NpgsqlDataReader reader = dbcmd.ExecuteReader();

		if (reader.HasRows) {
			if(reader.Read()){
				turnNum = ((int)reader ["roomcount"]);
				Debug.Log ("turnNum" + turnNum);
			}
		}
		reader.Close ();
		reader = null;
		dbcmd.Dispose ();
		string insertSQL = string.Format("INSERT INTO inroomstatus(pid, ign, roomid, turnid) " +
		                                 "VALUES('{0}', '{1}', '{2}', '{3}') ;", 
		                          pid, ign, currentRoomID, turnNum );
		dbcmd = new NpgsqlCommand (insertSQL, dbcon);
		dbcmd.ExecuteNonQuery();
		dbcmd.Dispose();

		string updateSQL = string.Format("UPDATE playerroom SET roomcount = roomcount+1 WHERE roomid = '{0}';", currentRoomID);
		dbcmd = new NpgsqlCommand (updateSQL, dbcon);


		//dbcmd = new NpgsqlCommand (insertSQL, dbcon);
		dbcmd.ExecuteNonQuery();
		dbcmd.Dispose();
		startGameController.setTurnID (turnNum);
		startGameController.setRoomID (currentRoomID);
		startGameController.setPID (pid);
		//Insert into inroom status
		HideLobbyPanel ();
		ShowCurrentGameRoomPanel ();

	}

//Create a room
	public void CreateRoom()
	{
		createRoomCreateButton.isEnabled = false;
		createRoomCreateButton.UpdateColor (true);
		NpgsqlCommand dbcmd;
		string roomName = String.Empty;
		roomName = createRoomNameInput.value;
		string insertSQL = string.Format("INSERT INTO playerroom(roomcount, game_is_ready, host_pid, roomname) " +
		                                 "VALUES('{0}', '{1}', '{2}', '{3}') ;", 
		                                 1, false, pid, roomName);
		dbcmd = new NpgsqlCommand (insertSQL, dbcon);
		dbcmd.ExecuteNonQuery();
		dbcmd.Dispose();

		updateCurrentRoomID ();
		//StartCoroutine( updateCurrentRoomID ());
		Debug.Log ("Created RoomID" + currentRoomID);
		//Insert player info to inroomstatus
		insertSQL = string.Format("INSERT INTO inroomstatus(pid, ign, roomid, turnid) " +
		                          "VALUES('{0}', '{1}', '{2}', '{3}') ;", 
		                                 pid, ign, currentRoomID, 0 );
		dbcmd = new NpgsqlCommand (insertSQL, dbcon);
		dbcmd.ExecuteNonQuery();
		ShowCurrentGameRoomPanel ();
		HideCreateRoomPanel ();
		turnNum = 0;
		isHost = true;
	}
	public void setCurrentRoomID(int newRoomID)
	{
		currentRoomID = newRoomID;
	}
	public void updateCurrentRoomID()
	{
		NpgsqlCommand dbcmd;
		string selectSQL = string.Format ("SELECT roomid FROM playerroom WHERE host_pid = '{0}' ORDER BY roomid DESC ", pid);
		dbcmd = new NpgsqlCommand (selectSQL, dbcon);
		NpgsqlDataReader reader = dbcmd.ExecuteReader();
		if (reader.HasRows) {
			reader.Read ();
			currentRoomID = (int)reader ["roomid"];
		}
		reader.Close();
		reader = null;
		dbcmd.Dispose ();

	}
	IEnumerator updateCurrentRoomID2()
	{
		currentRoomID = -1;
		NpgsqlCommand dbcmd;
		string selectSQL = string.Format ("SELECT roomid FROM inroomstatus WHERE pid = '{0}' ORDER BY roomid DESC ", pid);
		dbcmd = new NpgsqlCommand (selectSQL, dbcon);
		NpgsqlDataReader reader = dbcmd.ExecuteReader();
		if (reader.HasRows) {
			reader.Read ();
			currentRoomID = (int)reader ["roomid"];
		}
		reader.Close();
		reader = null;
		dbcmd.Dispose ();
		yield return null;
	}
//Change your IGN==============================================================================================================================
	public void ChangeIGN ()
	{
		NpgsqlCommand dbcmd;
	//Select statement
		string selectSQL = string.Format ("SELECT ign FROM users WHERE email != '{0}' AND ign = '{1}' ", username, preLobbyIGNInput.value);
		dbcmd = new NpgsqlCommand (selectSQL, dbcon);
		NpgsqlDataReader reader = dbcmd.ExecuteReader();

		if (reader.HasRows) {
			Debug.Log ("The ign is already in use by another user");
			preLobbyIGNInput.value = "what a noob";
			reader.Close ();
			reader = null;
			dbcmd.Dispose ();
			return;
		}
		reader.Close ();
		reader = null;
		dbcmd.Dispose ();
	//Run update statement
		ign = preLobbyIGNLabel.text;
		string sql = string.Format("UPDATE users SET ign = '{0}' WHERE email = '{1}';", ign, username);
		dbcmd = new NpgsqlCommand (sql, dbcon);
		dbcmd.ExecuteNonQuery();
		dbcmd.Dispose();

		
	}
	public void ChangeIGNJoinRoom ()
	{
		NpgsqlCommand dbcmd;
		//Select statement
		string selectSQL = string.Format ("SELECT ign FROM users WHERE email != '{0}' AND ign = '{1}' ", username, preLobbyIGNInput.value);
		dbcmd = new NpgsqlCommand (selectSQL, dbcon);
		NpgsqlDataReader reader = dbcmd.ExecuteReader();

		if (reader.HasRows) {
			Debug.Log ("The ign is already in use by another user");
			preLobbyIGNInput.value = String.Empty;
			reader.Close ();
			reader = null;
			dbcmd.Dispose ();
			return;
		}

		reader.Close ();
		reader = null;
		dbcmd.Dispose ();
		//Run update statement
		Debug.Log("PrelobbyIgnInput value:" + preLobbyIGNInput.value + "\nPrelobbyIGNLABEL VALUE:" + preLobbyIGNLabel.text);
		ign = preLobbyIGNLabel.text;
		string sql = string.Format("UPDATE users SET ign = '{0}' WHERE email = '{1}';", ign, username);
		dbcmd = new NpgsqlCommand (sql, dbcon);
		dbcmd.ExecuteNonQuery();
		dbcmd.Dispose();
		ShowLobbyPanel ();
	}
//Check IGN length
	public void CheckIGNValid()
	{
		if (preLobbyIGNInput.value.Length >= 4) {
			joinRoomButton.isEnabled = true;
			joinRoomButton.UpdateColor (true);
			createRoomButton.isEnabled = true;
			createRoomButton.UpdateColor (true);
		} else {
			joinRoomButton.isEnabled = false;
			joinRoomButton.UpdateColor (true);
			createRoomButton.isEnabled = false;
			createRoomButton.UpdateColor (true);
		}
	}
//Check room name length
	public void CheckRoomName()
	{
		if (createRoomNameInput.value.Length > 0) {
			createRoomCreateButton.isEnabled = true;
			createRoomCreateButton.UpdateColor (true);

		} else {
			createRoomCreateButton.isEnabled = false;
			createRoomCreateButton.UpdateColor (false);
		}
	}

	public void LeaveRoom()
	{
		NpgsqlCommand dbcmd;
		string deleteSQL, updateSQL;
		Debug.Log ("isHost:" + isHost);
		if (isHost) {
			deleteSQL = string.Format ("DELETE FROM playerroom WHERE roomid = '{0}';", currentRoomID);
			dbcmd = new NpgsqlCommand (deleteSQL, dbcon);
			dbcmd.ExecuteNonQuery ();
			dbcmd.Dispose ();
			/*
			deleteSQL = string.Format ("DELETE FROM inroomstatus WHERE roomid = '{0}';", currentRoomID);
			dbcmd = new NpgsqlCommand (deleteSQL, dbcon);
			dbcmd.ExecuteNonQuery ();
			dbcmd.Dispose ();
			*/
		} else {
			updateSQL = string.Format("UPDATE playerroom SET roomcount = roomcount-1 WHERE roomid = '{0}';", currentRoomID);
			dbcmd = new NpgsqlCommand (updateSQL, dbcon);
			dbcmd.ExecuteNonQuery();
			dbcmd.Dispose();

			deleteSQL = string.Format ("DELETE FROM inroomstatus WHERE pid = '{0}'  ;", pid);
			dbcmd = new NpgsqlCommand (deleteSQL, dbcon);
			dbcmd.ExecuteNonQuery ();
			dbcmd.Dispose ();

		}
		currentRoomID = -1;
		turnNum = 0;
		startGameController.resetIDs ();
		isHost = false;
		Debug.Log ("Left the room");
		HideCurrentGameRoomPanel ();
		ShowLobbyPanel ();
		
	}

	public void startButtonUpdate()
	{
		Debug.Log("StartButtonUpdate()");
		startGameButton.isEnabled = false;
		startGameButton.UpdateColor(true);
		NpgsqlCommand dbcmd;
		string updateSQL, selectSQL;
		int roomCount = 0;
		if (isHost) {
			selectSQL = string.Format ("SELECT roomcount FROM playerroom WHERE roomid = '{0}';", currentRoomID);
			dbcmd = new NpgsqlCommand (selectSQL, dbcon);
			NpgsqlDataReader reader = dbcmd.ExecuteReader ();
			if (reader.HasRows) {
				if (reader.Read ()) {
					roomCount = (int)reader ["roomcount"];
				}
			}
			reader.Close ();
			reader = null;
			dbcmd.Dispose ();
			if (roomCount >= 2) {
				startGameButton.isEnabled = true;
				startGameButton.UpdateColor (true);
			}
		} 
	}
	public void fillLists()
	{
		NpgsqlCommand dbcmd;
		string updateSQL, selectSQL, insertSQL, deleteSQL;
		int roomCount, rowsAffected, thepid;
		string theign;

		selectSQL = string.Format ("SELECT pid, ign FROM inroomstatus WHERE roomid = '{0}' LIMIT 4;", currentRoomID);
		dbcmd = new NpgsqlCommand (selectSQL, dbcon);
		NpgsqlDataReader reader = dbcmd.ExecuteReader ();
		while (reader.HasRows)
		{
			if(reader.Read()){
				theign = reader["ign"].ToString();
				thepid = (int)(reader.GetInt64(0));
				startGameController.addIGN(theign);
				startGameController.addPID(thepid);
				
			}
		}
		reader.Close();
		reader = null;
		dbcmd.Dispose();

	}
	public void StartGame()
	{
		NpgsqlCommand dbcmd;
		string updateSQL, selectSQL, insertSQL, deleteSQL;
		int roomCount, rowsAffected, thepid;
		string theign;

		if (isHost) {
			updateSQL = string.Format ("UPDATE playerroom SET game_is_ready = TRUE WHERE roomid = '{0}';", currentRoomID);
			dbcmd = new NpgsqlCommand (updateSQL, dbcon);
		
		
			//dbcmd = new NpgsqlCommand (insertSQL, dbcon);
			dbcmd.ExecuteNonQuery ();
			dbcmd.Dispose ();

			insertSQL = string.Format("INSERT INTO ingameroom " +
			                          "VALUES('{0}', '{1}', {2}) ;", 
			                          0, currentRoomID, -1);
			dbcmd = new NpgsqlCommand (insertSQL, dbcon);
			dbcmd.ExecuteNonQuery();
			dbcmd.Dispose();

			fillLists();

		

			startGameController.setTurnID (0);
			startGameController.setRoomID (currentRoomID);
			startGameController.setPID (pid);
			Debug.Log ("Started game");
		}

	}
//Login panel controls===================================================================================================================================
	public void HideLoginPanel()
	{
		loginPanel.GetComponent<TweenAlpha>().PlayForward ();
	}
	public void ShowLoginPanel()
	{
		loginPanel.GetComponent<TweenAlpha>().PlayReverse ();
	}
	public void ShowLobbyPanel()
	{
		lobbyPanel.GetComponent<TweenAlpha>().PlayForward ();
		ShowRooms ();
	}
	public void ShowPrelobbyPanel()
	{
		prelobbyPanel.GetComponent<TweenAlpha>().PlayForward ();
		PrelobbyUpdatePlayerName ();
	}
	public void ShowRegisterPanel()
	{
		registerPanel.GetComponent<TweenAlpha>().PlayForward ();
	}
	public void ShowCreateRoomPanel()
	{
		createRoomPanel.GetComponent<TweenAlpha>().PlayForward ();
		createRoomCreateButton.isEnabled = true;
		createRoomCreateButton.UpdateColor (true);
	}
	public void ShowCurrentGameRoomPanel()
	{
		currentRoomPanel.GetComponent<TweenAlpha>().PlayForward ();	
	}
	public void HideRegisterPanel()
	{
		registerPanel.GetComponent<TweenAlpha>().PlayReverse ();
	}
	public void HidePrelobbyPanel()
	{
		prelobbyPanel.GetComponent<TweenAlpha>().PlayReverse ();
		
	}
	public void HideLobbyPanel()
	{
		lobbyPanel.GetComponent<TweenAlpha>().PlayReverse ();
	}
	public void HideCurrentGameRoomPanel()
	{
		currentRoomPanel.GetComponent<TweenAlpha>().PlayReverse ();	
	}
	public void HideCreateRoomPanel()
	{
		createRoomPanel.GetComponent<TweenAlpha>().PlayReverse ();
		
	}
}

