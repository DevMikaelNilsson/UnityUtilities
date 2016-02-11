using UnityEngine;
using System.Collections;

namespace mnUtilities.AI.Skynet
{
	[System.Serializable]
	public class GridZone
	{
		public Vector3 Position
		{
			get { return m_position; }
		}

		public GameObject GridObject
		{
			get { return m_gridGameObject; }
		}

		private GameObject m_gridGameObject = null;
		private Vector3 m_position = Vector3.zero;

		private Transform m_transformComponent = null;

		public void SetGameObject(GameObject newObject, Transform parent)
		{
			m_gridGameObject = newObject;
			if(SetTransform() == true)
				m_transformComponent.parent = parent;
		}

		public void CreateCollisionBox(Vector3 size)
		{
			if(SetTransform() == true)
			{
				BoxCollider colliderObject = m_gridGameObject.GetComponent<BoxCollider>();
				if(colliderObject == null)
					colliderObject = m_gridGameObject.AddComponent<BoxCollider>();

				colliderObject.size = size;
			}
		}

		public void SetPosition(Vector3 position)
		{
			if(SetTransform() == true)
				m_transformComponent.position = position;

			m_position = position;
		}

		private bool SetTransform()
		{
			if(m_gridGameObject != null)
			{
				if(m_transformComponent == null)
					m_transformComponent = m_gridGameObject.GetComponent<Transform>();

				return true;
			}

			m_transformComponent = null;
			return false;
		}
	}
}
