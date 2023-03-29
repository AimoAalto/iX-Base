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


	public partial class Overview_Pan1_Scr1
	{

		void Overview_Pan1_Scr1_Opened(System.Object sender, System.EventArgs e)
		{
			// Main menu painikkeiden visualisointi
			Globals.Tags.Menu_MainMenu_Btn_Anim.SetAnalog(0);

			// Alustetaan tuloradan valinta -1:ksi
			Globals.Tags.HMI_Overview_track_selected.Value = -1;

			// Seurataan tuloradan valintaa
			Globals.Tags.HMI_Overview_track_selected.ValueChange += HMI_Overview_track_selected_ValueChange;
		}

		/// <summary>
		/// HMI:n näytöltä on valittu lavapaikka tai tagi on nollattu skriptissä
		/// </summary>
		/// <param name="sender">HMI_Overview_track_selected</param>
		void HMI_Overview_track_selected_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			try
			{
				IScriptTag lahettaja = (IScriptTag)sender;

				// Katsotaan onko paikka valittu vai tagi nollattu
				if (lahettaja.Value > -1)
				{
					// Haetaan lavapaikan tila logiikasta ja valitaan sopiva ikkuna
					//if (Globals.Tags.GetTagValue("Line1_PLC_Aloitettu" + lahettaja.Value) == 0)
					int arvo = Globals.Tags.GetTagValue("Line1_PLC_Aloitettu" + lahettaja.Value);
					if (arvo == 0)
					{
						// Linja ei ole aloitettu, näytetään aloita-ikkuna
						Globals.Popup_StartProduction.Show();
					}
					else
					{
						// Linja on aloitettu, näytetään lopeta-ikkuna
						Globals.Popup_Stop_Production.Tulorata1.Show();
					}
				}
				else
				{
					// Tagi on nollattu skriptissä, älä tee mitään
				}
			}
			catch (Exception x)
			{
				Globals.Tags.Log(x.Message);
			}
		}

		void Overview_Pan1_Scr1_Closing(System.Object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Unsubscribe
			Globals.Tags.HMI_Overview_track_selected.ValueChange -= HMI_Overview_track_selected_ValueChange;
		}
	}
}
