using UnityEngine;
using System.Collections;

namespace mnUtilities.Transformation
{
	[AddComponentMenu("Transformation/SlerpRotation")]
	public class SlerpRotation : MonoBehaviour 
	{
		public enum LookAtAxis
		{
			Up = 0,
			Left = 1,
			Right = 2,
			Down = 3,
			Forward = 4
		}

		[Tooltip("Which axis the object will be rotated around.")]
		public LookAtAxis RotationAxis = LookAtAxis.Up;
		[Tooltip("The duration the circular lerp should be until it has reached the required rotation.")]
		public float Duration = 1.0f;

		private Quaternion m_startClerpRotation = Quaternion.identity;
		private Quaternion m_endClerpRotation = Quaternion.identity;
		private Transform m_transformComponent = null;
		private float m_elapsedTime = 0.0f;	
		private bool m_slerpIsActive = false;
		private float m_currentDuration = 0.0f;

		/// <summary>
		/// Internal Unity method.
		/// This method is called once per update cycle.
		/// If active, the method will perform a rotation of the object.
		/// </summary>
		void Update() 
		{
			if(m_currentDuration > 0.0f)
			{
				if(m_slerpIsActive == true)
				{
					m_elapsedTime += (Time.deltaTime * Time.timeScale);
					float procentage = (m_elapsedTime / m_currentDuration);

					if (procentage >= 1.0f)
					{
						m_slerpIsActive = false;
						procentage = 1.0f;
					}
			
					if(m_transformComponent != null)
						m_transformComponent.rotation = Quaternion.Slerp(m_startClerpRotation, m_endClerpRotation, procentage);
				}
			}
		}

		/// <summary>
		/// Rotates the object towards a position with the pre-defined duration.
		/// </summary>
		/// <param name="lookAtPosition">The position the object will be rotated towards.</param>
		/// <returns>The total amount of time it will take the object to finish the rotation.</returns>
		public float RotateTo(Vector3 lookAtPosition)
		{
			m_currentDuration = Duration;
			m_transformComponent = this.GetComponent<Transform>();
			return ActivateRotation(lookAtPosition);
		}

		/// <summary>
		/// Rotates the object towards a position.
		/// </summary>
		/// <param name="lookAtPosition">The position the object will be rotated towards.</param>
		/// <param name="timeDiff">A value which will alter the pre-defined rotation duration. If the value is set to 1.0f. There will be no difference. If the value is set to 0.5f the rotation will be twice as fast.</param>
		/// <returns>The total amount of time it will take the object to finish the rotation.</returns>
		public float RotateTo(Vector3 lookAtPosition, float timeDiff)
		{
			m_currentDuration = (Duration / timeDiff);
			m_transformComponent = this.GetComponent<Transform>();
			return ActivateRotation(lookAtPosition);
		}

		/// <summary>
		/// Rotates a specific object towards a position.
		/// </summary>
		/// <param name="objectToRotate">The object which will be rotated.</param>
		/// <param name="lookAtPosition">The position the object will be rotated towards.</param>
		/// <param name="timeDiff">A value which will alter the pre-defined rotation duration. If the value is set to 1.0f. There will be no difference. If the value is set to 0.5f the rotation will be twice as fast.</param>
		/// <returns>The total amount of time it will take the object to finish the rotation.</returns>
		public float RotateTo(Transform objectToRotate, Vector3 lookAtPosition, float timeDiff)
		{
			m_currentDuration = (Duration / timeDiff);
			m_transformComponent = objectToRotate;
			return ActivateRotation(lookAtPosition);
		}

		/// <summary>
		/// Rotates a specific object towards a position with the pre-defined duration.
		/// </summary>
		/// <param name="objectToRotate">The object which will be rotated.</param>
		/// <param name="lookAtPosition">The position the object will be rotated towards.</param>
		/// <returns>The total amount of time it will take the object to finish the rotation.</returns>
		public float RotateTo(Transform objectToRotate, Vector3 lookAtPosition)
		{
			m_currentDuration = Duration;
			m_transformComponent = objectToRotate;
			return ActivateRotation(lookAtPosition);
		}

		/// <summary>
		/// Activates the circular rotation.
		/// </summary>
		/// <param name="newLookAtPosition">The position the object will rotate towards.</param>
		/// <returns>The total amount of time it will take the object to finish the rotation.</returns>
		private float ActivateRotation(Vector3 newLookAtPosition)
		{
			Vector3 currentLookAtAxis = Vector3.zero;
			switch (RotationAxis)
			{
			case LookAtAxis.Up:
				currentLookAtAxis = m_transformComponent.up;
				break;
			case LookAtAxis.Down:
				currentLookAtAxis = -m_transformComponent.up;
				break;
			case LookAtAxis.Right:
				currentLookAtAxis = m_transformComponent.right;
				break;
			case LookAtAxis.Left:
				currentLookAtAxis = -m_transformComponent.right;
				break;
			case LookAtAxis.Forward:
				currentLookAtAxis = m_transformComponent.forward;
				break;
			}

			m_startClerpRotation = m_transformComponent.rotation;
			Vector3 lookAtPosition = (newLookAtPosition - m_transformComponent.position);
			if(lookAtPosition != Vector3.zero)
			{
				m_endClerpRotation = Quaternion.LookRotation(lookAtPosition, currentLookAtAxis);
				m_elapsedTime = 0.0f;
				m_slerpIsActive = true;
			}		

			return m_currentDuration;
		}
	}
}
