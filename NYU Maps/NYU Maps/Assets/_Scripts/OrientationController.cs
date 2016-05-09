using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrientationController : MonoBehaviour 
{
	public UIPanel lpTasksPanel;
	public UIPanel buildingTasksPanel;
	public UIPanel winnerDisplayPanel;

	public UITexture mainUIPanelBackdrop;

	public GameObject mainUIScalerContainer;
	public GameObject ipTasksScalerContainer;
	public GameObject buildingTasksScalerContainer;
	public GameObject winnerDisplayScalerContainer;

	public UILabel movesLabel;
	public UIButton rollDiceButton;
	public UIButton enterBuildingButton;
	public UIButton endTurnButton;

	public UILabel lpTaskButtonLabel;
	public UILabel enterBuildingButtonLabel;
	public UILabel winnerDisplayLabel;

	bool canMovePlayer;

	public void ScaleUI () 
	{
		float mainUIScaler = (float)Screen.height / mainUIPanelBackdrop.height;
		float xShiftToCenterValue = mainUIPanelBackdrop.width * mainUIScaler /2;
		mainUIScalerContainer.transform.localScale = new Vector3 (mainUIScaler, mainUIScaler, 1);
		mainUIScalerContainer.transform.localPosition = new Vector3 (-mainUIPanelBackdrop.width*mainUIScaler/2,0,0);
		ipTasksScalerContainer.transform.localScale = new Vector3 (mainUIScaler, mainUIScaler, 1);
		ipTasksScalerContainer.transform.localPosition = new Vector3 (-xShiftToCenterValue, 0, 0);
		buildingTasksScalerContainer.transform.localScale = new Vector3 (mainUIScaler, mainUIScaler, 1);
		buildingTasksScalerContainer.transform.localPosition = new Vector3 (-xShiftToCenterValue, 0, 0);
		winnerDisplayScalerContainer.transform.localScale = new Vector3 (mainUIScaler, mainUIScaler, 1);
		winnerDisplayScalerContainer.transform.localPosition = new Vector3 (-xShiftToCenterValue, 0, 0);
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

	public void ToggleLPTasksPanel()
	{
		if(lpTasksPanel.alpha == 0)
		{
			canMovePlayer = false;
			lpTaskButtonLabel.text = "Hide Tasks";
			lpTasksPanel.GetComponent<TweenAlpha>().PlayForward();
		}
		else
		{
			StartCoroutine(DelayedSetCanMovePlayer(true));
			lpTaskButtonLabel.text = "Show Tasks";
			lpTasksPanel.GetComponent<TweenAlpha>().PlayReverse();
		}
	}

	public void ToggleBuildingTasksPanel()
	{
		if(buildingTasksPanel.alpha == 0)
		{
			canMovePlayer = false;
			enterBuildingButtonLabel.text = "Exit Building";
			buildingTasksPanel.GetComponent<TweenAlpha>().PlayForward();
		}
		else
		{
			StartCoroutine(DelayedSetCanMovePlayer(true));
			enterBuildingButtonLabel.text = "Enter Building";
			buildingTasksPanel.GetComponent<TweenAlpha>().PlayReverse();
		}
	}

	public void ShowWinnerDisplayPanel()
	{
		winnerDisplayPanel.GetComponent<TweenAlpha> ().PlayForward ();
	}

	public bool GetCanMovePlayer()
	{
		Debug.Log ("canmove:" + canMovePlayer);
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
		canMovePlayer = val;
		yield return null;
	}

	public void CustomSetUIButtonState(UIButton button, bool state)
	{
		button.enabled = state;	
		if(state)
			button.SetState(UIButtonColor.State.Normal, true);
		else
			button.SetState(UIButtonColor.State.Disabled, true);
	}

	public void SetWinnerLabel(string name)
	{
		winnerDisplayLabel.text = name;
	}


}
