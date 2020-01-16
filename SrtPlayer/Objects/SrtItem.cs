using System;
using System.Collections.Generic;

namespace SrtPlayer.Objects
{
	public class SrtItem
	{
		#region Public properties

		public TimeSpan BeginTime { get; set; }

		public TimeSpan EndTime { get; set; }

		public List<string> Lines { get; set; }

		#endregion

		#region Constructors

		public SrtItem()
		{

		}

		#endregion
	}
}
