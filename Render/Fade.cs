using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace mnUtilities.Render
{
	public class Fade : MonoBehaviour 
	{
		/// <summary>
		/// Enum flag to determin what the current status is.
		/// </summary>
		private enum FadeStatus
		{
			/// <summary>
			/// The component is waiting to be activated by either a fade in or a fade out call.
			/// The component is running but doesn't perform any calculations.
			/// </summary>
			Waiting = 0,

			/// <summary>
			/// The component is currently performing a fade in calculation on the object and child(ren) if applicable.
			/// The object is set to be invisible at start, and will over time be fully visible.
			/// </summary>
			FadeIn = 1,

			/// <summary>
			/// The component is currently performing a fade out calculation on the object and child(ren) if applicable.
			/// The object is set to be fully visible at start, and will over time be complete invisible.
			/// </summary>
			FadeOut = 2
		}
		
		/// <summary>
		/// Object which will be faded out/in by the component. If left empty, the component will use the GameObject which it is attached to.		
		/// </summary>
		[Tooltip("Object which will be faded out/in by the component. If left empty, the component will use the GameObject which it is attached to.")]
		public GameObject FadingObject = null;
				
		/// <summary>
		/// Optional object to display instead of the FadingObject after the fade in is done. When fade in is completed the FadingObject is disabled and this object is enabled. 
		/// This is usefull if a object should only be transparent while fading
		/// </summary>
		[Tooltip("Optional object to display instead of the FadingObject after the fade in is done. When fade in is completed the FadingObject is disabled and this object is enabled. This is usefull if a object should only be transparent while fading.")]
		public GameObject OpaqueObject = null;
		
		/// <summary>
		/// Enable to perform a fade in on the object when it has been enabled/re-enabled.
		/// </summary>
		[Tooltip("Enable to perform a fade in on the object when it has been enabled/re-enabled.")]
		public bool FadeInOnStart = false;
		
		/// <summary>
		/// Delay (in seconds) until a fade in is started. This delay is only applicable when the 'FadeInOnStart' flag is enabled.
		/// </summary>
		[Tooltip("Delay (in seconds) until a fade in is started. This delay is only applicable when the 'FadeInOnStart' flag is enabled.")]
		public float DelayFadeIn = 1.0f;
		
		/// <summary>
		/// Enable to allow all the child object(s) to be included into the fade calculation.
		/// </summary>
		[Tooltip("Enable to allow all the child object(s) to be included into the fade calculation.")]
		public bool IncludeChildObjects = true;
		
		/// <summary>
		/// The amount of time (in seconds) it takes to fully fade in/out the object and its children objects.
		/// </summary>
		[Tooltip("The amount of time (in seconds) it takes to fully fade in/out the object and its children objects.")]
		public float Duration = 1.0f;

		private FadeStatus m_currentFadeStatus = FadeStatus.Waiting;
		private List<Renderer> m_meshRenderComponents = new List<Renderer>();
		private float m_elapsedTime = 0.0f;
		private float m_currentDuration = 0.0f;

		/// <summary>
		/// Internal Unity method.
		/// This method is called every time the object/component is enabled/re-enabled.
		/// If the internal render object list is empty, it will automaticaly filled.
		/// If flag is enabled, the method will start a fade in calculation.
		/// </summary>
		void OnEnable()
		{
			if(FadingObject == null)
				FadingObject = this.gameObject;
				
			if(m_meshRenderComponents.Count <= 0)
				LoadMeshRendererComponents();
							
			if(FadeInOnStart == true)
			{
				SetAlphaValue(0.0f);
				StartCoroutine(DelayStartFadeIn());
			}
		}
		
		private IEnumerator DelayStartFadeIn()
		{
			yield return new WaitForSeconds(DelayFadeIn);
			StartFadeIn();
		}

		/// <summary>
		/// Internal Unity method.
		/// This method is called once every update cycle.
		/// The method updates the objects visibillity if a fade calculation is currently
		/// active.
		/// </summary>
		void Update()
		{
			switch (m_currentFadeStatus)
			{
				case FadeStatus.FadeIn:
				case FadeStatus.FadeOut:
					FadeObject();
					break;
			}
		}

		/// <summary>
		/// Calculates the objects current visibillity.
		/// </summary>
		private void FadeObject()
		{
			m_elapsedTime += (Time.deltaTime * Time.timeScale);
			float procentage = Mathf.Clamp((m_elapsedTime / m_currentDuration), 0.0f, 1.0f);
			float currentAlphaValue = 1.0f;
			switch (m_currentFadeStatus)
			{
			case FadeStatus.FadeIn:
				currentAlphaValue = procentage;
				break;
			case FadeStatus.FadeOut:
				currentAlphaValue = (1.0f - procentage);
				break;
			default:
				Debug.Log(this + " - Component attempts to calculate the objects visibillity, but none of the fade options are active.");
				return;
			}
			
			SetAlphaValue(currentAlphaValue);

			if(procentage >= 1.0f)
				FadeObjectFinished();
		}
		
		/// <summary>
		/// This method gets called whenever the fade (in/out) calculation is finished, and reset the fade variables.
		/// </summary>
		private void FadeObjectFinished()
		{
			ToggleOpaqueObject(true);
			m_currentFadeStatus = FadeStatus.Waiting;
		}
		
		/// <summary>
		/// Set a alpha value on all registered render objects.
		/// </summary>
		/// <param name="currentAlphaValue">Current alpha value.</param>
		private void SetAlphaValue(float currentAlphaValue)
		{
			int objectCount = m_meshRenderComponents.Count;
			for(int i = 0; i < objectCount; ++i)
				SetMaterialAlpha(m_meshRenderComponents[i], currentAlphaValue);
		}

		/// <summary>
		/// Starts to fade in the current object and its children (if flag is enabled).
		/// The object(s) will start as invisible (alpha channel set to 0.0f), and will over time
		/// become more visible until complete visible at the end of the duration.
		/// </summary>
		public void StartFadeIn()
		{
			SetFadeProperties(FadeStatus.FadeIn, Duration);
		}

		/// <summary>
		/// Starts to fade in the current object and its children (if flag is enabled).
		/// The object(s) will start as invisible (alpha channel set to 0.0f), and will over time
		/// become more visible until complete visible at the end of the duration.
		/// </summary>
		/// <param name="duration">Amount of time (in seconds) the fade in should take.</param>
		public void StartFadeIn(float duration)
		{
			SetFadeProperties(FadeStatus.FadeIn, duration);
		}

		/// <summary>
		/// Starts to fade out the current object and its children (if flag is enabled).
		/// The object(s) will start as fully visible (alpha channel set to 1.0f), and will over time
		/// become more transparent until complete invisible at the end of the duration.
		/// </summary>
		public void StartFadeOut()
		{
			SetFadeProperties(FadeStatus.FadeOut, Duration);
		}

		/// <summary>
		/// Starts to fade out the current object and its children (if flag is enabled).
		/// The object(s) will start as fully visible (alpha channel set to 1.0f), and will over time
		/// become more transparent until complete invisible at the end of the duration.
		/// </summary>
		/// <param name="duration">Amount of time (in seconds) the fade out should take.</param>
		public void StartFadeOut(float duration)
		{
			SetFadeProperties(FadeStatus.FadeOut, duration);
		}

		/// <summary>
		/// Resets and update the fade data.
		/// The method will activate the fade calculation by default.
		/// </summary>
		/// <param name="newFadeStatus">The new fade status. The status decides what type of calculation will be applied to the object.</param>
		/// <param name="duration">Amount of time (in seconds) the fade should take.</param>
		private void SetFadeProperties(FadeStatus newFadeStatus, float duration)
		{
			m_elapsedTime = 0.0f;
			m_currentDuration = duration;
			m_currentFadeStatus = newFadeStatus;
			switch(m_currentFadeStatus)
			{
				case FadeStatus.FadeIn:
				case FadeStatus.FadeOut:
					ToggleOpaqueObject(false);
					break;
			}
		}
		
		/// <summary>
		/// Toggles the OpaqueObject to be enabled or disabled.
		/// Whenever the OpaqueObject is enabled, the fadingObject is disabled and vice versa.
		/// If the OpaqueObject is not valid or empty, this method will be ignored.
		/// </summary>
		/// <param name="setOpaqueAsActive">Set to true, to enable the OpaqueObject. Set to false otherwise.</param>
		private void ToggleOpaqueObject(bool setOpaqueAsActive)
		{	
			if(OpaqueObject != null)
			{
				if(setOpaqueAsActive == true)
				{
					OpaqueObject.SetActive(true);
					FadingObject.SetActive(false);
				}
				else
				{
					OpaqueObject.SetActive(false);
					FadingObject.SetActive(true);
				}
			}
		}

		/// <summary>
		/// Retrieves and stores the objects render component(s) so a fade can be made.
		/// If set to do so, the method will also store the render component(s) of the objects
		/// children as well.
		/// </summary>
		public void LoadMeshRendererComponents()
		{
			if(FadingObject != null)
			{
				m_meshRenderComponents.Clear();
				SkinnedMeshRenderer selfSkinnedMeshComponent = FadingObject.GetComponent<SkinnedMeshRenderer>();
				MeshRenderer selfMeshRenderComponent = FadingObject.GetComponent<MeshRenderer>();
				if(selfSkinnedMeshComponent != null)
					m_meshRenderComponents.Add(selfSkinnedMeshComponent);
				if (selfMeshRenderComponent != null)
					m_meshRenderComponents.Add(selfMeshRenderComponent);
	
				if(IncludeChildObjects == true)
				{
					SkinnedMeshRenderer []skinnedRender = FadingObject.GetComponentsInChildren<SkinnedMeshRenderer>();
					MeshRenderer[] meshRender = FadingObject.GetComponentsInChildren<MeshRenderer>();
	
					int objectCount = skinnedRender.Length;
					for(int i = 0; i < objectCount; ++i)
						m_meshRenderComponents.Add(skinnedRender[i]);
	
					objectCount = meshRender.Length;
					for (int i = 0; i < objectCount; ++i)
						m_meshRenderComponents.Add(meshRender[i]);
				}
			}
			else
				Debug.LogError(this + " - GameObject is either missing or not valid. Can not retrieve MeshRender component(s).");
		}

		/// <summary>
		/// Updates the current alpha value for a render components all registered materials.
		/// </summary>
		/// <param name="currentRenderObject">The current render component which stores the materials.</param>
		/// <param name="alphaValue">The new alpha value for the material(s).</param>
		private void SetMaterialAlpha(Renderer currentRenderObject, float alphaValue)
		{
			int objectCount = currentRenderObject.materials.Length;
			for (int i = 0; i < objectCount; ++i)
			{
				Color currentColor = currentRenderObject.materials[i].color;
				currentColor.a = alphaValue;
				currentRenderObject.materials[i].color = currentColor;
			}
		}
	}
}
