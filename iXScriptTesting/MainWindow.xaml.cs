using System.Windows;

namespace iXScriptTesting
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
#pragma warning disable IDE0052 // Remove unread private members
		readonly Neo.ApplicationFramework.Generated.Globals_ Globals;
#pragma warning restore IDE0052 // Remove unread private members

		public MainWindow()
		{
			Globals = new Neo.ApplicationFramework.Generated.Globals_();
			
			InitializeComponent();
		}
	}
}
