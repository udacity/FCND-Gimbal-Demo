using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseOverCheck : MonoBehaviour
{
	RectTransform rt;
	public UnityEvent mouseEnterEvent;
	public UnityEvent mouseExitEvent;

	bool hadMouse;

	void Awake ()
	{
		rt = (RectTransform) transform;
		hadMouse = false;
	}

	void Update ()
	{
		bool hasMouse = RectTransformUtility.RectangleContainsScreenPoint ( rt, Input.mousePosition );
		if ( hasMouse && !hadMouse )
			mouseEnterEvent.Invoke ();
		else
		if ( !hasMouse && hadMouse )
			mouseExitEvent.Invoke ();
		hadMouse = hasMouse;
	}
}