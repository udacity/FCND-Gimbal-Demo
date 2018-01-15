using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ColorSetter : MonoBehaviour
{
	public Renderer[] renderers;

	void OnEnable ()
	{
		var rs = GetComponentsInChildren<Renderer> ();
		var list = new List<Renderer> ();
		list.Add ( GetComponent<Renderer> () );
		foreach ( Renderer r in rs )
			if ( r.transform.parent == transform && r.gameObject.name == "Rivet" )
				list.Add ( r );
		
		renderers = list.ToArray ();
	}

	public void SetMaterial (Material m)
	{
		foreach ( Renderer r in renderers )
			r.material = m;
	}

	public void SetVisible (bool b)
	{
		foreach ( Renderer r in renderers )
			r.enabled = b;
	}
}