namespace Neo.ApplicationFramework.Generated
{
	using Neo.ApplicationFramework.Common.Designer;
	using Neo.ApplicationFramework.Interfaces;
	using Neo.ApplicationFramework.Interfaces.Tag;
	using Neo.ApplicationFramework.Tools.OpcClient;
    using System;
	using System.Collections.Generic;
	using System.Linq;
    
	
	/// <summary>
	/// Sisältää lavalokin tallentamisen ja siihen vaadittavat seurannat.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 26.7.2017</remarks>
    public partial class Lavaloki
    {	
		/// <summary>
		/// Liittyy kaikkiin tarvittaviin lavan valmistumisbittien ja robotin lavapaikan
		/// vientimäärien tagien ValueChange-eventteihin.
		/// </summary>
		/// <param name="sender">this</param>
		void Lavaloki_Created(System.Object sender, System.EventArgs e)
		{	
			try 
			{	        		
				foreach (Dictionary<int, int> robotti in _Konfiguraatio.RobotinLavapaikat.Values)
				{
					foreach (int lavapaikka in robotti.Keys)
					{
						Globals.Tags.GetTag("Line1_PLC_LavaValmis" + lavapaikka).ValueChange += Lava_Valmis;
					}
				}
				
				foreach (KeyValuePair<int, Dictionary<int, int>> robotti in _Konfiguraatio.RobotinLavapaikat)
				{
					foreach (int robotinLavapaikka in robotti.Value.Values)
					{
						Globals.Tags.GetTag("Rob" + robotti.Key + "_lavap" + robotinLavapaikka + "_plas").ValueChange += Lava_Aloitettu;
					}
				}
			}
			catch (Exception)
			{
				// Epäonnistuneen tagin hausta on jo näytetty virhe
			}	
		}

		/// <summary>
		/// Seuraa robotin nykyisen lavan vientimäärää ja merkitsee lavan aloitetuksi,
		/// kun lavalle asetetaan ensimmäiset kappaleet.
		/// </summary>
		/// <param name="sender">RobX_lavapX_plas</param>
		/// <exception cref="ArgumentException">Robotin tai robotin käyttämää lavapaikan numeroa ei voitu parsia sender:stä</exception>
		/// <exception cref="ConfigurationFaultException">Lavapaikkaa ei löytynyt robotin lavapaikoista _Konfiguraatiossa</exception>
		void Lava_Aloitettu(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			DesignerItemBase lahettaja_nimi = (DesignerItemBase)sender;
			IBasicTag lahettaja_arvo = (IBasicTag)sender;
			
			// Lava on aloitettu, kun ensimmäinen vienti on tehty
			if ((VariantValue)lahettaja_arvo.Value == 1)
			{
				int robotti = 0;
				int robottiLavapaikka = 0;
				if(!int.TryParse(lahettaja_nimi.Name.Substring(lahettaja_nimi.Name.IndexOf("_") - 1, 1), out robotti))
				{
					// Lavapaikan numeron parsinta epäonnistui
					throw new ArgumentException("Robotin numeroa ei voitu parsia lähettäjästä: " + lahettaja_nimi.Name);
				}
				if(!int.TryParse(lahettaja_nimi.Name.Substring(lahettaja_nimi.Name.IndexOf("_", 6) - 1, 1), out robottiLavapaikka))
				{
					// Robotin numeron parsinta epäonnistui
					throw new ArgumentException("Robotin lavapaikan numeroa ei voitu parsia lähettäjästä: " + lahettaja_nimi.Name);
				}
			
				int lavapaikka = -1;
				try
				{
					// Haetaan lavapaikan oma numero
					lavapaikka = _Konfiguraatio.RobotinLavapaikat[robotti].First(p => p.Value == robottiLavapaikka).Key;
				}
				catch (NullReferenceException ex)
				{
					// Virheellinen konfiguraatio
					throw new ConfigurationFaultException("Lavapaikan tietoja ei löytynyt robotilta.", lahettaja_nimi.Name, ex);
				}
				
				// Merkitään aloitusaika
				Globals.Tags.SetTagValue("Line1_Pallet_started" + lavapaikka, Globals.Tags.SystemTagDateTime.Value);
			}
		}
		
		/// <summary>
		/// Pyytää lavalokin päivityksen, kun lava valmistuu.
		/// </summary>
		/// <param name="sender">Line1_PLC_LavaValmisX</param>
		/// <exception cref="ArgumentException">Lavapaikan numeroa ei voitu parsia sender:n nimen lopusta.</exception>
		void Lava_Valmis(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			DesignerItemBase lahettaja_nimi = (DesignerItemBase)sender;
			IBasicTag lahettaja_arvo = (IBasicTag)sender;
			
			// Reagoidaan vain nousevaan reunaan
			if (((VariantValue)lahettaja_arvo.Value).Bool)
			{
				int lavapaikka = -1;
				
				if(!int.TryParse(lahettaja_nimi.Name.Substring(lahettaja_nimi.Name.Length - 1, 1), out lavapaikka))
				{
					// lavapaikkanumeron parsinta epäonnistui
					throw new ArgumentException("Lavapaikan numeroa ei voitu parsia lähettäjästä: " + lahettaja_nimi.Name);
				}
			
				// Lisätään merkintä lavalokiin
				foreach (Dictionary<int, int> lavapaikat in _Konfiguraatio.RobotinLavapaikat.Values)
				{
					if (lavapaikat.Keys.Contains(lavapaikka))
					{
						Paivita_Lavaloki(lavapaikka);
						return;
					}
				}
			}
		}
		
		/// <summary>
		/// Päivittää lavalokiin merkinnän valmistuneesta lavasta. Tallentaa 
		/// Lavan aloituksen ja valmistumisen aikaleiman, lavan lavapaikan, lavan tuotteen
		/// ja lavan laatikkomäärän.
		/// </summary>
		/// <param name="lavapaikka">Lavapaikan numero</param>
		void Paivita_Lavaloki(int roboLavapaikka)
		{		
			// Miltä lavapaikalta lava tulee
			int tulorata = -1;
			int robotti = 0;
			int lavapaikka = -1;

			// Haetaan tulorata, jolle lavapaikka on aloitettu
			foreach (KeyValuePair<int, Dictionary<int,int>> tuloradat in _Konfiguraatio.RobotinTuloradat)
			{
				foreach (int tulorataEhdokas in tuloradat.Value.Keys)
				{
					//Tarkistetaan vain aloitetut tuloradat
					if (Globals.Tags.GetTagValue("Line1_PLC_Aloitettu" + tulorataEhdokas).Bool)
					{
						if (Globals.Tags.GetTagValue("Line1_PLC_Lavapaikka_TK" + tulorataEhdokas) == roboLavapaikka)
						{
							// Tulorata löytyi, kerätään tiedot
							tulorata = tulorataEhdokas;
							robotti = tuloradat.Key;						
							foreach (KeyValuePair<int, int> paikkapari in _Konfiguraatio.RobotinLavapaikat[robotti])
							{
								if (paikkapari.Value == roboLavapaikka)
								{
									lavapaikka = paikkapari.Key;
								}
							}
						}
					}
				}
			}
			
			if (tulorata == -1 || robotti == 0 || lavapaikka ==-1)
			{
				// Lavapaikkatieto olikin ehtinyt nollautua. Ei lavalokimerkintää
				return;
			}
			
			// Lavan aloitushetki
			Globals.Tags.HMI_PalletLog_Started.Value = Globals.Tags.GetTagValue("Line1_Pallet_started" + lavapaikka);
			
			// Lavan valmistumishetki
			Globals.Tags.HMI_PalletLog_Ended.Value = Globals.Tags.SystemTagDateTime.Value;

			// Lavapaikka, jolta lava valmistui
			Globals.Tags.HMI_PalletLog_PalletPlace.Value = lavapaikka;

			// Tuote, jota lavalla oli
			Globals.Tags.HMI_PalletLog_ProductNo.Value = Globals.Tuotetiedot.ReseptinTuotenumero(Globals.Tags.GetTagValue("Line1_Rivinumero_TK" + tulorata).Int);
						
			// Lavan Laatikkomäärä
			Globals.Tags.HMI_PalletLog_pcs.Value = Globals.Tags.GetTagValue("Rob" + robotti + "_lavap" + roboLavapaikka + "_plaslviim").Int;
			
			// Kasvatetaan lokin monitoritagin arvoa -> lokiin lisätään arvo
			Globals.Tags.HMI_LogData_Pallet_Log.Value++;
			
			// Merkintä robotin lokiin
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Lava valmis paikalla " + roboLavapaikka);
		}		
    }
}
