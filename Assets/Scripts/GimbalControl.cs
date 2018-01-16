using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using dd = TMPro.TMP_Dropdown;

public enum AxisOrder { RPY, RYP, PRY, PYR, YRP, YPR };

[System.Serializable]
public class AxisSet
{
	public string name;
	public Vector3[] axes;
	[HideInInspector]
	public Quaternion[] rotations = new Quaternion[4];
	[HideInInspector]
	public Vector3 jetPosition;
	[Tooltip ("References to roll-pitch-yaw rings")]
	public Transform[] rings = new Transform[3];
}

[ExecuteInEditMode]
public class GimbalControl : MonoBehaviour
{
	public static bool IsMouseOnUI;

	public Material[] materials;
	public Transform[] rings;
	public Transform jet;
	public Slider[] sliders;
	public Toggle[] ringToggles;
	public dd dropdown;
	public GameObject[] globalAxes;
	public Transform[] globalAxisTips;
	public GameObject[] bodyAxes;
	public Transform[] bodyAxisTips;
	public Toggle globalToggle;
	public Toggle[] globalToggles;
	public Toggle bodyAxesToggle;
	public Toggle[] bodyFrameToggles;

	public AxisOrder axisOrder;
	AxisOrder lastAxisOrder;
	public AxisSet[] axisSets;
	// make this public to re-capture rotations
	public bool captureRotations;

	public Vector3 globalFrameScreenPos;

	Transform pitchRing;
	Transform rollRing;
	Transform yawRing;
	Camera cam;
	Transform globalFrameParent;

	bool[] sliderMouse = new bool[3];
	int sliderDown = -1;
	bool globalVisible;
	bool[] globalVisibles = new bool[3];
	bool bodyAxesVisible;
	bool[] bodyAxesVisibles = new bool[3];


	void Awake ()
	{
		#if UNITY_EDITOR
		if ( UnityEditor.EditorApplication.isPlaying )
		#endif
		SetAxisOrder ( AxisOrder.RPY );
		cam = Camera.main;
		if ( bodyAxes != null && bodyAxes.Length > 0 )
			globalFrameParent = globalAxes [ 0 ].transform.parent;
		if ( globalFrameParent != null )
			globalFrameParent.position = cam.ViewportToWorldPoint ( globalFrameScreenPos );
		for ( int i = 0; i < 3; i++ )
		{
			globalVisibles [ i ] = globalToggles [ i ];
			bodyAxesVisibles [ i ] = bodyFrameToggles [ i ];
		}
		OnGlobalToggle ( globalToggle.isOn );
		OnBodyToggle ( bodyAxesToggle.isOn );
	}

	void Update ()
	{
		#if UNITY_EDITOR
		if ( UnityEditor.EditorApplication.isPlaying )
			return;
		if ( axisOrder != lastAxisOrder )
		{
			lastAxisOrder = axisOrder;
			SetAxisOrder ( axisOrder );
		}
		if ( captureRotations )
		{
			captureRotations = false;
			int idx = (int) axisOrder;
			AxisSet thisSet = axisSets [ idx ];
			thisSet.rotations [ 0 ] = thisSet.rings [ 0 ].localRotation;
			thisSet.rotations [ 1 ] = thisSet.rings [ 1 ].localRotation;
			thisSet.rotations [ 2 ] = thisSet.rings [ 2 ].localRotation;
			thisSet.rotations [ 3 ] = jet.localRotation;
			thisSet.jetPosition = jet.localPosition;
		}
		#endif
	}

	void LateUpdate ()
	{
		if ( globalFrameParent != null )
			globalFrameParent.position = cam.ViewportToWorldPoint ( globalFrameScreenPos );
	}

	void OnGUI ()
	{
		if ( !globalVisible && !bodyAxesVisible )
			return;
		
		GUIStyle label = GUI.skin.label;
		int fontSize = label.fontSize;
		FontStyle fontStyle = label.fontStyle;
		
		label.fontSize = (int) ( 30f * Screen.height / 1080 );
		label.fontStyle = FontStyle.Bold;
		Vector2 screenPos;
		Vector2 size = new Vector2 ( 25, 25 );

		if ( globalVisible )
		{
			if ( globalVisibles [ 0 ] )
			{
				screenPos = cam.WorldToScreenPoint ( globalAxisTips [ 0 ].position );
				screenPos.y = Screen.height - screenPos.y - 10;
				GUI.color = Color.black;
				GUI.Label ( new Rect ( screenPos - size + Vector2.one, size * 2 ), "N" );
				GUI.color = Color.green;
				GUI.Label ( new Rect ( screenPos - size, size * 2 ), "N" );
			}

			if ( globalVisibles [ 1 ] )
			{
				screenPos = cam.WorldToScreenPoint ( globalAxisTips [ 1 ].position );
				screenPos.y = Screen.height - screenPos.y - 10;
				GUI.color = Color.black;
				GUI.Label ( new Rect ( screenPos - size + Vector2.one, size * 2 ), "E" );
				GUI.color = Color.red;
				GUI.Label ( new Rect ( screenPos - size, size * 2 ), "E" );
			}

			if ( globalVisibles [ 2 ] )
			{
				screenPos = cam.WorldToScreenPoint ( globalAxisTips [ 2 ].position );
				screenPos.y = Screen.height - screenPos.y - 10;
				GUI.color = Color.black;
				GUI.Label ( new Rect ( screenPos - size + Vector2.one, size * 2 ), "D" );
				GUI.color = Color.blue;
				GUI.Label ( new Rect ( screenPos - size, size * 2 ), "D" );
			}
		}

		if ( bodyAxesVisible )
		{
			if ( bodyAxesVisibles [ 0 ] )
			{
				screenPos = cam.WorldToScreenPoint ( bodyAxisTips [ 0 ].position );
				screenPos.y = Screen.height - screenPos.y - 10;
				GUI.color = Color.black;
				GUI.Label ( new Rect ( screenPos - size + Vector2.one, size * 2 ), "X" );
				GUI.color = Color.green;
				GUI.Label ( new Rect ( screenPos - size, size * 2 ), "X" );
			}

			if ( bodyAxesVisibles [ 1 ] )
			{
				screenPos = cam.WorldToScreenPoint ( bodyAxisTips [ 1 ].position );
				screenPos.y = Screen.height - screenPos.y - 10;
				GUI.color = Color.black;
				GUI.Label ( new Rect ( screenPos - size + Vector2.one, size * 2 ), "Y" );
				GUI.color = Color.red;
				GUI.Label ( new Rect ( screenPos - size, size * 2 ), "Y" );
			}

			if ( bodyAxesVisibles [ 2 ] )
			{
				screenPos = cam.WorldToScreenPoint ( bodyAxisTips [ 2 ].position );
				screenPos.y = Screen.height - screenPos.y - 10;
				GUI.color = Color.black;
				GUI.Label ( new Rect ( screenPos - size + Vector2.one, size * 2 ), "Z" );
				GUI.color = Color.blue;
				GUI.Label ( new Rect ( screenPos - size, size * 2 ), "Z" );
			}
		}
	}

	void ResetRotations ()
	{
		AxisSet thisSet = axisSets [ (int) axisOrder ];
		thisSet.rings [ 0 ].localRotation = thisSet.rotations [ 0 ];
		thisSet.rings [ 1 ].localRotation = thisSet.rotations [ 1 ];
		thisSet.rings [ 2 ].localRotation = thisSet.rotations [ 2 ];
		jet.localRotation = thisSet.rotations [ 3 ];
		jet.localPosition = thisSet.jetPosition;
	}

	public void ResetSliders ()
	{
		sliders [ 0 ].value = sliders [ 1 ].value = sliders [ 2 ].value = 0;
	}

	void UpdateRotation (int slider)
	{
		AxisSet axes = axisSets [ (int) axisOrder ];
		Transform ring = axes.rings [ slider ];
		ring.localRotation = axes.rotations [ slider ];

		ring.Rotate ( axes.axes [ slider ] * sliders [ slider ].value * 360f );
	}

	Transform RingFromSlider (int slider)
	{
		return axisSets [ (int) axisOrder ].rings [ slider ];
	}

	public void SetAxisOrder (int order)
	{
		SetAxisOrder ( (AxisOrder) order );
	}

	public void SetAxisOrder (AxisOrder order)
	{
		sliderDown = -1;
		sliderMouse [ 0 ] = sliderMouse [ 1 ] = sliderMouse [ 2 ] = false;

		AxisSet axes = axisSets [ (int) order ];
		rollRing = axes.rings [ 0 ];
		pitchRing = axes.rings [ 1 ];
		yawRing = axes.rings [ 2 ];

		rollRing.GetComponent<ColorSetter> ().SetMaterial ( materials [ 0 ] );
		pitchRing.GetComponent<ColorSetter> ().SetMaterial ( materials [ 1 ] );
		yawRing.GetComponent<ColorSetter> ().SetMaterial ( materials [ 2 ] );

		axisOrder = order;
		ResetRotations ();
		ResetSliders ();
	}

	public void OnSliderValueChanged (int sliderID)
	{
		UpdateRotation ( sliderID );
	}

	public void SetMouseInUI (bool isIn)
	{
		IsMouseOnUI = isIn;
	}

	public void OnResetButton ()
	{
		SetAxisOrder ( axisOrder );
		ringToggles [ 3 ].isOn = true;
		globalToggle.isOn = true;
		bodyAxesToggle.isOn = true;
	}

	public void OnMouseEnterSlider (int slider)
	{
//		Debug.Log ( "0" );
		if ( dropdown.IsExpanded )
			return;
		sliderMouse [ slider ] = true;
		// enable this for the yellow highlight
//		if ( sliderDown == -1 )
//		{
//			Transform ring = RingFromSlider ( slider );
//			ring.GetComponent<ColorSetter> ().SetMaterial ( materials [ 3 ] );
//		}
	}

	public void OnMouseExitSlider (int slider)
	{
//		Debug.Log ( "1" );
		sliderMouse [ slider ] = false;
		// enable this for the yellow highlight
//		if ( sliderDown != slider )
//		{
//			Transform ring = RingFromSlider ( slider );
//			ring.GetComponent<ColorSetter> ().SetMaterial ( materials [ slider ] );
//		}
	}

	public void OnMouseDownSlider (int slider)
	{
//		Debug.Log ( "2" );
		sliderDown = slider;
		// enable this for the yellow highlight
//		Transform ring = RingFromSlider ( slider );
//		ring.GetComponent<ColorSetter> ().SetMaterial ( materials [ 3 ] );
	}

	public void OnMouseUpSlider (int slider)
	{
//		Debug.Log ( "3" );
		sliderDown = -1;
		if ( sliderDown != slider )
		if ( !sliderMouse [ slider ] )
		{
			Transform ring = RingFromSlider ( slider );
			ring.GetComponent<ColorSetter> ().SetMaterial ( materials [ slider ] );
		}
	}

	public void OnRingToggleChanged (Toggle t)
	{
		bool gimbalOn = ringToggles [ 3 ].isOn;
		int index = ringToggles.IndexOf ( t );
		switch (index)
		{
		case 0:
			rollRing.GetComponent<ColorSetter> ().SetVisible ( t.isOn && gimbalOn );
			break;
		case 1:
			pitchRing.GetComponent<ColorSetter> ().SetVisible ( t.isOn && gimbalOn );
			break;
		case 2:
			yawRing.GetComponent<ColorSetter> ().SetVisible ( t.isOn && gimbalOn );
			break;
		case 3:
//			ringToggles [ 0 ].isOn = ringToggles [ 1 ].isOn = ringToggles [ 2 ].isOn = t.isOn;
			break;
		}
	}

	public void OnGlobalToggle (bool b)
	{
		globalVisible = b;
		int i = 0;
		foreach ( GameObject axis in globalAxes )
		{
			axis.SetActive ( b && globalVisibles [ i ] );
			i++;
		}
	}

	public void OnGlobalToggles (int toggle)
	{
		globalVisibles [ toggle ] = globalToggles [ toggle ].isOn;
		globalAxes [ toggle ].SetActive ( globalVisible && globalVisibles [ toggle ] );
	}

	public void OnBodyToggle (bool b)
	{
		bodyAxesVisible = b;
		int i = 0;
		foreach ( GameObject axis in bodyAxes )
		{
			axis.SetActive ( b && bodyAxesVisibles [ i ] );
			i++;
		}
	}

	public void OnBodyToggles (int toggle)
	{
		bodyAxesVisibles [ toggle ] = bodyFrameToggles [ toggle ].isOn;
		bodyAxes [ toggle ].SetActive ( bodyAxesVisible && bodyAxesVisibles [ toggle ] );
	}
}