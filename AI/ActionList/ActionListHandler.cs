using UnityEngine;
using System.Collections;

namespace mnUtilities.AI.ActionList
{
	public class ActionListHandler : MonoBehaviour 
	{
		/// <summary>
		/// A list of possible actions the AI handler can perform. The execution (or attempt to execute) priority is made from the top of the list, 
		/// going downwards. So the higher a action is in the list, the more important is the action.
		/// </summary>
		[Tooltip("A list of possible actions the AI handler can perform. The execution (or attempt to execute) priority is made from the top of the list, going downwards. So the higher a action is in the list, the more important is the actionh.")]
		public ActionListBase []ActionList = null;

		/// <summary>
		/// Internal Unity method.
		/// this method is called once every frame update.
		/// This method will go through the action list and attempt to execute
		/// the action(s) if possible.
		/// </summary>
		void Update()
		{
			int objectCount = ActionList.Length;
			for(int i = 0; i < objectCount; ++i)
			{
				if(ActionList[i].CanExecute() == true)
					ActionList[i].Execute();
				if(ActionList[i].Blocking == true)
					break;
			}
		}
	}
}
