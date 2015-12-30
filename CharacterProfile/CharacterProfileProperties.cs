using UnityEngine;
using System.Collections;

namespace mnUtilities.CharacterProfile
{
	public class CharacterProfileProperties : MonoBehaviour 
	{
		public bool LoadOnStart = true;
		public string Name = string.Empty;

		protected CharacterProfile m_characterProfile = null;

		void OnEnable()
		{
			if(LoadOnStart == true)
				LoadCharacterProfile(false);
		}

		public void LoadCharacterProfile(bool ceateIfMissing)
		{
			StartCoroutine(DelayLoadCharacterProfile(ceateIfMissing));
		}

		protected IEnumerator DelayLoadCharacterProfile(bool ceateIfMissing)
		{
			yield return new WaitForEndOfFrame();
			m_characterProfile = CharacterProfileHandler.Instance.FindCharacterProfile(Name, ceateIfMissing);
		}

		public void ToggleIsActive(bool isActive)
		{
			if(CheckForCharacterProfile() == false)
				return;

			m_characterProfile.SetIsActive(isActive);			
		}

		public void ToggleIsGameOver(bool isGameOver)
		{
			if(CheckForCharacterProfile() == false)
				return;

			m_characterProfile.SetIsGameOver(isGameOver);
		}

		public int SubtractHealth(int value)
		{
			if(CheckForCharacterProfile() == false)
				return -1;

			return m_characterProfile.SubtractHealth(value);
		}

		public int AddHealth(int value)
		{
			if(CheckForCharacterProfile() == false)
				return -1;

			return m_characterProfile.AddHealth(value);
		}

		public void SetHealth(int value)
		{
			if(CheckForCharacterProfile() == false)
				return;

			m_characterProfile.SetHealth(value);
		}

		private bool CheckForCharacterProfile()
		{
			if(m_characterProfile != null)
				return true;
			else
				Debug.LogWarning(this + " - CharacterProfile is either missing or not valid.");

			return false;
		}
	}
}
