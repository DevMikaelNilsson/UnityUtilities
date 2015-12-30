using UnityEngine;
using System.Collections;

namespace mnUtilities.CharacterProfile
{
	public class CharacterProfile
	{
		public string Name
		{
			get {return m_characterName;}
		}

		public int Health
		{
			get {return m_health;}
		}

		public bool IsActive
		{
			get {return m_isActive;}
		}

		public bool IsGameOver
		{
			get {return m_isGameOver;}
		}

		private string m_characterName = string.Empty;
		private bool m_isActive = true;
		private bool m_isGameOver = false;
		private int m_health = 100;

		public CharacterProfile(string name)
		{
			m_characterName = name;
		}

		public int SubtractHealth(int value)
		{
			m_health = UnityEngine.Mathf.Clamp((m_health - value), 0, m_health);
			return m_health;
		}

		public int AddHealth(int value)
		{
			m_health += value;
			return m_health;
		}

		public void SetHealth(int value)
		{
			m_health = value;
		}

		public void SetIsActive(bool isActive)
		{
			m_isActive = isActive;
		}

		public void SetIsGameOver(bool isGameOver)
		{
			m_isGameOver = isGameOver;
		}
	}
}
