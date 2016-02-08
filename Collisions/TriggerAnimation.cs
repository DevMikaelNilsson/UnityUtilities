using UnityEngine;
using System.Collections;
using System;

namespace mnUtilities.Collisions
{
	public class TriggerAnimation : MonoBehaviour 
	{
		/// <summary>
		/// Enable flag to trigger the animation every time the component is activated/re-activated.
		/// </summary>
		[Tooltip("Enable flag to trigger the animation every time the component is activated/re-activated.")]
		public bool TriggerOnStart = false;
		/// <summary>
		/// Reference to the Animator animation controller. Leave empty if this gameobject already has a Animator component attached.
		/// </summary>
		[Tooltip("Reference to the Animator animation controller. Leave empty if this gameobject already has a Animator component attached.")]
		public Animator AnimController = null;

		/// <summary>
		/// State string (name) of the animation which will be triggered by this component.
		/// </summary>
		[Tooltip("State string (name) of the animation which will be triggered by this component.")]
		public string AnimStateString = string.Empty;

		private bool m_isTriggered = false;

		/// <summary>
		/// Internal Unity method.
		/// This method is called every time the component is activated/re-activated.
		/// The method loads the required variables and checks if the animation should be triggered or not.
		/// </summary>
		void OnEnable()
		{	
			SetupAnimationTrigger();
			ResetTrigger();

			if(TriggerOnStart == true)
				ActivateTrigger();
		}

		/// <summary>
		/// Setups all variables which are required for the trigger to work properly.
		/// </summary>
		private void SetupAnimationTrigger()
		{
			if(AnimController == null)
				AnimController = this.GetComponent<Animator>();
		}
		/// <summary>
		/// Resets the trigger.
		/// If the trigger was previously activated, this method needs to be called
		/// before the trigger can be used once again.
		/// </summary>
		public void ResetTrigger()
		{
			m_isTriggered = false;
		}

		/// <summary>
		/// Activates the trigger by accessing the Animator and attempt to play the animation state.
		/// </summary>
		public void ActivateTrigger()
		{
			if(m_isTriggered == false)
			{
				m_isTriggered = true;
				SetupAnimationTrigger();
				if(AnimController != null && string.Equals(AnimStateString, string.Empty) == false)
				{
					try
					{
						AnimController.Play(AnimStateString);
					}
					catch(Exception e)
					{
						Debug.LogError(this + " - While attempting to play a Animation state (" + AnimStateString + "), the Component caused a exception:\n" + e.ToString());
					}
				}
			}
		}

		/// <summary>
		/// Internal Unity method.
		/// This method is called whenever a collision is registered.
		/// This method activates the trigger functionality.
		/// </summary>
		/// <param name="collision">Collision data about the other colliding object.</param>
		void OnCollisionEnter(Collision collision) 
		{
			ActivateTrigger();
		}

		/// <summary>
		/// Internal Unity method.
		/// This method is called whenever a collision is registered.
		/// This method activates the trigger functionality.
		/// </summary>
		/// <param name="other">Collision data about the other colliding object.</param>
		void OnTriggerEnter(Collider other)
		{
			ActivateTrigger();
		}
	}
}
