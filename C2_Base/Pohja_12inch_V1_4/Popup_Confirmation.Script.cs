namespace Neo.ApplicationFramework.Generated
{
	using System.Threading;
	
    
	/// <summary>
	/// Kysyy OK/Peruuta -kysymyksen ja antaa mahdollisuuden jäädä odottamaan vastausta
	/// konfirmaatioVahvistus-ManualResetEventin avulla.
	/// Vastaa MessageBox:a, jossa on OK/Peruuta napit, mutta kykenee käyttämään iX:n 
	/// usean kielen ominaisuutta.
	/// Kysymysdialogi sisältää projektikohtaisia tekstejä.
	/// </summary>
	/// <example>Käyttö:
	/// <code>
	/// // Esimerkki AP17023 lavalapun poisto lavalappujonosta
	/// // Resetoidaan vahvistuksen kyselyn odotusvahti
	///	((Popup_Confirmation)Globals.Popup_Confirmation.AdaptedObject).konfirmaatioVahvistus.Reset();
	///
	///	// Kysytään vahvistusta
	///	Globals.Tags.HMI_Confirmation_Text.SetAnalog(3); // Teksti, joka löytyy Multiple Languagesista
	///	Globals.Tags.HMI_Confirmation_Value.Value = (string)Pallet_sheet_queue.SelectedItem; // Teksti, jota ei löydy Multiple Languagesista
	///	Globals.Popup_Confirmation.Show();
	///				
	///	// Poistetaan tai ei poisteta, kun vastaus saadaan
	///	Thread toiminta = new Thread(() => 
	///		{
	///			// Odotetaan vastausta
	///			((Popup_Confirmation)Globals.Popup_Confirmation.AdaptedObject).konfirmaatioVahvistus.WaitOne();
	///							
	///			// Luetaan vastaus
	///			if (Globals.Tags.HMI_Confirmation_OK.Value.Bool)
	///			{
	///				// Tehdään mitä halutaan
	///				// Poistetaan lappu jonosta
	///				// Vain ObservableCollectionin omistajalanka voi lukea sitä -> käytetään Dispatcheria
	///				Dispatcher.Invoke(new Action(() =>
	///					Globals.Lavalappu.lavalappuJono.Remove((string)Pallet_sheet_queue.SelectedItem)));
	///			}
	///		});
	///				
	///	// Käynnistetään lanka odottamaan vastausta
	///	toiminta.Start();
	/// </code>
	///</example>
	/// <remarks>Viimeksi muokattu: SoPi 27.6.2017</remarks>
    public partial class Popup_Confirmation
    {
		/// <summary>
		/// Tapahtuma, joka aktivoidaan, kun ikkuna suljetaan.
		/// Käytetään kyselyä kutsuvissa funktioissa lukkona, jonka avulla 
		/// vastausta jäädään odottamaan.
		/// </summary>
		public ManualResetEvent konfirmaatioVahvistus = new ManualResetEvent(false);
		
		/// <summary>
		/// Kertoo odottavalle langalle, että vastaus on saatu ja toimintaa voi jatkaa.
		/// </summary>
		/// <param name="sender">this</param>
		void Popup_Confirmation_Closed(System.Object sender, System.EventArgs e)
		{
			konfirmaatioVahvistus.Set();
		}
		
		void Popup_Confirmation_Opened(System.Object sender, System.EventArgs e)
		{

		}
		
    }
}
