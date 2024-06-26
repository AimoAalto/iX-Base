namespace Neo.ApplicationFramework.Generated
{
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
		readonly object lockme = new object();
		System.Threading.Timer HeaderPaivitys;
		private readonly bool hmistart = false;

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
			if (Globals.Tags.TraceAll) Globals.Tags.Log("Ajotiedot Created (start)");

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
				Globals.Tags.Log(string.Format("Ajotiedot_Create: Interval error, use default (10000)\n{0}", x.Message));
			}

			#region HeaderPaivitys

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

			#endregion

			#region start/stop

			// Seurataan ja ilmoitetaan onnistuneesta aloituksesta ja lopetuksesta
			foreach (int tulorata in Globals._Konfiguraatio.CurrentConfig.Tuloradat.Keys)
			{
				string name = "";
				try
				{
					if (Globals.Tags.TraceAll)
						Globals.Tags.Log(string.Format("Ajotiedot AloitettuX / LopetettuX, tulorata : {0}", tulorata));

					//Globals.Tags.S7HMI_ToHMI_Line_21_CommBits_ProdStartOK.ResetTag();
					name = string.Format("S7HMI_ToHMI_Line_{0}_CommBits_ProdStartOK", tulorata);
					IBasicTag tag = Globals.Tags.GetTag(name);
					if (tag == null)
						Globals.Tags.Log(String.Format("Ajotiedot_Create: Unknown Tag {0}", name));
					else
						tag.ValueChange += Line1_PLC_AloitettuX_ValueChange;

					//Globals.Tags.S7HMI_ToHMI_Line_21_CommBits_ProdEndOK.ResetTag();
					name = string.Format("S7HMI_ToHMI_Line_{0}_CommBits_ProdEndOK", tulorata);
					tag = Globals.Tags.GetTag(name);
					if (tag == null)
						Globals.Tags.Log(String.Format("Ajotiedot_Create: Unknown Tag {0}", name));
					else
						tag.ValueChange += Line1_PLC_LopetettuX_ValueChange;
				}
				catch (Exception x)
				{
					Globals.Tags.Log(String.Format("Ajotiedot_Create: Unknown error {0}\n{1}", name, x.Message));
				}
			}

			#endregion
		}

		#region Ajossa

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
						string name = string.Format("S7HMI_Line{0}_ToHMI_Production_CommBits_ProdStartOK", tulorata);
						try
						{
							// Haetaan tuloradan tila
							IBasicTag tag = Globals.Tags.GetTag(name);
							if (tag == null)
								Globals.Tags.Log(String.Format("AjossaolevatTuotteet: Unknown Tag {0}\n", name));
							else
							{
								// Haetaan tuloradan tila
								if (Globals.Tags.GetTagValue(name).Bool)
								{
									// Haetaan tuloradalla ajossa oleva tuote ja lisätään listaan
									string nimi = HaeTuloradanTuote(tulorata);

									if (!string.IsNullOrEmpty(ajossa)) ajossa += "\n";
									// Haetaan termit TextLibrarysta valmiiksi käännettynä
									ajossa += msg0 // Ryhmittely
										+ " " + tulorata + " - " + nimi;
									ajossa += ", " + msg1; // LP

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
									/*name = "Line1_PLC_PalletPlaces" + tulorata;
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
									}*/
								}
								else
								{
									// Tulorata on lopetettu
									// Haetaan termit TextLibrarysta valmiiksi käännettynä
									ajossa += msg0 + " " + tulorata + " " // Ryhmittely
										+ msg2; // lopetettu
								}

								// Lisätään lopuksi rivin vaihto
								ajossa += "\n";
							}
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
					Globals.Tags.HMI_Overview_ProdDetails.Value = ajossa;
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.WriteLine(string.Format("Exception [AjossaOlevatTuotteet] {0}", ex.Message));
				}
		}

		#endregion

		#region lavapaikka / tulorata

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
					//Globals.Tags.S7HMI_ToHMI_Line_21_CommBits_ProdStartOK.ResetTag();
					if (Globals.Tags.GetTagValue(string.Format("S7HMI_ToHMI_Line_{0}_CommBits_ProdStartOK", tulorata)) > 0)
					{
						// Tarkistetaan ajaako tulorata tälle lavapaikalle
						int lp = Globals.Tags.GetTagValue("Line1_PLC_Lavapaikka_TK" + tulorata);
						if (lp == lavapaikka)
						{
							// Haetaan tuloradan ajettava tuote logiikasta ja sille nimi tietokannasta
							return HaeTuloradanTuote(tulorata);
						}

						// Monen aloitettavan lavapaikan hakufunktio
						//Globals.Tags.Line1_PLC_PalletPlaces1.ResetTag();
						var lps = ((GlobalDataItem)Globals.Tags.GetTag("Line1_PLC_PalletPlaces" + tulorata)).Values;

						if (lps != null)
							if (lps[lavapaikka]) // ? what value
							{
								// Haetaan tuloradan ajettava tuote logiikasta ja sille nimi tietokannasta
								return HaeTuloradanTuote(tulorata);
							}
					}
				}
				catch (Exception x)
				{
					Globals.Tags.Log(String.Format("HaeLavapaikantuote: tulorata {0}\n{1}\n{2}", tulorata, x.Message, x.InnerException));
				}
			}

			return "";
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
				//Globals.Tags.Line1_Rivinumero_TK1.ResetTag();
				int tuote = (int)Globals.Tags.GetTagValue("Line1_Rivinumero_TK" + tulorata.ToString());
				if (tuote > 0)
				{
					if (Globals.Tags.TraceAll) Globals.Tags.Log("HaeTuloradanTuote tulorata: " + tulorata.ToString() + " Kutsutaan Tuotenumerolla " + tuote);

					// Haetaan ajossa olevan tuotteen nimi tietokannasta
					return string.Format("{0} - {1}", 
						Globals.Tuotetiedot.ReseptinTuotenumero(tuote),
						Globals.Tuotetiedot.ReseptinNimi(tuote));
				}
				else
					return "";
			}
			catch (Exception x)
			{
				Globals.Tags.Log(String.Format("HaeTuloradanTuote: tulorata {0}\n{1}", tulorata, x.Message));
				return "n/a";
			}
		}

		#endregion

		#region start / stop events

		/// <summary>
		/// Update active devices and routes
		/// </summary>
		/// <returns></returns>
		/*public void UpdateActiveDevices(int no, bool clear)
		{
		}*/

		/// <summary>
		/// Näyttää popup-ikkunan onnistuneesta aloituksesta.
		/// Mahdollista suorittaa muita toimintoje, kun aloitus onnistuu.
		/// </summary>
		/// <param name="sender">Line1_PLC_AloitettuX</param>
		public void Line1_PLC_AloitettuX_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			IBasicTag lahettaja = (IBasicTag)sender;

			//if (Globals.Tags.AppStart_Timer >= 20 && (VariantValue)lahettaja_arvo.Value == 1)
			int arvo = (VariantValue)lahettaja.Value;
			Globals.Tags.Log(string.Format("[iX] Event ({0}): {1} Line1_PLC_AloitettuX_ValueChange {2}", DateTime.Now.ToString(), lahettaja.Name, arvo));
			if (Globals.Tags.AppStart_Timer >= 10 && arvo == 1)
			{
				// Nollataan trippimittari?

				// Resetoidaan aloitus kesken
				Globals.Tags.HMI_ProductionStarting.ResetTag();
				Globals.Tags.HMI_StartProd_RecipeSelected.ResetTag();

				if (hmistart) // start per infeed !!
				{
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
		public void Line1_PLC_LopetettuX_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			IBasicTag lahettaja = (IBasicTag)sender;

			int arvo = (VariantValue)lahettaja.Value;
			Globals.Tags.Log(string.Format("[iX] Event ({0}): {1} Line1_PLC_LopetettuX_ValueChange {2}", DateTime.Now.ToString(), lahettaja.Name, arvo));
			if (Globals.Tags.AppStart_Timer >= 10 && arvo == 1)
			{
				int i = lahettaja.Name.Length - 1;
				if (i > 0)
				{
					// S7HMI_Line1_ToHMI_Production_CommBits_ProdEndOK
					string[] items = lahettaja.Name.Split('_');

					string ratano = items[3];
					Globals.Tags.Log("[iX] Clear CommBits_Start / End [" + ratano + "]");
					try
					{
						//Globals.Tags.S7HMI_ToPLC_Line_21_CommBits_ProdStart.ResetTag();
						// S7HMI_ToHMI_Line_1_CommBits_ProdEndOK
						// S7HMI_ToHMI_Line_1_CommBits_ProdStartOK
						//S7HMI_ToHMI_Line_1_CommBits_ProdStarting
						// 
						string name = string.Format("S7HMI_ToPLC_Line_{0}_CommBits_ProdStart", ratano);
						IBasicTag tag = Globals.Tags.GetTag(name);
						if (tag != null) tag.ResetTag();

						//Globals.Tags.S7HMI_ToPLC_Line_1_CommBits_ProdEnd.ResetTag();
						name = string.Format("S7HMI_ToPLC_Line_{0}_CommBits_ProdEnd", ratano);
						tag = Globals.Tags.GetTag(name);
						if (tag != null) tag.ResetTag();

						//Globals.Tags.Line1_Rivinumero_TK1.ResetTag();
						name = string.Format("Line1_Rivinumero_TK{0}", ratano);
						tag = Globals.Tags.GetTag(name);
						if (tag != null) tag.ResetTag();

						//Globals.Tags.Line1_PLC_Lavapaikka_TK1.ResetTag();
						name = string.Format("Line1_PLC_Lavapaikka_TK{0}", ratano);
						tag = Globals.Tags.GetTag(name);
						if (tag != null) tag.ResetTag();

						//Globals.Tags.Line1_PLC_KuvioNro_TK1.ResetTag();
						name = string.Format("Line1_PLC_KuvioNro_TK{0}", ratano);
						tag = Globals.Tags.GetTag(name);
						if (tag != null) tag.ResetTag();
					}
					catch (Exception x)
					{
						Globals.Tags.Log(String.Format("Line1_PLC_LopetettuX_ValueChange: ratanumero {0}\n{1}", ratano, x.Message));
					}
				}
				// Toiminta, kun lopetettu
				Globals.Tags.HMI_StopProduction_CloseMe.SetAnalog(Globals.Tags.HMI_Overview_TrackSelected.Value);

				/*bool b1 = Globals.Tags.S7HMI_Line1_ToHMI_Production_CommBits_ProdStartOK.Value;
				bool b2 = Globals.Tags.S7HMI_Line2_ToHMI_Production_CommBits_ProdStartOK.Value;
				bool b3 = Globals.Tags.S7HMI_Line3_ToHMI_Production_CommBits_ProdStartOK.Value;
				bool b4 = Globals.Tags.S7HMI_Line4_ToHMI_Production_CommBits_ProdStartOK.Value;
				if (!b1 && !b2 && !b3 && !b4) UpdateActiveDevices(-1, true);
				*/
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
			//if (Globals.Tags.SystemTagCurrentScreenId.Value == 10002) Globals.Tags.SystemTagNewScreenId.SetAnalog(10001);

			Globals.Popup_Success.Closed -= Popup_Success_Closed;
		}

		#endregion
	}
}
