using SrtPlayer.Objects;
using System;
using System.IO;
using System.Xml.Serialization;

namespace SrtPlayer.Helpers
{
	public static class UserProfileHelper
	{
		private const string UserProfileDirectory = ".srtplayer";

		private const string UserProfileFileName = "userprofile.xml";

		public static UserProfile LoadUserProfile()
		{
			FileInfo userProfileFile = GetUserProfileFileInfo();

			if (userProfileFile.Exists)
			{
				FileStream fileStream = userProfileFile.OpenRead();
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserProfile));

				try
				{
					return xmlSerializer.Deserialize(fileStream) as UserProfile;
				}
				catch
				{
				}
			}

			return new UserProfile();
		}

		public static void SaveUserProfile(UserProfile userProfile)
		{
			FileInfo userProfileFile = GetUserProfileFileInfo();

			if (!Directory.Exists(userProfileFile.DirectoryName))
				Directory.CreateDirectory(userProfileFile.DirectoryName);

			FileStream fileStream = new FileStream(userProfileFile.FullName, FileMode.OpenOrCreate);

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserProfile));

			xmlSerializer.Serialize(fileStream, userProfile);
		}

		private static FileInfo GetUserProfileFileInfo()
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), UserProfileDirectory, UserProfileFileName);

			return new FileInfo(path);
		}
	}
}
