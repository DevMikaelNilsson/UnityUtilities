using UnityEditor;
using UnityEngine;
using System.Collections;
using mnUtilities.Utilities;

[CustomEditor(typeof(ObjectPoolObject))]
public class ObjectPoolObjectEditor : Editor
{
	override public void OnInspectorGUI()
	{
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update();

		GUILayout.Space(10.0f);
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("This object is added automatically to all objects\nwhich are created from a Object Pool Manager.", GUILayout.MinHeight(30.0f));
		EditorGUILayout.EndVertical();

		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties();
	}
}
