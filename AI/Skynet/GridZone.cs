using UnityEngine;
using System.Collections;

namespace AI.Skynet
{
	[System.Serializable]
	public class GridZone
	{
		/// <summary>
		/// Get the world position where the GridZone object is set.
		/// </summary>
		public Vector3 Position
		{
			get { return m_position; }
		}

		/// <summary>
		/// Get the GamObject which the GridZone object is attached to.
		/// </summary>
		public GameObject GridObject
		{
			get { return m_gridGameObject; }
		}

		private Bounds m_boundingBox;
		private GameObject m_gridGameObject = null;
		private Vector3 m_position = Vector3.zero;
		private Transform m_transformComponent = null;

		/// <summary>
		/// Attach the component to a GameObject and (optional) its parent.
		/// </summary>
		/// <param name="newObject">Object which the component will be attached to.</param>
		/// <param name="parent">(Optional)Parent object for the component.</param>
		public void SetGameObject(GameObject newObject, Transform parent)
		{
			m_gridGameObject = newObject;
			if(SetTransform() == true)
				m_transformComponent.parent = parent;
		}

		/// <summary>
		/// Performs a check if a point is located within the internal Bounds area.
		/// </summary>
		/// <param name="point">The world vector position point which will be checked.</param>
		/// <returns>True if the point is within the bouing area. Return false otherwise.</returns>
		public bool ContainsInBoundingBox(Vector3 point)
		{
			if(m_boundingBox != null)
				return m_boundingBox.Contains(point);

			return false;
		}

		/// <summary>
		/// Creates a Bounds object around the object.
		/// The Bound will be placed on the internal position vector.
		/// </summary>
		/// <param name="size">Size of the Bounds object.</param>
		public void CreateCollisionBox(Vector3 size)
		{
			CreateCollisionBox(m_position, size);
		}

		/// <summary>
		/// Creates a Bounds object around the object.
		/// </summary>
		/// <param name="position">The position where the Bounds object will be placed.</param>
		/// <param name="size">Size of the Bounds object.</param>
		public void CreateCollisionBox(Vector3 position, Vector3 size)
		{
			if(SetTransform() == true)
			{
				BoxCollider colliderObject = m_gridGameObject.GetComponent<BoxCollider>();
				if(colliderObject == null)
					colliderObject = m_gridGameObject.AddComponent<BoxCollider>();

				colliderObject.size = size;
				m_boundingBox = colliderObject.bounds;
			}
			else 
				m_boundingBox = new Bounds(position, size);
		}

		/// <summary>
		/// Set the component on a specific position.
		/// </summary>
		/// <param name="position">World position where the component is placed.</param>
		public void SetPosition(Vector3 position)
		{
			if(SetTransform() == true)
				m_transformComponent.position = position;

			m_position = position;
		}

		/// <summary>
		/// Set a local Transform reference.
		/// Setting the Transform is only possible if the component is attached to a GameObject.
		/// If the component is not attached to a GameObject the Transform component is ignored.
		/// </summary>
		/// <returns>True if the compnent has a valid Transform component. Return false otherwise.</returns>
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
