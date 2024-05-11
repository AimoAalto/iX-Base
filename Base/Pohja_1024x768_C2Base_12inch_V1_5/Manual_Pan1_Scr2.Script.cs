//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
	public partial class Manual_Pan1_Scr2
	{
		public Neo.ApplicationFramework.Generated.Kasiajot kasiajot = new Neo.ApplicationFramework.Generated.Kasiajot();
		
		/// <summary>
		/// Poistaa kaikki valinnat
		/// </summary>
		void ManualResetButtons()
		{
			kasiajot.ManualResetButtons();
		}

		void Manual_Pan1_Scr2_Opened(System.Object sender, System.EventArgs e)
		{
			// Initissä viedään parametrit aputoiminnoille
			kasiajot.Init(this, "Man", Globals.Tags.S7HMI_DB_ToPLC_ManualCtrl_2);

			// Luo elementeille napit
			kasiajot.LuoClickHandlerit();
		}

		void Manual_Pan1_Scr2_Closed(System.Object sender, System.EventArgs e)
		{
			// Poista napit
			kasiajot.RemoveClickHandlers();

			// Poista manuaalitilan valinta
			Globals.Tags.S7HMI_DB_ToPLC_AutoAreaCMD_ManModeSelection_2.ResetTag();
		}
	}
}
