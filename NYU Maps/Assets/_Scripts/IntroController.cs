using UnityEngine;
using System.Collections;
using System;
using System.Data;
using Npgsql;



public class IntroController : MonoBehaviour {
	public GameObject loginContainer;
	public GameObject lobbyContainer;
	public UITexture loginTexture;
	public UITexture lobbyTexture;
	public UIInput loginInput;
	public UIPanel loginPanel;
	public UIPanel lobbyPanel;
	private string username;
	// Use this for initialization
	void Start () 
	{
		float heightContainerScaler = (float)(Screen.height+4)/(float)loginTexture.height;	
		loginContainer.transform.localScale = new Vector3(heightContainerScaler, heightContainerScaler, 1f);
		lobbyContainer.transform.localScale = new Vector3(heightContainerScaler, heightContainerScaler, 1f);
		string connectionString =
			"Server= pdc-amd01.poly.edu;" +
				"Database=kl1983;" +
				"User ID=kl1983;" +
				"Password=z86udpx%;";
		
		NpgsqlConnection dbcon;
		//UnityNpgsqlTypes dbcon;
		dbcon = new NpgsqlConnection(connectionString);
		dbcon.Open();
		Debug.Log ("Connected");
		NpgsqlCommand dbcmd = dbcon.CreateCommand();
		
		string sql =
			"SELECT email, pid " +
				"FROM users";
		dbcmd.CommandText = sql;
		NpgsqlDataReader reader = dbcmd.ExecuteReader();
		while(reader.Read()) {
			string FirstName = (string) reader["email"];
			int LastName = (int) reader["lastname"];
			Console.WriteLine("Name: " +
			                  FirstName + " " + LastName);
			Debug.Log (FirstName);
		}
		// clean up
		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		dbcon.Close();
		dbcon = null;

	}

	void Update () {

	}
	public void GetSubmittedUsername()
	{

		username = loginInput.value; 
		Debug.Log (username);
	}
	public void HideLoginPanel ()
	{
		loginPanel.GetComponent<TweenAlpha> ().PlayForward ();
	}
	public void ShowOtherPanel ()
	{
		lobbyPanel.GetComponent<TweenAlpha> ().PlayForward ();
	}
}

