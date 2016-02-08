using UnityEngine;
using System.Collections;

namespace mnUtilities.AI.ActionList
{
	public class AiActionListBase : MonoBehaviour
	{
		public Transform Target = null;
		protected bool m_actionIsBlocking = false;
		protected bool m_canExecuteAction = false;
		protected Transform m_transformComponent = null;

		public bool Blocking
		{
			get {return m_actionIsBlocking;}
		}

		public virtual bool CanExecute()
		{
			return m_canExecuteAction;
		}

		public virtual void Execute()
		{

		}
	}
}
