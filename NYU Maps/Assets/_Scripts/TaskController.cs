using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TaskController : MonoBehaviour 
{
	public GameObject buildingTasksParent;
	public GameObject playerTasksParent;
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
			taskObj.transform.SetParent(buildingTasksParent.transform);

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

	public void AssignLocalPlayerTasks(List<string> taskDataList, Player localPlayer, int numOfTasks)
	{
		List<int> randomNumbers = GenerateRandomNumbers (taskDataList.Count - 1, numOfTasks);
		for(int i = 0; i < numOfTasks; i++)
		{
			GameObject taskObj = Instantiate(taskPrefab);
			Task taskComp = taskObj.GetComponent<Task>();
			taskObj.transform.SetParent(playerTasksParent.transform);
			string[] taskData = taskDataList[randomNumbers[i]].Split(';');
			int taskID = int.Parse(taskData[0]);
			string taskName = taskData[1];
			taskComp.Setup(taskID, taskName, -1);
			localPlayer.tasks.Add (taskComp);
		}
	}

	public List<int> GenerateRandomNumbers(int range, int quantity) 
	{
		List<int> numbers = new List<int>();
		int randomNumber = 0;
		
		while(numbers.Count < quantity)
		{    
			randomNumber = Random.Range (0, range);
			if(!numbers.Contains(randomNumber)) 
				numbers.Add(randomNumber);
		}
		
		return numbers;
	}
}
