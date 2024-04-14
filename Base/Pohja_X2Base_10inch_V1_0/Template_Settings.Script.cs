//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
	public partial class Template_Settings
	{
		void btnSubmenu_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.BtnHandler(
				1, // allways 10* seriaes screens
				Neo.ApplicationFramework.Generated.Tags.Screens.Settings,
				((Neo.ApplicationFramework.Controls.Script.ButtonCFAdapter)sender).Name,
				((Neo.ApplicationFramework.Controls.Script.ButtonCFAdapter)sender).Text.Length);
		}
	}
}
