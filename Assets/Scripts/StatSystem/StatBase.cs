using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StatSystem
{
	public class BaseStats : IBaseStats
	{
		public IStat strength
		{
			get;
			set;
		}

		public IStat attackRange { get; set; }

		public IStat dexterity
		{
			get;
			set;
		}

		public IStat movementSpeed
		{
			get;
			set;
		}

		public BaseStats(int str, int dex, int movement, int attackRangeValue)
		{
			strength = new Stat { name = "Strength", value = str };
			dexterity = new Stat { name = "Dexterity", value = dex };
			movementSpeed = new Stat { name = "Movement Speed", value = movement };
			attackRange = new Stat { name = "Attack Range", value = attackRangeValue };
		}

	}


}
