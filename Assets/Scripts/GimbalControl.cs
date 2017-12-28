using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AxisOrder { XYZ, XZY, YXZ, YZX, ZXY, ZYX };

public class GimbalControl : MonoBehaviour
{
	public Material[] materials;
	public Transform[] rings;

	Transform xRing;
	Transform yRing;
	Transform zRing;

	Quaternion[] initialRotations;


	void Awake ()
	{
		CaptureRotations ();
	}

	void CaptureRotations ()
	{
		initialRotations = new Quaternion[3];
		initialRotations [ 0 ] = rings [ 0 ].rotation;
		initialRotations [ 1 ] = rings [ 1 ].rotation;
		initialRotations [ 2 ] = rings [ 2 ].rotation;
	}

	void ResetRotations ()
	{
		rings [ 0 ].rotation = initialRotations [ 0 ];
		rings [ 1 ].rotation = initialRotations [ 1 ];
		rings [ 2 ].rotation = initialRotations [ 2 ];
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
			xRing = rings [ 1 ];
			yRing = rings [ 2 ];
			zRing = rings [ 0 ];
			break;

		case AxisOrder.ZXY:
			xRing = rings [ 2 ];
			yRing = rings [ 0 ];
			zRing = rings [ 1 ];
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

		ResetRotations ();
	}
}