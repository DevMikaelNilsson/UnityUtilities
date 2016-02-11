using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using mnUtilities.Utilities;
using System.Reflection;

[CustomEditor(typeof(InvokeMethod))]
public class InvokeMethodEditor : Editor 
{
	private SerializedProperty m_onFlickGameObject;
	private SerializedProperty m_onFlickComponent;
	private SerializedProperty m_onFlickMethod;

	public void OnEnable()
	{
		m_onFlickGameObject = serializedObject.FindProperty("ReceiveObject");
		m_onFlickComponent = serializedObject.FindProperty("ReceiveComponent");
		m_onFlickMethod = serializedObject.FindProperty("ReceiveMethod");
	}

	override public void OnInspectorGUI()
	{
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update();

		DisplayOnFlickMethod();

		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties();
	}

	private void DisplayOnFlickMethod()
	{
		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical("box");
		CreateLabel("On Invoke:", "Setup which GameObject, script and method which will be invoked.", true);

		CreatePropertyField(m_onFlickGameObject, "Receive Object");
		if (m_onFlickGameObject.objectReferenceValue != null)
		{
			List<string> componentList = new List<string>();
			componentList = LoadComponentsFromGameObject((GameObject)m_onFlickGameObject.objectReferenceValue);
			if (componentList != null && componentList.Count > 0)
			{
				int currentComponentIndex = GetCurrentIndex(m_onFlickComponent.stringValue, componentList);
				currentComponentIndex = EditorGUILayout.Popup("Receive script", currentComponentIndex, componentList.ToArray());
				m_onFlickComponent.stringValue = componentList[currentComponentIndex];

				List<string> methodList = new List<string>();
				LoadMethods(m_onFlickComponent.stringValue, methodList);

				if (methodList != null && methodList.Count > 0)
				{
					int currentMethodIndex = GetCurrentIndex(m_onFlickMethod.stringValue, methodList); ;
					currentMethodIndex = EditorGUILayout.Popup("Receive method", currentMethodIndex, methodList.ToArray());
					m_onFlickMethod.stringValue = methodList[currentMethodIndex];
				}
			}
		}

		EditorGUILayout.EndVertical();
	}

	private int GetCurrentIndex(string stringValue, List<string> ListOfStringElements)
	{
		if (ListOfStringElements != null)
		{
			if (m_onFlickMethod.stringValue != string.Empty)
			{
				int objectCount = ListOfStringElements.Count;
				for (int i = 0; i < objectCount; ++i)
				{
					if (string.Equals(ListOfStringElements[i], stringValue) == true)
						return i;
				}
			}
		}

		return 0;
	}
	
	public void CreateLabel(string text, string tooltipText, bool boldText)
	{

		GUIContent content = new GUIContent(text, tooltipText);
		if(boldText == false)
			EditorGUILayout.LabelField(content);
		else
			EditorGUILayout.LabelField(content, EditorStyles.boldLabel);
	}
	
	public void CreatePropertyField(SerializedProperty currentProperty, string tooltipText)
	{
		EditorGUILayout.BeginHorizontal("Box");
		EditorGUILayout.PropertyField(currentProperty, new GUIContent(tooltipText));
		EditorGUILayout.EndHorizontal();
	}
	
public List<string> LoadMethodsFromGameObject(GameObject currentGameObject)
	{
		List<string> listOfMethods = new List<string>();
		if (currentGameObject == null)
			return listOfMethods;

		Component[] objectCompoentList = currentGameObject.GetComponents(typeof(MonoBehaviour));
		int objectCount = objectCompoentList.Length;
		int index = 0;
		char[] Seperators = new char[2];
		Seperators[0] = '(';
		Seperators[1] = ')';
		while (index < objectCount)
		{
			string[] listOfTempNameStrings = objectCompoentList[index].ToString().Split(Seperators);
			LoadMethods(listOfTempNameStrings[1], listOfMethods);
			index += 1;
		}

		return listOfMethods;
	}

	public List<string> LoadComponentsFromGameObject(GameObject currentGameObject)
	{
		List<string> listOfComponents = new List<string>();
		if (currentGameObject == null)
			return listOfComponents;

		Component[] objectCompoentList = currentGameObject.GetComponents(typeof(MonoBehaviour));
		
		int objectCount = objectCompoentList.Length;
		char[] Seperators = new char[2];
		Seperators[0] = '(';
		Seperators[1] = ')';

		for(int i = 0; i < objectCount; ++i)
		{
			if(objectCompoentList[i] == null)
				continue;

			string[] listOfTempNameStrings = objectCompoentList[i].ToString().Split(Seperators);
			listOfComponents.Add(listOfTempNameStrings[1]);
		}

		return listOfComponents;
	}

	public void LoadMethods(string methodNameString, List<string> listOfMethods)
	{
		Assembly[] referencedAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
		int referenceCount = referencedAssemblies.Length;
		int iterator = 0;

		while (iterator < referenceCount)
		{
			System.Type type = referencedAssemblies[iterator].GetType(methodNameString);

			if (type != null)
			{	// I want all the declared methods from the specific class.
				System.Reflection.MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

				int objectCount = methods.Length;
				int index = 0;
				char[] Seperators = new char[2];
				Seperators[0] = ' ';
				Seperators[1] = '(';

				while (index < objectCount)
				{
					string[] listOfTempNameStrings = methods[index].ToString().Split(Seperators);
					listOfMethods.Add(listOfTempNameStrings[1]);
					index += 1;
				}
				return;
			}

			iterator += 1;
		}
	}	
}
