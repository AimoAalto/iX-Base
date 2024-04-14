//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
	using System.Windows.Forms;
	using System;
	using System.Drawing;
	using Neo.ApplicationFramework.Tools;
	using Neo.ApplicationFramework.Common.Graphics.Logic;
	using Neo.ApplicationFramework.Controls;
	using Neo.ApplicationFramework.Interfaces;


	public partial class Overview_Pan1_Scr1
	{

		void Overview_Pan1_Scr1_Opened(System.Object sender, System.EventArgs e)
		{
			// Main menu painikkeiden visualisointi
			Globals.Tags.Menu_MainMenu_Btn_Anim.SetAnalog(0);

			// Alustetaan tuloradan valinta -1:ksi
			Globals.Tags.HMI_Overview_TrackSelected.Value = -1;
		}
	}
}