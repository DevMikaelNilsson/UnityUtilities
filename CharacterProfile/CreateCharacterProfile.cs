using UnityEngine;
using System.Collections;
using System.Text;

namespace mnUtilities.CharacterProfile
{
	public class CreateCharacterProfile : CharacterProfileProperties
	{
		public bool GenerateName = true;

		void OnEnable()
		{
			if(GenerateName == true)
			{
				StringBuilder newName = new StringBuilder();
				newName.Append(this.gameObject.name);
				newName.Append("_");
				newName.Append(this.gameObject.GetInstanceID());
				Name = newName.ToString();
			}

			if(LoadOnStart == true)
				LoadCharacterProfile(true);
		}
	}
}
