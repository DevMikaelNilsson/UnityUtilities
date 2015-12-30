using UnityEngine;
using System.Collections;

namespace mnUtilities.Pathfinding
{
	public class PathfinderBase : MonoBehaviour 
	{
		public enum PathfinderStatus
		{
			/// <summary>
			/// The Pathfinder is active but is not moving.
			/// </summary>
			Waiting = 0,
			/// <summary>
			/// The Pathfinder is currently moving on a valid path.
			/// </summary>
			Moving = 1,
			/// <summary>
			/// The Pathfinder has reached the destination position on a valid path.
			/// </summary>
			Finished = 2,		
			/// <summary>
			/// The Pathfinder has found a valid path but has not begun to move towards the destination position.
			/// </summary>
			PathFound = 3,
			
			/// <summary>
			/// The Pathfinder did not succeed in finding a valid path to a destination.
			/// </summary>
			PathNotFound = 4,
			
			/// <summary>
			/// The Pathfinder component is currently disabled and is not moving.
			/// </summary>
			Disabled = 5
		}

		/// <summary>
		/// The objects current status.
		/// </summary>
		[Tooltip("The objects current status.")]
		public PathfinderStatus ObjectStatus = PathfinderStatus.Disabled;
		/// <summary>
		/// Reference to a NavMeshAgent. If left empty, the component will automatic attach a NavMeshAgent to the GameObject.
		/// </summary>
		[Tooltip("Reference to a NavMeshAgent. If left empty, the component will automatic attach a NavMeshAgent to the GameObject.")]
		public NavMeshAgent PathAgent = null;
		/// <summary>
		/// Reference to a NavMeshObstacle component. If used, the NavMeshObstacle will be activated everytime the NavMeshAgent is currently not active
		/// to avoid getting pushed around by other NavMeshAgents.
		/// </summary>
		[Tooltip("Reference to a NavMeshObstacle component. The NavMeshObstacle will be activated everytime the NavMeshAgent is currently not active to avoid getting pushed around by other NavMeshAgents. If left empty, the component will automatic attach a NavMeshAgent to the GameObject.")]
		public NavMeshObstacle PathMeshObstacle = null;
		/// <summary>
		/// Enable flag to make the Pathfinder to start moving direct when the object is enabled/re-enabled.
		/// </summary>
		[Tooltip("Enable flag to make the Pathfinder to start moving direct when the object is enabled/re-enabled.")]
		public bool ActiveOnStart = true;

		/// <summary>
		/// A delay (in seconds) in between the PathMeshObstacle is disabled and the PathAgent is enabled and active. There can be issues when attempting to find a valid path when 
		/// disabling the MeshObstacle and enabling the PathAgent within a given time frame. To avoid a issue there is a delay from the MeshObstacle is disabled and the PathAgent is active.
		/// This delay can also be set through code, by waiting calling the 'CreateDestinationPath()' method manually.
		/// </summary>
		[Tooltip("A delay (in seconds) in between the PathMeshObstacle is disabled and the PathAgent is enabled and active. There can be issues when attempting to find a valid path when disabling the MeshObstacle and enabling the PathAgent within a given time frame. To avoid a issue there is a delay from the MeshObstacle is disabled and the PathAgent is active. This delay can also be set through code, by waiting calling the 'CreateDestinationPath()' method manually.")]
		public float ActivationDelay = 1.0f;

		/// <summary>
		/// Enable flag to perform a check so the Pathfinder object will be placed on a valid position on a NavMesh area. 
		/// This means that the object can be placed on a different position than it had when it was enabled/re-enabled.
		/// </summary>
		[Tooltip("Enable flag to perform a check so the Pathfinder object will be placed on a valid position on a NavMesh area. This means that the object can be placed on a different position than it had when it was enabled/re-enabled.")]
		public bool PositionOnNavMesh = true;
		/// <summary>
		/// The maximum distance from the objects initial position where the Pathfinder will be placed, when the 'PositionOnNavMesh' flag is enabled.
		/// </summary>
		[Tooltip("The maximum distance from the objects initial position where the Pathfinder will be placed, when the 'PositionOnNavMesh' flag is enabled.")]
		public float MaxPositionDistance = 10.0f;
		/// <summary>
		/// The minimum distance between the Pathfinders current position and the destination position.
		/// If the distance is less than this value, the Pathfinder will consider to has reached its destination.
		/// </summary>
		[Tooltip("The minimum distance between the Pathfinders current position and the destination position. If the distance is less than this value, the Pathfinder will consider to has reached its destination.")]
		public float MinDistance = 1.0f;
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
		/// A minimum desired velocity the Agent is allowed to have (calculated with NavMeshAgent.Velocity.sqrMagnitude). If the Agents velocity is below this value
		/// the object status will be automatically set as 'Finished' after a given delay (see SetAsFinishAfterDelay). This value is only applicable when the object is moving
		/// towards a destination (the object has a valid path to follow).
		/// </summary>
		[Tooltip("A minimum desired velocity the Agent is allowed to have (calculated with NavMeshAgent.Velocity.sqrMagnitude). If the Agents velocity is below this value the object status will be automatically set as 'Finished' after a given delay (see SetAsFinishAfterDelay). This value is only applicable when the object is moving towards a destination (the object has a valid path to follow).")]
		public float MinDesiredVelocity = 0.1f;
		/// <summary>
		/// A delay (in seconds) until the objects status is automatically set as 'Finished' if the objects velocity (while moving) is below specified minim velocity (see MinDesiredVelocity).
		/// </summary>
		[Tooltip("A delay (in seconds) until the objects status is automatically set as 'Finished' if the objects velocity (while moving) is below specified minim velocity (see MinDesiredVelocity).")]
		public float SetAsFinishAfterDelay = 1.0f;
		
		protected Transform m_transformComponent = null;
		protected Vector3 m_destinationPosition = Vector3.zero;
		protected bool m_ObjectIsUnderMinVelocity = false;

		/// <summary>
		/// Stops the pathfinding.
		/// </summary>
		protected void StopPathfinding()
		{
			if(PathAgent.isActiveAndEnabled == true)
				PathAgent.Stop();
				
			PathAgent.enabled = false;
			PathMeshObstacle.enabled = true;
			ObjectStatus = PathfinderStatus.Disabled;
		}
		
		/// <summary>
		/// Resumes the pathfinding.
		/// </summary>
		protected void ResumePathfinding()
		{
			if(PathAgent.isActiveAndEnabled == false)
			{
				ObjectStatus = PathfinderStatus.Waiting;
				StartCoroutine(DelayActivatePathAgent());
			}
		}		
		
		/// <summary>
		/// The method locates a valid position on the current NavMesh.
		/// The object will be automatic re-positioned on the NavMesh object.
		/// </summary>
		protected void SetPositionOnNavMesh()
		{
			NavMeshHit hit;
			if(NavMesh.SamplePosition(m_transformComponent.position, out hit, MaxPositionDistance, NavMesh.AllAreas) == true)
				m_transformComponent.position = hit.position;
		}

		/// <summary>
		/// Updates the Animator object with data which 
		/// is generated by the component.
		/// </summary>
		protected void UpdateAnimationData()
		{
			float translateVelocity = PathAgent.velocity.sqrMagnitude;
			switch(ObjectStatus)
			{
			case PathfinderRandomPosition.PathfinderStatus.Moving:
				if(translateVelocity < MinDesiredVelocity)
					translateVelocity = MinDesiredVelocity;
				break;
			default:
				if(translateVelocity < MinDesiredVelocity)
					translateVelocity = 0.0f;
				break;
			}
		
			if(AnimatorObject != null)
			{
				AnimatorObject.SetFloat(MovementParam, translateVelocity);
			}
		}

		/// <summary>
		/// Enables the Pathfinder and attempts to create a valid path to the destination.
		/// Do note that not all Pathfinding types will use a fixed position, but randomizes a point instead, to generate a valid path.
		/// </summary>
		/// <param name="destination">The destination point which the Pathfinder should create a valid path towards.</param>
		public void EnablePathAgent(Vector3 destination)
		{
			m_destinationPosition = destination;
			EnablePathAgent();
		}
	

		/// <summary>
		/// Enables the Pathfinder and locates a destination position based on the components settings.
		/// If a valid path is found, the object will start to move towards the destination position automatically. 
		/// </summary>
		public void EnablePathAgent()
		{
			if(PathAgent == null)
			{
				Debug.LogWarning(this + " - NavMeshAgent is either missing or not valid. Adding NavMeshAgent to object.");
				PathAgent = this.gameObject.AddComponent<NavMeshAgent>();
			}
			
			if(PathMeshObstacle == null)
			{
				Debug.LogWarning(this + " - NavMeshObstacle is either missing or not valid. Adding NavMeshObstacle to object.");
				PathMeshObstacle = this.gameObject.AddComponent<NavMeshObstacle>();
			}

			if(PathMeshObstacle.isActiveAndEnabled == true && PathAgent.isActiveAndEnabled == false)
				StartCoroutine(DelayActivatePathAgent());
		}

		/// <summary>
		/// Co-routine to disable the MeshObstacle and activating the PathAgent.
		/// There might be issues when disabling/enabling these two at the same time, because the PathAgent can not 
		/// find a path when the MeshObstacle is still blocking. So this co-routine will force a given delay before the PathAgent is active,
		/// letting the Pathfinder map be properly updated. This delay can be ignored (set delay value to 0.0f) if a delay is built into the component which 
		/// inherits this component.
		/// </summary>
		protected IEnumerator DelayActivatePathAgent()
		{
			if(PathMeshObstacle.enabled == true)
			{
				PathMeshObstacle.enabled = false;
				yield return new WaitForSeconds(ActivationDelay);
				PathAgent.enabled = true;
			}
			else
				Debug.LogWarning(this + " - PathMeshObstacle is already inactive. Can not activate DelayActivatePathAgent coroutine.");
		}

		/// <summary>
		/// Retrieves a random position based on the random point values,
		/// and attempts to set a destination point for the NavMeshAgent component.
		/// If there is no valid destination point, the method will call itself, through a
		/// co-routine at the end of the frame, in a attempt to find a new valid, destination point.
		/// The method will keep calling itself whenever a destination point is reached or not valid.
		/// The only way to stop the component calling itself is to disable the component. 
		/// See method 'StopPathfinding' for more information.
		/// </summary>
		protected PathfinderStatus CreateRandomPath(Vector3 minRandomAreaPoint, Vector3 maxRandomAreaPoint)
		{
			if(PathAgent.isActiveAndEnabled == false)
				return PathfinderStatus.Disabled;
			else if(PathAgent.isOnOffMeshLink == true || PathAgent.isOnNavMesh == true)
			{
				m_destinationPosition = RandomVector(minRandomAreaPoint, maxRandomAreaPoint);
				if(PathAgent.SetDestination(m_destinationPosition) == true)
					return PathfinderStatus.PathFound;
			}
			else
				Debug.LogWarning(this + " - NavMeshAgent is not position on a valid OffMeshLink and/or a NavMesh object. Can not calculate new position.");
			
			return PathfinderStatus.PathNotFound;
		}

		/// <summary>
		/// Creats a random vector3 within the min and max values.
		/// </summary>
		/// <param name="minValue">The minimum value.</param>
		/// <param name="maxValue">The maximum value.</param>
		/// <returns>A randomized vector3 within given boundaries.</returns>
		protected Vector3 RandomVector(Vector3 minValue, Vector3 maxValue)
		{
			Vector3 newVectorValue = Vector3.zero;
			newVectorValue.x = UnityEngine.Random.Range(minValue.x, maxValue.x);
			newVectorValue.y = UnityEngine.Random.Range(minValue.y, maxValue.y);
			newVectorValue.z = UnityEngine.Random.Range(minValue.z, maxValue.z);
			return newVectorValue;
		}

		/// <summary>
		/// Creates a path to a given destination.
		/// </summary>
		/// <param name="destination">The destination point which the Pathfinder should create a valid path towards.</param>
		/// <returns>>The current status.</returns>
		protected PathfinderStatus CreateDestinationPath(Vector3 destination)
		{
			m_destinationPosition = destination;
			return CreateDestinationPath();
		}

		/// <summary>
		/// Creates a path to a given destination.
		/// The destination should be set before this method is called.
		/// If the method fails to find a valid path to the destination position,
		/// the method will not attempt to find a new path due to the destination is fixed.
		/// In order to get a valid path, the method must be called manually.
		/// </summary>
		/// <returns>>The current status.</returns>
		protected PathfinderStatus CreateDestinationPath()
		{
			if(PathAgent.isActiveAndEnabled == false)
				return PathfinderStatus.Disabled;
			else if(PathAgent.isOnOffMeshLink == true || PathAgent.isOnNavMesh == true)
			{
				if(PathAgent.SetDestination(m_destinationPosition) == true)
					return PathfinderStatus.PathFound;
			}
			else
				Debug.LogWarning(this + " - NavMeshAgent is not position on a valid OffMeshLink and/or a NavMesh object. Can not calculate new position.");
				
			return PathfinderStatus.PathNotFound;
		}

		/// <summary>
		/// Checks the current agent velocity.
		/// The method checks if the current agent velocity below the minimum allowed
		/// velocity value. If the value is below, a co-routine will be activated, and when 
		/// the co-routine delay is up, and the velocity is still below minimum value, the
		/// agent will be set as finished. 
		/// This will avoid issues like if the object is blocked by another object, and the 
		/// agent may find a new path to follow. 
		/// NOTE: This method only sets the agent as finished and do not fire any new path calculations.
		/// </summary>
		protected void CheckForAgentVelocity()
		{
			if(PathAgent.velocity.sqrMagnitude < MinDesiredVelocity)
			{
				if(m_ObjectIsUnderMinVelocity == false)
				{
					m_ObjectIsUnderMinVelocity = true;
					StartCoroutine(DelayForceFinish());
				}
			}
			else
				m_ObjectIsUnderMinVelocity = false;
		}

		/// <summary>
		/// Co-routine to force set a agent as being finished after a given delay (in seconds).
		/// Forcing the finish flag upon the agent should only be done if the object velocity is below
		/// the minimum velocity value.
		/// </summary>
		/// <returns>The force finish.</returns>
		protected IEnumerator DelayForceFinish()
		{
			yield return new WaitForSeconds(SetAsFinishAfterDelay);
			if(m_ObjectIsUnderMinVelocity == true)
				ObjectStatus = PathfinderStatus.Finished;
				
			m_ObjectIsUnderMinVelocity = false;
		}

		/// <summary>
		/// Update and set the current status.
		/// </summary>
		/// <returns>The current object status.</returns>
		protected PathfinderStatus SetObjectStatus()
		{
			switch(ObjectStatus)
			{
			case PathfinderStatus.PathFound:
				ObjectStatus = PathfinderStatus.Moving;
				break;
			case PathfinderStatus.Moving:
				float distanceToDestination = Vector3.Distance(m_transformComponent.position, m_destinationPosition);
				if(distanceToDestination <= MinDistance)
					ObjectStatus = PathfinderStatus.Finished;
				else
					CheckForAgentVelocity();
				break;
			}				
			
			return ObjectStatus;
		}
	}
}
