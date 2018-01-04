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
	public dd dropdown;

	public AxisOrder axisOrder;
	AxisOrder lastAxisOrder;
	public AxisSet[] axisSets;
	// make this public to re-capture rotations
	public bool captureRotations;

	Transform pitchRing;
	Transform rollRing;
	Transform yawRing;

	bool[] sliderMouse = new bool[3];
	int sliderDown = -1;
//	bool[] sliderDown = new bool[3];


	void Awake ()
	{
		#if UNITY_EDITOR
		if ( UnityEditor.EditorApplication.isPlaying )
		#endif
		SetAxisOrder ( AxisOrder.RPY );
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

/*		switch ( order )
		{
		case AxisOrder.RPY:
			yawRing = rings [ 0 ];
			pitchRing = rings [ 1 ];
			rollRing = rings [ 2 ];

			break;

		case AxisOrder.RYP:
			pitchRing = rings [ 0 ];
			yawRing = rings [ 1 ];
			rollRing = rings [ 2 ];
			break;

		case AxisOrder.PRY:
			yawRing = rings [ 0 ];
			rollRing = rings [ 1 ];
			pitchRing = rings [ 2 ];
			break;

		case AxisOrder.PYR:
			rollRing = rings [ 0 ];
			yawRing = rings [ 1 ];
			pitchRing = rings [ 2 ];
			break;

		case AxisOrder.YRP:
			pitchRing = rings [ 0 ];
			rollRing = rings [ 1 ];
			yawRing = rings [ 2 ];
			break;

		case AxisOrder.YPR:
			rollRing = rings [ 0 ];
			pitchRing = rings [ 1 ];
			yawRing = rings [ 2 ];
			break;
		}*/

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
	}

	public void OnMouseEnterSlider (int slider)
	{
//		Debug.Log ( "0" );
		if ( dropdown.IsExpanded )
			return;
		sliderMouse [ slider ] = true;
		if ( sliderDown == -1 )
		{
			Transform ring = axisSets [ (int) axisOrder ].rings [ slider ];
			ring.GetComponent<ColorSetter> ().SetMaterial ( materials [ 3 ] );
		}
	}

	public void OnMouseExitSlider (int slider)
	{
//		Debug.Log ( "1" );
		sliderMouse [ slider ] = false;
		if ( sliderDown != slider )
		{
			Transform ring = axisSets [ (int) axisOrder ].rings [ slider ];
			ring.GetComponent<ColorSetter> ().SetMaterial ( materials [ slider ] );
		}
	}

	public void OnMouseDownSlider (int slider)
	{
//		Debug.Log ( "2" );
		sliderDown = slider;
		Transform ring = axisSets [ (int) axisOrder ].rings [ slider ];
		ring.GetComponent<ColorSetter> ().SetMaterial ( materials [ 3 ] );
	}

	public void OnMouseUpSlider (int slider)
	{
//		Debug.Log ( "3" );
		sliderDown = -1;
		if ( sliderDown != slider )
		if ( !sliderMouse [ slider ] )
		{
			Transform ring = axisSets [ (int) axisOrder ].rings [ slider ];
			ring.GetComponent<ColorSetter> ().SetMaterial ( materials [ slider ] );
		}
	}
}