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
	public GameObject lpTaskLabelPrefab;
	public UILabel buildingTasksPanelTitleLabel;

	public GameObject[] buildingTaskPrefabs;
	public UILabel[] buildingTaskNameLabels;
	public UILabel[] buildingTaskQuantityLabels;
	public UIButton[] buildingTaskCompleteButtons;

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

	public void AssignLocalPlayerTasks(List<string> taskDataList, Player localPlayer, int numOfTasks)
	{
		List<int> randomNumbers = GenerateRandomNumbers (taskDataList.Count - 1, numOfTasks);
		for(int i = 0; i < numOfTasks; i++)
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

	public void SetBuildingPanelTitle(string buildingName)
	{
		buildingTasksPanelTitleLabel.text = buildingName;
	}

	public void InitializeLPTaskLabels (List<Task> tasks)
	{
		for(int i = 0; i < tasks.Count; i++)
		{
			GameObject taskLabel = GameObject.Instantiate(lpTaskLabelPrefab);
			taskLabel.transform.SetParent(lpTaskLabelParent.transform);
			taskLabel.name = tasks[i].taskName + " Label";
			taskLabel.GetComponent<UILabel>().text = tasks[i].taskName;
			tasks[i].label = taskLabel.GetComponent<UILabel>();
			taskLabel.transform.localScale = new Vector3(1,1,1);
			taskLabel.transform.localPosition = new Vector3(0,-i * (taskLabel.GetComponent<UILabel>().height),0);
		}
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

	//Create Button To Fetch Task Latest Data For Building thne display the shet
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
					if(gameFlowController.GetLocalPlayer().tasks[j].taskName.Equals(visitedBuilding.availableTasks[i].taskName))
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

	public IEnumerator UpdateBuildingTaskDataLabels(List<string> taskDataList)
	{
		Debug.Log ("UpdateBuildingTaskDataLabels");
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
		int number = int.Parse (taskNumber);
		if(visitedBuilding.availableTasks[number].quantity > 0)
		{
			string taskName = visitedBuilding.availableTasks[number].taskName;
			Debug.Log (taskName);
			inGameDBController.DecrementBuildingTaskQuantity (taskName);
		}
		orientationController.CustomSetUIButtonState (buildingTaskCompleteButtons [number], false);
	}
}
