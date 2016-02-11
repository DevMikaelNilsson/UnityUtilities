using UnityEngine;
using System.Collections;

namespace mnUtilities.AI.Skynet
{
	public class Skynet : MonoBehaviour 
	{
		public Transform Target = null;
		public bool CreateGrid = true;

		private Grid m_gridComponent = null;

		void OnEnable()
		{
			if(CreateGrid == true)
			{
				if(m_gridComponent == null)
					m_gridComponent = this.gameObject.GetComponent<Grid>();
				if(m_gridComponent == null)
					m_gridComponent = this.gameObject.AddComponent<Grid>();

				m_gridComponent.InitializeGrid();
			}
		}
	}
}
