﻿using System;
using System.Windows;
using System.Windows.Input;
using SrtPlayer.Objects;
using SrtPlayer.ViewModels;

namespace SrtPlayer.Windows
{
	public partial class SubtitlesWindow : Window
	{
		SubtitlesViewModel _subtitlesViewModel;

		public SubtitlesWindow(SrtFile srtFile)
		{
			InitializeComponent();

			_subtitlesViewModel = new SubtitlesViewModel(srtFile);

			base.DataContext = _subtitlesViewModel;

			this.Topmost = true;

			double topPos = SystemParameters.WorkArea.Height - this.Height;
			if (topPos > 0)
				this.Top = topPos;

			double leftPos = (SystemParameters.WorkArea.Width - this.Width) / 2;
			if (leftPos > 0)
				this.Left = leftPos;
		}

		private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				this.DragMove();
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				if (e.ChangedButton == MouseButton.Left)
					this.DragMove();
			}
			catch
			{
			}
		}

		private void Grid_MouseEnter(object sender, MouseEventArgs e)
		{
			this._subtitlesViewModel.IsOptionsVisible = true;
		}

		private void Grid_MouseLeave(object sender, MouseEventArgs e)
		{
			this._subtitlesViewModel.IsOptionsVisible = false;
		}

		private void btn_stop_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Window_Deactivated(object sender, EventArgs e)
		{
			this._subtitlesViewModel.WindowDeactivated();
		}
	}
}
