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

			List<FrameworkElement> cbs = GetElements("CBInfeed");
			HideCBs(cbs);

			int index = 0;
			foreach (int tr in tracks)
			{
				if (index < cbs.Count)
				{
					ActivateInfeedCB((System.Windows.Controls.CheckBox)cbs[index], tr);
				}
				index++;
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

			// Sivu ladattu
			avattu = true;
		}

		void ActivateInfeedCB(System.Windows.Controls.CheckBox cb, int no)
		{
			// ugly... and translations TextLibrary
			cb.Content = String.Format("Infeed {0}", no);
			cb.Tag = no;
			if (Globals.Tags.HMI_Overview_track_selected.Value.Int == no) cb.IsChecked = true;
			cb.Visibility = Visibility.Visible;
		}

		void HideCBs(List<FrameworkElement> cbs)
		{
			foreach (FrameworkElement cb in cbs)
			{
				cb.Visibility = Visibility.Hidden;
				((System.Windows.Controls.CheckBox)cb).IsChecked = false;
			}
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

			List<FrameworkElement> cbs = GetElements("CBPallet");
			HideCBs(cbs);

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

				// Päivitetään lavapaikkojen valintaboxit
				InitPlaceBoxit(valittuKuvionumero);

				// Näytä käärintäkenttä
				ComboBox_Wrapping.Visible = true;
				Text_Wrapping.Visible = true;
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

		void ActivatePalletCB(System.Windows.Controls.CheckBox cb, int no, bool _checked)
		{
			cb.Content = String.Format(" {0}", no);
			cb.Tag = no;
			cb.IsChecked = _checked;
			cb.Visibility = Visibility.Visible;
		}

		/// <summary>
		/// Päivittää aloitettavissa olevat sopivat lavapaikat CheckBoxeihin ja asettaa
		/// niistä näkyviin niin monta kuin niitä voi aloittaa. Valitsee oletuksena 
		/// ensimmäisen _Konfiguraatio.LavapaikanKuviot määritetyn kelpaavan lavapaikan.
		/// </summary>
		/// <param name="tulorata">Aloitettava tulorata</param>
		/// <param name="kuviono">Valitun tuotteen kuvionumero</param>
		void InitPlaceBoxit(int kuviono)
		{
			List<FrameworkElement> cbs = GetElements("CBPallet");
			// piilotetaan valintalaatikot
			HideCBs(cbs);

			if (kuviono < 0) return;

			// Lavapaikkojen tarkistus 
			List<int> palletplaces = new List<int>();
			foreach (int pp in Globals._Konfiguraatio.CurrentConfig.PatternAllowedPalletPlaces(robottiNo, kuviono))
			{
				// Ei lisätä jo aloitettuja lavapaikkoja
				if (Globals.Tags.GetTagValue("Rob" + robottiNo + "_lavap"
					+ Globals._Konfiguraatio.CurrentConfig.Lavapaikat[pp]
					+ "_pkuv") < 0.5)
				{
					palletplaces.Add(pp);
				}
			}

			try
			{
				int index = 0;
				// Näytetään oikea määrä valintalaatikoita
				foreach (int pp in palletplaces)
				{
					if (index < cbs.Count)
					{
						ActivatePalletCB((System.Windows.Controls.CheckBox)cbs[index], pp, (index == 0));
					}
					index++;
				}
			}
			catch
			{
				// Laatikoita oli vähemmän kuin valittavia lavapaikkoja, muu virhe
				Globals.Tags.HMI_Error_TextValue.SetAnalog(22);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
				Globals.Popup_Error.Show();
			}
		}

		/// <summary>
		/// Lataa ja päivittää kuvion tiedot näytölle.
		/// Kuvion nimi, kuvion kuva.
		/// </summary>
		int HaeKuvionTiedot()
		{
			int valittuKuvionumero = Globals.Tags.ProdReg_PalletPattern.Value.Int;

			Lavaus.Kuvio Kuvio = new Lavaus.Kuvio();
			Kuvio.Validoi = false;
			Kuvio.JSON = _Konfiguraatio.PatternDirectory + "Kuvio" + valittuKuvionumero + ".json";

			// Yritetään ladata tiedosto
			try
			{
				Kuvio.Lataa();
				Globals.Robotit.LisaaLokiin(robottiNo, "Aloitusruudussa kuvio " + valittuKuvionumero + " ladattu.");
			}
			catch (Exception ex)
			{
				// Kuvion lataus epäonnistui
				Globals.Robotit.LisaaLokiin(robottiNo, "Aloitusruudussa kuvion " + valittuKuvionumero + " lataus epäonnistui: " + ex.Message);
				Globals.Tags.HMI_Error_TextValue.SetAnalog(4);
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
						string polku = _Konfiguraatio.PictureDirectory + Kuvio.Nykyinen.PalletizingImageFilename;
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
				// Yhteystarkistukset
				bool comm_ok = (Globals.Tags.GetTagValue("HMI_CommFault_Rob" + robottiNo) == 0)
					&& (Globals.Tags.Line1_Comm_Fault_PLC1.Value == 0)
					&& (Globals.Tags.GetTagValue("Line1_PLC_R" + robottiNo + "_ManAuto") == 1);

				comm_ok = true;  // debug

				if (comm_ok)
				{
					//Communications OK!
					Globals.Tags.Line1_Comm_OK.Value = 1;

					#region tuloradat

					List<int> _tuloradat = new List<int>();
					List<FrameworkElement> cbs = GetElements("CBInfeed");

					// Käydään valintalaatikot läpi
					int index = 0;
					foreach (int tr in tracks)
					{
						if (index < cbs.Count)
							if (((System.Windows.Controls.CheckBox)cbs[index]).IsChecked == true)
							{
								bool started = (bool)Globals.Tags.GetTagValue("Line1_PLC_Aloitettu" + tr);
								// Tarkistetaan, että tulorata ei ole jo aloitettu
								if (started)
								{
									// Lavapaikka on jo aloitettu tai robotilla on väärä tieto!
									Globals.Robotit.LisaaLokiin(robottiNo, "Aloituksessa valitulla robotin tuloradalla " + tr + "on jo aloitus.");
									Globals.Tags.HMI_Error_TextValue.SetAnalog(3);
									Globals.Tags.HMI_Error_AdditionalInfo.Value = "Infeed Track " + tr + " of robot " + robottiNo;
									Globals.Popup_Error.Show();
									return;
								}
								_tuloradat.Add(tr);
							}
						index++;
					}

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

					// Valitaan aloitettavat lavapaikat 1 ... n
					// Huom! Tässä projektissa voi valita kehityshetkellä vain yhden lavapaikan. Usean lavapaikan valinta on vain luonnos, jota ei ole testattu. -SoPi 6/2017
					List<int> lavapaikat = new List<int>();

					cbs = GetElements("CBPallet");
					foreach (System.Windows.Controls.CheckBox cb in cbs)
					{
						if (cb.IsChecked == true)
							lavapaikat.Add((int)cb.Tag);
					}

					// Tarkistetaan, että lavapaikka on valittu
					if (lavapaikat.Count == 0)
					{
						// Herjataan puuttuvasta lavapaikan valinnasta
						Globals.Tags.HMI_Error_TextValue.SetAnalog(2);
						Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
						Globals.Popup_Error.Show();
						return;
					}

					List<int> rlavapaikat = new List<int>();

					// Käydään valintalaatikot läpi
					foreach (int pp in lavapaikat)
					{
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
					}

					// Kirjoitetaan aloitettavat lavapaikat muistiin
					// Asetetaan lavapaikan numeron mukainen bitti
					foreach (int tr in _tuloradat)
					{
						GlobalDataItem lavapaikatTagi = (GlobalDataItem)Globals.Tags.GetTag("Line1_PLC_PalletPlaces" + tr);
						if (lavapaikatTagi != null)
							for (int i = 1; i < lavapaikatTagi.ArraySize; i++)
							{
								lavapaikatTagi[i].Value = lavapaikat.Contains(i);
							}
					}

					#endregion

					#region infeeds per patterns check

					//
					string s_patterns = Globals.Tags.ProdReg_InfeedTrackPattern.Value;
					if (string.IsNullOrEmpty(s_patterns) || s_patterns.Trim().CompareTo("0") == 0)
						s_patterns = Globals.Tags.ProdReg_PalletPattern.Value;

					List<int> lst = new List<int>();
					string[] values = s_patterns.Split(',');
					if (values.Length > 0)
					{
						int no = 0;
						foreach (string item in values)
							if (int.TryParse(item, out no)) lst.Add(no);
					}

					Dictionary<int, int> rtulorata_kuvio = new Dictionary<int, int>();

					// Valitaan robotille aloitettavat tuloradat
					foreach (int pno in lst)
					{
						foreach (int tr in Globals._Konfiguraatio.CurrentConfig.AllowedPatterns[pno].Tuloradat)
						{
							if (_tuloradat.Contains(tr))
							{
								foreach (int item in lst)
								{
									if (item != pno)
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
								}
								rtulorata_kuvio.Add(tr, pno);
							}
						}
					}

					#endregion

					if (rlavapaikat.Count > 0 && rtulorata_kuvio.Count > 0)
					{
						Globals.Robotit.LisaaLokiin(robottiNo, "Aloituksen lähetys alkaa.");

						foreach (int _tr in rtulorata_kuvio.Keys)
						{
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
								Globals.Tags.HMI_Aloitus_kesken.Value = true;
							}
							else
							{
								// Tyhjennetään näyttö
								TextStartConds.Text = "Kuviota ei ole olemassa";
								ClearSelection();
								return;
							}

							// VÄLIKKEET ROBOTILLE
							string pahvit = Globals.Tags.ProdReg_Spacers.Value;

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
								Globals.Robotit.LisaaLokiin(robottiNo, "Kerrosmäärä: " + Globals.Tags.ProdReg_LayerCount.Value);
								Int16 kerrokset = Convert.ToInt16(Globals.Tags.ProdReg_LayerCount.Value.Int);
								Globals.Robotit.AsetaKerrosmaara(robottiNo, Command_Id, lavapaikka, kerrokset);

								// Robotille paikan nopeus ja tartunta- ja jättöviive
								Globals.Robotit.LisaaLokiin(robottiNo, "Viiveet: " + Globals.Tags.ProdReg_Robot1_Speed_Full.Value + "; " + Globals.Tags.ProdReg_Robot1_Acceleration_Full.Value + "; " + Globals.Tags.ProdReg_PickDelay.Value + "; " + Globals.Tags.ProdReg_PlaceDelay.Value);
								Globals.Robotit.PaikkaNopeus(robottiNo, Command_Id, lavapaikka,
									Globals.Tags.ProdReg_Robot1_Speed_Full.Value,
									Globals.Tags.ProdReg_Robot1_Acceleration_Full.Value,
									Globals.Tags.ProdReg_PickDelay.Value,
									Globals.Tags.ProdReg_PlaceDelay.Value);

								// Lavaus Offset
								Globals.Robotit.LisaaLokiin(robottiNo, "Offset: X " + Globals.Tags.ProdReg_Robot1_X_Centering.Value + ", Y " + Globals.Tags.ProdReg_Robot1_Y_Centering.Value);
								Globals.Robotit.PaikkaOffset(robottiNo, Command_Id, lavapaikka,
									Globals.Tags.ProdReg_Robot1_X_Centering.Value.Double,
									Globals.Tags.ProdReg_Robot1_Y_Centering.Value.Double);

								Globals.Robotit.LisaaLokiin(robottiNo, "Lavapaikan " + lavapaikka + " aloitus valmis.");
							}

							// Lopetetaan aloitus
							Globals.Robotit.LisaaLokiin(robottiNo, "Aloituksen lähetys valmis.");
							Globals.Robotit.AloituksenLopetus(robottiNo, Command_Id);

							#endregion

							#region Aloitus logiikalle	

							Globals.Tags.SetTagValue("HMI_StartProduction_PLC_Aloitettu", false);

							// Lavatyyppi 
							Globals.Tags.SetTagValue("Line1_PLC_PalletType" + _tr, Globals.Tags.ProdReg_PalletType.Value);

							// Käärintä byte 0 = ei 1 = on
							//Globals.Tags.SetTagValue("Line1_PLC_Kaarinta_TK" + _tr, Globals.Tags.HMI_StartProd_Wrapping.Value.UShort);
							Globals.Tags.SetTagValue("Line1_PLC_WrappingProg" + _tr, Globals.Tags.ProdReg_WrappingProgram.Value);

							// Lavapaikka
							// Logiikka käyttöö samoja numeroita kuin robotti, joten lavapaikat-listasta löytyy oikea
							Globals.Tags.SetTagValue("Line1_PLC_Lavapaikka_TK" + _tr, lavapaikat.FirstOrDefault());

							// Reseptin rivinro omaan talteen
							Globals.Tags.SetTagValue("Line1_Rivinumero_TK" + _tr, Globals.Tags.ProdReg_RiviNro.Value);

							//------------------------------------------------------------------------------------------------
							// UUDET TUOTEREKISTERIN TAGIT LOGIIKKAAN 19.10.2020
							Globals.Tags.SetTagValue("Line1_PLC_Length" + _tr, Globals.Tags.ProdReg_Product_Length.Value);
							Globals.Tags.SetTagValue("Line1_PLC_Width" + _tr, Globals.Tags.ProdReg_Product_Width.Value);
							Globals.Tags.SetTagValue("Line1_PLC_Height" + _tr, Globals.Tags.ProdReg_Product_Height.Value);
							//------------------------------------------------------------------------------------------------

							// Odotetaan hetki, että tagit menevät varmasti logiikalle
							System.Threading.Timer aloituskasky = new System.Threading.Timer((args) =>
								{
									this.Dispatcher.Invoke((Action)(() =>
										{
										// Logiikan aloituskäsky
										Globals.Tags.SetTagValue("Line1_PLC_Aloitus" + _tr, true);
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

		/// <summary>
		/// Hakee valintalaatikot. Laatikot ovat listassa
		/// numerojärjestyksessä.
		/// </summary>
		/// <returns>PlaceBox-elementit</returns>
		List<FrameworkElement> GetElements(string namepart)
		{
			string filter = string.Format("m_{0}", namepart);
			// Alustetaan combobox-valikot aina kun valinta muuttuu
			List<FrameworkElement> elementit = new List<FrameworkElement>();

			// Ikkunan alla on vain yksi elementti (ContentPresenter)
			FrameworkElement contentPresenter = (FrameworkElement)VisualTreeHelper.GetChild(this, 0);

			// Ikkunan alla on vain yksi elementti (ElementCanvas)
			FrameworkElement elementCanvas = (FrameworkElement)VisualTreeHelper.GetChild(contentPresenter, 0);

			// ElementCanvasin alla on näkyvät elementit
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(elementCanvas); i++)
			{
				FrameworkElement lapsi = (FrameworkElement)VisualTreeHelper.GetChild(elementCanvas, i);

				if (lapsi is System.Windows.Controls.CheckBox)
					if (lapsi.Name.StartsWith(filter))
					{
						// Lisätään listaan
						elementit.Add(lapsi);
					}
			}

			// Järjestetään lista lopussa olevan numeron perusteella
			// Elementin nimi on m_PlaceBoxX
			elementit.Sort(delegate (FrameworkElement x, FrameworkElement y)
				{
					int xNumero, yNumero;
					int.TryParse(x.Name.Substring(10), out xNumero);
					int.TryParse(y.Name.Substring(10), out yNumero);
					return xNumero.CompareTo(yNumero);
				});

			return elementit;
		}
	}
}