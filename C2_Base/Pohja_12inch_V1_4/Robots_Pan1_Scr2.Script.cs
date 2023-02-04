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
    
    
    public partial class Robots_Pan1_Scr2
    {
		int robotti = 2;
		
		void Robots_Pan1_Scr2_Opened(System.Object sender, System.EventArgs e)
		{
			
		}
		
		/// Lähettää numeronapin painalluksen robotille.
		void Button_Virtoip_1_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Kuittaus 1 - " + Numero.Value);
			Globals.Robotit.robotit[robotti].KuittaaHairio(1, Convert.ToInt16(Numero.Value));
		}
		
		/// Lähettää numeronapin painalluksen robotille.
		void Button_Virtoip_2_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Kuittaus 2 - " + Numero.Value);
			Globals.Robotit.robotit[robotti].KuittaaHairio(2, Convert.ToInt16(Numero.Value));
		}
		
		/// Lähettää numeronapin painalluksen robotille.
		void Button_Virtoip_3_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Kuittaus 3 - " + Numero.Value);
			Globals.Robotit.robotit[robotti].KuittaaHairio(3, Convert.ToInt16(Numero.Value));
		}
		
		/// Lähettää numeronapin painalluksen robotille.
		void Button_Virtoip_4_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Kuittaus 4 - " + Numero.Value);
			Globals.Robotit.robotit[robotti].KuittaaHairio(4, Convert.ToInt16(Numero.Value));
		}
		
		/// Lähettää numeronapin painalluksen robotille.
		void Button_Virtoip_5_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Kuittaus 5 - " + Numero.Value);
			Globals.Robotit.robotit[robotti].KuittaaHairio(5, Convert.ToInt16(Numero.Value));
		}
		
		/// Lähettää numeronapin painalluksen robotille.
		void Button_Virtoip_6_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Kuittaus 6 - " + Numero.Value);
			Globals.Robotit.robotit[robotti].KuittaaHairio(6, Convert.ToInt16(Numero.Value));	
		}
		
		/// Lähettää numeronapin painalluksen robotille.
		void Button_Virtoip_7_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Kuittaus 7 - " + Numero.Value);
			Globals.Robotit.robotit[robotti].KuittaaHairio(7, Convert.ToInt16(Numero.Value));
		}
		
		/// Lähettää numeronapin painalluksen robotille.		
		void Button_Virtoip_8_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Kuittaus 8 - " + Numero.Value);
			Globals.Robotit.robotit[robotti].KuittaaHairio(8, Convert.ToInt16(Numero.Value));
		}
		
		/// Lähettää robotille pyynnön ajaa huoltoasemaan.
		void Button_ServicePos_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Ajopyyntö huoltoasemaan.");
			Globals.Robotit.robotit[robotti].AjaHuoltoon();
			Globals.Tags.Rob1_ServicePosSent.Value = true;
		}

	}
}
