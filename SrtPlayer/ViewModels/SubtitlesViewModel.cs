using SrtPlayer.Helpers;
using SrtPlayer.Objects;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace SrtPlayer.ViewModels
{
	public class SubtitlesViewModel : AbstractViewModel
	{
		#region Fields

		private SrtFile _srtFile;

		private SrtItem _actualSrtItem;

		private string _firstSubtitleLine;

		private string _secondSubtitleLine;

		private string _actualTimerInfo;

		private TimeSpan _actualTimer;

		private DispatcherTimer _dispatcherTimer;

		private Stopwatch _stopwatch;

		private TimeSpan _endTime;

		private bool _isPlayVisible;

		private TimeSpan _timeGap;

		private bool _isOptionsVisible;

		private SrtFile _nextFile;

		private string _fileTitle;

		private double _canvasOpacity;

		// not 0 because the mouse enter event wouldn't be trigger
		private double _transparentOpacity = 0.005d;

		private double _normalOpacity = 0.3d;

		private bool _isResumeFile;

		#endregion

		#region Properties

		public UserProfile UserProfile { get; set; }

		public string FirstSubtitleLine
		{
			get
			{
				return this._firstSubtitleLine;
			}
			set
			{
				if (value != this._firstSubtitleLine)
				{
					this._firstSubtitleLine = value;
					NotifyPropertyChanged();

					SetCanvasOpacity();
				}
			}
		}

		public string SecondSubtitleLine
		{
			get
			{
				return this._secondSubtitleLine;
			}
			set
			{
				if (value != this._secondSubtitleLine)
				{
					this._secondSubtitleLine = value;
					NotifyPropertyChanged();
				}
			}
		}

		public TimeSpan ActualTimer
		{
			get
			{
				return this._actualTimer;
			}
			set
			{
				if (value != this._actualTimer)
				{
					this._actualTimer = value;
					SetActualTimeInfo();
				}
			}
		}

		public string ActualTimerInfo
		{
			get
			{
				return this._actualTimerInfo;
			}
			set
			{
				if (value != this._actualTimerInfo)
				{
					this._actualTimerInfo = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string TimeInterval
		{
			get
			{
				return this.UserProfile.TimeIntervalMs.ToString();
			}
			set
			{
				if (value != this.UserProfile.TimeIntervalMs.ToString()
					&& int.TryParse(value, out int msTime))
				{
					this.UserProfile.TimeIntervalMs = msTime;
					NotifyPropertyChanged();
				}
			}
		}

		public bool IsPlayVisible
		{
			get
			{
				return this._isPlayVisible;
			}
			set
			{
				if (value != this._isPlayVisible)
				{
					this._isPlayVisible = value;
					NotifyPropertyChanged();
					NotifyPropertyChanged("IsPauseVisible");
				}
			}
		}

		public bool IsPauseVisible
		{
			get
			{
				return !this._isPlayVisible;
			}
		}

		public TimeSpan ElapsedTime
		{
			get
			{
				return this._stopwatch.Elapsed.Add(this._timeGap);
			}
		}

		public bool AutoPlayWindowDeactivated
		{
			get
			{
				return this.UserProfile.AutoPlayWindowDeactived;
			}
			set
			{
				if (value != this.UserProfile.AutoPlayWindowDeactived)
				{
					this.UserProfile.AutoPlayWindowDeactived = value;
					NotifyPropertyChanged();
				}
			}
		}

		public bool PlayNextFile
		{
			get
			{
				return this.UserProfile.PlayNextDirectoryFile;
			}
			set
			{
				if (value != this.UserProfile.PlayNextDirectoryFile)
				{
					this.UserProfile.PlayNextDirectoryFile = value;
					NotifyPropertyChanged();
				}
			}
		}

		public bool IsOptionsVisible
		{
			get
			{
				return this._isOptionsVisible;
			}
			set
			{
				if (value != this._isOptionsVisible)
				{
					this._isOptionsVisible = value;
					NotifyPropertyChanged();

					SetCanvasOpacity();
				}
			}
		}

		public string FileTitle
		{
			get
			{
				return this._fileTitle;
			}
			set
			{
				if (value != this._fileTitle)
				{
					this._fileTitle = value;
					NotifyPropertyChanged();
				}
			}
		}

		public double CanvasOpacity
		{
			get
			{
				return this._canvasOpacity;
			}
			set
			{
				if (value != this._canvasOpacity)
				{
					this._canvasOpacity = value;
					NotifyPropertyChanged();
				}
			}
		}

		public RelayCommand TimeBackwardCommand { get; set; }

		public RelayCommand TimeForwardCommand { get; set; }

		public RelayCommand PlayPauseCommand { get; set; }

		public RelayCommand CloseCommand { get; set; }

		#endregion

		#region Constructors

		public SubtitlesViewModel(SrtFile srtFile, UserProfile userProfile, bool isResumeFile)
		{
			this.UserProfile = userProfile;
			this._isResumeFile = isResumeFile;

			InitializeFile(srtFile);

			this.TimeBackwardCommand = new RelayCommand(c => { this.ExecuteTimeBackwardCommand(); }, c => this.CanExecuteTimeBackwardCommand());
			this.TimeForwardCommand = new RelayCommand(c => { this.ExecuteTimeForwardCommand(); }, c => this.CanExecuteTimeForwardCommand());
			this.PlayPauseCommand = new RelayCommand(c => { this.ExecutePlayPauseCommand(); }, c => this.CanExecutePlayPauseCommand());
		}

		#endregion

		#region Methods

		public void WindowDeactivated()
		{
			if (this.AutoPlayWindowDeactivated)
			{
				this.ExecutePlayPauseCommand(true);
			}
		}

		public void SaveUserProfile()
		{
			this.UserProfile.LastPlayedFile = this._srtFile.FileInfo.FullName;
			this.UserProfile.LastPlayedFileTimeTicks = this.ActualTimer.Ticks;
			UserProfileHelper.SaveUserProfile(this.UserProfile);
		}

		private void InitializeFile(SrtFile srtFile)
		{
			this._srtFile = srtFile;

			this.FileTitle = this._srtFile.FileInfo.Name;

			if (this._srtFile != null && this._srtFile.SrtItems?.Count > 0)
				this._endTime = this._srtFile.SrtItems.Last().EndTime;

			this._stopwatch = new Stopwatch();
			this._dispatcherTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(1)
			};
			this._dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
			this.IsPlayVisible = true;

			if (this._isResumeFile)
			{
				this._timeGap = new TimeSpan(this.UserProfile.LastPlayedFileTimeTicks);
			}
			else
			{
				this._timeGap = TimeSpan.Zero;
			}

			this.ActualTimer = this.ElapsedTime;

			SetActualSrtItem();

			if (this.PlayNextFile)
			{
				Thread loadNextFileThread = new Thread(this.LoadNextFile);
				loadNextFileThread.Start();
			}
		}

		private bool CanExecuteTimeBackwardCommand()
		{
			return this.ActualTimer > TimeSpan.Zero;
		}

		private void ExecuteTimeBackwardCommand()
		{
			TimeSpan futureTime = this.ActualTimer.Subtract(this.UserProfile.TimeInterval);

			if (futureTime > TimeSpan.Zero)
				this._timeGap = this._timeGap.Subtract(this.UserProfile.TimeInterval);
			else
				this._timeGap = this._timeGap.Subtract(this.ActualTimer);

			this.ActualTimer = this.ElapsedTime;
		}

		private bool CanExecuteTimeForwardCommand()
		{
			return this.ActualTimer < this._endTime;
		}

		private void ExecuteTimeForwardCommand()
		{
			TimeSpan futureTime = this.ActualTimer.Add(this.UserProfile.TimeInterval);

			if (futureTime < this._endTime)
				this._timeGap = this._timeGap.Add(this.UserProfile.TimeInterval);
			else
				this._timeGap = this._timeGap.Add(this._endTime.Subtract(this.ActualTimer));

			this.ActualTimer = this.ElapsedTime;
		}

		private bool CanExecutePlayPauseCommand()
		{
			return this.ActualTimer < this._endTime;
		}

		private void ExecutePlayPauseCommand(bool playOnly = false)
		{
			if (!this._dispatcherTimer.IsEnabled)
			{
				this._dispatcherTimer.Start();
				this._stopwatch.Start();
				this.IsPlayVisible = false;
			}
			else if (!playOnly)
			{
				this._dispatcherTimer.Stop();
				this._stopwatch.Stop();
				this.IsPlayVisible = true;
			}
		}

		private void DispatcherTimer_Tick(object sender, EventArgs e)
		{
			this.ActualTimer = this.ElapsedTime;

			if (this.ActualTimer < this._endTime)
			{
				if (this._actualSrtItem == null
					|| this.ActualTimer < this._actualSrtItem.BeginTime
					|| this.ActualTimer > this._actualSrtItem.EndTime)
				{
					SetActualSrtItem();
				}
			}
			else
			{
				if (this.PlayNextFile && this._nextFile != null)
				{
					this.InitializeFile(this._nextFile);
				}
				else
				{
					this.ExecutePlayPauseCommand();
					this.IsOptionsVisible = true;
				}
			}
		}

		private void SetActualSrtItem()
		{
			var actualItem = this._srtFile.SrtItems.Find(item => item.BeginTime <= this.ActualTimer && this.ActualTimer <= item.EndTime);

			if (actualItem != null)
			{
				this._actualSrtItem = actualItem;
				this.FirstSubtitleLine = this._actualSrtItem.Lines.Count > 0 ? this._actualSrtItem.Lines[0] : string.Empty;
				this.SecondSubtitleLine = this._actualSrtItem.Lines.Count > 1 ? this._actualSrtItem.Lines[1] : string.Empty;
			}
			else
			{
				SetDefaultSubtitles();
			}
		}

		private void LoadNextFile()
		{
			this._nextFile = this._srtFile.FindNextFile();
			if (this._nextFile != null)
			{
				this._nextFile.LoadFile();
			}
		}

		private void SetDefaultSubtitles()
		{
			this.FirstSubtitleLine = this.ActualTimer > TimeSpan.Zero ? string.Empty : this._srtFile.FileInfo.Name;
			this.SecondSubtitleLine = string.Empty;
		}

		private void SetActualTimeInfo()
		{
			this.ActualTimerInfo = this._actualTimer.ToString("hh':'mm':'ss'.'ff");
		}

		private void SetCanvasOpacity()
		{
			this.CanvasOpacity =
				string.IsNullOrEmpty(this.FirstSubtitleLine) && !this.IsOptionsVisible
					? this._transparentOpacity
					: this._normalOpacity;
		}

		public void WindowClosed()
		{
			this.SaveUserProfile();
			this._dispatcherTimer.Stop();
			this._stopwatch.Stop();
		}

		#endregion

	}
}
