using UnityEngine;
using System.Collections;

namespace mnUtilities.Pathfinding
{
	public class PathfinderMoveTo : PathfinderBase 
	{
		public float MinDistanceToDestination = 1.0f;

		void OnEnable()
		{
			InitializeNavAgentBase();
		}

		void Update()
		{
			if(IsPathfinderActive == true && NavAgent.isOnNavMesh == true)
			{
				if(NavAgent.remainingDistance <= MinDistanceToDestination)
					StopPathfinder();
			}
		}
	}
}
