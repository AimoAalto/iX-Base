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
    
    
    public partial class Popup_NewPickPallet
    {
		
		void Popup_NewPickPallet_Opened(System.Object sender, System.EventArgs e)
		{

		}
		
		void Button_OK_Click(System.Object sender, System.EventArgs e)
		{
			// Miltä lavapaikalta lava tulee
			int lavapaikka = Globals.Tags.HMI_PalletChange.Value.Int;
			int robotti = 1;
			
			// Lokimerkintä
			Globals.Robotit.LisaaLokiin(robotti, "Ryhmittelylavanvaihto tuloradalle " + lavapaikka);

			Globals.Robotit.TeeRyhmittelynLavanvaihto(robotti, lavapaikka);
		}
    }
}