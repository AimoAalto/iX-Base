namespace Neo.ApplicationFramework.Generated
{
	using System;


	/// <summary>
	/// Toimii pohjana kaikille hälytys-pääkategorian sivuille
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 6.7.2017</remarks>
	public partial class Template_Alarms
	{
		void Alarms_Template_Opened(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.HMI_MainMenu_BtnAnim.SetAnalog(6);
		}

		void btnSubmenu_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.BtnHandler(
				Globals.Tags.Settings_PanelNumber.Value,
				Neo.ApplicationFramework.Generated.Tags.Screens.Alarms,
				((Neo.ApplicationFramework.Controls.Script.ButtonAdapter)sender).Name,
				((Neo.ApplicationFramework.Controls.Script.ButtonAdapter)sender).Text.Length);
		}
	}
}
