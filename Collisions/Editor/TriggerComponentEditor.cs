using UnityEngine;
using UnityEditor;
using System.Collections;
using mnUtilities.Collisions;
using System.Collections.Generic;

[CustomEditor(typeof(TriggerComponent))]
public class TriggerComponentEditor : EditorUtillities.BaseEditor
{

	private SerializedProperty m_triggerProperties;
	private SerializedProperty m_disableOnStartup;

	public void OnEnable()
	{
		m_triggerProperties = serializedObject.FindProperty("EnableComponents");
		m_disableOnStartup = serializedObject.FindProperty("DisableOnStartup");
	}

	override public void OnInspectorGUI()
	{
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update();

		CreatePropertyField(m_disableOnStartup, "Disable On Startup:", "Enable flag to disables the component(s) at startup. All these component(s) are re-enabled when a trigger (collision) is registered.");
		CreateArrayPropertyField();

		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties();
	}

	public void CreateArrayPropertyField()
	{
		if(CreateButton("Add", "Add element to list.") == true)
			m_triggerProperties.arraySize += 1;

		int objectCount = m_triggerProperties.arraySize;
		for(int i = 0; i < objectCount; ++i)
		{

			SerializedProperty currentElement = m_triggerProperties.GetArrayElementAtIndex(i);
			SerializedProperty triggerObject = currentElement.FindPropertyRelative("TriggerObject");
			SerializedProperty triggerComponent = currentElement.FindPropertyRelative("TriggerComponent");

			CreatePropertyField(triggerObject, "Trigger Object:", "The object which the Component is attached to.");

			if(triggerObject.objectReferenceValue != null)
			{
				GameObject triggerGameObject = (GameObject)triggerObject.objectReferenceValue;
				Behaviour []methodList = triggerGameObject.GetComponents<Behaviour>();
				List<string> behaviourNames = LoadMethodsFromGameObject(methodList);

				Behaviour currentBehaviour = (Behaviour)triggerComponent.objectReferenceValue;
				int currentIndex = GetCurrentBehaviourIndex(methodList, currentBehaviour);
				currentIndex = CreatePopupList(behaviourNames.ToArray(), currentIndex, "Component:", "Component which will be enabled/re-enabled whenever a trigger (collision) is registered.");
				currentBehaviour = methodList[currentIndex];
				triggerComponent.objectReferenceValue = currentBehaviour;
			}


			bool removeButtonIsPressed = CreateButton("Remove", "Removes the current element from the list.");

			if(removeButtonIsPressed == true)
			{
				m_triggerProperties.DeleteArrayElementAtIndex(i);
				break;
			}
		}
	}
}
