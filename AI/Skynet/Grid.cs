using UnityEngine;
using System.Collections;

namespace mnUtilities.AI.Skynet
{
	public class Grid : MonoBehaviour 
	{
		public Vector3 EndPosition = Vector3.zero;
		public Vector3 Size = Vector3.one;
		public bool CreateGameObjects = false;

		private Transform m_transformComponent = null;
		private GridZone []m_gridComponents = null;

		public void InitializeGrid()
		{
			if(m_transformComponent == null)
				m_transformComponent = this.GetComponent<Transform>();

			Vector3 positionDistance = (EndPosition - m_transformComponent.position);
			int xCount = Mathf.FloorToInt(Mathf.Abs(positionDistance.x) / Size.x);
			int yCount = Mathf.FloorToInt(Mathf.Abs(positionDistance.y) / Size.y);
			int zCount = Mathf.FloorToInt(Mathf.Abs(positionDistance.z) / Size.z);
			xCount = Mathf.Clamp(xCount, 1, xCount);
			yCount = Mathf.Clamp(yCount, 1, yCount);
			zCount = Mathf.Clamp(zCount, 1, zCount);


			float xDirection = Mathf.Clamp(positionDistance.x, -1.0f, 1.0f);
			float yDirection = Mathf.Clamp(positionDistance.y, -1.0f, 1.0f);
			float zDirection = Mathf.Clamp(positionDistance.z, -1.0f, 1.0f);

			m_gridComponents = new GridZone[((xCount * yCount) * zCount)];
			int arrayCount = 0;
			for(int z = 0; z < zCount; ++z)
			{
				for(int x = 0; x < xCount; ++x)
				{
					for(int y = 0; y < yCount; ++y)
					{
						Vector3 pos = CalculatePosition(x, y, z, xDirection, yDirection, zDirection);
						m_gridComponents[arrayCount] = CreateNewZone(pos, this.transform);
						arrayCount++;
					}
				}
			}
		}

		private Vector3 CalculatePosition(int xIndex, int yIndex, int zIndex, float xDir, float yDir, float zDir)
		{
			float x = (m_transformComponent.position.x + ((Size.x * xIndex) * xDir));
			float y = (m_transformComponent.position.y + ((Size.y * yIndex) * yDir));
			float z = (m_transformComponent.position.z + ((Size.z * zIndex) * zDir));
			return new Vector3(x, y, z);
		}

		private GridZone CreateNewZone(Vector3 position, Transform parent)
		{
			GridZone newZoneGrid = new GridZone();
			if(CreateGameObjects == true && Application.isEditor == true)
			{
				newZoneGrid.SetGameObject(new GameObject(), parent);
				newZoneGrid.CreateCollisionBox(Size);
			}

			newZoneGrid.SetPosition(position);
			return newZoneGrid;
		}

		public GridZone RegisterObjectWithZone(Transform objectToRegister)
		{
			float closestDistance = 0.0f;
			int objectCount = m_gridComponents.Length;
			GridZone currentGridZone = null;
			for(int i = 0; i < objectCount; ++i)
			{
				float distance = Vector3.Distance(objectToRegister.position, m_gridComponents[i].Position);

				if(currentGridZone == null)
				{
					currentGridZone = m_gridComponents[i];
					closestDistance = distance;
				}
				else
				{
					Vector3 positionDistance = (objectToRegister.position - m_gridComponents[i].Position);

					if(distance < closestDistance)
					{
						currentGridZone = m_gridComponents[i];
						closestDistance = distance;
					}
				}
			}

			return currentGridZone;
		}

		public bool UnregisterObjectWithZone(Transform objectToUnregister)
		{

			return false;
		}

		public bool UnregisterObjectWithZone(Transform objectToUnregister, GridZone registeredZone)
		{

			return false;
		}
	}
}
