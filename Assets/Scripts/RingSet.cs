using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RingSet
{
	public int this[int i] { get { return rings [ i ]; } }

	[SerializeField]
	int[] rings = new int[3];
}