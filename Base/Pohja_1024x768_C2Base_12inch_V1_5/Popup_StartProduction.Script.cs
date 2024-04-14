namespace Neo.ApplicationFramework.Generated
{
	using Neo.ApplicationFramework.Controls.Script;
	using Neo.ApplicationFramework.Tools.OpcClient;
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Drawing;
	using System.Linq;
	using System.Windows;
	using System.Windows.Forms;
	using System.Windows.Media;


	public partial class Popup_StartProduction
	{
		/// <summary>
		/// Jotkut toiminnot suoritetaan vain kun sivu on jo avattu.
		/// </summary>
		bool avattu = false;
		/// <summary>
		/// Aloitettavan tuloradan robotti.
		/// </summary>
		int robottiNo;
		/// sallitut tuloradat
		List<int> tracks = new List<int>();

		/// <summary>
		/// Alustaa sivun tiedot, kun sivu avautuu.
		/// Hakee robotin numeron tuloradan perusteella.
		/// Tyhjentää kaikki aiemmat tiedot ja valinnat.
		/// Hakee kaikki sopivat tuotteet näytölle ilman suodatusta.
		/// Täyttää mahdolliset suodatusvalinnat.
		/// </summary>
		/// <param name="sender">this</param>
		void Start_Production1_Opened(System.Object sender, System.EventArgs e)
		{
			robottiNo = Globals.Tags.HMI_Selected_RobotNo.Value;

			if (robottiNo == 0)
			{
				//todo: PopupMessageBox
				// Robotin numeron parsinta epäonnistui
				System.Windows.MessageBox.Show("Robottia ei ole valittu");
			}

			// Siirretään aliakselle myös näytön käytettäväksi
			robottiNumero = (short)robottiNo;

			// hae sallitut tuloradat
			tracks = Globals._Konfiguraatio.CurrentConfig.AllowedInfeedTracks(robottiNo);

			if (tracks.Count == 0)
			{
				//todo: PopupMessageBox
				System.Windows.MessageBox.Show("Robotilla ei ole sallittuja tuloratoja");
			}
			
			// Siivotaan kuvion tiedot aluksi
			ClearSelection();

			// Haetaan tuotelista
			ListProducts();

			#region Suodatustietojen hakeminen

			// Tyhjennetään suodatuslaatikot
			Suodatus1.Items.Clear();

			// Lisätään tyhjä valinta
			Suodatus1.Items.Add("");
			Suodatus1.SelectedItem = "";

			if (tracks.Count > 0)
			{
				List<int> places = Globals._Konfiguraatio.CurrentConfig.AllowedPalletPlaces(robottiNo, tracks[0]);
				foreach (int lp in places) Suodatus1.Items.Add(lp);
				places.Clear();
			}

			#endregion
			
			#region valmiit lavapaikat


			for (int i = 1; i < 12; i++)
			{
				string name = string.Format("S7HMI_PPStates_Place_{0}_ReadyForProdStart", i);
				LightweightTag tag = (LightweightTag)Globals.Tags.GetTag(name);
				if (tag == null)
				{
					Globals.Tags.Log(String.Format("Unknown Tag {0}", name));
				}
				else
				{
					bool ready = (bool)tag.Value;
					if (ready)
					{
						CbSelectedPalletPlace.Items.Add(i);
					}
				}
			}
			
			#endregion	

			// Sivu ladattu
			avattu = true;
		}

		/// <summary>
		/// Tyhjentää aiemmin valitun reseptin ja piilottaa kaikki reseptin 
		/// valinnan jälkeen näytettävät kentät.
		/// </summary>
		void ClearSelection()
		{
			//Tyhjätään aiemmin tuotantoon valittu resepti
			Globals.Tags.HMI_StartProd_RecipeSelected.Value = "";

			// Kuvion tekstit pois
			Desc_Text.Text = "";
			Pattern_Picture.Visible = false;

			// Piilota käärintäkenttä
			ComboBox_Wrapping.Visible = false;
			Text_Wrapping.Visible = false;
		}

		/// <summary>
		/// Päivittää tuotelistan ja tyhjentää valitun tuotteen, kun hakuehto muuttuu.
		/// </summary>
		/// <param name="sender">this.Suodatus1</param>
		void Suodatus1_SelectionChanged(System.Object sender, System.EventArgs e)
		{
			if (avattu)
			{
				// Haetaan valittavissa olevat tuotteet
				ListProducts();

				// Siivotaan kuvion tiedot 
				ClearSelection();
			}
		}

		/// <summary>
		/// Päivittää tuotelistan ja tyhjentää valitun tuotteen, kun hakuehto muuttuu.
		/// </summary>
		/// <param name="sender">this.Hakukentta2</param>
		void Hakukentta2_ValueChanged(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			if (avattu)
			{
				ListProducts();

				// Siivotaan kuvion tiedot 
				ClearSelection();
			}
		}

		/// <summary>
		/// Tyhjää ensin edellisen valinnan tiedot ja täyttää sitten ruudun valitun reseptin 
		/// tiedoilla ja valinnoilla.
		/// Lataa reseptin tietokannasta.
		/// Näyttää valitun reseptin nimen ja tuotenumeron.
		/// Näyttää lavapaikkojen valinnat.
		/// Lataa kuvion tiedot.
		/// </summary>
		/// <param name="sender">this.ListBox1</param>
		void ListBox1_SelectionChanged(System.Object sender, System.EventArgs e)
		{
			if (avattu)
			{
				// Varmistetaan, ettei eventti tullut kun valinta tyhjennettiin
				if (ListBoxProducts.SelectedItem == null)
				{
					return;
				}

				// Siivotaan kuvion tiedot aluksi
				ClearSelection();

				// Luetaan nimi näytölle 
				Resepti r = (Resepti)ListBoxProducts.SelectedItem;
				Globals.Tags.HMI_StartProd_RecipeSelected.Value = r.Numero + " - " + r.Nimi;

				// Valitaan resepti myös taustalle			
				Globals.Tags.HMI_StartProd_pallet_privis.ResetTag();
				Globals.Tuotetiedot.LoadRecipe(r.Nimi);

				// Ladataan kuvion tiedot
				int valittuKuvionumero = HaeKuvionTiedot();

				// Näytä käärintäkenttä
				//ComboBox_Wrapping.Visible = true;
				//Text_Wrapping.Visible = true;
			}
		}

		/// <summary>
		/// Hakee kaikki hakuehtoihin sopivat tuotteet tietokannasta ja päivittää
		/// listan ListBox1:een.
		/// </summary>
		/// <returns>Palauttaa hakuehtoihin sopivat tuotteet DataSet-muodossa.</returns>
		void ListProducts()
		{
			// Tyhjennetään lista
			ListBoxProducts.Items.Clear();

			if (tracks.Count > 0)
				try
				{
					int filterpalletplace = 0;
					// Onko hakutekstiä (tuotenimi tai numero)
					string filterproduct = m_Hakukentta2.Text.Trim();

					// Onko lavapaikkasuodatus
					if (Suodatus1.SelectedItem != null && Suodatus1.SelectedItem.ToString() != "")
						filterpalletplace = int.Parse(Suodatus1.SelectedItem.ToString());

					List<int> patterns = Globals._Konfiguraatio.CurrentConfig.PatternsForInfeed(tracks[0], filterpalletplace);
					DataSet tuoteLista = Globals.Tuotetiedot.HaeReseptit(patterns.ToArray(), filterproduct);

					// Päivitetään reseptien nimet näytölle
					if (tuoteLista.Tables.Count > 0)
					{
						// Käydään kaikki reseptit läpi
						List<Resepti> Reseptit = new List<Resepti>();
						foreach (DataRow rivi in tuoteLista.Tables[0].Rows)
						{
							// Lisätään resepti listaan
							Reseptit.Add(new Resepti()
							{
								Nimi = rivi["FieldName"].ToString(),
								Numero = Convert.ToInt32(rivi["Tuotenumero"]),
								RiviNro = Convert.ToInt32(rivi["RiviNro"])
							});
						}

						// Luetaan kaikki reseptit näytölle halutussa järjestyksessä
						foreach (Resepti r in Reseptit.OrderBy(i => i.Numero).ThenBy(j => j.Nimi))
						{
							// Lisätään resepti näytölle
							ListBoxProducts.Items.Add(r);
						}
					}
				}
				catch (Exception ex)
				{
					// Reseptien lataus epäonnistui
					Globals.Tags.HMI_Error_TextValue.SetAnalog(6);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message + "; " + ex.InnerException.Message;
					Globals.Popup_Error.Show();
				}
		}

		/// <summary>
		/// Lataa ja päivittää kuvion tiedot näytölle.
		/// Kuvion nimi, kuvion kuva.
		/// </summary>
		int HaeKuvionTiedot()
		{
			int valittuKuvionumero = Globals.Tags.HMI_ProdReg_PalletPattern.Value.Int;

			Lavaus.Kuvio Kuvio = new Lavaus.Kuvio();
			Kuvio.Validoi = false;
			Kuvio.JSON = _Konfiguraatio.PatternDirectory + "Kuvio" + valittuKuvionumero + ".json";

			// Yritetään ladata tiedosto
			try
			{
				Kuvio.Lataa();
				Globals.Robotit.LisaaLokiin(robottiNo, "Aloitusruudussa kuvio " + valittuKuvionumero + " ladattu.");
				
				Lavaus.Converters conv = new Lavaus.Converters();
				conv.Debug = true;
				List<string> messages = conv.JSONtoMessages(Kuvio.Nykyinen, "001", true);
			}
			catch (Exception ex)
			{
				// Kuvion lataus epäonnistui
				Globals.Robotit.LisaaLokiin(robottiNo, "Aloitusruudussa kuvion " + valittuKuvionumero + " lataus epäonnistui: " + ex.Message);
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.PatternLoadFailed);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
				Globals.Popup_Error.Show();
				return -1;
			}

			// Jos kuvio on olemassa päivitetään näyttö            
			if (Kuvio.Nykyinen != null)
			{
				Desc_Text.Text = Kuvio.Nykyinen.PatternName + " - " + Kuvio.Nykyinen.CreationDate + "\n" + Kuvio.Nykyinen.Description + "\n" + Kuvio.Nykyinen.PalletTypes[0].Name;

				// Ladataan kuvion kuva
				try
				{
					Pattern_Picture.Visible = false;
					Pattern_Picture.Image = null;
					Pattern_Picture.Refresh();

					// Ladataan kuvion kuva, jos on olemassa
					if (Kuvio.Nykyinen.PalletizingImageFilename != null)
					{
						string fname = Kuvio.Nykyinen.PalletizingImageFilename;
						if (string.IsNullOrEmpty(fname)) fname = string.Format("{0}.jpg", valittuKuvionumero);	
						
						string polku = _Konfiguraatio.PictureDirectory + fname;
						if (System.IO.File.Exists(polku))
						{
							Pattern_Picture.Image = System.Drawing.Image.FromFile(polku);
							Pattern_Picture.SizeMode = PictureBoxSizeMode.Zoom;
							Pattern_Picture.Refresh();

							Pattern_Picture.Visible = true;
							Pattern_Picture.Dock = DockStyle.Fill;

							// Kuva ladattu onnistuneesti
							Globals.Robotit.LisaaLokiin(robottiNo, "Aloitusruudussa kuvion " + valittuKuvionumero + " kuva " + Kuvio.Nykyinen.PalletizingImageFilename + " ladattu.");
						}
						else
						{
							// Kuvatiedostoa ei ole
							Globals.Robotit.LisaaLokiin(robottiNo, "Kuvion " + valittuKuvionumero + " kuvatiedostoa " + Kuvio.Nykyinen.PalletizingImageFilename + " ei löydy.");
						}
					}
					else
					{
						// Kuvaa ei ole määritetty
						Globals.Robotit.LisaaLokiin(robottiNo, "Kuvion " + valittuKuvionumero + " kuvaa ei ole määritetty.");
					}
				}
				catch (Exception ex)
				{
					// Kuvion kuvan lataaminen epäonnistui
					Globals.Robotit.LisaaLokiin(robottiNo, "Kuvion " + valittuKuvionumero + " kuvan " + Kuvio.Nykyinen.PalletizingImageFilename + " lataaminen epäonnistui: " + ex.Message);
					Globals.Tags.HMI_Error_TextValue.SetAnalog(5);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
					Globals.Popup_Error.Show();
					return -1;
				}
			}
			return valittuKuvionumero;
		}

		/// <summary>
		/// Suorittaa tuotannon aloituksen ja siihen liittyvät tarkistukset ja rutiinit.
		/// Tarkistaa, että yhteys logiikkaan ja robotille on OK.
		/// Tarkistaa, että sekä Robotti että PLC ovat automaatilla.
		/// Tarkistaa, että valitut tiedot ovat sallittuja.
		/// Siirtää tuotetiedot robotille ja pyytää aloitusta.
		/// Siirtää tuotetiedot logiikalle ja pyytää aloitusta.
		/// </summary>
		/// <param name="sender">this.Start_Production</param>
		void Start_Production_btn_Click(System.Object sender, System.EventArgs e)
		{
			if (Globals.Tags.HMI_StartProd_RecipeSelected.Value == "" || ListBoxProducts.SelectedItem == null)
			{
				// Herjataan puuttuvasta tuotevalinnasta
				Globals.Tags.HMI_Error_TextValue.SetAnalog(1);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
				Globals.Popup_Error.Show();
			}
			else
			{
				/*Globals.Tags.HMI_CommFault_Rob1.ResetTag();
				Globals.Tags.HMI_CommFault_Rob2.ResetTag();
				Globals.Tags.Line1_PLC_R1_ManAuto.ResetTag();
				*/
				// Yhteystarkistukset
				bool comm_ok = (Globals.Tags.GetTagValue("HMI_CommFault_Rob1") == 0)
					&& (Globals.Tags.GetTagValue("HMI_CommFault_Rob2") == 0)
					&& (Globals.Tags.HMI_CommFault_PLC1.Value == 0)
					&& (Globals.Tags.GetTagValue(string.Format("Line1_PLC_R{0}_ManAuto", robottiNo)) == 1);

				comm_ok = true;  // debug

				if (comm_ok)
				{
					Globals.Tags.Rob1_lavap0_pkuv.ResetTag();
					
					#region tuloradat

					int tr = Globals.Tags.HMI_SelectedInfeedLine.Value;
					List<int> _tuloradat = new List<int>();
					if (tr > 0)
						_tuloradat.Add(tr);

					// Tarkistetaan, että tulorata on valittu
					if (_tuloradat.Count == 0)
					{
						// Herjataan puuttuvasta tulorata valinnasta
						Globals.Tags.HMI_Error_TextValue.SetAnalog(3);
						Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
						Globals.Popup_Error.Show();
						return;
					}

					#endregion

					#region Lavapaikat

					List<int> rlavapaikat = new List<int>();
					int pp = Globals.Tags.HMI_SelectedPalletPlace.Value;
					
					// hae robotin lavapaikka numero
					int rlp = Globals._Konfiguraatio.CurrentConfig.Lavapaikat[pp];
					// Tarkistetaan, että lavapaikka ei ole jo aloitettu
					if (Globals.Tags.GetTagValue("Rob" + robottiNo + "_lavap" + rlp + "_pkuv") >= 0.5)
					{
						// Lavapaikka on jo aloitettu tai robotilla on väärä tieto!
						Globals.Robotit.LisaaLokiin(robottiNo, "Aloituksessa valitulla robotin lavapaikalla " + rlp + "on jo kuvio.");
						Globals.Tags.HMI_Error_TextValue.SetAnalog(3);
						Globals.Tags.HMI_Error_AdditionalInfo.Value = "Pallet place " + rlp + " of robot " + robottiNo;
						Globals.Popup_Error.Show();
						return;
					}
					else
						rlavapaikat.Add(rlp);
					
					Globals.Tags.Line1_PLC_PalletPlaces0.Value = pp;

					//int tr = Globals.Tags.GetHashCode.Value;
					// Kirjoitetaan aloitettavat lavapaikat muistiin
					// Asetetaan lavapaikan numeron mukainen bitti
					/*GlobalDataItem lavapaikatTagi = (GlobalDataItem)Globals.Tags.GetTag("Line1_PLC_PalletPlaces" + tr);
					if (lavapaikatTagi != null)
						for (int i = 1; i < lavapaikatTagi.ArraySize; i++)
						{
							lavapaikatTagi[i].Value = lavapaikat.Contains(i);
						}*/

					#endregion

					#region infeeds per patterns check

					//
					string s_patterns = Globals.Tags.HMI_ProdReg_PalletPattern.Value;
					if (string.IsNullOrEmpty(s_patterns) || s_patterns.Trim().CompareTo("0") == 0)
						s_patterns = Globals.Tags.HMI_ProdReg_PalletPattern.Value;

					System.Diagnostics.Trace.WriteLine(s_patterns);
					
					List<int> lst = new List<int>();
					string[] values = s_patterns.Split(',');
					if (values.Length > 0)
					{
						int no = 0;
						foreach (string item in values)
							if (int.TryParse(item, out no)) lst.Add(no);
					}

					Dictionary<int, int> rtulorata_kuvio = new Dictionary<int, int>();

					// debug reasons...
					rtulorata_kuvio.Add(tr, lst[0]);
					
					// Valitaan robotille aloitettavat tuloradat
					/*foreach (int pno in lst)
					{
						System.Diagnostics.Trace.WriteLine("Allowed Pattern [" + pno.ToString() + "]");
						if (Globals._Konfiguraatio.CurrentConfig.AllowedPatterns.ContainsKey(pno))
						{
							foreach (int tr in Globals._Konfiguraatio.CurrentConfig.AllowedPatterns[pno].Tuloradat)
							{
								if (_tuloradat.Contains(tr))
								{
									foreach (int item in lst)
									{
										if (item != pno)
										{
											System.Diagnostics.Trace.WriteLine("6.2 ["+item.ToString()+"]");
											if (Globals._Konfiguraatio.CurrentConfig.AllowedPatterns.ContainsKey(pno))
											{
												if (Globals._Konfiguraatio.CurrentConfig.AllowedPatterns[item].Tuloradat.Contains(tr))
												{
													// tuloradalle on kaksi kuviota
													Globals.Robotit.LisaaLokiin(robottiNo, "Aloituksessa valitulla tuloradalla " + tr + "on useampia kuvioita.");
													Globals.Tags.HMI_Error_TextValue.SetAnalog(3);
													Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
													Globals.Popup_Error.Show();
													return;
												}
											}
											else
											{
												Globals.Tags.HMI_Error_TextValue.SetAnalog(38); // UnknownPatternNumber
												Globals.Tags.HMI_Error_AdditionalInfo.Value = "Pallet pattern number " + pno.ToString() + " not allowed for robot " + robottiNo;
												Globals.Popup_Error.Show();
												return;
											}
										}
									}
									rtulorata_kuvio.Add(tr, pno);
								}
							}
						}
						else
						{
							Globals.Tags.HMI_Error_TextValue.SetAnalog(38); // UnknownPatternNumber
							Globals.Tags.HMI_Error_AdditionalInfo.Value = "Pallet pattern number " + pno.ToString() + " not allowed for robot " + robottiNo;
							Globals.Popup_Error.Show();
							return;
						}
					}*/

					#endregion

					if (rlavapaikat.Count > 0 && rtulorata_kuvio.Count > 0)
					{
						Globals.Robotit.LisaaLokiin(robottiNo, "Aloituksen lähetys alkaa.");

						foreach (int _tr in rtulorata_kuvio.Keys)
						{
							Globals.Tags.HMI_Starting_Infeed.SetAnalog(_tr);
							Globals.Tags.HMI_Starting_Palletplace.SetAnalog(pp);
							Globals.Tags.HMI_Starting_ProdNo.SetAnalog(Globals.Tags.HMI_ProdReg_ProductNo.Value);
							Globals.Tags.HMI_Starting_OrderNo.SetAnalog(Globals.Tags.HMI_SelectedOrderno.Value);

							#region Aloitus robotille

							int rtr = Globals._Konfiguraatio.CurrentConfig.Tuloradat[_tr];
							int kuviono = rtulorata_kuvio[rtr];
							// Alustetaan command ID sanomia varten
							string Command_Id = string.Empty;
							// Luetaan kuvio
							Lavaus.Kuvio Kuvio = new Lavaus.Kuvio();
							Kuvio.Validoi = false;
							Kuvio.JSON = _Konfiguraatio.PatternDirectory + "Kuvio" + kuviono + ".json";

							// Yritetään ladata tiedosto
							try
							{
								Kuvio.Lataa();
							}
							catch (Exception ex)
							{
								// Lataus epäonnistui
								Globals.Robotit.LisaaLokiin(robottiNo, "Kuvion " + kuviono + " lataus epäonnistui: " + ex.Message);
								// Kuvion lataus epäonnistui
								Globals.Tags.HMI_Error_TextValue.SetAnalog(4);
								Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
								Globals.Popup_Error.Show();
								return;
							}

							// Jos kuvio on olemassa päivitetään näyttö
							if (Kuvio.Nykyinen != null)
							{
								TextStartConds.Text = "Luettu: " + Kuvio.JSON;
								try
								{
									// aloitetaan yksi tulorata kerrallaan loopissa
									List<int> rtuloradat = new List<int>();
									rtuloradat.Add(rtr);
									Globals.Robotit.LisaaLokiin(robottiNo, string.Format("Aloitus tulorata {0} kuviolla {1}", rtr, kuviono));
									Command_Id = Globals.Robotit.TeeAloitus(robottiNo, rtuloradat, rlavapaikat, kuviono, Kuvio);
								}
								catch (Exception ex)
								{
									Globals.Robotit.LisaaLokiin(robottiNo, "Aloituksen teko epäonnistui: " + ex.Message);
									Globals.Tags.HMI_Error_TextValue.SetAnalog(7);
									Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
									Globals.Popup_Error.Show();
									return;
								}
							}
							else
							{
								// Tyhjennetään näyttö
								TextStartConds.Text = "Kuviota ei ole olemassa";
								ClearSelection();
								return;
							}

							// VÄLIKKEET ROBOTILLE
							string pahvit = Globals.Tags.HMI_ProdReg_Spacers.Value;

							// Varmistetaan, että string on olemassa
							if (pahvit == "")
							{
								pahvit = "0";
							}

							// Tarkistetaan, että lähetetään tarpeeksi kerroksia
							string[] valikkeet = pahvit.Split(';');

							// Välikkeitä pitää olla lavan kerrokset + aluskerros
							if (valikkeet.Length < Kuvio.Nykyinen.Layers + 1)
							{
								for (int i = valikkeet.Length; i < Kuvio.Nykyinen.Layers + 1; i++)
								{
									// Lisätään loppuun välikkeettömiä kerroksia kuvion maksimiin asti
									pahvit += ";0";
								}
							}

							// Lähetetään samat tiedot kaikille aloituksen kohteena oleville lavapaikoille
							foreach (int lavapaikka in rlavapaikat)
							{
								Globals.Robotit.LisaaLokiin(robottiNo, "Aloitetaan lavapaikka " + lavapaikka + ".");

								// Asetetaan välikkeet
								Globals.Robotit.LisaaLokiin(robottiNo, "Välipahvit: '" + pahvit + "'");
								Globals.Robotit.AsetaPahvit(robottiNo, Command_Id, lavapaikka, pahvit);

								// Robotille kerrosmäärä
								Globals.Robotit.LisaaLokiin(robottiNo, "Kerrosmäärä: " + Globals.Tags.HMI_ProdReg_LayerCount.Value);
								Int16 kerrokset = Convert.ToInt16(Globals.Tags.HMI_ProdReg_LayerCount.Value.Int);
								Globals.Robotit.AsetaKerrosmaara(robottiNo, Command_Id, lavapaikka, kerrokset);

								// Robotille paikan nopeus ja tartunta- ja jättöviive
								Globals.Robotit.LisaaLokiin(robottiNo, "Viiveet: " + Globals.Tags.HMI_ProdReg_Robot1_Speed_Full.Value + "; " + Globals.Tags.HMI_ProdReg_Robot1_Acceleration_Full.Value + "; " + Globals.Tags.HMI_ProdReg_PickDelay.Value + "; " + Globals.Tags.HMI_ProdReg_PlaceDelay.Value);
								Globals.Robotit.PaikkaNopeus(robottiNo, Command_Id, lavapaikka,
									Globals.Tags.HMI_ProdReg_Robot1_Speed_Full.Value,
									Globals.Tags.HMI_ProdReg_Robot1_Acceleration_Full.Value,
									Globals.Tags.HMI_ProdReg_PickDelay.Value,
									Globals.Tags.HMI_ProdReg_PlaceDelay.Value);

								// Lavaus Offset
								Globals.Robotit.LisaaLokiin(robottiNo, "Offset: X " + Globals.Tags.HMI_ProdReg_Robot1_X_Centering.Value + ", Y " + Globals.Tags.HMI_ProdReg_Robot1_Y_Centering.Value);
								Globals.Robotit.PaikkaOffset(robottiNo, Command_Id, lavapaikka,
									Globals.Tags.HMI_ProdReg_Robot1_X_Centering.Value.Double,
									Globals.Tags.HMI_ProdReg_Robot1_Y_Centering.Value.Double);

								Globals.Robotit.LisaaLokiin(robottiNo, "Lavapaikan " + lavapaikka + " aloitus valmis.");
							}

							// Lopetetaan aloitus
							Globals.Robotit.LisaaLokiin(robottiNo, "Aloituksen lähetys valmis.");
							Globals.Robotit.AloituksenLopetus(robottiNo, Command_Id);

							#endregion

							#region Aloitus logiikalle	
			
							// Käärintä byte 0 = ei 1 = on
							//Globals.Tags.SetTagValue("Line1_PLC_Kaarinta_TK" + _tr, Globals.Tags.HMI_StartProd_Wrapping.Value.UShort);
							//Globals.Tags.SetTagValue("S7HMI_ToPLC_Line_{0}_CommBits_WrappingProg" + _tr, Globals.Tags.HMI_ProdReg_WrappingProgram.Value);

							// Lavapaikka
							// Logiikka käyttöö samoja numeroita kuin robotti, joten lavapaikat-listasta löytyy oikea
							//Globals.Tags.Line1_PLC_Lavapaikka_TK1.ResetTag(); 
							Globals.Tags.SetTagValue("Line1_PLC_Lavapaikka_TK" + _tr, pp);
							Globals.Tags.SetTagValue(string.Format("S7HMI_ToPLC_Line_{0}_TargetPallet", _tr), pp);
							
							//Globals.Tags.S7HMI_ToPLC_Line1_Recipe_Common_PalletType.SetAnalog(Globals.Tags.HMI_ProdReg_PalletType.Value);
							//Globals.Tags.S7HMI_ToPLC_Line_1_Recipe_PalletType.ResetTag();
							Globals.Tags.SetTagValue(string.Format("S7HMI_ToPLC_Line_{0}_Recipe_PalletType", _tr), Globals.Tags.HMI_ProdReg_PalletType.Value);

							// Käärintä byte 0 = ei 1 = on
							//Globals.Tags.S7HMI_ToPLC_Line1_Recipe_GroupPacking_WrappingProg.SetAnalog(Globals.Tags.WrappingProg.Value);

							// Reseptin rivinro omaan talteen
							Globals.Tags.Line1_Rivinumero_TK1.ResetTag();
							Globals.Tags.SetTagValue("Line1_Rivinumero_TK" + _tr, Globals.Tags.HMI_ProdReg_RiviNro.Value);
							Globals.Tags.SetTagValue("Line1_PLC_KuvioNro_TK" + _tr, kuviono);

							//tuloradan tuote
							//Globals.Tags.S7HMI_ToPLC_Line_21_Recipe_ProductNumber.SetAnalog(0);
							Globals.Tags.SetTagValue(string.Format("S7HMI_ToPLC_Line_{0}_Recipe_ProductNumber", _tr), Globals.Tags.HMI_ProdReg_ProductNo.Value);
						
							//------------------------------------------------------------------------------------------------
							//Globals.Tags.S7HMI_ToPLC_Line_21_Recipe_ProductLength.SetAnalog(0);
							Globals.Tags.SetTagValue(string.Format("S7HMI_ToPLC_Line_{0}_Recipe_ProductLength", _tr), Globals.Tags.HMI_ProdReg_Product_Length.Value);
							//Globals.Tags.S7HMI_ToPLC_Line_21_Recipe_ProductWidth.SetAnalog(0);
							Globals.Tags.SetTagValue(string.Format("S7HMI_ToPLC_Line_{0}_Recipe_ProductWidth", _tr), Globals.Tags.HMI_ProdReg_Product_Width.Value);
							//Globals.Tags.S7HMI_ToPLC_Line_21_Recipe_ProductHeight.SetAnalog(0);
							Globals.Tags.SetTagValue(string.Format("S7HMI_ToPLC_Line_{0}_Recipe_ProductHeight", _tr), Globals.Tags.HMI_ProdReg_Product_Height.Value);

							//Globals.Tags.S7HMI_ToPLC_Line_21_OrderNo.SetString(Globals.Tags.HMI_SelectedOrderno.Value);
							Globals.Tags.SetTagValue(string.Format("S7HMI_ToPLC_Line_{0}_OrderNo", _tr), Globals.Tags.HMI_SelectedOrderno.Value);
								
							// Odotetaan hetki, että tagit menevät varmasti logiikalle
							System.Threading.Timer aloituskasky = new System.Threading.Timer((args) =>
								{
									this.Dispatcher.Invoke((Action)(() =>
										{
										// Logiikan aloituskäsky 
										//Globals.Tags.S7HMI_ToPLC_Line_21_CommBits_ProdStart.ResetTag();
										Globals.Tags.SetTagValue(string.Format("S7HMI_ToPLC_Line_{0}_CommBits_ProdStart", _tr), true);
										}));
								}, null, 1000, System.Threading.Timeout.Infinite);

							#endregion
						}
					}
					else //Jos lavapaikkaa tai tulorataa ei ole valittu
					{
						// Herjataan puuttuvasta lavapaikan valinnasta
						Globals.Tags.HMI_Error_TextValue.SetAnalog(2);
						Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
						Globals.Popup_Error.Show();
						return;
					}
				}
				else //Jos aloitusehdot (kommunikointi virhe) ei kelvolliset
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog(8);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
					Globals.Popup_Error.Show();
				}
			}
		}
		
		void BtnStop_Click(System.Object sender, System.EventArgs e)
		{
			int tr = Globals.Tags.HMI_SelectedInfeedLine.Value;
			Globals.Robotit.TeeLopetus(1, tr);
		}
		
		void CbSelectedPalletPlace_SelectionChanged(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.HMI_SelectedPalletPlace.SetAnalog((int)CbSelectedPalletPlace.SelectedItem);
		}
	}
}