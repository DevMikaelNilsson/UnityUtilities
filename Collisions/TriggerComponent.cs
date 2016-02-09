using UnityEngine;
using System.Collections;
using mnUtilities.Utilities;

namespace mnUtilities.Collisions
{
	public class TriggerComponent : MonoBehaviour 
	{
		[System.Serializable]
		public struct TriggerProperties
		{
			public GameObject TriggerObject;
			public Behaviour TriggerComponent;
		}

		/// <summary>
		/// Enable flag to disables the component(s) at startup. All these component(s) are re-enabled when a trigger (collision) is registered.
		/// </summary>
		[Tooltip("Enable flag to disables the component(s) at startup. All these component(s) are re-enabled when a trigger (collision) is registered.")]
		public bool DisableOnStartup = true;

		public float TriggerDelay = 0.0f;

		/// <summary>
		/// Component(s) which will be enabled/re-enabled whenever a trigger (collision) is registered.
		/// </summary>
		[Tooltip("Component(s) which will be enabled/re-enabled whenever a trigger (collision) is registered.")]
		public TriggerProperties []EnableComponents = null;

		public InvokeMethodHandler []InvokeOnTrigger = null;

		private bool m_isTriggered = false;

		/// <summary>
		/// Internal Unity method.
		/// This method is called everytime the compnent is enalbed/re-enabled.
		/// The method sets all values to their default values.
		/// </summary>
		void OnEnable()
		{
			if(DisableOnStartup == true)
				ToggleTrigger(false);
		}

		/// <summary>
		/// Toggles the component to activate its trigger or to disable the trigger.
		/// Activating the object(s) and component(s) can only be done once. If the object(s) and component(s) are already
		/// activated by this trigger when this method is called, then the method is ignored. 
		/// The object(s) and component(s) needs to be resetted/disabled (ex. by calling this method with a 'false' parameter) before they can be triggered/enabled once more.
		/// </summary>
		/// <param name="isActive">Set to true to activate trigger. Set to false to reset/disable the triggere object(s) and component(s).</param>
		public void ToggleTrigger(bool isActive)
		{
			if(isActive == true)
				StartCoroutine(DelayToggleTrigger(TriggerDelay, true));
			else
				StartCoroutine(DelayToggleTrigger(0.0f, false));
		}

		private IEnumerator DelayToggleTrigger(float delay, bool isActive)
		{
			yield return new WaitForSeconds(delay);
			if(m_isTriggered == false)
			{
				int objectCount = EnableComponents.Length;
				for(int i = 0; i < objectCount; ++i)
				{
					if(EnableComponents[i].TriggerComponent != null)
						EnableComponents[i].TriggerComponent.enabled = isActive;
				}

				if(isActive == true)
				{
					objectCount = InvokeOnTrigger.Length;
					for(int i = 0; i < objectCount; ++i)
					{
						if(InvokeOnTrigger[i] != null)
							InvokeOnTrigger[i].InvokeMethod();
					}
				}
			}

			m_isTriggered = isActive;
		}

		/// <summary>
		/// Internal Unity method.
		/// This method is called whenever a collision is registered.
		/// This method activates the trigger functionality.
		/// </summary>
		/// <param name="collision">Collision data about the other colliding object.</param>
		void OnCollisionEnter(Collision collision) 
		{
			ToggleTrigger(true);
		}

		/// <summary>
		/// Internal Unity method.
		/// This method is called whenever a collision is registered.
		/// This method activates the trigger functionality.
		/// </summary>
		/// <param name="other">Collision data about the other colliding object.</param>
		void OnTriggerEnter(Collider other)
		{
			ToggleTrigger(true);
		}
	}
}
