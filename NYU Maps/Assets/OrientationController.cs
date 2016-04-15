using UnityEngine;
using System.Collections;

public class OrientationController : MonoBehaviour 
{
	public UITexture mainUIBackdrop;
	public UILabel movesLabel;
	public UITexture rollDiceButton;
	public UITexture endTurnButton;

	void Start () 
	{
		float mainUIScaler = (float)Screen.height / mainUIBackdrop.height;
		mainUIBackdrop.transform.localScale = new Vector3 (mainUIScaler, mainUIScaler, 1);
		mainUIBackdrop.transform.localPosition = new Vector3 (-mainUIBackdrop.width*mainUIScaler/2,0,0);
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
}
