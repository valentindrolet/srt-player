using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SrtPlayer.Objects
{
	public abstract class AbstractViewModel : INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
