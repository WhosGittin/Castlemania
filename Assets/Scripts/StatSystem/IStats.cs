using System;

namespace StatSystem
{
	public interface IStat
	{
		public string name { get; set; }
		public int value { get; set; }
	}

	public interface IBaseStats
	{
		public IStat strength { get; set; }
		public IStat dexterity { get; set; }
		public IStat movementSpeed { get; set; }
	}
}
