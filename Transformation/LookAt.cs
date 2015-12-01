using UnityEngine;
using System.Collections;

namespace mnUtilities.Transformation
{
	[AddComponentMenu("Transformation/LookAt")]
	public class LookAt : MonoBehaviour 
	{
		public enum LookAtAxis
		{
			Up = 0,
			Left = 1,
			Right = 2,
			Down = 3,
			Forward = 4
		}

		public bool ContinousUpdate = true;
		public Transform LookAtObject = null;
		public LookAtAxis Axis = LookAtAxis.Up;

		private Transform m_transformComponent = null;

		// Update is called once per frame
		void LateUpdate () 
		{	
			if(ContinousUpdate == false)
				return;

			SetLookAt(LookAtObject, Axis);
		}

		public void SetLookAt()
		{
			SetLookAt(LookAtObject, Axis);
		}

		public void SetLookAt(Vector3 position)
		{
				if (m_transformComponent == null)
				m_transformComponent = this.GetComponent<Transform>();

			if (m_transformComponent != null)
			{
				Vector3 currentLookAtAxis = Vector3.zero;
				switch (Axis)
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

				m_transformComponent.LookAt(position, currentLookAtAxis);		
			}
		}

		public void SetLookAt(Transform lookAtObject, LookAtAxis axis)
		{
			if (m_transformComponent == null)
				m_transformComponent = this.GetComponent<Transform>();

			if (m_transformComponent != null)
			{
				Vector3 currentLookAtAxis = Vector3.zero;
				switch (axis)
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

				m_transformComponent.LookAt(LookAtObject, currentLookAtAxis);
			}	
		}
	}
}
