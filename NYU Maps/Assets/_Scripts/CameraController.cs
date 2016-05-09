using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	Camera cam;
	public float sensitivity;

	Vector3 lastMousePosition;
	Vector3 deltaMouse;
	Vector2 lastTouchPosition;
	Vector2 deltaTouch;

	float startingSeparation;
	float startingFOV;
	const float MIN_FOV = 25;
	const float MAX_FOV = 75;

	void Start () 
	{
		cam = Camera.main;
	}

	void Update()
	{
		#if UNITY_EDITOR

		if (Input.GetMouseButton(0)) 
		{
			if (Input.GetKey(KeyCode.LeftShift)) 
			{
				if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.LeftShift)) 
				{
					startingSeparation = Vector2.Distance(new Vector2(0, 0), Input.mousePosition);
					startingFOV = cam.fieldOfView;
				}
				else 
				{
					float separation = Vector2.Distance(new Vector2(0, 0), Input.mousePosition);
					if (separation == 0) 
						separation = 1;
					float mult = startingSeparation / separation;
					cam.fieldOfView = Mathf.Clamp(mult * startingFOV, MIN_FOV, MAX_FOV);
				}
			}
			else
			{
				if (Input.GetMouseButtonDown(0))
				{
					lastMousePosition = Input.mousePosition;
				}
				
				if (Input.GetMouseButton(0))
				{
					deltaMouse = Input.mousePosition - lastMousePosition;
					cam.transform.Translate(-deltaMouse.x * sensitivity, 0, -deltaMouse.y * sensitivity, 0);
					lastMousePosition = Input.mousePosition;
				}
			}

		}

		#endif

		if (Input.touchCount == 1) 
		{
			if (Input.touches[0].phase == TouchPhase.Began)
			{
				lastTouchPosition = Input.touches[0].position;
			}
			else
			{
				deltaTouch = Input.touches[0].position - lastTouchPosition;
				cam.transform.Translate(-deltaTouch.x * sensitivity, 0, -deltaTouch.y * sensitivity, 0);
				lastTouchPosition = Input.touches[0].position;
			}

		}
		else if (Input.touchCount == 2) 
		{
			if (Input.touches[0].phase == TouchPhase.Began || Input.touches[1].phase == TouchPhase.Began) 
			{
				startingSeparation = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
				startingFOV = cam.fieldOfView;
			}
			else 
			{
				float separation = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
				if (separation == 0) 
					separation = 1;
				float mult = startingSeparation / separation;
				cam.fieldOfView = Mathf.Clamp(mult * startingFOV, MIN_FOV, MAX_FOV);
			}
			if (Input.touches[0].phase == TouchPhase.Ended) 
				lastTouchPosition = Input.touches[1].position;
			else if (Input.touches[1].phase == TouchPhase.Ended) 
				lastTouchPosition = Input.touches[0].position;
		}
	}
}
