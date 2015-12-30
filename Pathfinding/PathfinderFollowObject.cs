using UnityEngine;
using System.Collections;

namespace mnUtilities.Pathfinding
{
	public class PathfinderFollowObject : PathfinderBase 
	{
		public enum FollowObjectType
		{
			FollowObject = 0,
			FollowRandomObject = 1,
		}

		public FollowObjectType TypeOfRandomPathfinding = FollowObjectType.FollowObject;

		public Transform ObjectToFollow = null;
		public float Duration = 1.0f;
		public float MinDistanceToObject = 1.0f;
		public float MaxDistanceToObject = 10.0f;

		private float m_elapsedTime = 0.0f;
		private float m_currentDistance = 0.0f;

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
				PathMeshObstacle.enabled = true;
				
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
			m_elapsedTime += (Time.deltaTime * Time.timeScale);
			float procentage = (m_elapsedTime / Duration);

			if(PathAgent.isActiveAndEnabled == true)
			{
				if(procentage >= 1.0f)
					EnableFollowobjectPathAgent();

				switch(TypeOfRandomPathfinding)
				{
					case FollowObjectType.FollowObject:
						UpdateFollowObjectPositionStatus();
						break;
					case FollowObjectType.FollowRandomObject:
						break;
				}
			}
			else if(procentage >= 1.0f && CheckDistanceToObject() == true)
				EnableFollowobjectPathAgent();
			
			UpdateAnimationData();
		}

		public bool CheckDistanceToObject()
		{
			if(ObjectToFollow != null)
			{
				m_currentDistance = Vector3.Distance(ObjectToFollow.position, m_transformComponent.position);
				if(m_currentDistance < MinDistanceToObject || m_currentDistance > MaxDistanceToObject)
					return false;
			}

			return true;
		}
		
		/// <summary>
		/// Co-routine which enables the NavMeshAgent
		/// at the end of the frame.
		/// </summary>
		private IEnumerator DelayEnablePathAgent()
		{
			yield return new WaitForEndOfFrame();
			EnableFollowobjectPathAgent();
		}
					
		public void EnableFollowobjectPathAgent()
		{
			m_elapsedTime = 0.0f;
			ObjectStatus = PathfinderStatus.Waiting;
			Vector3 destinationPosition = m_transformComponent.position;
			if(ObjectToFollow != null)
			{
				switch(TypeOfRandomPathfinding)
				{
					case FollowObjectType.FollowObject:
						destinationPosition = ObjectToFollow.position;
						break;
					case FollowObjectType.FollowRandomObject:
						break;
				}

				if(CheckDistanceToObject() == true)
				{
					EnablePathAgent(destinationPosition);
					PathfinderStatus status = CreateDestinationPath();
					switch(status)
					{
						case PathfinderStatus.Finished:
						case PathfinderStatus.Moving:
						case PathfinderStatus.PathFound:
						case PathfinderStatus.Waiting:
							ObjectStatus = PathfinderStatus.Moving;
							return;
					}
				}
			}

			StartCoroutine(DelayEnablePathAgent());
		}

		#region FollowObject
			protected void UpdateFollowObjectPositionStatus()
			{
				PathfinderStatus status = SetObjectStatus();
				switch(PathAgent.pathStatus)
				{
					case NavMeshPathStatus.PathComplete:
						switch(ObjectStatus)
						{
							case PathfinderStatus.Finished:
								StopPathfinding();
								break;
							case PathfinderStatus.Moving:
								break;
							default:
								EnableFollowobjectPathAgent();
								break;
						}

					
						break;
					default:
						//EnableFollowobjectPathAgent();
						break;
				}
			}
		#endregion

		#region FollowRandomObject

		#endregion
	}
}
