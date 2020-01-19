using Microsoft.Win32;
using SrtPlayer.Helpers;
using SrtPlayer.Objects;
using SrtPlayer.Windows;
using System.IO;
using System.Windows;

namespace SrtPlayer.ViewModels
{
	public class MainViewModel : AbstractViewModel
	{
		#region Fields

		private bool _isFilePlayChoiceVisible;

		private bool _isResumeFile;

		private bool _isPlayNewFile;

		private string _newSrtFile;

		private string _resumeLastSrtFileName;

		#endregion

		#region Properties

		public bool IsFilePlayChoiceVisible
		{
			get
			{
				return this._isFilePlayChoiceVisible;
			}
			set
			{
				if (value != this._isFilePlayChoiceVisible)
				{
					this._isFilePlayChoiceVisible = value;
					NotifyPropertyChanged();
				}
			}
		}

		public bool IsResumeFile
		{
			get
			{
				return this._isResumeFile;
			}
			set
			{
				if (value != this._isResumeFile)
				{
					this._isResumeFile = value;
					NotifyPropertyChanged();
					this.IsPlayNewFile = !this._isResumeFile;
				}
			}
		}

		public bool IsPlayNewFile
		{
			get
			{
				return this._isPlayNewFile;
			}
			set
			{
				if (value != this._isPlayNewFile)
				{
					this._isPlayNewFile = value;
					NotifyPropertyChanged();
					this.IsResumeFile = !this._isPlayNewFile;
				}
			}
		}
		
		public string ResumeLastSrtFile { get; set; }

		public string ResumeLastSrtFileName
		{
			get
			{
				return this._resumeLastSrtFileName;
			}
			set
			{
				if (value != this._resumeLastSrtFileName)
				{
					this._resumeLastSrtFileName = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string NewSrtFile
		{
			get
			{
				return this._newSrtFile;
			}
			set
			{
				if (value != this._newSrtFile)
				{
					this._newSrtFile = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string SrtFileFullName
		{
			get
			{
				return this.IsResumeFile
					? this.ResumeLastSrtFile
					: this.NewSrtFile;
			}
		}

		public UserProfile UserProfile { get; set; }

		public RelayCommand ChooseFileCommand { get; set; }

		public RelayCommand PlayFileCommand { get; set; }

		#endregion

		#region Constructor

		public MainViewModel()
		{
			this.ChooseFileCommand = new RelayCommand(c => { this.ExecuteChooseFileCommand(); }, c => this.CanExecuteChooseFileCommand());
			this.PlayFileCommand = new RelayCommand(c => { this.ExecutePlayFileCommand(); }, c => this.CanExecutePlayFileCommand());

			this.LoadUserProfile();
		}

		#endregion

		#region Methods

		private void LoadUserProfile()
		{
			this.UserProfile = UserProfileHelper.LoadUserProfile();

			if (!string.IsNullOrEmpty(this.UserProfile.LastPlayedFile))
			{
				this.ResumeLastSrtFile = this.UserProfile.LastPlayedFile;
				this.ResumeLastSrtFileName = new FileInfo(this.ResumeLastSrtFile).Name;
				this.IsFilePlayChoiceVisible = true;
				this.IsResumeFile = true;
			}
		}

		private bool CanExecuteChooseFileCommand()
		{
			return !this.IsResumeFile;
		}

		private void ExecuteChooseFileCommand()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				Filter = ".srt files|*.srt"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				this.NewSrtFile = openFileDialog.FileName;
			}
		}

		private bool CanExecutePlayFileCommand()
		{
			return !string.IsNullOrEmpty(this.SrtFileFullName);
		}

		private void ExecutePlayFileCommand()
		{
			SrtFile srtFile = new SrtFile(this.SrtFileFullName);
			string error = srtFile.LoadFile();

			if (string.IsNullOrEmpty(error))
			{
				SubtitlesViewModel subtitlesViewModel = new SubtitlesViewModel(srtFile, this.UserProfile, this.IsResumeFile);
				new SubtitlesWindow(subtitlesViewModel).Show();
			}
			else
			{
				MessageBox.Show(error);
			}
		}

		#endregion
	}
}
