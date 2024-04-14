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
    
    
    public partial class Popup_BitOperation
    {
		void Btn_I16x2_Click(System.Object sender, System.EventArgs e)
		{
			int x = Globals.Tags.INT16.Value;
			x = x * 2;
			Globals.Tags.INT16.Value = x;
		}
		
		void Btn_I32x2_Click(System.Object sender, System.EventArgs e)
		{
			int x = Globals.Tags.INT32.Value;
			x = x * 2;
			Globals.Tags.INT32.Value = x;
		}
    }
}
