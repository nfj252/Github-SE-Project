using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TaskController : MonoBehaviour {

	public GameObject taskParent;
	public GameObject taskPrefab;
	BuildingController buildingController;
	
	public void Setup () 
	{
		buildingController = FindObjectOfType<BuildingController> ();
	}
	
	public void AssignBuildingTasks(List<string> taskDataList, List<Building> buildings)
	{
		for(int i = 0; i < taskDataList.Count; i++)
		{
			GameObject taskObj = Instantiate(taskPrefab);
			Task taskComp = taskObj.GetComponent<Task>();
			taskObj.transform.SetParent(taskParent.transform);

			string[] taskData = taskDataList[i].Split(';');
			int taskID = int.Parse(taskData[0]);
			string taskName = taskData[1];
			int quantity = int.Parse(taskData[2]);
			taskComp.Setup(taskID, taskName, quantity);

			string[] buildingNames = taskData[3].Split('-');
			for(int j = 0; j < buildingNames.Length; j++)
			{
				Building buildingRef = buildings.Find(x => x.buildingName == buildingNames[j]);
				if(buildingRef != null)
					buildingRef.availableTasks.Add(taskComp);
			}

		}
	}


}
