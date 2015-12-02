using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using mnUtilities.Utilities;

[CustomEditor(typeof(ObjectPoolManager))]
public class ObjectPoolEditor : Editor
{
	private SerializedProperty m_objectCreateDelayTime;
	private SerializedProperty m_objectCFinishedDelayTime;

	private ObjectPoolManager m_currentPoolManager = null;
	private List<GameObject> m_objectPoolList = new List<GameObject>();
	private List<int> m_objectCountList = new List<int>();
	public void OnEnable()
	{
		GameObject currentTargetObject = GameObject.Find(target.name);
		if (currentTargetObject != null)
			m_currentPoolManager = currentTargetObject.GetComponent<ObjectPoolManager>();

		m_objectCreateDelayTime = serializedObject.FindProperty("TimeDelayBetweenCreatingNewObjects");
		m_objectCFinishedDelayTime = serializedObject.FindProperty("TimeDelayToSendActionNotification");
		if (m_currentPoolManager != null)
		{
			m_objectPoolList = m_currentPoolManager.ObjectPoolList;
			m_objectCountList = m_currentPoolManager.ObjectCountList;
		}
	}

	override public void OnInspectorGUI()
	{
		if (m_currentPoolManager == null)
		{
			Debug.LogWarning("Pool manager script is not found. Editor script can not continue.");
			return;
		}

		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		serializedObject.Update();

		GUILayout.Space(10.0f);
		EditorGUILayout.BeginVertical("Box");

		EditorGUILayout.LabelField("The time delay between creating new objects.\nShorter time delay will cost more,\nbut will create all objects faster.", GUILayout.MinHeight(50.0f));
		EditorGUILayout.BeginHorizontal("Box");
		EditorGUILayout.LabelField("Time delay:", GUILayout.MinWidth(150.0f));
		m_objectCreateDelayTime.floatValue = EditorGUILayout.FloatField("", m_objectCreateDelayTime.floatValue);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.LabelField("Time delay to send out a Action call when\nthe Object pool manager is finished\nwith creating all objects.", GUILayout.MinHeight(50.0f));
		EditorGUILayout.BeginHorizontal("Box");
		EditorGUILayout.LabelField("Time delay:", GUILayout.MinWidth(150.0f));
		m_objectCFinishedDelayTime.floatValue = EditorGUILayout.FloatField("", m_objectCFinishedDelayTime.floatValue);
		EditorGUILayout.EndHorizontal();



		GUILayout.Space(10.0f);
		EditorGUILayout.BeginVertical("Box");
		if (GUILayout.Button("Add object slot") == true)
		{
			m_currentPoolManager.ObjectPoolList.Add(null);
			m_currentPoolManager.ObjectCountList.Add(0);
		}

		int objectListCount = m_objectPoolList.Count;
		for (int i = 0; i < objectListCount; ++i)
		{
			EditorGUILayout.BeginHorizontal("Box");
			if (GUILayout.Button("Remove") == true)
			{
				m_currentPoolManager.ObjectCountList.RemoveAt(i);
				m_currentPoolManager.ObjectPoolList.RemoveAt(i);
				break;
			}

			m_objectPoolList[i] = (GameObject)EditorGUILayout.ObjectField(m_objectPoolList[i], typeof(GameObject), true);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal("Box");
			EditorGUILayout.LabelField("Min object count:", GUILayout.MinWidth(150.0f));
			m_objectCountList[i] = EditorGUILayout.IntField("", m_objectCountList[i]);
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.EndVertical();

		// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties();
	}
}
