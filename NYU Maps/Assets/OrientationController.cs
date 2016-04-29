using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrientationController : MonoBehaviour 
{
	public UIPanel lpTasksPanel;

	public UITexture mainUIBackdrop;
	public UITexture lpTasksBackdrop;
	public UILabel movesLabel;
	public UITexture rollDiceButton;
	public UITexture endTurnButton;

	public GameObject lpTaskLabelParent;
	public GameObject lpTaskLabelPrefab;
	public UILabel lpTaskButtonLabel;

	public void ScaleUI () 
	{
		float mainUIScaler = (float)Screen.height / mainUIBackdrop.height;
		mainUIBackdrop.transform.localScale = new Vector3 (mainUIScaler, mainUIScaler, 1);
		mainUIBackdrop.transform.localPosition = new Vector3 (-mainUIBackdrop.width*mainUIScaler/2,0,0);
		lpTasksBackdrop.transform.localScale = new Vector3 (mainUIScaler, mainUIScaler, 1);
	}

	public void SetMovesLabel(string content)
	{
		movesLabel.text = content;
	}

	public void SetEndTurnButtonStatus(bool status)
	{
		endTurnButton.GetComponent<BoxCollider>().enabled = status;
		if (status)
			endTurnButton.GetComponent<TweenAlpha> ().PlayReverse ();
		else
			endTurnButton.GetComponent<TweenAlpha> ().PlayForward ();
	}

	public void SetRollDiceButtonStatus(bool status)
	{
		rollDiceButton.GetComponent<BoxCollider>().enabled = status;
		if (status)
			rollDiceButton.GetComponent<TweenAlpha> ().PlayReverse ();
		else
			rollDiceButton.GetComponent<TweenAlpha> ().PlayForward ();
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

	public void ToggleLPTasksPanel()
	{
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
	}
}
