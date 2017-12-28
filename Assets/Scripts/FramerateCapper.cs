using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateCapper : MonoBehaviour
{
	public bool cap = true;
	void Awake ()
	{
		if ( cap )
		{
			if ( QualitySettings.vSyncCount == 1 )
				Application.targetFrameRate = 60;
			else
			if ( QualitySettings.vSyncCount == 2 )
				Application.targetFrameRate = 30;
			else
				Application.targetFrameRate = 60;
		}
	}
}