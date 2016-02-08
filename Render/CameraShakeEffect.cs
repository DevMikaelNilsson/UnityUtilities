using UnityEngine;
using System.Collections;

namespace mnUtilities.Render
{
	public class CameraShakeEffect : MonoBehaviour 
	{
		/// <summary>
		/// The camera object which the shake effect should be affecting.
		/// The effect will not work without a valid camera object.
		/// </summary>
		public Camera AffectedCamera = null;
	
		[Tooltip("Set to true to set the camera back to its orginal position and rotation (the orginal position and rotation is set when the component is enabled/re-enabled) before a shake effect is activated.")]
		public bool ResetCameraPosition = true;
	
		[Tooltip("Set to true to allow to activate a new shake effect, even when a current shake effect is active.")]
		public bool AllowToOverrideActiveShake = false;

		/// <summary>
		/// Allows/disallows the effect to change the position of the camera during the effect.
		/// </summary>
		public bool ChangeCameraPosition = false;
	
		/// <summary>
		/// Allows/disallows the effec to change the rotation of the camera during the effect.
		/// </summary>
		public bool ChangeCameraRotation = true;

		/// <summary>
		/// The starting intensity value of the shake when the effect is activated.
		/// The intensity value will decrease with time, depending on the shake decay value. The current intesity value
		/// is stored internally.
		/// </summary>
		public float coef_shake_intensity = 0.3f;

		/// <summary>
		/// This controls the decay (or time) it takes for the shake effect to stop. The higher this value is, the faster
		/// will the effect subside and disappear. The lower it is the longer will the effect last.
		/// </summary>
		public float shake_decay = 0.002f;
	
		/// <summary>
		/// Internal variable to store the current intensity value for the shake effect.
		/// </summary>
		private float m_currentShakeIntensity = 0.0f; 
	
		private float m_currentShakeDecay = 0.0f;

		/// <summary>
		/// Internal variable to store the original position of the camera before the shake effect was activated.
		/// </summary>
		private Vector3 m_originalPosition = Vector3.zero;  
	
		/// <summary>
		/// Internal variable to store the original rotation of the camera before the shake effect was activated.
		/// </summary>
		private Quaternion m_originalRotation = Quaternion.identity;  
	  
		void OnEnable()
		{
			m_originalPosition = transform.position;
			m_originalRotation = transform.rotation;
		}

		/// <summary>
		/// Internal Unity method.
		/// This method is called once every frame. But after all other updates has been done.
		/// The method updates the camera depending on the current shake variables. The shake effect is only calculated
		/// when the internal current intensity variable is above 0 (zero).
		/// </summary>
		void LateUpdate()
		{
			if (m_currentShakeIntensity > 0)
			{
				if(ChangeCameraPosition == true)
					AffectedCamera.transform.position = m_originalPosition + Random.insideUnitSphere * m_currentShakeIntensity;
	
				if(ChangeCameraRotation == true)
					AffectedCamera.transform.rotation = new Quaternion(
								m_originalRotation.x + Random.Range(-m_currentShakeIntensity, m_currentShakeIntensity) * 0.2f,
								m_originalRotation.y + Random.Range(-m_currentShakeIntensity, m_currentShakeIntensity) * 0.2f,
								m_originalRotation.z + Random.Range(-m_currentShakeIntensity, m_currentShakeIntensity) * 0.2f,
								m_originalRotation.w + Random.Range(-m_currentShakeIntensity, m_currentShakeIntensity) * 0.2f);
	
				m_currentShakeIntensity -= m_currentShakeDecay;
			}
			else if(m_currentShakeIntensity <= 0)
				m_currentShakeIntensity = 0.0f;
		}

		/// <summary>
		/// Activates the shake effect.
		/// The shake effect is applied to the camera which is set through
		/// the public variable. If a Camera is not set, the method will attempt
		/// to find a Camera component on its GameObject to apply the shake effect to.
		/// </summary>
		public void Shake()
		{
			ActivateShake(coef_shake_intensity, shake_decay);
		}

		/// <summary>
		/// Activates the shake effect.
		/// The shake effect is applied to the inparameter camera.
		/// If the inparameter Camera is empty or not valid, the method the method will attempt
		/// to find a Camera component on its GameObject to apply the shake effect to. 
		/// </summary>
		/// <param name="useCamera"></param>
		public void Shake(Camera useCamera)
		{
			AffectedCamera = useCamera;
			ActivateShake(coef_shake_intensity, shake_decay);
		}

		public void Shake(float intensity, float decay)
		{
			ActivateShake(intensity, decay);
		}
	
		public void Shake(Camera useCamera, float intensity, float decay)
		{
			ActivateShake(intensity, decay);
		}

		private void ActivateShake(float shakeItensity, float shakeDecay)
		{
			if (AllowToOverrideActiveShake == false && m_currentShakeIntensity > 0.0f)
				return;
	
			if (ResetCameraPosition == true)
			{
				transform.position = m_originalPosition;
				transform.rotation = m_originalRotation;
			}
			else
			{
				m_originalPosition = transform.position;
				m_originalRotation = transform.rotation;
			}
	
			m_currentShakeIntensity = shakeItensity;
			m_currentShakeDecay = shakeDecay;
			
			if (AffectedCamera == null)
			{
				Camera currentCameraObject = this.GetComponent<Camera>();
				if (currentCameraObject != null)
					AffectedCamera = currentCameraObject;
				else
				{
					Debug.LogError(this + " - No valid Camera component is found. Can not perform Camera shake effect.");
					m_currentShakeIntensity = 0.0f;
				}
			}
		}
	}
}
