using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SrtPlayer.Objects
{
	public class SrtFile
	{
		#region Public properties

		public FileInfo FileInfo { get; set; }

		public List<SrtItem> SrtItems { get; set; }

		#endregion

		#region Constructor

		public SrtFile(string fileName)
		{
			this.FileInfo = new FileInfo(fileName);
			this.SrtItems = new List<SrtItem>();
		}

		#endregion

		#region Methods

		public string LoadFile()
		{
			try
			{
				// 0 -> id | 1 -> times | 2 -> lines
				int actualItemPart = 0;

				SrtItem srtItem = null;
				using (StreamReader sr = new StreamReader(
					this.FileInfo.FullName,
					Encoding.GetEncoding("iso-8859-1")))
				{
					while (!sr.EndOfStream)
					{
						string actualLine = sr.ReadLine();

						if (!string.IsNullOrEmpty(actualLine))
						{
							switch (actualItemPart)
							{
								case 0:
									srtItem = new SrtItem();
									srtItem.Lines = new List<string>();
									actualItemPart++;
									break;
								case 1:
									int endBeginTime = actualLine.IndexOf(' ');
									srtItem.BeginTime = TimeSpan.Parse(actualLine.Substring(0, endBeginTime).Replace(',', '.'));

									int beginEndTime = actualLine.LastIndexOf(' ') + 1;
									srtItem.EndTime = TimeSpan.Parse(actualLine.Substring(beginEndTime, actualLine.Length - beginEndTime).Replace(',', '.'));

									actualItemPart++;
									break;
								case 2:
									srtItem.Lines.Add(actualLine);
									break;
							}
						}
						else
						{
							this.SrtItems.Add(srtItem);
							actualItemPart = 0;
						}
					}
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}

			return this.SrtItems.Count > 0
				? string.Empty
				: "No subtitle were found";
		}

		public SrtFile FindNextFile()
		{
			var actualDirectory = this.FileInfo.Directory;
			var directoryFiles = actualDirectory.EnumerateFiles().ToList();

			int actualFileIndex = directoryFiles.FindIndex(f => f.Name == this.FileInfo.Name);

			var nextFileInfo = directoryFiles.ElementAt(actualFileIndex + 1);
			if (nextFileInfo != null)
			{
				return new SrtFile(nextFileInfo.FullName);
			}

			return null;
		}

		#endregion
	}
}
