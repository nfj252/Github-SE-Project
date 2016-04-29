using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchInputController : MonoBehaviour 
{
	public LayerMask touchInputMask;
	public int touchSenseLeeway;
	
	GameflowController gameFlowController;
	InGameDBController inGameDBController;
	TileController tileController;
	OrientationController orientationController;
	enum layerValues {tile = 8};
	List<GameObject> touchList = new List<GameObject>();
	List<GameObject> touchesOld;
	RaycastHit hit;
	Vector3 lastMousePosition;
	Vector2 lastTouchPosition;
	
	void Start()
	{
		touchesOld = new List<GameObject>();
		gameFlowController = FindObjectOfType<GameflowController>();
		inGameDBController = FindObjectOfType<InGameDBController> ();
		tileController = FindObjectOfType<TileController> ();
		orientationController = FindObjectOfType<OrientationController> ();
	}

	void Update () 
	{
		#if UNITY_EDITOR
		if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)) 
		{
			touchList.Clear();
			
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, touchInputMask))
			{			
				GameObject recipient = hit.transform.gameObject;
				if(recipient.GetComponent<Tile>().location != gameFlowController.GetCurrentPlayer().location && recipient.GetComponent<Tile>().tileType == "None")
					touchesOld.Add(recipient);
				Debug.Log("Touched " + recipient.name);
				float tileXDistance = gameFlowController.GetLocalPlayer().location.x - recipient.GetComponent<Tile>().location.x;
				float tileYDistance = gameFlowController.GetLocalPlayer().location.y - recipient.GetComponent<Tile>().location.y;
				int totalTileDistance = (int)(Mathf.Abs(tileXDistance) + Mathf.Abs(tileYDistance));

				if (Input.GetMouseButtonDown(0))
				{
					recipient.SendMessage("OnTouchDown",hit.point,SendMessageOptions.DontRequireReceiver);
					if(recipient.GetComponent<Tile>().tileType == "None")
					{
						if (!gameFlowController.GetCanMovePlayer() || totalTileDistance > gameFlowController.GetRemainingMoves())
							recipient.GetComponent<MeshRenderer>().material.color = tileController.redColor;
						else
							recipient.GetComponent<MeshRenderer>().material.color = tileController.blueColor;
					}
					lastMousePosition = Input.mousePosition;
				}
				
				if (Input.GetMouseButtonUp(0))
				{
					recipient.SendMessage("OnTouchUp",hit.point,SendMessageOptions.DontRequireReceiver);
					if(recipient.layer == (int)layerValues.tile)
					{
						if((Input.mousePosition - lastMousePosition).sqrMagnitude < new Vector3(touchSenseLeeway,touchSenseLeeway,touchSenseLeeway).sqrMagnitude)
						{
							if(gameFlowController.GetCanMovePlayer() && gameFlowController.GetIsLocalPlayerTurn() && 
							   (recipient.GetComponent<Tile>().tileType == "None" || recipient.GetComponent<Tile>().tileType == "Entrance"))
							{
								if(totalTileDistance <= gameFlowController.GetRemainingMoves())
								{
									inGameDBController.MovePlayer(gameFlowController.GetLocalPlayerID(),recipient.GetComponent<Tile>().location);
									if(recipient.GetComponent<Tile>().tileType == "Entrance")
									{
										orientationController.SetBuildingPanelTitle(recipient.GetComponent<Tile>().building.buildingName);
										orientationController.SetEnterBuildingButtonStatus(true);
									}
									else
										orientationController.SetEnterBuildingButtonStatus(false);
									gameFlowController.SetRemainingMoves(gameFlowController.GetRemainingMoves() - totalTileDistance);
								}
							}
						}
					}
					for(int i = 0; i < touchesOld.Count; i++)
						touchesOld[i].GetComponent<MeshRenderer>().material.color = tileController.defaultColor;
					touchesOld.Clear();
				}
			}
			
			foreach (GameObject g in touchesOld) 
			{
				if(!touchList.Contains(g))
				{
					g.SendMessage("OnTouchExit",hit.point,SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		#endif
		
		if (Input.touchCount == 1) 
		{
			touchList.Clear();
			
			foreach (Touch touch in Input.touches) 
			{
				Ray ray = Camera.main.ScreenPointToRay(touch.position);
				
				if (Physics.Raycast (ray, out hit, Mathf.Infinity, touchInputMask)) 
				{
					GameObject recipient = hit.transform.gameObject;
					if(recipient.GetComponent<Tile>().location != gameFlowController.GetCurrentPlayer().location || recipient.GetComponent<Tile>().tileType == "None")
						touchesOld.Add(recipient);
					float tileXDistance = gameFlowController.GetLocalPlayer().location.x - recipient.GetComponent<Tile>().location.x;
					float tileYDistance = gameFlowController.GetLocalPlayer().location.y - recipient.GetComponent<Tile>().location.y;
					int totalTileDistance = (int)(Mathf.Abs(tileXDistance) + Mathf.Abs(tileYDistance));
					
					if (touch.phase == TouchPhase.Began)
					{
						recipient.SendMessage("OnTouchDown",hit.point,SendMessageOptions.DontRequireReceiver);
						lastTouchPosition = Input.touches[0].position;
						if(recipient.GetComponent<Tile>().tileType == "None")
						{
							if(!gameFlowController.GetCanMovePlayer() || totalTileDistance > gameFlowController.GetRemainingMoves())
								recipient.GetComponent<MeshRenderer>().material.color = tileController.redColor;
							else
								recipient.GetComponent<MeshRenderer>().material.color = tileController.blueColor;
						}
					}
					if (touch.phase == TouchPhase.Ended)
					{
						recipient.SendMessage("OnTouchUp",hit.point,SendMessageOptions.DontRequireReceiver);
						if(recipient.layer == (int)layerValues.tile)
						{
							if(Mathf.Abs(Vector2.Distance(Input.touches[0].position,lastTouchPosition)) < touchSenseLeeway)
							{
								if(gameFlowController.GetCanMovePlayer() && gameFlowController.GetIsLocalPlayerTurn() && 
								   (recipient.GetComponent<Tile>().tileType == "None" || recipient.GetComponent<Tile>().tileType == "Entrance"))
								{
									if(totalTileDistance <= gameFlowController.GetRemainingMoves())
									{
										inGameDBController.MovePlayer(gameFlowController.GetLocalPlayerID(),recipient.GetComponent<Tile>().location); 
										if(recipient.GetComponent<Tile>().tileType == "Entrance")
										{
											orientationController.SetBuildingPanelTitle(recipient.GetComponent<Tile>().building.buildingName);
											orientationController.SetEnterBuildingButtonStatus(true);
										}
										else
											orientationController.SetEnterBuildingButtonStatus(false);
										gameFlowController.SetRemainingMoves(gameFlowController.GetRemainingMoves() - totalTileDistance);
									}
								}
							}
						}
						for(int i = 0; i < touchesOld.Count; i++)
							touchesOld[i].GetComponent<MeshRenderer>().material.color = tileController.defaultColor;
						touchesOld.Clear();
					}
					if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) 
					{
						recipient.SendMessage("OnTouchStay",hit.point,SendMessageOptions.DontRequireReceiver);
					}
					if (touch.phase == TouchPhase.Canceled)
					{
						recipient.SendMessage("OnTouchExit",hit.point,SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			foreach (GameObject g in touchesOld) 
			{
				if(!touchList.Contains(g))
				{
					g.SendMessage("OnTouchExit",hit.point,SendMessageOptions.DontRequireReceiver);

				}
			}	
		}
	}

}
