using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCam : MonoBehaviour
{
	public Transform camBase;
	public Transform camTransform;

	public float moveSpeed = 5;
	public float rotationSpeed = 180;

	Quaternion baseInitialRotation;
	Vector3 baseInitialPosition;
	Vector3 camInitialPosition;

//	bool mouseIsNearSliders;
	bool leftMouseDown;
	bool rightMouseDown;

	void Awake ()
	{
		baseInitialPosition = camBase.position;
		baseInitialRotation = camBase.rotation;
		camInitialPosition = camTransform.localPosition;
	}

	void Update ()
	{
		Vector2 mouse = new Vector2 ( Input.GetAxis ( "Mouse X" ), Input.GetAxis ( "Mouse Y" ) );
		Vector2 mousePos = Input.mousePosition;
		// check if the mouse is in the top right corner near the sliders
//		mouseIsNearSliders = mousePos.x > Screen.width - 300 * ( 1f * Screen.width / 1920 );
//		mouseIsNearSliders &= mousePos.y > Screen.height - 250 * ( 1f * Screen.height / 1080 );

		// update left mouse down status. disallow down if we're near the sliders
		if ( Input.GetMouseButtonDown ( 0 ) && !GimbalControl.IsMouseOnUI )
		{
			leftMouseDown = true;
		}
		if ( Input.GetMouseButtonUp ( 0 ) )
			leftMouseDown = false;

		// update right mouse down status. disallow down if we're near the sliders
		if ( Input.GetMouseButtonDown ( 1 ) && !GimbalControl.IsMouseOnUI )
		{
			rightMouseDown = true;
		}
		if ( Input.GetMouseButtonUp ( 1 ) )
			rightMouseDown = false;

		// if the mouse is down, rotate the camera
		if ( leftMouseDown )
		{
			camBase.Rotate ( Vector3.up * mouse.x * rotationSpeed * Time.deltaTime, Space.World );
			camBase.Rotate ( Vector3.right * -mouse.y * rotationSpeed * Time.deltaTime, Space.Self );

		}
		// if right mouse is down, move the camera
		if ( rightMouseDown )
		{
			Vector3 forward = Vector3.ProjectOnPlane ( camBase.forward, Vector3.up ).normalized * -mouse.y;
			Vector3 right = camBase.right * -mouse.x;
			camBase.Translate ( ( forward + right ) * moveSpeed * Time.deltaTime, Space.World );
		}

		if ( Input.GetKeyDown ( KeyCode.R ) )
		{
			camBase.position = baseInitialPosition;
			camBase.rotation = baseInitialRotation;
			camTransform.localPosition = camInitialPosition;
		}
	}

	void OnGUI ()
	{
		float aspect = 1f * Screen.height / 1080;

		GUIStyle label = GUI.skin.label;
		label.fontSize = (int) ( 20f * aspect );
		label.alignment = TextAnchor.MiddleRight;
		float height = label.CalcHeight ( new GUIContent ( "RLMBMOC" ), Screen.width );
//		float height = 25f * aspect;

		Rect r = new Rect ( 0, Screen.height - height - 5, Screen.width - 15, height );
		GUI.color = Color.black;
		GUI.Label ( new Rect ( r.x + 1, r.y + 1, r.width, r.height ), "R - Reset Camera" );
		GUI.color = Color.white;
		GUI.Label ( r, "R - Reset Camera" );

		r.y -= height;
		GUI.color = Color.black;
		GUI.Label ( new Rect ( r.x + 1, r.y + 1, r.width, r.height ), "RMB - Move Camera" );
		GUI.color = Color.white;
		GUI.Label ( r, "RMB - Move Camera" );

		r.y -= height;
		GUI.color = Color.black;
		GUI.Label ( new Rect ( r.x + 1, r.y + 1, r.width, r.height ), "LMB - Orient Camera" );
		GUI.color = Color.white;
		GUI.Label ( r, "LMB - Orient Camera" );
	}
}