using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Task : MonoBehaviour 
{
	public string taskName;
	public UILabel label;
	public int quantity;
	public bool completed;

	public void Setup (string iTaskName, int iQuantity) 
	{
		taskName = iTaskName;
		quantity = iQuantity;
		completed = false;
		this.name = taskName;
		label = null;
	}
}
