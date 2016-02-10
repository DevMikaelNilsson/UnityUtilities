using UnityEngine;
using System.Collections;

namespace mnUtilities.Pathfinding
{
	public class PathfinderFollowTarget : MonoBehaviour
	{
		public float MinDistanceToDestination = 1.0f;
		public Transform FollowObject = null;
		public float delay = 1.5f;

		private float m_elapsedTIme = 0.0f;
		private PathfinderBase m_pathfinder = null;

			void OnEnable()
			{
				if(m_pathfinder == null)
					m_pathfinder = this.GetComponent<PathfinderBase>();
				if(FollowObject != null)
					m_pathfinder.StartPathfinder(FollowObject.position);
			}


			void LateUpdate()
			{
			if(m_pathfinder == null)
				return;

			if(m_pathfinder.IsPathfinderActive == true && m_pathfinder.NavAgent.isOnNavMesh == true)
				{
				if(m_pathfinder.NavAgent.remainingDistance <= MinDistanceToDestination)
					{
						if(m_elapsedTIme >= delay)
						{
							m_pathfinder.StopPathfinder();
							m_elapsedTIme  = 0.0f;
						}
						else
							m_elapsedTIme += (Time.deltaTime * Time.timeScale);
					}
				}
				else if(m_pathfinder.IsPathfinderActive == false)
				{
					if(m_elapsedTIme >= delay)
					{
					if(Vector3.Distance(FollowObject.position, this.transform.position) > MinDistanceToDestination)
					{
						if(m_pathfinder.IsPathfinderActive == false)
							m_pathfinder.StartPathfinder(FollowObject.position);
						else
							m_pathfinder.UpdatePathfinder(FollowObject.position);

						m_elapsedTIme = 0.0f;
					}
					}
					else
						m_elapsedTIme += (Time.deltaTime * Time.timeScale);
				}

			}

	}
}
