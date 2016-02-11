using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System;

namespace mnUtilities.Utilities
{
	[AddComponentMenu("Utilities/InvokeMethod")]
	public class InvokeMethod : MonoBehaviour 
	{
		public GameObject ReceiveObject = null;
		public string ReceiveComponent = string.Empty;
		public string ReceiveMethod = string.Empty;

		/// <summary>
		/// The cached method info object used for when invoking the method when a press gesture is recognized.
		/// </summary>
		private MethodInfo m_methodInfoComponent = null;

		/// <summary>
		/// The component object the method is attached to.
		/// </summary>
		private Component m_componentObject = null;

		void OnEnable()
		{
			LoadMethod();
		}

		/// <summary>
		/// Invokes the registered method.
		/// </summary>
		public void InvokeMethod()
		{
			InvokeMethod(null);
		}

		/// <summary>
		/// Invokes the registered method.
		/// </summary>
		/// <param name="parameters">Array of parameters which will be send with the invoke.</param>
		public void InvokeMethod(object []parameters)
		{
			// Eventhough the invoke method should always be activated, I keep the send message method for backup.
			// The SendMessage probably costs more performance when called every FixedUpdate, and should be avoided.
			if (m_methodInfoComponent != null && m_componentObject != null)
			{
				try
				{
					m_methodInfoComponent.Invoke(m_componentObject, parameters);
				}
				catch(Exception e)
				{
					Debug.LogError(this + " - Failed to invoke method:\n" + e.ToString());
				}
			}
			else if (ReceiveObject != null)
				ReceiveObject.SendMessage(ReceiveMethod, SendMessageOptions.RequireReceiver);
		}

		/// <summary>
		/// Invokes the registered method.
		/// </summary>
		public void InvokeMethod()
		{
			InvokeMethod(null);
		}

		/// <summary>
		/// Tries to find the proper MethodInfo object based on the GameObject and a method string.
		/// </summary>
		public void LoadMethod(GameObject newReceiveObject, string newReceiveMethod)
		{
			ReceiveObject = newReceiveObject;
			ReceiveMethod = newReceiveMethod;
			LoadMethod();
		}

		/// <summary>
		/// Tries to find the proper MethodInfo object based on the GameObject and a method string.
		/// </summary>
		public void LoadMethod()
		{
			MethodInfo[] ListOFMethods = GetAssemblyMethodArray(ReceiveObject);
			if (ListOFMethods == null)
				return;

			int objectCount = ListOFMethods.Length;
			for (int i = 0; i < objectCount; ++i)
			{
				if (ListOFMethods[i].Name.Equals(ReceiveMethod) == true)
				{
					m_methodInfoComponent = ListOFMethods[i];
					m_componentObject = GetAssemblyComponent(ReceiveObject, ReceiveMethod, null);
					if(m_componentObject != null)
					{
						ReceiveComponent = m_componentObject.ToString();
						break;
					}
				}
			}
		}

		/// <summary>
		/// Adds all methods which is referenced to the current object.
		/// </summary>
		/// <param name="currentObject">The current object which the methods will be collected from.</param>
		/// <returns>An MethodInfo array with all added methods.</returns>
		private MethodInfo[] GetAssemblyMethodArray(GameObject currentObject)
		{
			if (currentObject == null)
				return null;

			List<MethodInfo> MethodList = new List<MethodInfo>();
			Component[] objectComponentList = currentObject.GetComponents(typeof(MonoBehaviour));
			Assembly[] referencedAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
			int componentCount = objectComponentList.Length;
			int assemblyCount = referencedAssemblies.Length;

			for (int componentIndex = 0; componentIndex < componentCount; ++componentIndex)
			{
				if (objectComponentList[componentIndex] == null)
					continue;

				System.Type componentType = objectComponentList[componentIndex].GetType();
				string componentNameString = componentType.ToString();

				for (int assemblyIndex = 0; assemblyIndex < assemblyCount; ++assemblyIndex)
				{
					System.Type type = referencedAssemblies[assemblyIndex].GetType(componentNameString);

					if (type != null)
					{
						System.Reflection.MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
						for (int methodIterator = 0; methodIterator < methods.Length; ++methodIterator)
							MethodList.Add(methods[methodIterator]);
					}
				}
			}

			return MethodList.ToArray();
		}

		/// <summary>
		/// Retrieves a component from the current GameObject which matches with the method inparameter.
		/// The method tries to find out which of the objects components the method belongs to and returns it.
		/// </summary>
		/// <param name="currentObject">The object the method is connected to.</param>
		/// <param name="methodName">The name of the method which needs to be found and invoked.</param>
		/// <param name="inparameters">Optional inparameters for the method.</param>
		private Component GetAssemblyComponent(GameObject currentObject, string methodName, object[] inparameters)
		{
			Component[] objectComponentList = currentObject.GetComponents(typeof(MonoBehaviour));
			int componentObjectCount = objectComponentList.Length;

			Assembly[] referencedAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
			int referenceAssemblyObjectCount = referencedAssemblies.Length;

			for (int i = 0; i < componentObjectCount; ++i)
			{
				if (objectComponentList[i] == null)
					continue;

				System.Type componentType = objectComponentList[i].GetType();
				string componentNameString = componentType.ToString();

				for (int assemblyObjectIterator = 0; assemblyObjectIterator < referenceAssemblyObjectCount; ++assemblyObjectIterator)
				{
					System.Type assemblyComponentType = referencedAssemblies[assemblyObjectIterator].GetType(componentNameString);
					if (assemblyComponentType != null && methodName != string.Empty)
					{
						MethodInfo foundMethodObject = null;
						try
						{
							foundMethodObject = assemblyComponentType.GetMethod(methodName);
						}
						catch
						{
							Debug.LogWarning(this + " - Multiple methods with the same name (" + methodName + ") was found. The method can not choose which of these methods to get. Please recheck the mentod names.");
						}

						if (foundMethodObject != null)
							return objectComponentList[i];
					}
				}
			}

			StringBuilder errorString = new StringBuilder();
			errorString.Append(this.ToString());
			errorString.Append(" - ");
			errorString.Append(this.gameObject.ToString());
			errorString.Append(" - No valid component was found for '");
			errorString.Append(currentObject.ToString());
			errorString.Append("' with the method '");
			errorString.Append(methodName.ToString());
			errorString.Append("'.");
			errorString.Append("Make sure the method in the affect script is set as public, and not private.");
			Debug.LogError(errorString.ToString());
			return null;
		}
	}
}
