using UnityEngine;
using System.Collections;

namespace mnUtilities.Pathfinding
{
	[AddComponentMenu("Pathfinding/PathfinderRandomPosition")]
	public class PathfinderRandomPosition : PathfinderBase 
	{
		public enum RandomPathType
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
		}

		public RandomPathType TypeOfRandomPathfinding = RandomPathType.ContinousRandomPosition;
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
				switch(TypeOfRandomPathfinding)
				{
					case RandomPathType.ContinousRandomPosition:
						UpdateContinousRandomPositionStatus();
						break;
					case RandomPathType.OneTimeRandomPosition:
						UpdateOneTimeRandomPositionStatus();
						break;
				}
			}
			
			UpdateAnimationData();
		}
		
		/// <summary>
		/// Co-routine which enables the NavMeshAgent
		/// at the end of the frame.
		/// </summary>
		private IEnumerator DelayEnablePathAgent()
		{
			yield return new WaitForEndOfFrame();
			EnableRandomPositionPathAgent();
		}
								
		/// <summary>
		/// Enables the Pathfinder and locates a destination position based on the components settings.
		/// If a valid path is found, the object will start to move towards the destination position automatically. 
		/// </summary>
		public void EnableRandomPositionPathAgent()
		{
			EnablePathAgent();
			ObjectStatus = PathfinderStatus.Waiting;				
			switch(TypeOfRandomPathfinding)
			{
				case RandomPathType.ContinousRandomPosition:
					ActivateContinousRandomPath();
					break;
				case RandomPathType.OneTimeRandomPosition:
					ActivateOneTimeRandomPath();
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
			PathfinderStatus status = CreateRandomPath(MinRandomAreaPoint, MaxRandomAreaPoint);
			if(status == PathfinderStatus.PathNotFound)
				StartCoroutine(DelayEnablePathAgent());
			else
				ObjectStatus = PathfinderStatus.Moving;
		}

		/*
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
				ObjectStatus = PathfinderStatus.PathNotFound;
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
		*/
		
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
					ActivateOneTimeRandomPath();
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
						ActivateContinousRandomPath();
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
