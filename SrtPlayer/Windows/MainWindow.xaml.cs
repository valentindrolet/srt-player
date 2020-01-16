using Microsoft.Win32;
using System;
using System.Windows;
using SrtPlayer.Objects;

namespace SrtPlayer.Windows
{
	public partial class MainWindow : Window
	{
		private SubtitlesWindow _subtitlesWindow;

		private SrtFile _srtFile;

		public MainWindow()
		{
			InitializeComponent();

			WindowStartupLocation = WindowStartupLocation.CenterScreen;
		}

		private void btn_play_Click(object sender, RoutedEventArgs e)
		{
			_subtitlesWindow = new SubtitlesWindow(this._srtFile);

			_subtitlesWindow.Show();
		}

		private void btn_fileDiag_Click(object sender, RoutedEventArgs e)
		{
			btn_play.IsEnabled = false;
	
			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				Filter = ".srt files|*.srt"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				txb_fileName.Text = openFileDialog.FileName;

				_srtFile = new SrtFile(txb_fileName.Text);
				string error = _srtFile.LoadFile();

				if (string.IsNullOrEmpty(error))
				{
					btn_play.IsEnabled = true;
				}
				else
				{
					MessageBox.Show(error);
				}
			}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			if (_subtitlesWindow != null)
			{
				_subtitlesWindow.Close();
			}
		}
	}
}
