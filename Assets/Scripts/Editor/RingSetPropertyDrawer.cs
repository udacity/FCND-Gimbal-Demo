using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof (RingSet))]
public class RingSetPropertyDrawer : PropertyDrawer
{
	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		return 36;
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		int width = (int) Mathf.Max ( position.width / 3, 90f );

		EditorGUI.LabelField ( new Rect ( position.x, position.y, position.width, 20 ), "Ring Indices", EditorStyles.centeredGreyMiniLabel );
//		int width = 50;
		Rect r = new Rect ( position.x + 40, position.y + 25, width, 18 );
		SerializedProperty ringsProp = property.FindPropertyRelative ( "rings" );
		string[] labels = { "Roll", "Pitch", "Yaw" };
		float labelWidth = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 70;
		for (int i = 0; i < 3; i++)
		{
			SerializedProperty elementProp = ringsProp.GetArrayElementAtIndex ( i );
			EditorGUI.BeginProperty ( r, new GUIContent (), elementProp );
			EditorGUI.BeginChangeCheck ();
			int newVal = EditorGUI.IntField ( r, labels[i], elementProp.intValue );
			if ( EditorGUI.EndChangeCheck () )
			{
				elementProp.intValue = newVal;
			}
			EditorGUI.EndProperty ();
			r.x += r.width;
		}
		EditorGUIUtility.labelWidth = labelWidth;
//		EditorGUI.PropertyField ( r, ringsProp.GetArrayElementAtIndex ( 0 ) );//, new GUIContent ( "Roll" ) );
//		r.x += r.width;
//		EditorGUI.PropertyField ( r, ringsProp.GetArrayElementAtIndex ( 1 ), new GUIContent ( "Pitch" ) );
//		r.x += r.width;
//		EditorGUI.PropertyField ( r, ringsProp.GetArrayElementAtIndex ( 2 ), new GUIContent ( "Yaw" ) );
	}
}