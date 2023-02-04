namespace Neo.ApplicationFramework.Generated
{
	using System;
    
    
	/// <summary>
	/// Toimii pohjana kaikille Robotti-pääkategorian sivuille
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 6.7.2017</remarks>
	public partial class Template_Robots
	{
		void Diagnostics_Robot_Template_Opened(System.Object sender, System.EventArgs e)
		{
		}
		
		void btnSubmenu_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.BtnHandler(
				Globals.Tags.Settings_PanelNumber.Value, 
				Neo.ApplicationFramework.Generated.Tags.Screens.Robots,
				((Neo.ApplicationFramework.Controls.Script.ButtonAdapter)sender).Name, 
				((Neo.ApplicationFramework.Controls.Script.ButtonAdapter)sender).Text.Length);
		}
	}
}
