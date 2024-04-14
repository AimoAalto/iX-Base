using System;

namespace Core.Api.DataSource
{
	public sealed class ValueChangedEventArgs : EventArgs
	{
		private readonly object o = null;

		public ValueChangedEventArgs(object value) { o = value;  }

		public object Value { get { return o; } }
	}
}