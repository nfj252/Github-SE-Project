using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TaskController : MonoBehaviour 
{
	GameflowController gameFlowController;
	OrientationController orientationController;
	BuildingController buildingController;
	InGameDBController inGameDBController;
	
	public GameObject buildingTasksParent;
	public GameObject playerTasksParent;
	public GameObject taskPrefab;

	public GameObject lpTaskLabelParent;

	public UILabel buildingTasksPanelTitleLabel;
	public GameObject[] buildingTaskPrefabs;
	public UILabel[] buildingTaskNameLabels;
	public UILabel[] buildingTaskQuantityLabels;
	public UIButton[] buildingTaskCompleteButtons;

	public GameObject[] playerTaskPrefabs;
	public UILabel[] playerTaskNameLabels;

	Building visitedBuilding;



	public void Setup () 
	{
		gameFlowController = FindObjectOfType<GameflowController> ();
		orientationController = FindObjectOfType<OrientationController> ();
		buildingController = FindObjectOfType<BuildingController> ();
		inGameDBController = FindObjectOfType<InGameDBController> ();
	}
	
	public void AssignBuildingTasks(List<string> taskDataList, List<Building> buildings)
	{
		for(int i = 0; i < taskDataList.Count; i++)
		{
			GameObject taskObj = Instantiate(taskPrefab);
			Task taskComp = taskObj.GetComponent<Task>();
			taskObj.transform.SetParent(buildingTasksParent.transform);

			string[] taskData = taskDataList[i].Split(';');
			string taskName = taskData[0];
			int quantity = int.Parse(taskData[1]);
			taskComp.Setup(taskName, quantity);

			string[] buildingNames = taskData[2].Split('-');

			for(int j = 0; j < buildingNames.Length; j++)
			{
				Building buildingRef = buildings.Find(x => x.buildingName == buildingNames[j]);
				if(buildingRef != null)
					buildingRef.availableTasks.Add(taskComp);
			}
		}
	}

	public void AssignLocalPlayerTasks(List<string> taskDataList, Player localPlayer)
	{
		List<int> randomNumbers = GenerateRandomNumbers (taskDataList.Count - 1, playerTaskPrefabs.Length);
		for(int i = 0; i < playerTaskPrefabs.Length; i++)
		{
			GameObject taskObj = Instantiate(taskPrefab);
			Task taskComp = taskObj.GetComponent<Task>();
			taskObj.transform.SetParent(playerTasksParent.transform);
			string[] taskData = taskDataList[randomNumbers[i]].Split(';');
			string taskName = taskData[0];
			taskComp.Setup(taskName, -1);
			localPlayer.tasks.Add (taskComp);
		}
	}

	List<int> GenerateRandomNumbers(int range, int quantity) 
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

	public void SetBuildingPanelTitle(string buildingName)
	{
		buildingTasksPanelTitleLabel.text = buildingName;
	}

	public void InitializeLPTaskLabels (List<Task> tasks)
	{
		for(int i = 0; i < tasks.Count; i++)
			playerTaskNameLabels[i].text = tasks[i].taskName;
	}

	public void SetVisitedBuilding(Building currentBuilding)
	{
		visitedBuilding = currentBuilding;
		PrepareBuildingTaskLabels ();
	}

	public Building GetVisitedBuilding()
	{
		return visitedBuilding;
	}

	void PrepareBuildingTaskLabels ()
	{
		for(int i = 0; i < buildingTaskPrefabs.Length; i++)
		{
			if(i < visitedBuilding.availableTasks.Count)
			{
				buildingTaskPrefabs[i].SetActive(true);
				buildingTaskNameLabels[i].text = visitedBuilding.availableTasks[i].taskName;
				if(visitedBuilding.availableTasks[i].quantity == -1)
					buildingTaskQuantityLabels[i].text = "-";
				else
					buildingTaskQuantityLabels[i].text = visitedBuilding.availableTasks[i].quantity.ToString();
				for(int j = 0; j < gameFlowController.GetLocalPlayer().tasks.Count; j++)
				{
					if(gameFlowController.GetLocalPlayer().tasks[j].taskName.Equals(visitedBuilding.availableTasks[i].taskName) && !gameFlowController.GetLocalPlayer().tasks[j].completed)
					{
						orientationController.CustomSetUIButtonState(buildingTaskCompleteButtons[i], true);
						break;
					}
					else
						orientationController.CustomSetUIButtonState(buildingTaskCompleteButtons[i], false);
				}
			}
			else
				buildingTaskPrefabs[i].SetActive(false);
		}
	}

	public IEnumerator UpdateBuildingTaskQuantityLabels(List<string> taskDataList)
	{
		inGameDBController.FetchTaskData ();
		for(int i = 0; i < taskDataList.Count; i++)
		{
			string[] row = taskDataList[i].Split(';');
			string[] rowBuildingNames = row[2].Split('-');
			for(int j = 0; j < rowBuildingNames.Length; j++)
			{
				if(rowBuildingNames[j].Equals (visitedBuilding.buildingName))
				{
					for(int k = 0; k < buildingTaskNameLabels.Length; k++)
					{
						if(buildingTaskNameLabels[k].text.Equals(row[0]))
						{
							buildingTaskQuantityLabels[k].text = row[1];
							if(buildingTaskQuantityLabels[k].text.Equals("-1"))
								buildingTaskQuantityLabels[k].text = "N/A";

							break;
						}
					}
					break;
				}
			}
		}
		yield return null;
	}
	
	public void CompleteBuildingTask(string taskNumber)
	{
		int itemNumber = int.Parse (taskNumber);
		string itemName = visitedBuilding.availableTasks[itemNumber].taskName;
		if(visitedBuilding.availableTasks[itemNumber].quantity > 0)
			inGameDBController.DecrementBuildingTaskQuantity (itemName);
		orientationController.CustomSetUIButtonState (buildingTaskCompleteButtons [itemNumber], false);
		for(int i = 0; i < gameFlowController.GetLocalPlayer().tasks.Count; i++)
		{
			if(gameFlowController.GetLocalPlayer().tasks[i].taskName.Equals (itemName))
			{
				gameFlowController.GetLocalPlayer().tasks[i].completed = true;
				playerTaskNameLabels[i].color = Color.green;
				break;
			}
		}
	}
}
