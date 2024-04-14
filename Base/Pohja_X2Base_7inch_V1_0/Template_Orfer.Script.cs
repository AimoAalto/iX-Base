//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
	using System;

	public partial class Template_Orfer
	{
		private object lockme = new object();
		
		void Button_Menu_Click(System.Object sender, System.EventArgs e)
		{
			lock (lockme)
			{
				if (Globals.Tags.ScreenChangePending.Value.Bool == true)
				{
					Globals.Tags.Log("Screen change pending");
					return;
				}
				
				string btn_name = ((Neo.ApplicationFramework.Controls.Script.ButtonCFAdapter)sender).Name;

				Globals.Tags.Log(string.Format("ShowMainScreen button: {0}", btn_name));

				try
				{
					string aux = "";

					// Erotetaan napin nimestä numero
					for (int i = 0; i < btn_name.Length; i++)
					{
						if (Char.IsDigit(btn_name[i]))
							aux += btn_name[i];
					}

					int num = Convert.ToInt16(aux);

					// asetussivu on yhteinen
					int screenid;
					if (num == 7) screenid = (int)10000;
					else screenid = Globals.Tags.HMI_Settings_PanelNumber.Value * 10000;
					screenid += ((int)num * 100);
					screenid += 1;

					Globals.Tags.SystemTagNewScreenId.SetAnalog(screenid);

					Globals.Tags.Menu_MainMenu_Btn_Anim.SetAnalog(num);
					Globals.Tags.Menu_SubMenu_Group_Visibility.SetAnalog(1);
					Globals.Tags.Menu_SubMenu_Btn_Anim.SetAnalog(1);
					Globals.Tags.ScreenChangePending.SetTag();
				}
				catch (Exception x)
				{
					Globals.Tags.ScreenChangePending.ResetTag();
					Globals.Tags.Log(string.Format("ShowScreen button: {0}. Exception: {1}", btn_name, x.Message));
				}
			}
		}
		
		void Template_Orfer_Opened(System.Object sender, System.EventArgs e)
		{
			if (Globals.Tags.TraceAll) Globals.Tags.Log(string.Format("Opened screen: {0}", Globals.Tags.SystemTagNewScreenId.Value));
			Globals.Tags.ScreenChangePending.ResetTag();
		}
	}
}
