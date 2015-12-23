using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EditorUtillities
{
	public class BaseEditor : Editor
	{
		public void CreatePropertyField(SerializedProperty currentProperty, string text, string tooltipText)
		{
			GUIContent content = new GUIContent(text, tooltipText);
			EditorGUILayout.PropertyField(currentProperty, content);
		}

		public bool CreateButton(string text, string tooltipText)
		{
			GUIContent content = new GUIContent(text, tooltipText);
			return GUILayout.Button(content);
		}

		public List<string> LoadMethodsFromGameObject(Behaviour []ListOfMethods)
		{
			List<string> listOfMethods = new List<string>();
			int objectCount = ListOfMethods.Length;
			char[] Seperators = new char[2];
			Seperators[0] = '(';
			Seperators[1] = ')';
			for(int i = 0; i < objectCount; ++i)
			{
				string[] listOfTempNameStrings = ListOfMethods[i].ToString().Split(Seperators);
				listOfMethods.Add(listOfTempNameStrings[1]);
			}

			return listOfMethods;
		}

		public int CreatePopupList(string []currentArray, int currentIndex, string text, string tooltipText)
		{
			if(currentArray != null)
			{
				GUIContent content = new GUIContent(text, tooltipText);
				GUIContent []displayedOptions = new GUIContent[currentArray.Length];
				for(int i = 0; i < displayedOptions.Length; ++i)
				{
					displayedOptions[i] = new GUIContent(currentArray[i]);
				}

				return EditorGUILayout.Popup(content, currentIndex, displayedOptions);
			}

			return 0;
		}

		public int GetCurrentBehaviourIndex(Behaviour []methodList, Behaviour currentBehaviour)
		{
			if(currentBehaviour != null)
			{
				int objectCount = methodList.Length;
				for(int i = 0; i < objectCount; ++i)
				{
					if(string.Equals(methodList[i].ToString(), currentBehaviour.ToString()) == true)
						return i;
				}
			}

			return 0;
		}

		public void CreateArrayPropertyField(SerializedProperty arrayObject, string text, string tooltipText)
		{
			if(CreateButton("Add", "Add element to list.") == true)
				arrayObject.arraySize += 1;

			int objectCount = arrayObject.arraySize;
			for(int i = 0; i < objectCount; ++i)
			{
				EditorGUILayout.BeginHorizontal();
				CreatePropertyField(arrayObject.GetArrayElementAtIndex(i), text, tooltipText);
				bool removeButtonIsPressed = CreateButton("Remove", "Removes the current element from the list.");
				EditorGUILayout.EndHorizontal();
				if(removeButtonIsPressed == true)
				{
					arrayObject.DeleteArrayElementAtIndex(i);
					break;
				}
			}
		}
		
		public bool CreateFoldOut(bool displayFoldOut, string text, string tooltipText)
		{
			GUIContent content = new GUIContent(text, tooltipText);
			return EditorGUILayout.Foldout(displayFoldOut, content);
		}
	
		public void CreateLabel(string text, string tooltipText, bool boldText)
		{
			GUIContent content = new GUIContent(text, tooltipText);
			if(boldText == false)
				EditorGUILayout.LabelField(content);
			else
				EditorGUILayout.LabelField(content, EditorStyles.boldLabel);
		}		
	}
}
