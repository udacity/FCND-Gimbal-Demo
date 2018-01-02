using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	public AxisOrder axisOrder;
	AxisOrder lastAxisOrder;
	public AxisSet[] axisSets;
	// make this public to re-capture rotations
	public bool captureRotations;

	Transform pitchRing;
	Transform rollRing;
	Transform yawRing;


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
			thisSet.rotations[0] = thisSet.rings[0].localRotation;
			thisSet.rotations[1] = thisSet.rings[1].localRotation;
			thisSet.rotations[2] = thisSet.rings[2].localRotation;
//			thisSet.rotations [ 0 ] = rings [ 0 ].localRotation;
//			thisSet.rotations [ 1 ] = rings [ 1 ].localRotation;
//			thisSet.rotations [ 2 ] = rings [ 2 ].localRotation;
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
//		rings [ 0 ].localRotation = thisSet.rotations [ 0 ];
//		rings [ 1 ].localRotation = thisSet.rotations [ 1 ];
//		rings [ 2 ].localRotation = thisSet.rotations [ 2 ];
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
		switch ( order )
		{
		case AxisOrder.RPY:
			rollRing = rings [ 0 ];
			pitchRing = rings [ 1 ];
			yawRing = rings [ 2 ];

			break;

		case AxisOrder.RYP:
			rollRing = rings [ 0 ];
			yawRing = rings [ 1 ];
			pitchRing = rings [ 2 ];
			break;

		case AxisOrder.PRY:
			pitchRing = rings [ 0 ];
			rollRing = rings [ 1 ];
			yawRing = rings [ 2 ];
			break;

		case AxisOrder.PYR:
			pitchRing = rings [ 0 ];
			yawRing = rings [ 1 ];
			rollRing = rings [ 2 ];
			break;

		case AxisOrder.YRP:
			yawRing = rings [ 0 ];
			rollRing = rings [ 1 ];
			pitchRing = rings [ 2 ];
			break;

		case AxisOrder.YPR:
			yawRing = rings [ 0 ];
			pitchRing = rings [ 1 ];
			rollRing = rings [ 2 ];
			break;
		}

		pitchRing.GetComponent<Renderer> ().material = materials [ 0 ];
		rollRing.GetComponent<Renderer> ().material = materials [ 1 ];
		yawRing.GetComponent<Renderer> ().material = materials [ 2 ];

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
}