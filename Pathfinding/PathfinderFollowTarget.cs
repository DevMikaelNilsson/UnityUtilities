using UnityEngine;
using System.Collections;

namespace mnUtilities.Pathfinding
{
	public class PathfinderFollowTarget : PathfinderBase
	{
		/// <summary>
		/// Enable flag to allow the object to move towards a given object.
		/// The object will move towards the object as long as this flag is enabled and that the
		/// distance between these two objects are greater than the minimum allowed distance.
		/// </summary>
		[Tooltip("Enable flag to allow the object to move towards a given object. The object will move towards the object as long as this flag is enabled and that the distance between these two objects are greater than the minimum allowed distance.")]
		public bool IsActive = true;

		/// <summary>
		/// Reference to the targeted object which the object will follow.
		/// </summary>
		[Tooltip("Reference to the targeted object which the object will follow.")]
		public Transform TargetObject = null;

		/// <summary>
		/// The minimum distance to the current destination.
		/// If the object is equal or less to the destination the object 
		/// will automatically stop moving until the distance is greater than this value.
		/// </summary>
		[Tooltip("The minimum distance to the current destination. If the object is equal or less to the destination the object will automatically stop moving until the distance is greater than this value.")]
		public float MinDistanceToDestination = 1.0f;

		/// <summary>
		/// A delay (in seconds) the object has before executing a action by itself (starting or stopping).
		/// </summary>
		[Tooltip("A delay (in seconds) the object has before executing a action by itself (starting or stopping).")]
		public float StatusActionDelay = 1.5f;

		private float m_elapsedTIme = 0.0f;

		/// <summary>
		/// Internal Unity method.
		/// This method is called once every time the object is enabled/re-enabled.
		/// If the 'IsActive' flag is enabled, then the method will automatically start to follow
		/// the target object.
		/// </summary>
		void OnEnable()
		{
			InitializeNavAgentBase();

			if(TargetObject != null && IsActive == true)
				StartPathfinder();
		}

		/// <summary>
		/// Starts the pathfinder and moves it towards the target object.
		/// </summary>
		public void StartPathfinder()
		{
			if(Vector3.Distance(TargetObject.position, m_transformComponent.position) > MinDistanceToDestination)
			{
				if(TargetObject != null && IsActive == true)
					StartPathfinder(TargetObject.position);
				else
					Debug.LogError(this + " - Either the Target object is invalid (" + TargetObject + ") or the 'IsActive' flag is set to false (" + IsActive + ").");
			}
		}

		/// <summary>
		/// Updates the pathfinder with the targets new current position.
		/// </summary>
		public void UpdatePathfinder()
		{
			if(Vector3.Distance(TargetObject.position, m_transformComponent.position) > MinDistanceToDestination)
			{
				if(TargetObject != null && IsActive == true)
					UpdatePathfinder(TargetObject.position);
				else
					Debug.LogError(this + " - Either the Target object is invalid (" + TargetObject + ") or the 'IsActive' flag is set to false (" + IsActive + ").");
			}
		}

		/// <summary>
		/// Internal Unity method.
		/// This method is called once every frame.
		/// The method will check the distance towards the target object
		/// and depending on the result, it may stop, update or start the pathfinder navigation.
		/// </summary>
		void LateUpdate()
		{
			if(IsPathfinderActive == true)
			{
				if(NavAgent.remainingDistance <= MinDistanceToDestination)
				{
					if(IsStatusActionDelayActive() == true)
						StopPathfinder();
				}
			}
			else
			{
				if(IsActive == true)
				{
					if(IsStatusActionDelayActive() == true)
					{
						if(IsPathfinderActive == false)
							StartPathfinder();
						else
							UpdatePathfinder();
					}
				}
			}
		}

		/// <summary>
		/// Determines wether the delay is still active or not.
		/// </summary>
		/// <returns>True if the delay time is reached. Return false otherwise.</returns>
		private bool IsStatusActionDelayActive()
		{
			if(m_elapsedTIme >= StatusActionDelay)
			{
				m_elapsedTIme  = 0.0f;
				return true;
			}

			m_elapsedTIme += (Time.deltaTime * Time.timeScale);
			return false;
		}
	}
}
