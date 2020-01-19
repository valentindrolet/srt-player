using System;
using System.Xml;
using System.Xml.Serialization;

namespace SrtPlayer.Objects
{
	[Serializable()]
	public class UserProfile
	{
		[XmlElement("TimeIntervalMs")]
		public double TimeIntervalMs
		{
			get
			{
				return TimeInterval.TotalMilliseconds;
			}
			set
			{
				TimeInterval = TimeSpan.FromMilliseconds(value);
			}
		}

		[XmlIgnore]
		public TimeSpan TimeInterval { get; set; }

		[XmlElement("AutoPlayWindowDeactived")]
		public bool AutoPlayWindowDeactived { get; set; }

		[XmlElement("PlayNextDirectoryFile")]
		public bool PlayNextDirectoryFile { get; set; }

		[XmlElement("LastPlayedFile")]
		public string LastPlayedFile { get; set; }

		[XmlElement("LastPlayedFileTimeTicks")]
		public long LastPlayedFileTimeTicks { get; set; }

		public UserProfile()
		{
			TimeIntervalMs = 500;
			AutoPlayWindowDeactived = false;
			PlayNextDirectoryFile = true;
			LastPlayedFile = string.Empty;
			LastPlayedFileTimeTicks = 0;
		}
	}
}
