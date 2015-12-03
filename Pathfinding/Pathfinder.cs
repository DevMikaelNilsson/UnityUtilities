using UnityEngine;
using System.Collections;

namespace mnUtilities.Pathfinding
{
  [AddComponentMenu("Pathfinding/Pathfinder")]
	public class Pathfinder : MonoBehaviour 
	{
		#region :: General member variables
			
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
			
			public enum TypeOfPathfinding
			{
				/// <summary>
				/// The Pathfinder takes a random position and moves towards the destination position.
				/// When the Pathfinder has reached the destination, the Pathfinder will automatic find a new random position
				/// and start to move towards the destination. The Pathfinder will continue to find a random destination until manually stopped.
				/// </summary>
				ContinousRandomPosition = 0,
				/// <summary>
				/// The Pathfinder takes a random position and moves towads the destination position.
				/// When the Pathfinder has reached the destination, the Pathfinder will stay at that specific position and needs to be
				/// manually activated in order to get a new random destination position and move towards it.
				/// </summary>
				OneTimeRandomPosition = 1,
				/// <summary>
				/// The Pathfinder needs to be manually activated with a specific destination position to activate the Pathfinder
				/// to find a valid path and move towards the destination position.
				/// </summary>
				OneTimeManualPosition = 2
			}

			/// <summary>
			/// The objects current status.
			/// </summary>
			[Tooltip("The objects current status.")]
			[HideInInspector]
			public PathfinderStatus ObjectStatus = PathfinderStatus.Disabled;
			/// <summary>
			/// What type of Pathfinding the component should use.
			/// </summary>
			[Tooltip("What type of Pathfinding the component should use.")]
			public TypeOfPathfinding TypeOfActivePathfinding = TypeOfPathfinding.OneTimeManualPosition;
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
			
			protected Transform m_transformComponent = null;
			protected Vector3 m_destinationPosition = Vector3.zero;
			
		#endregion
		
		#region :: ContinousRandomPosition member variables
			/// <summary>
			/// Minimum position which will be used when creating a random destination position.
			/// </summary>
			[Tooltip("Minimum position which will be used when creating a random destination position.")]
			public Vector3 MinRandomAreaPoint = Vector3.zero;
			/// <summary>
			/// Maximum position which will be used when creating a random destination position.
			/// </summary>
			[Tooltip("Maximum position which will be used when creating a random destination position.")]
			public Vector3 MaxRandomAreaPoint = Vector3.one;
		#endregion
		
		#region :: General Methods
			/// <summary>
			/// Internal Unity method.
			/// This method is called when the object is enabled/re-enabled.
			/// The method will set all required variables and, if enabled, 
			/// the component will activate the NavMesh pathfinding.
			/// </summary>
			void OnEnable()
			{
				if(m_transformComponent == null)
					m_transformComponent = this.GetComponent<Transform>();
					
				if(PathAgent != null)
					PathAgent.enabled = false;
				if(PathMeshObstacle != null)
					PathMeshObstacle.enabled = false;
					
				if(PositionOnNavMesh == true)
					SetPositionOnNavMesh();
					
				if(ActiveOnStart == true)
					StartCoroutine(DelayEnablePathAgent());
			}
			
			/// <summary>
			/// Internal Unity method.
			/// This method is called once every frame.
			/// The method updates the status for the Pathfinder component.
			/// </summary>
			void Update()
			{
				if(PathAgent.isActiveAndEnabled == true)
				{
					switch(TypeOfActivePathfinding)
					{
						case TypeOfPathfinding.ContinousRandomPosition:
							UpdateContinousRandomPositionStatus();
							break;
						case TypeOfPathfinding.OneTimeManualPosition:
							UpdateOneTimeManualPositionStatus();
							break;
						case TypeOfPathfinding.OneTimeRandomPosition:
							UpdateOneTimeRandomPositionStatus();
							break;
					}
				}
				
				UpdateAnimationData();
			}
			
			/// <summary>
			/// Updates the Animator object with data which 
			/// is generated by the component.
			/// </summary>
			protected void UpdateAnimationData()
			{
				if(AnimatorObject != null)
				{
					AnimatorObject.SetFloat(MovementParam, PathAgent.velocity.magnitude);
				}
			}
			
			/// <summary>
			/// Co-routine which enables the NavMeshAgent
			/// at the end of the frame.
			/// </summary>
			private IEnumerator DelayEnablePathAgent()
			{
				yield return new WaitForEndOfFrame();
				EnableInternalPathAgent();
			}
			
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
					PathMeshObstacle.enabled = false;			
					StartCoroutine(DelayEnablePathAgent());
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
			/// Enables the Pathfinder and locates a destination position based on the components settings.
			/// This method is only used internal if the flag "ActiveOnStart" is enabled.
			/// </summary>
			private void EnableInternalPathAgent()
			{
				switch(TypeOfActivePathfinding)
				{
					case TypeOfPathfinding.OneTimeRandomPosition:
					case TypeOfPathfinding.ContinousRandomPosition:
						EnablePathAgent();
						break;
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
					
				if(PathAgent.isActiveAndEnabled == false)
					PathAgent.enabled = true;
				if(PathMeshObstacle.isActiveAndEnabled == true)
					PathMeshObstacle.enabled = false;
				
				ObjectStatus = PathfinderStatus.Waiting;				
				switch(TypeOfActivePathfinding)
				{
					case TypeOfPathfinding.ContinousRandomPosition:
						ActivateContinousRandomPath();
						break;
					case TypeOfPathfinding.OneTimeRandomPosition:
						ActivateOneTimeRandomPath();
						break;
					case TypeOfPathfinding.OneTimeManualPosition:
						ActivateOneTimeManualPath();
						break;
				}
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
			private void CreateRandomPath()
			{
				if(PathAgent.isActiveAndEnabled == false)
				{
					Debug.LogWarning(this + " - NavMeshAgent is either not active and enabled. Can not set destination position.");
					ObjectStatus = PathfinderStatus.Disabled;
					return;
				}
				else if(PathAgent.isOnOffMeshLink == true || PathAgent.isOnNavMesh == true)
				{
					m_destinationPosition = TabUtilities.Math.RandomVector(MinRandomAreaPoint, MaxRandomAreaPoint);
					if(PathAgent.SetDestination(m_destinationPosition) == true)
					{
						ObjectStatus = PathfinderStatus.PathFound;
						return;
					}
				}
				else
					Debug.LogWarning(this + " - NavMeshAgent is not position on a valid OffMeshLink and/or a NavMesh object. Can not calculate new position.");
				
				ObjectStatus = PathfinderStatus.PathNotFound;
				StartCoroutine(DelayEnablePathAgent());
			}
			
			/// <summary>
			/// Creates a path to a given destination.
			/// The destination should be set before this method is called.
			/// If the method fails to find a valid path to the destination position,
			/// the method will not attempt to find a new path due to the destination is fixed.
			/// In order to get a valid path, the method must be called manually.
			/// </summary>
			private void CreateDestinationPath()
			{
				if(PathAgent.isActiveAndEnabled == false)
				{
					Debug.LogWarning(this + " - NavMeshAgent is either not active and enabled. Can not set destination position.");	
					ObjectStatus = PathfinderStatus.Disabled;	
					return;
				}
				else if(PathAgent.isOnOffMeshLink == true || PathAgent.isOnNavMesh == true)
				{
					if(PathAgent.SetDestination(m_destinationPosition) == true)
					{
						ObjectStatus = PathfinderStatus.PathFound;
						return;
					}
				}
				else
					Debug.LogWarning(this + " - NavMeshAgent is not position on a valid OffMeshLink and/or a NavMesh object. Can not calculate new position.");
					
				ObjectStatus = PathfinderStatus.PathNotFound;
			}
			
			/// <summary>
			/// Update and set the current status.
			/// </summary>
			/// <returns>The current object status.</returns>
			private PathfinderStatus SetObjectStatus()
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
					break;
				}
				
				return ObjectStatus;
			}
			
		#endregion
		
		#region :: OneTimeManualPosition Methods		
		/// <summary>
		/// Updates the OneTimeManualPositionStatus status.
		/// The method updates the objects status, but does not 
		/// activate a new destination when the object has reached its current destionation.
		/// </summary>
		protected void UpdateOneTimeManualPositionStatus()
		{
			switch(PathAgent.pathStatus)
			{
			case NavMeshPathStatus.PathComplete:
				SetObjectStatus();
				break;
			default:
				ObjectStatus = PathfinderStatus.Waiting;
				break;
			}
		}
		
		/// <summary>
		/// Locates a random position and attempts to find a valid
		/// path to the destination position.
		/// </summary>
		protected void ActivateOneTimeManualPath()
		{
			CreateDestinationPath();
		}
		
		#endregion
		
		#region :: OneTimeRandomPosition Methods
			/// <summary>
			/// Updates the OneTimeRandomPositions status.
			/// The method updates the objects status, but does not 
			/// activate a new destination when the object has reached its current destionation.
			/// </summary>
			protected void UpdateOneTimeRandomPositionStatus()
			{
				switch(PathAgent.pathStatus)
				{
				case NavMeshPathStatus.PathComplete:
					SetObjectStatus();
					break;
				default:
					ObjectStatus = PathfinderStatus.Waiting;
					break;
				}
			}
			
			/// <summary>
			/// Locates a random position and attempts to find a valid
			/// path to the destination position.
			/// </summary>
			protected void ActivateOneTimeRandomPath()
			{
				CreateRandomPath();
			}
		#endregion
		
		#region :: ContinousRandomPosition Methods
			/// <summary>
			/// Updates the status for the Continous Random Position pathfinding.
			/// The method checks if the path is still complete and if the minimum distance to the target
			/// position is reached or not. If the target is reached, or not reachable, a new path will automatically be set.
			/// </summary>
			protected void UpdateContinousRandomPositionStatus()
			{
				switch(PathAgent.pathStatus)
				{
					case NavMeshPathStatus.PathComplete:
						if(SetObjectStatus() != PathfinderStatus.Moving)
							ActivateContinousRandomPath();
						break;
					default:
						ObjectStatus = PathfinderStatus.Waiting;
						StartCoroutine(DelayEnablePathAgent());
						break;
				}
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
			protected void ActivateContinousRandomPath()
			{
				CreateRandomPath();
			}
		#endregion
	}
}
