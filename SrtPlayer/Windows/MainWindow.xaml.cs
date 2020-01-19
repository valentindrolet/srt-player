using SrtPlayer.ViewModels;
using System;
using System.Windows;

namespace SrtPlayer.Windows
{
	public partial class MainWindow : Window
	{
		private MainViewModel _mainViewModel;

		public MainWindow()
		{
			InitializeComponent();

			WindowStartupLocation = WindowStartupLocation.CenterScreen;

			this._mainViewModel = new MainViewModel();
			base.DataContext = this._mainViewModel;
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			App.Current.Shutdown();
		}
	}
}
