using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Task : MonoBehaviour 
{
	public int taskID;
	public string taskName;
	public int quantity;
	public UILabel label;

	public void Setup (int iTaskID, string iTaskName, int iQuantity) 
	{
		taskID = iTaskID;
		taskName = iTaskName;
		quantity = iQuantity;
		this.name = taskName;
	}
}
