using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace mnUtilities.Utilities
{
	[SerializeField]
	[AddComponentMenu("Utilities/ObjectPoolManager")]
	public class ObjectPoolManager : MonoBehaviour
	{
		/// <summary>
		/// Searchable types for the object poos.
		/// These types are used for retrieving a object
		/// from the pool. 
		/// </summary>
		public enum GetObjectByType
		{
			/// <summary>
			/// Get a object based on its name.
			/// </summary>
			Name = 0,

			/// <summary>
			/// Get a object based on its tag.
			/// </summary>
			Tag = 1,

			/// <summary>
			/// Get a object based on a GameObject.
			/// </summary>
			GameObject = 2
		}

		/// <summary>
		/// The time delay between creating new objects. Shorter time delay will demand
		/// more CPU time, but will create all objects faster.
		/// </summary>
		public float TimeDelayBetweenCreatingNewObjects = 0.5f;

		/// <summary>
		/// Time delay to send out a Action call when the Object pool manager is finished with
		/// creating all objects.
		/// </summary>
		public float TimeDelayToSendActionNotification = 0.5f;

		/// <summary>
		/// List of all objects which are placed inside the object pool.
		/// </summary>
		[SerializeField]
		public List<GameObject> ObjectPoolList = new List<GameObject>();

		[HideInInspector]
		[SerializeField]
		public List<int> ObjectCountList = new List<int>();

		[SerializeField]
		private List<List<GameObject>> m_internalObjectPool = new List<List<GameObject>>();

		/// <summary>
		/// A Action call which calls a method after the first initial batch of object is created.
		/// </summary>
		private Action m_doneCreatingObjects = null;

		/// <summary>
		/// Start this instance. Creates the given number of objects in each individual pool.
		/// </summary>
		IEnumerator Start()
		{
			int objectCount = ObjectPoolList.Count;
			for (int i = 0; i < objectCount; ++i)
			{
				List<GameObject> tempList = new List<GameObject>();
				m_internalObjectPool.Add(tempList);
			}

			int internalListCount = m_internalObjectPool.Count;
			for (int listIt = 0; listIt < internalListCount; ++listIt)
			{
				if (ObjectPoolList[listIt] == null)
					continue;

				for (int i = 0; i < ObjectCountList[listIt]; ++i)
					StartCoroutine(CreateObjectAndAddToList(ObjectPoolList[listIt], listIt, TimeDelayBetweenCreatingNewObjects));
			}

			StartCoroutine(ActivateFinishedCreatingObjectsAction());
			yield return 0;
		}

		/// <summary>
		/// Creates the object and add to list.
		/// </summary>
		/// <returns>
		/// No return.
		/// </returns>
		/// <param name='currentGameObject'>
		/// Current game object to create a instance from.
		/// </param>
		/// <param name='listIteratorValue'>
		/// List iterator value. 
		/// </param>
		/// <param name='delayTime'>
		/// Delay time until creating the object.
		/// </param>
		protected IEnumerator CreateObjectAndAddToList(GameObject currentGameObject, int listIteratorValue, float delayTime)
		{
			yield return new WaitForSeconds(delayTime);

			GameObject tempObject = (GameObject)GameObject.Instantiate(currentGameObject);
			tempObject.name = ObjectPoolList[listIteratorValue].name;
			//StartCoroutine(AddComponentToObject(tempObject, delayTime));

			ObjectPoolObject objectPoolObjectScript = tempObject.GetComponent<ObjectPoolObject>();
			if(objectPoolObjectScript == null)
				objectPoolObjectScript = (ObjectPoolObject)tempObject.AddComponent(typeof(ObjectPoolObject));

			objectPoolObjectScript.ObjectPoolManagerObject = this;
			DeactivateObject(tempObject);
			m_internalObjectPool[listIteratorValue].Add(tempObject);
		}

		protected IEnumerator AddComponentToObject(GameObject tempObject, float delayTime)
		{
			yield return new WaitForSeconds((delayTime * 40.2f));
			ObjectPoolObject test = (ObjectPoolObject)tempObject.AddComponent(typeof(ObjectPoolObject));
			test.ObjectPoolManagerObject = this;
		}

		/// <summary>
		/// Retrieves a object from the object pool.
		/// </summary>
		/// <param name="objectType">The search type you want to search for a available object.</param>
		/// <param name="typeString">Search string based on the object type.</param>
		/// <param name="activateDirect">Set to true if the object should be activated directly. Usefull if you want to place the object before it becomes active.</param>
		/// <returns>A GameObject if the object was found in the object pool. Null otherwise.</returns>
		public GameObject GetObjectFromPool(GetObjectByType objectType, string typeString, bool activateDirect, string callOnMethodWhenCollected)
		{
			List<GameObject> tempList = FindObjectPoolList(objectType, typeString);

			if (tempList != null && tempList.Count > 0)
			{
				int index = 0; //(tempList.Count - 1);
				int listIndex = GetListIndex(tempList);
				GameObject returnGameObject = tempList[index];
				tempList.RemoveAt(index);
				StartCoroutine(CheckForObjectPoolCount(tempList, listIndex));

				if (activateDirect == true)
					ActivatetObject(returnGameObject);
				if (callOnMethodWhenCollected != string.Empty && returnGameObject != null)
					returnGameObject.SendMessage(callOnMethodWhenCollected, SendMessageOptions.DontRequireReceiver);

				return returnGameObject;
			}

			return null;
		}

		/// <summary>
		/// Gets the object from pool.
		/// </summary>
		/// <returns>
		/// A GameObject if the object was found in the object pool. Null otherwise.
		/// </returns>
		/// <param name='objectType'>
		/// The search type you want to search for a available object
		/// </param>
		/// <param name='typeString'>
		/// Search string based on the object type.
		/// </param>
		/// <param name='activateDirect'>
		/// Set to true if the object should be activated directly. Usefull if you want to place the object before it becomes active.
		/// </param>
		/// <param name='isVisible'>
		/// Set to true if the object should be visible directly. Set to false if it should be invisible from the start.
		/// </param>
		public GameObject GetObjectFromPool(GetObjectByType objectType, string typeString, bool activateDirect, bool isVisible, string callOnMethodWhenCollected)
		{
			GameObject objectFromPool = GetObjectFromPool(objectType, typeString, activateDirect, callOnMethodWhenCollected);
			if (objectFromPool != null)
			{
				Renderer objectFromPoolRenderObject = objectFromPool.GetComponent<Renderer>();
				if (objectFromPoolRenderObject != null)
					objectFromPoolRenderObject.enabled = isVisible;
				else
				{
					SpriteRenderer spriteRenderComponent = objectFromPool.GetComponent<SpriteRenderer>();
					if (spriteRenderComponent != null)
						spriteRenderComponent.enabled = isVisible;
				}

				Renderer []childRenderObjects = (Renderer[])objectFromPool.GetComponentsInChildren<Renderer>();
				int objectCount = childRenderObjects.Length;
				for(int i = 0; i < objectCount; ++i)
					childRenderObjects[i].enabled = isVisible;

				if (callOnMethodWhenCollected != string.Empty)
					objectFromPool.SendMessage(callOnMethodWhenCollected, SendMessageOptions.DontRequireReceiver);
			}

			return objectFromPool;
		}

		/// <summary>
		/// Deactivates a object which is a part of the object pool.
		/// </summary>
		/// <param name="currentObject">Object to deactivate.</param>
		private void DeactivateObject(GameObject currentObject)
		{
			if (currentObject != null)
				currentObject.SetActive(false);
		}

		/// <summary>
		/// Activates a object which will be released from the object pool.
		/// </summary>
		/// <param name="currentObject"></param>
		private void ActivatetObject(GameObject currentObject)
		{
			if(currentObject != null)
				currentObject.SetActive(true);
		}

		/// <summary>
		/// Set the object to be visible or invisible.
		/// </summary>
		/// <param name='currentObject'>
		/// Object which will be affected.
		/// </param>
		/// <param name='isVisible'>
		/// True if the object should be visible.\n
		/// False if the object should be invisible.
		/// </param>
		private void ToggleVisible(GameObject currentObject, bool isVisible)
		{
			Renderer currentRenderObject = currentObject.GetComponent<Renderer>();
			if(currentRenderObject != null)
				currentRenderObject.enabled = isVisible;
		}

		/// <summary>
		/// Returns and adds a object to the existing object pool.
		/// </summary>
		/// <param name="objectType">The search type you want to search for existing object pool.</param>
		/// <param name="currentGameObject">GameObject to search for.</param>
		/// <returns>True if the object was successfully added to the object pool. False otherwise.</returns>
		public bool ReturnObjectToObjectPool(GetObjectByType objectType, GameObject currentGameObject)
		{
			if (currentGameObject == null)
				return false;

			int internalListCount	= ObjectPoolList.Count;
			int index				= 0;
			while(index < internalListCount)
			{
				string currentObjectTypeString = string.Empty;
				string returningObjectString = string.Empty;

				if(ObjectPoolList[index] != null)
				{
					switch (objectType)
					{
					case GetObjectByType.Tag:
						currentObjectTypeString = ObjectPoolList[index].tag;
						returningObjectString = currentGameObject.tag;
						break;
					case GetObjectByType.Name:
						currentObjectTypeString = ObjectPoolList[index].name;
						returningObjectString = currentGameObject.name;
						break;
					case GetObjectByType.GameObject:
						currentObjectTypeString = ObjectPoolList[index].ToString();
						returningObjectString = currentGameObject.ToString();
						break;
					default:
						return false;
					}

					if (string.Equals(currentObjectTypeString, returningObjectString) == true)
					{
						if(currentGameObject != null)
						{
							DeactivateObject(currentGameObject);
							m_internalObjectPool[index].Add(currentGameObject);
							return true;
						}
					}
				}
				index += 1;
			}

			return false;
		}

		/// <summary>
		/// Get the internal index value of a certain object list.
		/// </summary>
		/// <param name="currentList">The list which you want to get the index value for.</param>
		/// <returns>The index value. Returns -1 if the list was not found.</returns>
		private int GetListIndex(List<GameObject> currentList)
		{
			int internalListCount	= m_internalObjectPool.Count;
			int index				= 0;
			while(index < internalListCount)
			{
				if (m_internalObjectPool[index] == currentList)
					return index;
				else
					index += 1;
			}
			return -1;
		}

		/// <summary>
		/// Performs a search operation to find a matching object pool.
		/// </summary>
		/// <param name="objectType">The type of search</param>
		/// <param name="typeString">Search string based on the type given above.</param>
		/// <returns>Returns the object list which belongs to the searchewd object.</returns>
		private List<GameObject> FindObjectPoolList(GetObjectByType objectType, string typeString)
		{
			int internalListCount	= m_internalObjectPool.Count;
			int index				= 0;
			while(index < internalListCount)
			{
				string currentObjectTypeString = string.Empty;

				if(ObjectPoolList[index] != null)
				{
					switch (objectType)
					{
					case GetObjectByType.Tag:
						currentObjectTypeString = ObjectPoolList[index].tag;
						break;
					case GetObjectByType.Name:
						currentObjectTypeString = ObjectPoolList[index].name;
						break;
					case GetObjectByType.GameObject:
						currentObjectTypeString = ObjectPoolList[index].name.ToString();
						break;
					default:
						return null;
					}
					 
					if (string.Equals(currentObjectTypeString, typeString) == true)
						return m_internalObjectPool[index];
					else
						index += 1;
				}
				else
					index += 1;
			}

			return null;
		}

		/// <summary>
		/// Checks for how many objects there is in a object pool.
		/// If there isn't any objects left, then it will automatically create new ones.
		/// </summary>
		/// <param name="currentObjectList">The list Which will be checked.</param>
		/// <param name="index">The index value for the above list.</param>
		/// <returns>0 when finished.</returns>
		private IEnumerator CheckForObjectPoolCount(List<GameObject> currentObjectList, int index)
		{
			yield return new WaitForSeconds(3.05f);

			int minNumberOfObjectsInPool = ObjectCountList[index];

			if (currentObjectList.Count == 0)
			{
				int iterator = 0;
				while(iterator < minNumberOfObjectsInPool)
				{
					if(iterator >= minNumberOfObjectsInPool-1)
						yield return 0;

					if(ObjectPoolList != null)
					{
						if (ObjectPoolList[index] != null)
					{
						float delayValue = (Time.fixedTime * (float)iterator);
						StartCoroutine(CreateObjectAndAddToList(ObjectPoolList[index], iterator, delayValue));
					}
					}
					iterator += 1;
				}
			}

			yield return 0;
		}

		/// <summary>
		/// Forces the Pool mananger to create a new batch of all objects.
		/// </summary>
		public void ForceAddNewObjectsToObjectPool()
		{
			int internalListCount	= m_internalObjectPool.Count;
			int listIt				= 0;
			while(listIt < internalListCount)
			{
				int minNumberOfObjectsInPool	= ObjectCountList[listIt];
				int index						= 0;
				while(index < minNumberOfObjectsInPool)
				{
					if (ObjectPoolList[listIt] == null)
						continue;
					float DelayValue = (0.5f * (float)index);
					StartCoroutine(CreateObjectAndAddToList(ObjectPoolList[listIt], listIt, DelayValue));
					index += 1;
				}
				listIt += 1;
			}
		}

		/// <summary>
		/// Adds a action to notify when the pool manager is finished creating all objects.
		/// </summary>
		/// <param name='newAction'>
		/// The action to add to the object pool manager.
		/// </param>
		public void AddActionToFinishedCreatingObjects(Action newAction)
		{
			m_doneCreatingObjects += newAction;
		}

		/// <summary>
		/// Activates when the object pool manager is finished creating objects.
		/// </summary>
		protected IEnumerator ActivateFinishedCreatingObjectsAction()
		{
			yield return new WaitForSeconds(TimeDelayToSendActionNotification);

			if (Time.timeScale != 0.0f)
			{
				if (m_doneCreatingObjects != null)
					m_doneCreatingObjects();
			}
		}

		/// <summary>
		/// Raises the level was loaded event.
		/// </summary>
		/// <param name='level'>
		/// The loaded level.
		/// </param>
		private void OnLevelWasLoaded(int level)
		{
			m_doneCreatingObjects = null;
		}
	}
}
