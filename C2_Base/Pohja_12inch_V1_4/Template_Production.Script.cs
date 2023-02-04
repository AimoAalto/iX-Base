//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
    using System;
    
    
    public partial class Template_Production
    {
		
		void Template_Production_Opened(System.Object sender, System.EventArgs e)
		{
		}
		
		void btnSubmenu_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.BtnHandler(
				Globals.Tags.Settings_PanelNumber.Value, 
				Neo.ApplicationFramework.Generated.Tags.Screens.Production,
				((Neo.ApplicationFramework.Controls.Script.ButtonAdapter)sender).Name, 
				((Neo.ApplicationFramework.Controls.Script.ButtonAdapter)sender).Text.Length);
		}
    }
}
