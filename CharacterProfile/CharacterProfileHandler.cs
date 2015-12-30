using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace mnUtilities.CharacterProfile
{
	public class CharacterProfileHandler : MonoBehaviour 
	{
		private static Dictionary<int, CharacterProfile> m_characterProfileList = new Dictionary<int, CharacterProfile>();
		private static CharacterProfileHandler m_currentInstance = null;
	
		public static CharacterProfileHandler Instance
		{
			get
			{
				if (m_currentInstance == null)
					m_currentInstance = (CharacterProfileHandler)FindObjectOfType(typeof(CharacterProfileHandler));

				// If it is still null, create a new instance
				if (m_currentInstance == null)
				{
					GameObject obj = new GameObject("CharacterProfileHandler");
					m_currentInstance = (CharacterProfileHandler)obj.AddComponent(typeof(CharacterProfileHandler));
				}

				return m_currentInstance;
			}
		}

		private void AddCharacterProfileToList(CharacterProfile profile)
		{
			m_characterProfileList.Add(m_characterProfileList.Count, profile);
		}

		private bool RemoveCharacterProfileFromList(CharacterProfile profile)
		{
			int objectCount = m_characterProfileList.Count;
			for(int i = 0; i < objectCount; ++i)
			{
				if(profile == m_characterProfileList[i])
				{
					m_characterProfileList.Remove(i);
					return true;
				}
			}

			return false;
		}

		private CharacterProfile GetCharacterProfile(string name)
		{
			int objectCount = m_characterProfileList.Count;
			for(int i = 0; i < objectCount; ++i)
			{
				if(string.Equals(name, m_characterProfileList[i].Name) == true)
					return m_characterProfileList[i];
			}

			return null;
		}

		public CharacterProfile CreateNewCharacterProfile(string name)
		{
			if(string.Equals(name, string.Empty) == false)
			{
				CharacterProfile newProfile = new CharacterProfile(name);
				AddCharacterProfileToList(newProfile);
				return newProfile;
			}
			else
				Debug.LogError(this + " - CharacterProfile must have a valid name.");

			return null;
		}

		public CharacterProfile FindCharacterProfile(string name, bool createNewIfNotFound)
		{
			CharacterProfile foundProfile = GetCharacterProfile(name);
			if(foundProfile == null && createNewIfNotFound == true)
				return CreateNewCharacterProfile(name);

			return foundProfile;
		}
	}
}
