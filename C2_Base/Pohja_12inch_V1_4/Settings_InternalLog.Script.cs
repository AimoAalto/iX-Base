//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
    using System;
    
    
    public partial class Settings_InternalLog
    {
		
		void Settings_InternalLog_Opened(System.Object sender, System.EventArgs e)
		{
			// Loki
			LataaLoki();
		}
		
		/// <summary>
		/// Päivittää robottin virhelokin näytölle. Siirtää näkymän ja valinnan
		/// uusimpiin tapahtumiin.
		/// </summary>
		void LataaLoki()
		{
			// Tyhjennetään lokit			
			Virhelista.Items.Clear();

			// Lisätään kaikki rivit
			foreach (string a in Globals.Tags.__Log)
			{
				Virhelista.Items.Add(a);
			}
			
			// Scrollataan listaa mukana
			Virhelista.SelectedIndex = Virhelista.Items.Count - 1;
			Virhelista.AdaptedObject.CastTo<Neo.ApplicationFramework.Controls.WindowsControls.ListBox>().ScrollIntoView(Virhelista.SelectedItem);
			/**/
		}
		
		void Btn_Update_Click(System.Object sender, System.EventArgs e)
		{
			LataaLoki();
		}
    }
}
