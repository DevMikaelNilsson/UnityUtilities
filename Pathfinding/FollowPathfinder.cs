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
		public Pathfinder PathfinderObject = null;
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
			if(this.GetComponent<Pathfinder>() != null)
				Debug.LogWarning(this + " - This object has a Pathfinder component attached to it. This can cause unexpected behavior and errors and its not recommended to have both components attached to the same object.")
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
				Vector3 currentDirection = (m_transformComponent.position - (m_pathfinderTransformComponent.position + OffsetPosition));
				float currentVelocity = currentDirection.sqrMagnitude;
				UpdatePosition(currentVelocity);
				UpdateAnimationData(currentVelocity);
			}
			
			m_lastPosition = m_transformComponent.position;
		}
		
		/// <summary>
		/// Get/Set the current Pathfinder status.
		/// </summary>
		/// <value>The current Pathfinder status.</value>
		public Pathfinder.PathfinderStatus ObjectStatus
		{
			get
			{
				if(PathfinderObject != null)
					return PathfinderObject.ObjectStatus;
				else
				{
					Debug.LogWarning(this + " - PathfinderObject is either missing or not valid. Can not retrieve proper Object status.");
					return Pathfinder.PathfinderStatus.Disabled;
				}
			}
			
			set
			{
				if(PathfinderObject != null)
					PathfinderObject.ObjectStatus = value;
				else
					Debug.LogWarning(this + " - PathfinderObject is either missing or not valid. Can not set Object status.");
			}
		}
		
		/// <summary>
		/// Enables the Pathfinder and locates a destination position based on the components settings.
		/// If a valid path is found, the object will start to move towards the destination position automatically. 
		/// </summary>
		public void EnablePathAgent()
		{
			if(PathfinderObject != null)
				PathfinderObject.EnablePathAgent();
		}
		
		/// <summary>
		/// Enables the Pathfinder and attempts to create a valid path to the destination.
		/// Do note that not all Pathfinding types will use a fixed position, but randomizes a point instead, to generate a valid path.
		/// </summary>
		/// <param name="destination">The destination point which the Pathfinder should create a valid path towards.</param>
		public void EnablePathAgent(Vector3 destination)
		{
			if(PathfinderObject != null)
				PathfinderObject.EnablePathAgent(destination);
		}		
		
		/// <summary>
		/// Loads the required variables and objects in order to
		// be able to make the object follow the Pathfinder object.
		/// </summary>
		private void LoadPathfinderComponents()
		{
			if(m_patfinderNavMeshObject == null)
				m_patfinderNavMeshObject = PathfinderObject.PathAgent;
			
			if(m_patfinderNavMeshObject != null)
			{	
				if(m_pathfinderTransformComponent == null)
					m_pathfinderTransformComponent = PathfinderObject.gameObject.GetComponent<Transform>();
			}
		}
		
		/// <summary>
		/// Updates the Animator parameter(s) with data.
		/// </summary>
		/// <param name="translateVelocity">The current translation velocity.</param>
		private void UpdateAnimationData(float translateVelocity)
		{
			if(AnimatorObject != null)
			{
				AnimatorObject.SetFloat(MovementParam, translateVelocity);
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
				if (velocity > 0.1f) 
				{					
					m_transformComponent.position = Vector3.Lerp(m_transformComponent.position, m_pathfinderTransformComponent.position, (Time.deltaTime * SmoothTranslate));
					Quaternion currentLookRotation = Quaternion.LookRotation(m_transformComponent.position - m_lastPosition);
					m_transformComponent.rotation = Quaternion.Lerp(m_transformComponent.rotation, currentLookRotation, (Time.deltaTime * SmoothRotation));
				}
			}			
		}
	}
}
