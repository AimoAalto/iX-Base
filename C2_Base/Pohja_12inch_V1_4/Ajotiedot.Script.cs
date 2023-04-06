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


	/// <summary>
	/// Päivittää päänäytölle sijoitettavat ajotiedot taustalla määritetyin intervallein. 
	/// Sisältää hakufunktiot tuloradalla ja lavapaikalla ajettaville tuotteille.
	/// Ilmoittaa käyttäjälle onnistuneesta aloituksesta.
	/// </summary>
	/// <remarks>Viimeksi päivitetty: SoPi 20.3.2018</remarks>
	public partial class Ajotiedot
	{
		object lockme = new object();
		System.Threading.Timer HeaderPaivitys;
		string msg0 = "";
		string msg1 = "";
		string msg2 = "";

		/// <summary>
		/// Ajastaa ajotietojen päivityksen päänäytölle taustalla asetustiedostossa
		/// määritetyin aikavälein.
		/// Aloittaa Aloitettu-tagien seurannan.
		/// </summary>
		/// <param name="sender">this</param>
		void Ajotiedot_Created(System.Object sender, System.EventArgs e)
		{
			if (Globals.Tags.TraceAll) System.Diagnostics.Trace.WriteLine("Ajotiedot Created (start)");

			msg0 = new TextLibrary().Terms.Messages[0].Message;
			msg1 = new TextLibrary().Terms.Messages[1].Message;
			msg2 = new TextLibrary().Terms.Messages[2].Message;

			int interval = 10000;
			try
			{
				interval = Globals._Konfiguraatio.CurrentConfig.Aikavali("Ajotiedot");
			}
			catch (Exception x)
			{
				Globals.Tags.Log(String.Format("Ajotiedot_Create: Interval error, use default\n{1}", x.Message));
			}

			/**/
			HeaderPaivitys = new System.Threading.Timer((args) =>
				{
					if (Globals.Tags.TraceAll) System.Diagnostics.Trace.WriteLine("Ajotiedot HeaderPaivitys");

					// Mitataan kauanko operaatioissa kestää
					Stopwatch takeTime = new Stopwatch();
					takeTime.Start();

					AjossaOlevatTuotteet();

					takeTime.Stop();

					if (Globals.Tags.TraceAll) System.Diagnostics.Trace.WriteLine(string.Format("Ajotiedot time : {0} (ticks)", takeTime.ElapsedTicks));

					// Suoritetaan määritetyin välein (default 10s)
					HeaderPaivitys.Change(Math.Max(0, interval - takeTime.ElapsedMilliseconds), Timeout.Infinite);
				}, null, 5000, Timeout.Infinite);
			/**/

			// Seurataan ja ilmoitetaan onnistuneesta aloituksesta ja lopetuksesta
			foreach (int tulorata in Globals._Konfiguraatio.CurrentConfig.Tuloradat.Keys)
			{
				string name = "";
				try
				{
					if (Globals.Tags.TraceAll)
						System.Diagnostics.Trace.WriteLine(string.Format("Ajotiedot AloitettuX / LopetettuX, tulorata : {0}", tulorata));

					name = "Line1_PLC_Aloitettu" + tulorata;
					IBasicTag tag = Globals.Tags.GetTag(name);
					if (tag == null)
						Globals.Tags.Log(String.Format("Ajotiedot_Create: Unknown Tag {0}", name));
					else
						tag.ValueChange += Line1_PLC_AloitettuX_ValueChange;

					name = "Line1_PLC_Lopetettu" + tulorata;
					tag = Globals.Tags.GetTag(name);
					if (tag == null)
						Globals.Tags.Log(String.Format("Ajotiedot_Create: Unknown Tag {0}", name));
					else
						tag.ValueChange += Line1_PLC_LopetettuX_ValueChange;
				}
				catch (Exception x)
				{
					Globals.Tags.Log(String.Format("Ajotiedot_Create: Unknown error {0} [{1}]", name, x.Message));
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
			lock (lockme)
				try
				{
					// Alustetaan ajossa olevat tuotteet
					string ajossa = string.Empty;

					// Kerätään lavapaikkojen statustagit talteen
					List<int> statukset = new List<int>();

					// Käydään kaikkien sovelluksen tuloratojen tilanne läpi
					foreach (int tulorata in Globals._Konfiguraatio.CurrentConfig.Tuloradat.Keys)
					{
						string name = "Line1_PLC_Aloitettu" + tulorata;
						try
						{
							// Haetaan tuloradan tila
							IBasicTag tag = Globals.Tags.GetTag(name);
							if (tag == null)
								Globals.Tags.Log(String.Format("AjossaolevatTuotteet: Unknown Tag {0}", name));
							else
							{
								// Haetaan tuloradan tila
								if (Globals.Tags.GetTagValue(name).Bool)
								{
									// Haetaan tuloradalla ajossa oleva tuote ja lisätään listaan
									string nimi = HaeTuloradanTuote(tulorata);

									// Haetaan termit TextLibrarysta valmiiksi käännettynä
									ajossa += msg0 // Ryhmittely
									+ " " + tulorata + " - " + nimi;
									ajossa += ", " + msg1 // LP
										+ " ";

									// // Lavapaikan numeron haku
									// foreach (Dictionary<int, int> lavapaikat in _Konfiguraatio.RobotinLavapaikat.Values)
									// {
									// 	foreach (KeyValuePair<int, int> lavapaikka in lavapaikat)
									// 	{
									// 		if (lavapaikka.Value == Globals.Tags.GetTagValue("Line1_PLC_Lavapaikka_TK" + tulorata))
									// 		{
									// 			ajossa += lavapaikka.Key.ToString();
									// 												
									// 			// Merkitään myös lavapaikka aloitetuksi
									// 			statukset.Add(lavapaikka.Key);
									// 		}
									// 	}
									// }

									// Monen lavapaikan aloituksen tarkastelu
									name = "Line1_PLC_PalletPlaces" + tulorata;
									var lp = ((GlobalDataItem)Globals.Tags.GetTag(name)).Values;

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
									ajossa += msg0 + " " + tulorata + " " // Ryhmittely
										+ msg2; // lopetettu
								}
							}

							// Lisätään lopuksi rivin vaihto
							ajossa += "\n";
						}
						catch (Exception x)
						{
							Globals.Tags.Log(String.Format("AjossaolevatTuotteet: {0}", x.Message));
						}
					}

					// Päivitetään lavapaikkojen status
					foreach (int lavapaikka in Globals._Konfiguraatio.CurrentConfig.Lavapaikat.Keys)
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

					// Sijoitetaan saatu tulos näytölle
					Globals.Tags.HMI_Overview_prod_details.Value = ajossa;
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.WriteLine(string.Format("Exception [AjossaOlevatTuotteet] {0}", ex.Message));
				}
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
			foreach (int tulorata in Globals._Konfiguraatio.CurrentConfig.Tuloradat.Keys)
			{
				try
				{
					// Haetaan tuloradan tila
					if (Globals.Tags.GetTagValue("Line1_PLC_Aloitettu" + tulorata.ToString()) > 0)
					{
						// // Tarkistetaan ajaako tulorata tälle lavapaikalle
						// if (Globals.Tags.GetTagValue("Line1_PLC_Lavapaikka_TK" + tulorata) == lavapaikka)
						// {
						// 	// Haetaan tuloradan ajettava tuote logiikasta ja sille nimi tietokannasta
						// 	return HaeTuloradanTuote(tulorata);								
						// }

						// Monen aloitettavan lavapaikan hakufunktio
						var lp = ((GlobalDataItem)Globals.Tags.GetTag("Line1_PLC_PalletPlaces" + tulorata)).Values;

						if (lp[lavapaikka])
						{
							// Haetaan tuloradan ajettava tuote logiikasta ja sille nimi tietokannasta
							return HaeTuloradanTuote(tulorata);
						}
					}
				}
				catch (Exception x)
				{
					Globals.Tags.Log(String.Format("HaeLavapaikantuote: tulorata {0} [{1}]", tulorata, x.Message));
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
			try
			{
				// Tulorata on ajossa - Haetaan ajettava tuote logiikasta
				int tuote = (int)Globals.Tags.GetTagValue("Line1_Rivinumero_TK" + tulorata.ToString());

				//Globals.Tags.Log("Kutsutaan Tuotenumerolla " + tuote);
				//System.Diagnostics.Trace.WriteLine("[iX] Get: Line1_Rivinumero_TK " + tuote);
				Globals.Tags.Log("HaeTuloradanTuote tulorata: " + tulorata.ToString() + " Kutsutaan Tuotenumerolla " + tuote);

				// Haetaan ajossa olevan tuotteen nimi tietokannasta
				return Globals.Tuotetiedot.ReseptinTuotenumero(tuote) + " - " + Globals.Tuotetiedot.ReseptinNimi(tuote);
			}
			catch (Exception x)
			{
				Globals.Tags.Log(String.Format("HaeTuloradanTuote: tulorata {0} [{1}]", tulorata, x.Message));
				return "n/a";
			}
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

			//if (Globals.Tags.AppStart_Timer >= 20 && (VariantValue)lahettaja_arvo.Value == 1)
			int arvo = (VariantValue)lahettaja_arvo.Value;
			Globals.Tags.Log(string.Format("[iX] Event ({0}): {1} Line1_PLC_AloitettuX_ValueChange {2}", DateTime.Now.ToString(), lahettaja_nimi.Name, arvo));
			if (Globals.Tags.AppStart_Timer >= 10 && arvo == 1)
			{
				// Nollataan trippimittari?

				// TODO: rethink.... if (Globals.Tags.HMI_InfeedStartCount.Value.Int <= 0)
				{
					// Resetoidaan aloitus kesken
					Globals.Tags.HMI_Aloitus_kesken.Value = false;

					// Näytetään popup
					Globals.Tags.HMI_Success_TextValue.SetAnalog(2);
					Globals.Popup_Success.Show();

					// Suljetaan aloitusikkuna, kun popup-suljetaan
					Globals.Popup_Success.Closed += Popup_Success_Closed;
				}
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

			int arvo = (VariantValue)lahettaja_arvo.Value;
			System.Diagnostics.Trace.WriteLine(string.Format("[iX] Event ({0}): {1} Line1_PLC_LopetettuX_ValueChange {2}", DateTime.Now.ToString(), lahettaja_nimi.Name, arvo));
			if (Globals.Tags.AppStart_Timer >= 10 && arvo == 1)
			{
				int i = lahettaja_nimi.Name.Length - 1;
				if (i > 0)
				{
					string ratano = lahettaja_nimi.Name.Substring(i, 1);
					Globals.Tags.Log("[iX] Clear Line1_PLC_Aloitus " + ratano);
					try
					{
						Globals.Tags.HMI_InfeedStartCount.DecrementAnalog(1);

						string name = "Line1_PLC_Aloitus" + ratano;
						IBasicTag tag = Globals.Tags.GetTag(name);
						if (tag != null) tag.ResetTag();

						name = "Line1_PLC_TulorataTuote" + ratano;
						tag = Globals.Tags.GetTag(name);
						if (tag != null) tag.ResetTag();
					}
					catch (Exception x)
					{
						Globals.Tags.Log(String.Format("Line1_PLC_LopetettuX_ValueChange: ratanumero {0} [{1}]", ratano, x.Message));
					}
				}
				// Toiminta, kun lopetettu
				Globals.Tags.HMI_StopProduction_CloseMe.SetAnalog(Globals.Tags.HMI_Overview_track_selected.Value);
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
