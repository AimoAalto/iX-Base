namespace Neo.ApplicationFramework.Generated
{
	using System;
    
    
	/// <summary>
	/// Toimii pohjana kaikille Robotti-pääkategorian sivuille
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 6.7.2017</remarks>
	public partial class Template_Robots
	{		
		void btnSubmenu_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.BtnHandler(
				Globals.Tags.Settings_PanelNumber.Value, 
				Neo.ApplicationFramework.Generated.Tags.Screens.Robots,
				((Neo.ApplicationFramework.Controls.Script.ButtonAdapter)sender).Name, 
				((Neo.ApplicationFramework.Controls.Script.ButtonAdapter)sender).Text.Length);
		}
		
		void Button_Virtoip_Click(System.Object sender, System.EventArgs e)
		{
			int robotti = Globals.Tags.HMI_RobotNo.Value;
			Int16 additionalno = Globals.Tags.HMI_AdditionalRobotErrorNo.Value;
			string btn_name = ((Neo.ApplicationFramework.Controls.Script.ButtonAdapter)sender).Name;
			
			try
			{
				string msg = string.Format("Virtoip r:{0} button: {1}", robotti, btn_name);
				System.Diagnostics.Trace.WriteLine(msg);
				
				string aux = "";

				// Erotetaan napin nimestä numero
				for (int i = 0; i < btn_name.Length; i++)
				{
					if (Char.IsDigit(btn_name[i]))
						aux += btn_name[i];
				}

				int num = Convert.ToInt16(aux);
			
				Globals.Robotit.LisaaLokiin(robotti, string.Format("Kuittaus {0} - {1}", num, additionalno));
				Globals.Robotit.KuittaaHairio(robotti, num, additionalno);
			}
			catch (Exception x)
			{
				Globals.Tags.Log(string.Format("Virtoip Exception button: {0}. Exception: {1}", btn_name, x.Message));
			}
		}
		
		void Button_ServicePos_Click(System.Object sender, System.EventArgs e)
		{
			try
			{
				int robotti = Globals.Tags.HMI_RobotNo.Value;
				Globals.Robotit.LisaaLokiin(robotti, "Ajopyyntö huoltoasemaan.");
				Globals.Robotit.AjaHuoltoon(robotti);
				//Globals.Tags.Rob1_ServicePosSent.Value = true;
				Globals.Tags.SetTagValue(string.Format("Rob{0}_ServicePosSent", robotti), true);
			}
			catch (Exception x)
			{
				Globals.Tags.Log(string.Format("Drive to servicepos Exception: {0}", x.Message));
			}
		}
		
		void Template_Robots_Opened(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.HMI_AdditionalRobotErrorNo.ResetTag();
		}
	}
}
