namespace Neo.ApplicationFramework.Generated
{
	using Neo.ApplicationFramework.Common.Designer;
	using Neo.ApplicationFramework.Interfaces;
	using Neo.ApplicationFramework.Interfaces.Tag;
	using Neo.ApplicationFramework.Tools.OpcClient;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using System.Windows.Forms;
    
    
	/// <summary>
	/// Päivittää päänäytölle sijoitettavat ajotiedot taustalla määritetyin intervallein. 
	/// Sisältää hakufunktiot tuloradalla ja lavapaikalla ajettaville tuotteille.
	/// Ilmoittaa käyttäjälle onnistuneesta aloituksesta.
	/// </summary>
	/// <remarks>Viimeksi päivitetty: SoPi 20.3.2018</remarks>
    public partial class Ajotiedot
    {
		System.Threading.Timer HeaderPaivitys;
		
		/// <summary>
		/// Ajastaa ajotietojen päivityksen päänäytölle taustalla asetustiedostossa
		/// määritetyin aikavälein.
		/// Aloittaa Aloitettu-tagien seurannan.
		/// </summary>
		/// <param name="sender">this</param>
		void Ajotiedot_Created(System.Object sender, System.EventArgs e)
		{
			HeaderPaivitys = new System.Threading.Timer((args) => {
				// Mitataan kauanko operaatioissa kestää
				Stopwatch takeTime = new Stopwatch();
				takeTime.Start();
				
				AjossaOlevatTuotteet();
				
				// Suoritetaan määritetyin välein (default 10s)
				takeTime.Stop();
				HeaderPaivitys.Change(Math.Max(0, _Konfiguraatio.Aikavalit["Ajotiedot"] - takeTime.ElapsedMilliseconds), Timeout.Infinite);
				}, null, 0, Timeout.Infinite);
			
			// Seurataan ja ilmoitetaan onnistuneesta aloituksesta
			foreach (Dictionary<int,int> robotti in _Konfiguraatio.RobotinTuloradat.Values)
			{
				foreach (int tulorata in robotti.Keys)
				{
					Globals.Tags.GetTag("Line1_PLC_Aloitettu" + tulorata).ValueChange += Line1_PLC_AloitettuX_ValueChange;
				}
			}
			
			// Seurataan ja tehdään toimintoja lopetuksessa
			foreach (Dictionary<int,int> robotti in _Konfiguraatio.RobotinTuloradat.Values)
			{
				foreach (int tulorata in robotti.Keys)
				{
					Globals.Tags.GetTag("Line1_PLC_Lopetettu" + tulorata).ValueChange += Line1_PLC_LopetettuX_ValueChange;
				}
			}
		}

		/// <summary>
		/// Päivittää päänäytölle (HMI_Overview_prod_details) tuloratojen statuksen. 
		/// Jos tulorata on aloitettu, näytetään myös tuloradalla ajettava resepti 
		/// ja aloitetut lavapaikat.
		/// HUOM! Funktiosta löytyy käsittely usealle lavapaikalle ja vain yhdelle
		/// lavapaikalle.
		/// </summary>
		void AjossaOlevatTuotteet()
		{
			// Alustetaan ajossa olevat tuotteet
			string ajossa = string.Empty;

			// Kerätään lavapaikkojen statustagit talteen
			List<int> statukset = new List<int>();
						
			// Käydään kaikkien sovelluksen tuloratojen tilanne läpi
			foreach (Dictionary<int, int> robottinRadat in _Konfiguraatio.RobotinTuloradat.Values)
			{
				foreach (int tulorata in robottinRadat.Keys)
				{
					// Haetaan tuloradan tila
					if(Globals.Tags.GetTagValue("Line1_PLC_Aloitettu" + tulorata).Bool)
					{
						// Haetaan tuloradalla ajossa oleva tuote ja lisätään listaan
						string nimi = HaeTuloradanTuote(tulorata);
						
						// Haetaan termit TextLibrarysta valmiiksi käännettynä
						ajossa += Globals.TextLibrary.Terms.Messages[0].Message // Ryhmittely
							+ " " + tulorata + " - " + nimi;
					
						ajossa += ", " 
							+ Globals.TextLibrary.Terms.Messages[1].Message // LP
							+ " ";
					
//						// Lavapaikan numeron haku
//						foreach (Dictionary<int, int> lavapaikat in _Konfiguraatio.RobotinLavapaikat.Values)
//						{
//							foreach (KeyValuePair<int, int> lavapaikka in lavapaikat)
//							{
//								if (lavapaikka.Value == Globals.Tags.GetTagValue("Line1_PLC_Lavapaikka_TK" + tulorata))
//								{
//									ajossa += lavapaikka.Key.ToString();
//																		
//									// Merkitään myös lavapaikka aloitetuksi
//									statukset.Add(lavapaikka.Key);
//								}
//							}
//						}
						
						// Monen lavapaikan aloituksen tarkastelu
						var lp = ((GlobalDataItem)Globals.Tags.GetTag("Line1_PLC_PalletPlaces" + tulorata)).Values;
					
						bool loytyi = false;
						for (int i = 0; i < lp.Length; i++)
						{
							if (lp[i])
							{
								if (loytyi)
								{
									ajossa += " & ";
								}
								ajossa += i.ToString();
								loytyi = true;
								
								// Merkitään myös lavapaikka aloitetuksi
								statukset.Add(i);
							}
						}
					}
					else
					{
						// Tulorata on lopetettu
						// Haetaan termit TextLibrarysta valmiiksi käännettynä
						ajossa += Globals.TextLibrary.Terms.Messages[0].Message // Ryhmittely
							+ " " + tulorata + " "
							+ Globals.TextLibrary.Terms.Messages[2].Message; // lopetettu
					}

					// Lisätään lopuksi rivin vaihto
					ajossa += "\n";
				}
			}
			
			// Päivitetään lavapaikkojen status
			foreach (Dictionary<int, int> lavapaikat in _Konfiguraatio.RobotinLavapaikat.Values)
			{
				foreach (int lavapaikka in lavapaikat.Keys)
				{
					if (statukset.Contains(lavapaikka))
					{
						Globals.Tags.SetTagValue("HMI_InProduction_Pallet" + lavapaikka, true);
					}
					else
					{
						Globals.Tags.SetTagValue("HMI_InProduction_Pallet" + lavapaikka, false);
					}
				}
			}
			
			// Sijoitetaan saatu tulos näytölle
			Globals.Tags.HMI_Overview_prod_details.Value = ajossa;
		}
		
		/// <summary>
		/// Hakee lavapaikalla käsiteltävän tuotteen etsimällä, mille tuloradalle 
		/// lavapaikka on aloitettu ja hakee tuloradalla ajettavan tuotteen numeron ja nimen.
		/// HUOM! Sisältää erilaisen käsittelyn, jos usealle lavapaikalle aloitus mahdollista.
		/// </summary>
		/// <param name="lavapaikka">Lavapaikan numero</param>
		/// <returns>Tuloradalla ajettavan tuotteen nimi. Jos tuotetta ei löydy, palauttaa "Tuntematon"</returns>
		public string HaeLavapaikanTuote(int lavapaikka)
		{		
			// Käydään läpi kaikki tuloradat ja tutkitaan mille tuloradalle lavapaikka on aloitettuna
			// Käydään kaikkien sovelluksen tuloratojen tilanne läpi
			foreach (Dictionary<int, int> robotinRadat in _Konfiguraatio.RobotinTuloradat.Values)
			{
				foreach (int tulorata in robotinRadat.Keys)
				{
					// Haetaan tuloradan tila
					if (Globals.Tags.GetTagValue("Line1_PLC_Aloitettu" + tulorata.ToString()) > 0)
					{
//						// Tarkistetaan ajaako tulorata tälle lavapaikalle
//						if (Globals.Tags.GetTagValue("Line1_PLC_Lavapaikka_TK" + tulorata) == lavapaikka)
//						{
//							// Haetaan tuloradan ajettava tuote logiikasta ja sille nimi tietokannasta
//							return HaeTuloradanTuote(tulorata);								
//						}
						
						// Monen aloitettavan lavapaikan hakufunktio
						var lp = ((GlobalDataItem)Globals.Tags.GetTag("Line1_PLC_PalletPlaces" + tulorata)).Values;
						
						if (lp[lavapaikka])
						{
							// Haetaan tuloradan ajettava tuote logiikasta ja sille nimi tietokannasta
							return HaeTuloradanTuote(tulorata);								
						}
					}
				}
			}
			
			return " ";
		}
		
		/// <summary>
		/// Hakee tuloradalla ajettavan tuotteen nimen perustuen logiikkaan tallennettuun 
		/// tietokannan rivinumeroon.
		/// </summary>
		/// <param name="tulorata">Tuloradan numero</param>
		/// <returns>Tuloradalla ajettavan tuotteen numero ja nimi. Jos tuotetta ei löydy, palauttaa "Tuntematon"</returns>
		public string HaeTuloradanTuote(int tulorata)
		{
			// Tulorata on ajossa - Haetaan ajettava tuote logiikasta
			int tuote = (int)Globals.Tags.GetTagValue("Line1_Rivinumero_TK" + tulorata.ToString());
               
			Globals.Tags.Log("Kutsutaan Tuotenumerolla " + tuote);
			System.Diagnostics.Trace.WriteLine("[iX] Get: Line1_Rivinumero_TK " + tuote);
			
			// Haetaan ajossa olevan tuotteen nimi tietokannasta
			return  Globals.Tuotetiedot.ReseptinTuotenumero(tuote) + " - " + Globals.Tuotetiedot.ReseptinNimi(tuote);
		}

		/// <summary>
		/// Näyttää popup-ikkunan onnistuneesta aloituksesta.
		/// Mahdollista suorittaa muita toimintoje, kun aloitus onnistuu.
		/// </summary>
		/// <param name="sender">Line1_PLC_AloitettuX</param>
		void Line1_PLC_AloitettuX_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			DesignerItemBase lahettaja_nimi = (DesignerItemBase)sender;
			IBasicTag lahettaja_arvo = (IBasicTag)sender;
			//			Jos tarvitaan tuloradan numeroa aloitustoimintoihin
			//			int tulorata = 0;
			//			if(!int.TryParse(lahettaja_nimi.FullName.Substring(lahettaja_nimi.FullName.Length - 1, 1), out tulorata))
			//			{
			//				// lavapaikkanumeron parsinta epäonnistui
			//				throw new ArgumentException("Tuloradan numeroa ei voitu parsia lähettäjästä: " + lahettaja_nimi.FullName.ToString());
			//			}
			
			//if (Globals.Tags.AppStart_Timer >= 20 && (VariantValue)lahettaja_arvo.Value == 1)
			int arvo = (VariantValue)lahettaja_arvo.Value;
			System.Diagnostics.Trace.WriteLine("[iX] Event: Line1_PLC_AloitettuX_ValueChange " + arvo);
 			if (Globals.Tags.AppStart_Timer >= 20 && arvo == 1)
			{				
				// Nollataan trippimittari?
				
				// Resetoidaan aloitus kesken
				Globals.Tags.HMI_Aloitus_kesken.Value = false;

				// Näytetään popup
				Globals.Tags.HMI_Success_TextValue.SetAnalog(2);
				Globals.Popup_Success.Show();
				
				// Suljetaan aloitusikkuna, kun popup-suljetaan
				Globals.Popup_Success.Closed += Popup_Success_Closed;
			}
		}

		/// <summary>
		/// Suorittaa toimintoja, kun tuotanto on lopetettu.
		/// </summary>
		/// <param name="sender">Line1_PLC_LopetettuX</param>
		void Line1_PLC_LopetettuX_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			DesignerItemBase lahettaja_nimi = (DesignerItemBase)sender;
			IBasicTag lahettaja_arvo = (IBasicTag)sender;
			//			Jos tarvitaan tuloradan numeroa aloitustoimintoihin
			//			int tulorata = 0;
			//			if(!int.TryParse(lahettaja_nimi.FullName.Substring(lahettaja_nimi.FullName.Length - 1, 1), out tulorata))
			//			{
			//				// lavapaikkanumeron parsinta epäonnistui
			//				throw new ArgumentException("Tuloradan numeroa ei voitu parsia lähettäjästä: " + lahettaja_nimi.FullName.ToString());
			//			}
			
			int arvo = (VariantValue)lahettaja_arvo.Value;
			System.Diagnostics.Trace.WriteLine("[iX] Event: " + lahettaja_nimi.Name + " Line1_PLC_LopetettuX_ValueChange " + arvo);
			if (Globals.Tags.AppStart_Timer >= 20 && arvo == 1)
			{								
				// Toiminta, kun lopetettu
				Globals.Tags.Stop_Production_CloseMe.SetAnalog(Globals.Tags.HMI_Overview_track_selected.Value);
				
				int i = lahettaja_nimi.Name.Length-1;
				if (i > 0)
				{
					string ratano = lahettaja_nimi.Name.Substring(i, 1);
					System.Diagnostics.Trace.WriteLine("[iX] Clear Line1_PLC_Aloitus " + ratano);
					((GlobalDataItem)Globals.Tags.GetTag("Line1_PLC_Aloitus" + ratano)).ResetTag();
				}
			}
		}	

		/// <summary>
		/// Sulkee aloitusikkunan, kun ilmoitus aloituksesta suljetaan.
		/// </summary>
		/// <param name="sender">Popup_Success</param>
		void Popup_Success_Closed(System.Object sender, System.EventArgs e)
		{
			// Suljetaan aloitusikkuna, jos on vielä auki
			Globals.Popup_StartProduction.Close();
			
			Globals.Popup_Success.Closed -= Popup_Success_Closed;
		}
    }
}
