namespace Neo.ApplicationFramework.Generated
{
	using System.Threading;
	
    
	/// <summary>
	/// Kysyy Kyllä/Ei/Peruuta -kysymyksen ja antaa mahdollisuuden jäädä odottamaan vastausta
	/// yesNoKysely-ManualResetEventin avulla.
	/// Vastaa MessageBox:a, jossa on Yes/No/peruuta napit, mutta kykenee käyttämään iX:n 
	/// usean kielen ominaisuutta.
	/// Tesktidialogi sisältää projektikohtaisia tekstejä.
	/// </summary>
	/// <example>Käyttö:
	/// Katso Popup_Confimationin esimerkki
	///</example>
	/// <remarks>Viimeksi muokattu: SoPi 25.7.2017</remarks>
    public partial class Popup_Yes_No
    {
		/// <summary>
		/// Tapahtuma, joka aktivoidaan, kun ikkuna suljetaan.
		/// Käytetään kyselyä kutsuvissa funktioissa lukkona, jonka avulla 
		/// vastausta jäädään odottamaan.
		/// </summary>
		public ManualResetEvent yesNoKysely = new ManualResetEvent(false);
		
		void Popup_Yes_No_Opened(System.Object sender, System.EventArgs e)
		{

		}
		
		/// <summary>
		/// Kertoo odottavalle langalle, että vastaus on saatu ja toimintaa voi jatkaa.
		/// </summary>
		/// <param name="sender">this</param>
		void Popup_Confirmation_Closed(System.Object sender, System.EventArgs e)
		{
			yesNoKysely.Set();
		}
		

    }
}
