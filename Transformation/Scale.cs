using UnityEngine;
using System.Collections;

namespace mnUtilities.Transformation
{
	[AddComponentMenu("Transformation/Scale")]
	public class Scale : MonoBehaviour 
	{
		/// <summary>
		/// The supported types of scaling the component is supporting.
		/// </summary>
		public enum ScaleType
		{
			/// <summary>
			/// The scaling is done once from start to end, and then it will stop. The scaling can be manually restarted.
			/// </summary>
			OneTime = 0,

			/// <summary>
			/// The object is scaled endlessly in a loop from start to end.
			/// </summary>
			Loop = 1,

			/// <summary>
			/// The object is scaled endlessly. The scaling begins with the start scale, and then starts with the end scale etc etc. This gives a soft scale animation.
			/// </summary>
			PingPong = 2
		}

		[Tooltip("The object which will be rotated. If empty the object, which the component is attached to, will be scaled.")]
		public Transform ScaleObject = null;
		[Tooltip("Type of scale mode which can be used.\n\nOneTime:\nThe scaling is done once from start to end, and then it will stop. The scaling can be manually restarted.\n\nLoop:\nThe object is scaled endlessly in a loop from start to end.\n\nPingPong:\nThe object is scaled endlessly. The scaling begins with the start scale, and then starts with the end scale etc etc. This gives a soft scale animation.")]
		public ScaleType TypeOfScaleMode = ScaleType.OneTime;
		[Tooltip("The starting scale size for the object. The current object scale size is ignored and this scale value overwrites the current scale size.")]
		public Vector3 StartScaleSize = Vector3.one;
		[Tooltip("The ending scale size for the object.")]
		public Vector3 EndScaleSize = Vector3.zero;
		[Tooltip("The amount of time (in seconds) it takes to scale the object from the start scale size to the end scale size.")]
		public float Duration = 1.0f;
		[Tooltip("Flag to determin if the scale animation should be active from the start. The scale animation can be started manually.")]
		public bool ActiveAtStart = true;

		private float m_elapsedTime = 0.0f;
		private bool m_isActive = false;
		private bool m_scaleIsAscending = false;
		private Transform m_transformComponent = null;

		/// <summary>
		/// Internal Unity method.
		/// This method is called once every time the component is enabled.
		/// Setup all needed variables and components.
		/// </summary>
		void OnEnable () 
		{
			m_elapsedTime = 0.0f;
			m_isActive = ActiveAtStart;
			m_scaleIsAscending = false;

			if(ScaleObject != null)
				m_transformComponent = ScaleObject;
			else
				m_transformComponent = this.GetComponent<Transform>();
				
		}

		/// <summary>
		/// Internal Unity method.
		/// This method is called once per update cycle.
		/// If active, the method will scale the object.
		/// </summary>
		void Update () 
		{
			if(m_isActive == true)
			{
				m_elapsedTime += (Time.deltaTime * Time.timeScale);
				float procentage = (m_elapsedTime / Duration);

				switch(TypeOfScaleMode)
				{
					case ScaleType.PingPong:
						if(m_scaleIsAscending == true)
							procentage = (1.0f - procentage);
						break;
				}

				if(procentage <= 0.0f || procentage >= 1.0f)
				{
					switch(TypeOfScaleMode)
					{
						case ScaleType.Loop:
							m_elapsedTime = 0.0f;
							break;
						case ScaleType.PingPong:
							m_elapsedTime = 0.0f;
							if(m_scaleIsAscending == true)
								m_scaleIsAscending = false;
							else
								m_scaleIsAscending = true;
							break;
						default:
							m_isActive = false;
							break;
					}
				}

				m_transformComponent.localScale = Vector3.Lerp(StartScaleSize, EndScaleSize, procentage);				
			}
		}

		/// <summary>
		/// Activates the scale component with all the set variables.
		/// The objects scale value will be set to the starting scale value by default.
		/// </summary>
		public void StartScale()
		{
			m_elapsedTime = 0.0f;
			m_isActive = true;
		}

		/// <summary>
		/// Activates the scale component with all the set variables.
		/// The objects scale value will be set to the starting scale value by default.
		/// </summary>
		/// <param name="typeOfScaleMode">The type of scale mode which should be used. This variable overrides the existing scale mode setting for this component.</param>
		public void StartScale(ScaleType typeOfScaleMode)
		{
			m_elapsedTime = 0.0f;
			TypeOfScaleMode = typeOfScaleMode;
			m_isActive = true;
		}

		/// <summary>
		/// Stops the scale animation. 
		/// The internal timer values are resetted, so in order to properly restart
		/// the scale animation, the StartScale method should be called.
		/// </summary>
		/// <param name="resetScaleToStartValue">Set flag to true to reset the current scale value to the starting scale value. Set flag to false to leave the current scale value as it is.</param>
		public void StopScale(bool resetScaleToStartValue)
		{
			m_isActive = false;
			m_elapsedTime = 0.0f;
			if(resetScaleToStartValue == true)
				m_transformComponent.localScale = StartScaleSize;
		}

		/// <summary>
		/// Pauses the scale animation. No values are resetted.
		/// To resume the scale animation, call the ResumeScale method.
		/// </summary>
		public void PauseScale()
		{
			m_isActive = false;
		}

		/// <summary>
		/// Resumes the scale animation from the last active point.
		/// This method should only be called after the PauseScale method has been called, to
		/// avoid unwanted behaviours.
		/// </summary>
		public void ResumeScale()
		{
			m_isActive = true;
		}
	}
}
