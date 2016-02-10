using UnityEngine;
using System.Collections;

namespace mnUtilities.Pathfinding
{
	[AddComponentMenu("Pathfinding/FollowPathfinder")]
	public class FollowPathfinder : MonoBehaviour 
	{
		
		/// <summary>
		/// Reference to the Pathfinder object which this object will follow.
		/// It's recomended that the Pathfinder component is on a different GameObject.
		/// </summary>
		[Tooltip("Reference to the Pathfinder object which this object will follow. It's recomended that the Pathfinder component is on a different GameObject.")]
		public PathfinderBase PathfinderObject = null;
		/// <summary>
		/// A offset to the Pathfinder position. This offset vector will be added to the current Pathfinder component position at every update cycle.
		/// </summary>
		[Tooltip("A offset to the Pathfinder position. This offset vector will be added to the current Pathfinder component position at every update cycle.")]
		public Vector3 OffsetPosition = Vector3.zero;
		/// <summary>
		/// Smooth value for Linear interpolation of the position between this object and the Pathfinder object.
		/// </summary>
		[Tooltip("Smooth value for Linear interpolation of the position between this object and the Pathfinder object.")]
		public float SmoothTranslate = 2.0f;
		/// <summary>
		/// Smooth value for Linear interpolation of the rotation between this object and the Pathfinder object.
		/// </summary>
		[Tooltip("Smooth value for Linear interpolation of the rotation between this object and the Pathfinder object.")]
		public float SmoothRotation = 8.0f;
		/// <summary>
		/// Reference to a Animator object. If used the Pathfinder will update the Animators parameter(s) with usefull information.
		/// </summary>
		[Tooltip("Reference to a Animator object. If used the Pathfinder will update the Animators parameter(s) with usefull information.")]
		public Animator AnimatorObject = null;
		/// <summary>
		/// String parameter name which is a parameter for the Animator which will be updated by the Pathfinder with its current velocity (movement) value.
		/// </summary>
		[Tooltip("String parameter name which is a parameter for the Animator which will be updated by the Pathfinder with its current velocity (movement) value.")]
		public string MovementParam = string.Empty;
		
		/// <summary>
		/// The minimum allowed velocity for the object to follow the Pathfinder object.
		/// The velocity is calculated locally from the movement of this object and not the Pathfinder object.
		/// If the objects velocity is below this value, it will stop translating and rotating towards the Pathfinder object.
		/// The data sent to the AnimatorObject is also affected. If the Pathfinder object is flagged as moving, the value sent to the Animator cant be below this value.
		/// If the Pathfinder object is NOT flagged as moving, and the velocity is below this value, then the Animator will become 0.0f instead.
		/// Set minimum value to 0.0f to ignore this functionality.
		/// </summary>
		[Tooltip("The minimum allowed velocity for the object to follow the Pathfinder object. The velocity is calculated locally from the movement of this object and not the Pathfinder object. If the objects velocity is below this value, it will stop translating and rotating towards the Pathfinder object. The data sent to the AnimatorObject is also affected:\n\nIf the Pathfinder object is flagged as moving, the value sent to the Animator cant be below this value.\n\nIf the Pathfinder object is NOT flagged as moving, and the velocity is below this value, then the Animator will become 0.0f instead.\n\nSet minimum value to 0.0f to ignore this functionality.")]
		public float MinVelocity = 0.1f;
				
		private NavMeshAgent m_patfinderNavMeshObject = null;
		private Transform m_pathfinderTransformComponent = null;
		private Transform m_transformComponent = null;
		private Vector3 m_lastPosition = Vector3.zero;
		
		/// <summary>
		/// Internal Unity method.
		/// This method is called when the object is enabled/re-enabled.
		/// The method will set all required variables.
		/// </summary>
		void OnEnable () 
		{
			if(m_transformComponent == null)
				m_transformComponent = this.GetComponent<Transform>();
				
			if(PathfinderObject == null)
				Debug.LogWarning(this + " - Pathfinder component is either missing or not valid.");
			else
				LoadPathfinderComponents();
		}
		
		/// <summary>
		/// Internal Unity method.
		/// This method is called once every frame.
		/// The method updates the position and rotation for this object.
		/// </summary>
		void Update () 
		{
			if(PathfinderObject != null)
			{
				if(m_pathfinderTransformComponent == null)
					LoadPathfinderComponents();

				if(m_pathfinderTransformComponent != null)
				{

					Vector3 currentDirection = (m_transformComponent.position - (m_pathfinderTransformComponent.position + OffsetPosition));
					float currentVelocity = currentDirection.sqrMagnitude;
					UpdatePosition(currentVelocity);
				}
			}
			
			m_lastPosition = m_transformComponent.position;
		}
		
		/// <summary>
		/// Loads the required variables and objects in order to
		// be able to make the object follow the Pathfinder object.
		/// </summary>
		private void LoadPathfinderComponents()
		{
			if(m_patfinderNavMeshObject == null)
				m_patfinderNavMeshObject = PathfinderObject.NavAgent;
			
			if(m_patfinderNavMeshObject != null)
			{	
				if(m_pathfinderTransformComponent == null)
					m_pathfinderTransformComponent = PathfinderObject.gameObject.GetComponent<Transform>();
			}
		}

		/// <summary>
		/// Updates the objects position.
		/// The position and rotation are both linear interpolated as long as the object is moving.
		/// The method uses the local objects velocty and not the Pathfinder velocity, since it should be
		/// able to move on its own accord to the Pathfinder desitination position.
		/// </summary>
		/// <param name="velocity">The curren translation velocity.</param>
		private void UpdatePosition(float velocity)
		{
			LoadPathfinderComponents();				
			if(m_patfinderNavMeshObject != null)
			{					
				if(velocity > MinVelocity)
				{
					m_transformComponent.position = Vector3.Lerp(m_transformComponent.position, m_pathfinderTransformComponent.position, (Time.deltaTime * SmoothTranslate));
					Quaternion currentLookRotation = Quaternion.LookRotation(m_transformComponent.position - m_lastPosition);
					m_transformComponent.rotation = Quaternion.Lerp(m_transformComponent.rotation, currentLookRotation, (Time.deltaTime * SmoothRotation));
				}
			}		
		}

	}
}
