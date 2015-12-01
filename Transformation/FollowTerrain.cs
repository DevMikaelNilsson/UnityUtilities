using UnityEngine;
using System.Collections;

namespace mnUtilities.Transformation
{
	[AddComponentMenu("Transformation/FollowTerrain")]
	public class FollowTerrain : MonoBehaviour 
	{
		[Tooltip("Object which will be updated by this component. If this variable is empty, then the component will assign itself as object.")]
		public Transform AffectedObject = null;
		[Tooltip("A Child Transform object which will be used as a point where the raycast will be fired. If left empty the component will use the AffectedObjects position for the raycast.")]
		public Transform OffsetPoint = null;
		[Tooltip("Enable to set the orientation every LateUpdate cycle.")]
		public bool ContinousUpdate = true;
		[Tooltip("Enable to allow the component to smoothly rotate the object towards the calculated rotation. The smooth rotation will restart every time the rotation values are updated, which can be as often as once per update frame.")]
		public bool SmoothRotation = true;
		[Tooltip("The duration (in seconds) of the smooth rotation. This variable is only used when the 'SmoothRotation' flag is enabled.")]
		public float SmootValue = 1.0f;
		[Tooltip("All mask layers the component is allowed to search for the terrain object. If left empty the raycast may never get a proper hit.")]
		public string RaycastLayerMask = string.Empty;

		private int m_layerMaskValue = 0;
		private Quaternion m_oldRotation = Quaternion.identity;
		private Quaternion m_newRotation = Quaternion.identity;
		private Vector3 m_oldPosition = Vector3.zero;
		
		/// <summary>
		/// Internal Unity method.
		/// This method is called everytime the component is enabled/re-enabled.
		/// The method stores all required variables here.
		/// </summary>
		void OnEnable()
		{
			if(AffectedObject == null)
				AffectedObject = this.GetComponent<Transform>();
			if(OffsetPoint == null)
				OffsetPoint = AffectedObject;

			m_layerMaskValue = LayerMask.NameToLayer(RaycastLayerMask);
			m_oldRotation = AffectedObject.rotation;
			m_newRotation = AffectedObject.rotation;
			m_oldPosition = OffsetPoint.position;
		}

		/// <summary>
		/// Internal Unity method.
		/// This method is called at the end of every update cycle.
		/// This method updates the objects rotation if the flag is enabled.
		/// </summary>
		void LateUpdate()
		{
			if(ContinousUpdate == true)
				UpdateObjectRotation();
			
				if(SmoothRotation == true)
					AffectedObject.rotation = Quaternion.Slerp(m_newRotation, m_oldRotation, (Time.deltaTime * SmootValue));
				else
					AffectedObject.rotation = m_newRotation;
		}

		/// <summary>
		/// Updates the objects current rotation.
		/// The method will launch a raycast in the Vector3.down world direction, and if it has a hit (which will be regarded as terrain),
		/// the method will calculate the proper rotation value between the object and the terrain object.
		/// </summary>
		public void UpdateObjectRotation()
		{
			RaycastHit hitInfo;
			if (PerformRaycast(out hitInfo) == true)
			{
				m_oldRotation = AffectedObject.rotation;
				m_newRotation = CalculateLookRotation(-hitInfo.normal);
			}
		}

		/// <summary>
		/// Performs a raycast based on settings.
		/// </summary>
		/// <param name="hitInfo">This will store the collider data if any.</param>
		/// <returns>True if there was a raycast collision. False otherwise.</returns>
		private bool PerformRaycast(out RaycastHit hitInfo)
		{
			int layerMask = 1 << m_layerMaskValue;			
			Vector3 position = OffsetPoint.position;

			if(m_oldPosition == position)
			{
				hitInfo = new RaycastHit();
				return false;
			}
			else
			{
				m_oldPosition = position;
				return Physics.Raycast(position, Vector3.down, out hitInfo, layerMask);
			}
		}

		/// <summary>
		/// Calculates a look rotation for the current object.
		/// </summary>
		/// <param name="normalValue">A normal from the terrain which will be used to determin the final rotation value.</param>
		/// <returns>A rotation value which indicates which way the object should "look".</returns>
		private Quaternion CalculateLookRotation(Vector3 normalValue)
		{
			Vector3 new_forward = Vector3.Cross(normalValue, AffectedObject.right);
			return Quaternion.LookRotation(new_forward);
		}
	}
}
