using UnityEngine;
using System.Collections;

namespace UnityUtilities.Transformation
{
	[AddComponentMenu("Transformation/Translate")]
	public class Translate : MonoBehaviour 
	{
		[Tooltip("The object which will be translated. If empty the object, which the component is attached to, will be translated.")]
		public Transform TranslateObject = null;
		[Tooltip("Enable to allow the object to be translated when the component is enabled/re-enabled. If this flag is set to false, the translation can be manually activated.")]
		public bool ActiveOnStart = true;		
		[Tooltip("Which axis the object will be translated.")]
		public Vector3 TranslationAxis = Vector3.right;
		[Tooltip("Speed of the translation. This final position value is mutliplied with deltaTime and timeScale. \nEx:\nPosition += ((TranslationAxis * TranslationSpeed) * (Time.deltaTime * Time.timeScale)).")]
		public float TranslationSpeed = 1.0f;
		[Tooltip("If the translation should be based on the local axis (self) or the world (world).")]
		public Space RelativeTo = Space.Self;
		[Tooltip("Flag to determin if the translation should occur. As long as this flag is set to true, the object will be translated every update cycle.")]
		public bool Active = true;

		/// <summary>
		/// Internal Unity method.
		/// This method is called once when the object is enabled/re-enabled.
		/// </summary>
		void OnEnable()
		{
			Active = ActiveOnStart;
		}

		/// <summary>
		/// Internal Unity method.
		/// This method is called once per update cycle.
		/// If active, the method will perform a translation of the object.
		/// </summary>
		void Update()
		{
			if (TranslateObject == null)
				TranslateObject = this.GetComponent<Transform>();

			if (Active == true && TranslateObject != null)
			{
				Vector3 relativeTranslationSpeed = ((TranslationAxis * TranslationSpeed) * (Time.deltaTime * Time.timeScale));
				TranslateObject.Translate(relativeTranslationSpeed, RelativeTo);
			}
		}
		
		/// <summary>
		/// Deactivates the translation.
		/// The position of the object will not be resetted or changed by this method.
		/// </summary>
		public void Deactivate()
		{
			Active = false;
		}
		
		/// <summary>
		/// Activates the translation with pre-defined values.
		/// </summary>
		public void Activate()
		{
			Active = true;
		}
		
		/// <summary>
		/// Activates the translation.
		/// </summary>
		/// <param name="axis">Which axis the object will be translated.</param>
		public void Activate(Vector3 axis)
		{
			TranslationAxis = axis;
			Activate();
		}
		
		/// <summary>
		/// Activates the translation.
		/// </summary>
		/// <param name="axis">Which axis the object will be translated.</param>
		/// <param name="speed">Speed of the translation. This final position value is mutliplied with deltaTime and timeScale. \nEx:\nPosition += ((TranslationAxis * TranslationSpeed) * (Time.deltaTime * Time.timeScale)).</param>
		public void Activate(Vector3 axis, float speed)
		{
			TranslationAxis = axis;
			TranslationSpeed = speed;
			Activate();
		}
	}
}
