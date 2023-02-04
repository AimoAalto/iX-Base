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
    
    
    public partial class Settings_LogInternal
    {
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
			/*Virhelista.SelectedIndex = Virhelista.Items.Count - 1;
			Virhelista.AdaptedObject.CastTo<Neo.ApplicationFramework.Controls.WindowsControls.ListBox>()
			.ScrollIntoView(Virhelista.SelectedItem);
			*/
		}
		
		/// <summary>
		/// Lataa ensimmäisen robotin lokin ja liittyy seuraamaan sen muutoksia.
		/// </summary>
		/// <param name="sender">this</param>
		void Settings_LogInternal_Opened(System.Object sender, System.EventArgs e)
		{
			// Loki
			LataaLoki();
		}
		
		/// <summary>
		/// Irroittautuu lokin seurannasta sivun sulkeutuessa.
		/// </summary>
		/// <param name="sender">this</param>
		void Settings_Robot_Closing(System.Object sender, System.ComponentModel.CancelEventArgs e)
		{
		}
				
		/// <summary>
		/// Pysäyttää lokinäytön päivityksen.
		/// </summary>
		/// <param name="sender">this.Stop_update_btn</param>
		void Stop_update_btn_Click(System.Object sender, System.EventArgs e)
		{
			LataaLoki();
		}
    }
}
