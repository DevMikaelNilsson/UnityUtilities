using UnityEngine;
using System.Collections;

namespace AI.Skynet
{
	public class CreateGrid : MonoBehaviour 
	{
		/// <summary>
		/// End last point where a GridZone object will be placed.
		/// The starting point is the current position of this object and GridZone objects will be placed within the bounds of these two points.
		/// </summary>
		[Tooltip("End last point where a GridZone object will be placed. The starting point is the current position of this object and GridZone objects will be placed within the bounds of these two points.")]
		public Vector3 EndPosition = Vector3.zero;

		/// <summary>
		/// The size of a created GridZone object. The GridZone object will always be created as a 3D Box object.
		/// </summary>
		[Tooltip("The size of a created GridZone object. The GridZone object will always be created as a 3D Box object.")]
		public Vector3 Size = Vector3.one;

		/// <summary>
		/// Creates a child GameObject for each created GridZone object. The child object will automatically receive a CollisionBox added to visualize the GridZone object.
		/// This flag is normally just intended for debug/editor purposes and should not be used in a released environment.
		/// </summary>
		[Tooltip("Creates a child GameObject for each created GridZone object. This flag is normally just intended for debug/editor purposes and should not be used in a released environment.")]
		public bool CreateGameObjects = false;

		private Transform m_transformComponent = null;
		private GridZone []m_gridComponents = null;

		/// <summary>
		/// Initializes and creates a Grid based on the pre-defined settings.
		/// If a Grid is already created/existing, it will be overritten by calling
		/// this method.
		/// </summary>
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

		/// <summary>
		/// Calculates the position for a GridZone object.
		/// The position is calculated based on the components position, which index value the GridZone object has and which direction the entire grid should be positioned.
		/// </summary>
		/// <param name="xIndex">The current x-axis index value.</param>
		/// <param name="yIndex">The current y-axis index value.</param>
		/// <param name="zIndex">The current z-axis index value.</param>
		/// <param name="xDir">The direction in the x-axis which the grid is moving.</param>
		/// <param name="yDir">The direction in the y-axis which the grid is moving.</param>
		/// <param name="zDir">The direction in the z-axis which the grid is moving.</param>
		/// <returns>The new calculated position in the grid.</returns>
		private Vector3 CalculatePosition(int xIndex, int yIndex, int zIndex, float xDir, float yDir, float zDir)
		{
			float x = (m_transformComponent.position.x + ((Size.x * xIndex) * xDir));
			float y = (m_transformComponent.position.y + ((Size.y * yIndex) * yDir));
			float z = (m_transformComponent.position.z + ((Size.z * zIndex) * zDir));
			return new Vector3(x, y, z);
		}

		/// <summary>
		/// Creates the new GridZone object.
		/// </summary>
		/// <param name="position">The world position where the GridZone object will be positioned.</param>
		/// <param name="parent">Object which the GridZone object will be parented to. This variable is only applicable when the 'CreateGameObjects' flag is enabled.</param>
		/// <returns>Reference to the created GridZone object.</returns>
		private GridZone CreateNewZone(Vector3 position, Transform parent)
		{
			GridZone newZoneGrid = new GridZone();
			if(CreateGameObjects == true && Application.isEditor == true)
				newZoneGrid.SetGameObject(new GameObject(), parent);

			newZoneGrid.SetPosition(position);
			newZoneGrid.CreateCollisionBox(Size);
			return newZoneGrid;
		}

		/// <summary>
		/// Registers a object within a GridZone.
		/// The method searches for the closest GridZone object and registers the Object
		/// with that specific GridZone object. If the object is standing between two, more more, GridZone objects, 
		/// the first GridZone object which the object is within, will be returned.
		/// </summary>
		/// <param name="objectToRegister">Object to register.</param>
		/// <returns>The GridZone object which the object was registered with. If the object is not within the Grid, null will be returned by default.</returns>
		public GridZone RegisterObjectWithZone(Transform objectToRegister)
		{
			int objectCount = m_gridComponents.Length;
			for(int i = 0; i < objectCount; ++i)
			{
				if(m_gridComponents[i].ContainsInBoundingBox(objectToRegister.position) == true)
					return m_gridComponents[i];
			}

			return null;
		}
	}


}
