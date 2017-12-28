using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AxisOrder { XYZ, XZY, YXZ, YZX, ZXY, ZYX };

[System.Serializable]
public class RotationSet
{
	public Quaternion[] rotations;
	public Vector3 jetPosition;

	public RotationSet ()
	{
		rotations = new Quaternion[4];
	}
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
//	[HideInInspector]
	public RotationSet[] rotationSets;
	public bool captureRotations;

	Transform xRing;
	Transform yRing;
	Transform zRing;

	Quaternion[] initialRotations;


	void Awake ()
	{
		#if UNITY_EDITOR
		if ( UnityEditor.EditorApplication.isPlaying )
			SetAxisOrder ( AxisOrder.XYZ );
		#endif
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
			RotationSet thisSet = new RotationSet ();
			thisSet.rotations [ 0 ] = rings [ 0 ].localRotation;
			thisSet.rotations [ 1 ] = rings [ 1 ].localRotation;
			thisSet.rotations [ 2 ] = rings [ 2 ].localRotation;
			thisSet.rotations [ 3 ] = jet.localRotation;
			thisSet.jetPosition = jet.localPosition;
			rotationSets [ idx ] = thisSet;
		}
		#endif
	}

	void ResetRotations ()
	{
		int idx = (int) axisOrder;
		RotationSet thisSet = rotationSets [ idx ];
		if ( thisSet.rotations == null || thisSet.rotations.Length < 4 )
			thisSet.rotations = new Quaternion[4];
		rings [ 0 ].localRotation = thisSet.rotations [ 0 ];
		rings [ 1 ].localRotation = thisSet.rotations [ 1 ];
		rings [ 2 ].localRotation = thisSet.rotations [ 2 ];
		jet.localRotation = thisSet.rotations [ 3 ];
		jet.localPosition = thisSet.jetPosition;
	}

	public void ResetSliders ()
	{
		sliders [ 0 ].value = sliders [ 1 ].value = sliders [ 2 ].value = 0;
	}

	void UpdateRotations ()
	{
		RotationSet thisSet = rotationSets [ (int) axisOrder ];
		if ( thisSet.rotations == null || thisSet.rotations.Length < 4 )
			thisSet.rotations = new Quaternion[4];
		Vector3 euler = rings [ 0 ].localEulerAngles;
		euler.x = thisSet.rotations [ 0 ].eulerAngles.x + sliders [ 0 ].value * 360f;
		rings [ 0 ].localEulerAngles = euler;

		euler = rings [ 1 ].localEulerAngles;
		euler.y = thisSet.rotations [ 1 ].eulerAngles.y + sliders [ 1 ].value * 360f;
		rings [ 1 ].localEulerAngles = euler;

		euler = rings [ 2 ].localEulerAngles;
		euler.z = thisSet.rotations [ 2 ].eulerAngles.z + sliders [ 2 ].value * 360f;
		rings [ 2 ].localEulerAngles = euler;

		jet.localRotation = thisSet.rotations [ 3 ];
		jet.localPosition = thisSet.jetPosition;
	}

	void UpdateRotation (int ring)
	{
		RotationSet thisSet = rotationSets [ (int) axisOrder ];
		Vector3 euler = rings [ ring ].localEulerAngles;

	}

	public void SetAxisOrder (int order)
	{
		SetAxisOrder ( (AxisOrder) order );
	}

	public void SetAxisOrder (AxisOrder order)
	{
		switch ( order )
		{
		case AxisOrder.XYZ:
			xRing = rings [ 0 ];
			yRing = rings [ 1 ];
			zRing = rings [ 2 ];

			break;

		case AxisOrder.XZY:
			xRing = rings [ 0 ];
			yRing = rings [ 2 ];
			zRing = rings [ 1 ];
			break;

		case AxisOrder.YXZ:
			xRing = rings [ 1 ];
			yRing = rings [ 0 ];
			zRing = rings [ 2 ];
			break;

		case AxisOrder.YZX:
			xRing = rings [ 2 ];
			yRing = rings [ 0 ];
			zRing = rings [ 1 ];
			break;

		case AxisOrder.ZXY:
			xRing = rings [ 1 ];
			yRing = rings [ 2 ];
			zRing = rings [ 0 ];
			break;

		case AxisOrder.ZYX:
			xRing = rings [ 2 ];
			yRing = rings [ 1 ];
			zRing = rings [ 0 ];
			break;
		}

		xRing.GetComponent<Renderer> ().material = materials [ 0 ];
		yRing.GetComponent<Renderer> ().material = materials [ 1 ];
		zRing.GetComponent<Renderer> ().material = materials [ 2 ];

		axisOrder = order;
		ResetRotations ();
		ResetSliders ();
	}

	public void OnSliderValueChanged (int sliderID)
	{
		UpdateRotations ();
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