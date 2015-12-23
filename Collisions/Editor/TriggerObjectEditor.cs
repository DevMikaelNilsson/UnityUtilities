using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using TabUtilities.Collisions;

[CustomEditor(typeof(TriggerObject))]
public class TriggerObjectEditor : EditorUtillities.BaseEditor
{

	private SerializedProperty m_enableComponents;
	private SerializedProperty m_disableOnStartup;

	public void OnEnable()
	{
		m_enableComponents = serializedObject.FindProperty("EnableObjects");
		m_disableOnStartup = serializedObject.FindProperty("DisableOnStartup");
	}

	override public void OnInspectorGUI()
	{
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update();

		CreatePropertyField(m_disableOnStartup, "Disable On Startup:", "Enable flag to disables the object(s) at startup. All these object(s) are re-enabled when a trigger (collision) is registered.");
		CreateArrayPropertyField(m_enableComponents, "Trigger Objects:", "GameObjects which will be trigger by a registered collision by the component.");

		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties();
	}
}

