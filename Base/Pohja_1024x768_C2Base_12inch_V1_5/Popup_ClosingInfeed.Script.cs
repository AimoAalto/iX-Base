//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
	using Neo.ApplicationFramework.Controls.Script;
	using Neo.ApplicationFramework.Tools.OpcClient;
	using Neo.ApplicationFramework.Interfaces;
	using Neo.ApplicationFramework.Interfaces.Tag;
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Drawing;
	using System.Linq;
	using System.Windows;
	using System.Windows.Forms;
	using System.Windows.Media;
	using System.Runtime.InteropServices;
    
    
	public partial class Popup_ClosingInfeed
	{
		
		void Popup_ClosingInfeed_Opened(System.Object sender, System.EventArgs e)
		{
			try 
			{
				// 	hae tilaus- jne. tiedot
				int ci = Globals.Tags.HMI_ClosingInfeed.Value;
				//Globals.Tags.Line1_Rivinumero_TK1.ResetTag();
				int rivino = Globals.Tags.GetTagValue(string.Format("Line1_Rivinumero_TK{0}", ci));
				ANTuoteRiviNo.Value = rivino;
				ANProduct.Value = Globals.Ajotiedot.HaeTuloradanTuote(rivino);
				//Globals.Tags.Line1_PLC_KuvioNro_TK21.ResetTag();
				ANPatternNo.Value = Globals.Tags.GetTagValueString(string.Format("Line1_PLC_KuvioNro_TK{0}", ci));
				//Globals.Tags.S7HMI_ToHMI_Line_11_OrderNo.ResetTag();
				ANPatternNo.Value = Globals.Tags.GetTagValueString(string.Format("S7HMI_ToHMI_Line_{0}_OrderNo", ci));
			}
			catch (Exception)
			{
			}
		}
		
		void OK_btn_Click(System.Object sender, System.EventArgs e)
		{
			// set tag
			
			try 
			{
				string name = string.Format("S7HMI_Com_ToPLC_StopProduction_{0}", Globals.Tags.HMI_ClosingInfeed.Value);
				IBasicTag tag = Globals.Tags.GetTag(name);
				if (tag == null)
				{
					System.Windows.MessageBox.Show("Tarkista Tuloradan numero !");
				}
				else
				{
					tag.SetTag();
					Close();
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
