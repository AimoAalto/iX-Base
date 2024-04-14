//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
	using System;


	public partial class Settings_pan1_Log
	{
		void Settings_pan1_Log_Opened(System.Object sender, System.EventArgs e)
		{
			LataaLoki();
		}

		/// <summary>
		/// Päivittää robottin virhelokin näytölle. Siirtää näkymän ja valinnan
		/// uusimpiin tapahtumiin.
		/// </summary>
		void LataaLoki()
		{
			// Tyhjennetään lokit			
			ListBoxErrors.Items.Clear();

			// Lisätään kaikki rivit
			foreach (string a in Globals.Tags.__Log)
			{
				ListBoxErrors.Items.Add(a);
			}

			// Scrollataan listaa mukana (ei toimi WinCE:ssä)
			/*Virhelista.SelectedIndex = Virhelista.Items.Count - 1;
			Virhelista.AdaptedObject.CastTo<Neo.ApplicationFramework.Controls.WindowsControls.ListBox>()
			.ScrollIntoView(Virhelista.SelectedItem);
			*/
		}

		void BtnUpdate_Click(System.Object sender, System.EventArgs e)
		{
			LataaLoki();
		}
	}
}
