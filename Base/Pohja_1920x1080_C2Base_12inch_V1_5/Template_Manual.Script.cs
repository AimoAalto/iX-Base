//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
	using System;


	public partial class Template_Manual
	{
		void Template_Manual_Opened(System.Object sender, System.EventArgs e)
		{
		}

		void btnSubmenu_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.BtnHandler(
				Globals.Tags.HMI_Settings_PanelNumber.Value,
				Neo.ApplicationFramework.Generated.Tags.Screens.Manual,
				((Neo.ApplicationFramework.Controls.Script.ButtonAdapter)sender).Name,
				((Neo.ApplicationFramework.Controls.Script.ButtonAdapter)sender).Text.Length);
		}
		
		void Btn_Left_Click(System.Object sender, System.EventArgs e)
		{
			if (Globals._Konfiguraatio.CurrentConfig.DefaultManualScreen == 0)
			{
				Globals.Tags.Menu_SubMenu_Group_Visibility.SetAnalog(1);
				Globals.Tags.Menu_SubMenu_Btn_Anim.SetAnalog(1);
				Globals.Tags.SystemTagNewScreenId.SetAnalog(10401);
			}
			else if (Globals._Konfiguraatio.CurrentConfig.DefaultManualScreen >= 1
				&& Globals._Konfiguraatio.CurrentConfig.DefaultManualScreen < 8)
			{
				Globals.Tags.Menu_SubMenu_Group_Visibility.SetAnalog(1);
				Globals.Tags.Menu_SubMenu_Btn_Anim.SetAnalog(Globals._Konfiguraatio.CurrentConfig.DefaultManualScreen);
				Globals.Tags.SystemTagNewScreenId.SetAnalog(10400 + Globals._Konfiguraatio.CurrentConfig.DefaultManualScreen);
			}
		}
		
		void Btn_Right_Click(System.Object sender, System.EventArgs e)
		{
			if (Globals._Konfiguraatio.CurrentConfig.DefaultManualScreenGroup2 == 0)
			{
				Globals.Tags.Menu_SubMenu_Group_Visibility.SetAnalog(2);
				Globals.Tags.Menu_SubMenu_Btn_Anim.SetAnalog(8);
				Globals.Tags.SystemTagNewScreenId.SetAnalog(10408);
			}
			else if (Globals._Konfiguraatio.CurrentConfig.DefaultManualScreenGroup2 >= 8)
			{
				Globals.Tags.Menu_SubMenu_Group_Visibility.SetAnalog(2);
				Globals.Tags.Menu_SubMenu_Btn_Anim.SetAnalog(Globals._Konfiguraatio.CurrentConfig.DefaultManualScreenGroup2);
				Globals.Tags.SystemTagNewScreenId.SetAnalog(10400 + Globals._Konfiguraatio.CurrentConfig.DefaultManualScreenGroup2);
			}
		}
	}
}
