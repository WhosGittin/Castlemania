using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StatSystem
{
	public class PlayerStats : IBaseStats
	{
		private IStat _strength;
		public IStat strength
		{
			get => _strength;
			set => _strength = value;
		}

		private IStat _dexterity;
		public IStat dexterity
		{
			get => _dexterity;
			set => _dexterity = value;
		}

		public IStat _movementSpeed;
		public IStat movementSpeed
		{
			get => _movementSpeed;
			set => _movementSpeed = value;
		}

		public PlayerStats(int str, int dex, int movement)
		{
			_strength.name = "Strength";
			_strength.value = str;
			_dexterity.name = "Dexterity";
			_dexterity.value = dex;
			_movementSpeed.name = "Movement Speed";
			_movementSpeed.value = movement;
		}

	}


}
