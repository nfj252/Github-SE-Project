using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrientationController : MonoBehaviour 
{
	public UIPanel lpTasksPanel;
	public UIPanel buildingTasksPanel;

	public UITexture mainUIPanelBackdrop;

	public GameObject mainUIScalerContainer;
	public GameObject ipTasksScalerContainer;
	public GameObject buildingTasksScalerContainer;

	public UILabel movesLabel;
	public UIButton rollDiceButton;
	public UIButton enterBuildingButton;
	public UILabel enterBuildingButtonLabel;
	public UIButton endTurnButton;

	public GameObject lpTaskLabelParent;
	public GameObject lpTaskLabelPrefab;
	public UILabel lpTaskButtonLabel;

	public UILabel buildingPanelTitleLabel;
	public GameObject[] buildingTaskPrefabs;
	public UILabel[] buildingTaskNameLabels;
	public UILabel[] buildingTaskQuantityLabels;
	public UIButton[] buildingTaskCompleteButtons;

	bool canMovePlayer;

	public void ScaleUI () 
	{
		float mainUIScaler = (float)Screen.height / mainUIPanelBackdrop.height;

		mainUIScalerContainer.transform.localScale = new Vector3 (mainUIScaler, mainUIScaler, 1);
		mainUIScalerContainer.transform.localPosition = new Vector3 (-mainUIPanelBackdrop.width*mainUIScaler/2,0,0);
		ipTasksScalerContainer.transform.localScale = new Vector3 (mainUIScaler, mainUIScaler, 1);
		buildingTasksScalerContainer.transform.localScale = new Vector3 (mainUIScaler, mainUIScaler, 1);
	}

	public void SetMovesLabel(string content)
	{
		movesLabel.text = content;
	}

	public void SetEndTurnButtonStatus(bool status)
	{
		CustomSetUIButtonState (endTurnButton, status);
	}

	public void SetRollDiceButtonStatus(bool status)
	{
		CustomSetUIButtonState (rollDiceButton, status);
	}

	public void SetEnterBuildingButtonStatus(bool status)
	{
		CustomSetUIButtonState (enterBuildingButton, status);
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

	public void PrepareBuildingTaskLabels (Player localPlayer, Building building)
	{
		for(int i = 0; i < buildingTaskPrefabs.Length; i++)
		{
			if(i < building.availableTasks.Count)
			{
				buildingTaskPrefabs[i].SetActive(true);
				buildingTaskNameLabels[i].text = building.availableTasks[i].taskName;
				buildingTaskQuantityLabels[i].text = building.availableTasks[i].quantity.ToString();
				for(int j = 0; j < localPlayer.tasks.Count; j++)
				{
					if(localPlayer.tasks[j].taskName.Equals(building.availableTasks[i].taskName))
					{
						CustomSetUIButtonState(buildingTaskCompleteButtons[i], true);
						break;
					}
					else
						CustomSetUIButtonState(buildingTaskCompleteButtons[i], false);

				}
			}
			else
				buildingTaskPrefabs[i].SetActive(false);
		}
	}

	public void ToggleLPTasksPanel()
	{
		SetCanMovePlayer (false);
		if(lpTasksPanel.alpha == 0)
		{
			lpTaskButtonLabel.text = "Hide Tasks";
			lpTasksPanel.GetComponent<TweenAlpha>().PlayForward();
		}
		else
		{
			lpTaskButtonLabel.text = "Show Tasks";
			lpTasksPanel.GetComponent<TweenAlpha>().PlayReverse();
		}
		StartCoroutine (DelayedSetCanMovePlayer (true));
	}

	public void ToggleBuildingTasksPanel()
	{
		SetCanMovePlayer (false);
		if(buildingTasksPanel.alpha == 0)
		{
			enterBuildingButtonLabel.text = "Exit Building";
			buildingTasksPanel.GetComponent<TweenAlpha>().PlayForward();
		}
		else
		{
			enterBuildingButtonLabel.text = "Enter Building";
			buildingTasksPanel.GetComponent<TweenAlpha>().PlayReverse();
		}
		StartCoroutine (DelayedSetCanMovePlayer (true));
	}

	public void SetBuildingPanelTitle(string buildingName)
	{
		buildingPanelTitleLabel.text = buildingName;
	}

	public bool GetCanMovePlayer()
	{
		return canMovePlayer;
	}

	public void SetCanMovePlayer(bool value)
	{
		canMovePlayer = value;
	}

	public IEnumerator DelayedSetCanMovePlayer(bool val)
	{
		yield return new WaitForEndOfFrame ();
		yield return new WaitForSeconds (.1f);
		canMovePlayer = true;
		yield return null;
	}

	void CustomSetUIButtonState(UIButton button, bool state)
	{
		button.enabled = state;	
		if(state)
			button.SetState(UIButtonColor.State.Normal, true);
		else
			button.SetState(UIButtonColor.State.Disabled, true);
	}
}
