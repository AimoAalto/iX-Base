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
    
	   
	/// <summary>
	/// Tuotannon aloituksen ikkuna. Ikkuna avautuu, kun lopetettu 
	/// (sininen) tulorata valitaan päänäytöltä. Suodatukseen kelpaavat 
	/// lavapaikat haetaan kerran sivun avautuessa. Robotti valitaan valitun 
	/// tuloradan perusteella sivun avautuessa. Reseptilista (ListBox1) 
	/// päivitetään, joka kerta, kun suodatustiedot vaihtuvat.
	/// Lavapaikkojen valinnat (ComboBox1-8), kuvion tiedot, ja käärinnän 
	/// valinta näytetään, kun resepti valitaan. Tuotannon aloitus -painikkeen 
	/// painalluksella tarkistetaan valittujen tietojen oikeellisuus, 
	/// kommunikointiyhteydet. Tämän jälkeen aloituksen tiedot lähetetään 
	/// robotille ja logiikalle.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 22.3.2018</remarks>
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
		/// <summary>
		/// Aloitettava tulorata.
		/// </summary>
		int tulorata;
		/// <summary>
		/// Valitun kuvion rajoitustiedot.
		/// </summary>
		Kuviotiedot valittuKuvio;
		
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

			// Tulorata
			tulorata = Globals.Tags.HMI_Overview_track_selected.Value;
			
			// Robotti
			// Katsotaan kummalle robotille tulorata on
			robottiNo = 0;
			foreach (KeyValuePair<int, Dictionary<int, int>> robotti in _Konfiguraatio.RobotinTuloradat)
			{
				if (robotti.Value.ContainsKey(tulorata))
				{
					robottiNo = robotti.Key;
					break;
				}
			}
			if(robottiNo == 0)
			{
				// Robotin numeron parsinta epäonnistui
				throw new ConfigurationFaultException("Robotin numeroa ei voitu löytää tuloradan avulla:", "Tulorata " + tulorata);
			}
			// Siirretään aliakselle myös näytön käytettäväksi
			robottiNumero = robottiNo;
						
			//Tyhjätään aiemmin tuotantoon valittu resepti
			Globals.Tags.HMI_StartProd_RecipeSelected.Value = "";
						
			// Siivotaan kuvion tiedot aluksi
			Tyhjenna();
			
			// Haetaan tuotelista
			HaeTuotteet();

			#region Suodatustietojen hakeminen
            // Tyhjennetään suodatuslaatikot
            Suodatus1.Items.Clear();
			
			// Lisätään tyhjä valinta
			Suodatus1.Items.Add("");
			Suodatus1.SelectedItem = "";
			   
			// Katso mille lavapaikoille on olemassa yhteinen kuvio tuloradan kanssa
			foreach (int lavapaikka in _Konfiguraatio.RobotinLavapaikat[robottiNo].Keys)
			{
				// Yritetään hakea yhdistelmälle kuvio
				try 
				{	        
					int[] kuviot = Globals.Robotit.HaeSallitutKuviotLavapaikalla(tulorata, lavapaikka);
					if (kuviot.Length > 0)
					{
						Suodatus1.Items.Add(lavapaikka);
					}
				}
				catch (Exception)
				{
					// Kuvioa ei ollut, jatketaan
				}
			}	
            #endregion
					
			// Sivu ladattu
			avattu = true;
		}
		
		/// <summary>
		/// Päivittää aloitettavissa olevat sopivat lavapaikat ComboBoxeihin ja asettaa
		/// niistä näkyviin niin monta kuin niitä voi aloittaa. Valitsee oletuksena 
		/// ensimmäisen _Konfiguraatio.LavapaikanKuviot määritetyn kelpaavan lavapaikan.
		/// </summary>
		/// <param name="tulorata">Aloitettava tulorata</param>
		void AlustaPlaceBoxit(int tulorata)
		{
			// Alustetaan combobox-valikot aina kun valinta muuttuu
			// Haetaan valintalaatikot
			List<FrameworkElement> elementit = HaePlaceBoxit();
			
			// Tyhjätään ja piilotetaan valintalaatikot
			foreach (FrameworkElement elementti in elementit)
			{
				((Controls.WindowsControls.ComboBox)elementti).Items.Clear();
				((Controls.WindowsControls.ComboBox)elementti).Items.Add("");
				elementti.Visibility = Visibility.Hidden;
			}
								
			// Lavapaikkojen tarkistus 
			int lavapaikkoja = 0;
			foreach (int lavapaikka in _Konfiguraatio.RobotinLavapaikat[robottiNo].Keys)
			{
				// Tarkistetaan käykö kuvio lavapaikalle
				if (valittuKuvio.sallitutLavapaikat.Contains(lavapaikka))
				{									
					// Ei lisätä jo aloitettuja lavapaikkoja
					if (Globals.Tags.GetTagValue("Rob" + robottiNo + "_lavap" + _Konfiguraatio.RobotinLavapaikat[robottiNo][lavapaikka] + "_pkuv") < 0.5)
					{
						lavapaikkoja++;
						
						// Lisätään lavapaikka valintalaatikoihin
						foreach (FrameworkElement elementti in elementit)
						{
							((Controls.WindowsControls.ComboBox)elementti).Items.Add(lavapaikka.ToString());
						}
					}
				}
			}
			
			// Näytetään oikea määrä valintalaatikoita
			elementit.ElementAt(0).Visibility = Visibility.Visible;
			try
			{
				for (int i = 0; i < lavapaikkoja; i++)
				{
					elementit.ElementAt(i).Visibility = Visibility.Visible;
				}
			}
			catch 
			{
				// Laatikoita oli vähemmän kuin valittavia lavapaikkoja
				Globals.Tags.HMI_Error_TextValue.SetAnalog(22);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
				Globals.Popup_Error.Show();
			}
				
			// Tehdään oletusvalinta
			if (((Controls.WindowsControls.ComboBox)elementit.First()).Items.Count > 1)
			{
				((Controls.WindowsControls.ComboBox)elementit.First()).SelectedItem = 
					((Controls.WindowsControls.ComboBox)elementit.First()).Items[1];
			}
		}
		
		/// <summary>
		/// Lataa ja päivittää kuvion tiedot näytölle.
		/// Kuvion nimi, kuvion kuva.
		/// </summary>
		void HaeKuvionTiedot()
		{		
			// Haetaan kuvion tiedot JSON-tiedostosta
			valittuKuvio = Kuviotiedot.LataaKuviot()
				.Where(p => p.numero == Globals.Tags.ProdReg_PalletPattern.Value.Int)
				.SingleOrDefault();
					
			Lavaus.Kuvio Kuvio = new Lavaus.Kuvio();
			Kuvio.Validoi = false;
			Kuvio.JSON = @"C:\Lavaus\Kuviot\" + "Kuvio" + valittuKuvio.numero + ".json";

			// Yritetään ladata tiedosto
			try
			{
				Kuvio.Lataa();
				Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Aloitusruudussa kuvio " + valittuKuvio.numero + " ladattu.");
			}
			catch (Exception ex)
			{
				// Kuvion lataus epäonnistui
				Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Aloitusruudussa kuvion " + valittuKuvio.numero + " lataus epäonnistui: " + ex.Message);
				Globals.Tags.HMI_Error_TextValue.SetAnalog(4);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
				Globals.Popup_Error.Show();
				return;
			}

			// Jos kuvio on olemassa päivitetään näyttö            
			if (Kuvio.Nykyinen != null)
			{
				Desc_Text.Text = Kuvio.Nykyinen.PatternName + " - " + Kuvio.Nykyinen.CreationDate + "\n" + Kuvio.Nykyinen.Description + "\n" + Kuvio.Nykyinen.PalletTypes[0].Name;
				
				// Ladataan kuvion kuva
				try 
				{	  
					m_Pattern_Picture.Visibility = System.Windows.Visibility.Hidden;
					Pattern_Picture.Image = null;
					Pattern_Picture.Refresh();
				
					// Ladataan kuvion kuva, jos on olemassa
					if (Kuvio.Nykyinen.PalletizingImageFilename != null)
					{	
						string polku = @"C:\Lavaus\Kuvat\" + Kuvio.Nykyinen.PalletizingImageFilename;
						if(System.IO.File.Exists(polku))
						{
							Pattern_Picture.Image = Image.FromFile(polku);
							Pattern_Picture.SizeMode = PictureBoxSizeMode.Zoom;
							Pattern_Picture.Refresh();

							m_Pattern_Picture.Visibility = System.Windows.Visibility.Visible;
							Pattern_Picture.Dock = DockStyle.Fill;
							
							// Kuva ladattu onnistuneesti
							Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Aloitusruudussa kuvion " + valittuKuvio.numero + " kuva " + Kuvio.Nykyinen.PalletizingImageFilename + " ladattu.");
						}
						else
						{
							// Kuvatiedostoa ei ole
							Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Kuvion " + valittuKuvio.numero + " kuvatiedostoa " + Kuvio.Nykyinen.PalletizingImageFilename + " ei löydy.");							
						}
					}
					else
					{
						// Kuvaa ei ole määritetty
						Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Kuvion " + valittuKuvio.numero + " kuvaa ei ole määritetty.");							
					}
				}
				catch (Exception ex)
				{
					// Kuvion kuvan lataaminen epäonnistui
					Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Kuvion " + valittuKuvio.numero + " kuvan " + Kuvio.Nykyinen.PalletizingImageFilename + " lataaminen epäonnistui: " + ex.Message);
					Globals.Tags.HMI_Error_TextValue.SetAnalog(5);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
					Globals.Popup_Error.Show();
					return;
				}
			}
		}

		/// <summary>
		/// Hakee lavapaikkojen valintalaatikot. Laatikot ovat listassa
		/// numerojärjestyksessä.
		/// </summary>
		/// <returns>PlaceBox-elementit</returns>
		List<FrameworkElement> HaePlaceBoxit()
		{
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

				// Napataan vain lavapaikkaelementit
				if (lapsi.Name.Contains("PlaceBox"))
				{
					// Lisätään listaan
					elementit.Add(lapsi);
				}
			}
			
			// Järjestetään lista lopussa olevan numeron perusteella
			// Elementin nimi on m_PlaceBoxX
			elementit.Sort(delegate(FrameworkElement x, FrameworkElement y)
				{
					int xNumero, yNumero;
					int.TryParse(x.Name.Substring(10), out xNumero);
					int.TryParse(y.Name.Substring(10), out yNumero);
					return xNumero.CompareTo(yNumero);
				});
			
			return elementit;
		}
		
		/// <summary>
		/// Hakee kaikki hakuehtoihin sopivat tuotteet tietokannasta ja päivittää
		/// listan ListBox1:een.
		/// </summary>
		/// <returns>Palauttaa hakuehtoihin sopivat tuotteet DataSet-muodossa.</returns>
		DataSet HaeTuotteet()
		{
			// Tyhjennetään lista
			ListBox1.Items.Clear();

			try
			{
				DataSet tuoteLista;
				
				// Onko hakutekstiä tai lavapaikkasuodatus
				if (Hakukentta2.Value.ToString() != "0" && Suodatus1.SelectedItem != null && Suodatus1.SelectedItem.ToString() != "")
				{
					// Haetaan tuloradalla, hakuehdolla ja lavapaikalla
					tuoteLista = Globals.Tuotetiedot.HaeReseptit(Globals.Robotit.HaeSallitutKuviotLavapaikalla(tulorata, int.Parse(Suodatus1.SelectedItem.ToString())), Hakukentta2.Value.ToString());
				}
				else if (Hakukentta2.Value.ToString() != "0")
				{
					// Haetaan vain tuloradalla ja hakuehdolla
					tuoteLista = Globals.Tuotetiedot.HaeReseptit(Globals.Robotit.HaeSallitutKuviot(tulorata), Hakukentta2.Value.ToString());
				}
				else if (Suodatus1.SelectedItem != null && Suodatus1.SelectedItem.ToString() != "")
				{
					// Haetaan vain tuloradalla ja lavapaikalla
					tuoteLista = Globals.Tuotetiedot.HaeReseptit(Globals.Robotit.HaeSallitutKuviotLavapaikalla(tulorata, int.Parse(Suodatus1.SelectedItem.ToString())));
				}
				else
				{
					// Haetaan kaikki tuloradan tuotteet
					tuoteLista = Globals.Tuotetiedot.HaeReseptit(Globals.Robotit.HaeSallitutKuviot(tulorata));
				}
				
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
					foreach(Resepti r in Reseptit.OrderBy(i => i.Numero).ThenBy(j => j.Nimi))
					{
						// Lisätään resepti näytölle
						ListBox1.Items.Add(r);
					}
				}
				
				return tuoteLista;
			}
			catch(Exception ex)
			{
				// Reseptien lataus epäonnistui
				Globals.Tags.HMI_Error_TextValue.SetAnalog(6);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message + "; " + ex.InnerException.Message;
				Globals.Popup_Error.Show();
				return new DataSet();
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
				HaeTuotteet();
				
				// Siivotaan kuvion tiedot 
				Tyhjenna();
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
				if(ListBox1.SelectedItem == null)
				{
					return;
				}

				// Siivotaan kuvion tiedot aluksi
				Tyhjenna();
			
				// Luetaan nimi näytölle 
				Resepti r = (Resepti)ListBox1.SelectedItem;
				Globals.Tags.HMI_StartProd_RecipeSelected.Value = r.Numero + " - " + r.Nimi;	
			
				// Valitaan resepti myös taustalle			
				Globals.Tags.HMI_StartProd_pallet_privis.ResetTag();		
				Globals.Tuotetiedot.LoadRecipe(r.Nimi);
				
				// Ladataan kuvion tiedot
				HaeKuvionTiedot();
				
				// Päivitetään lavapaikkojen valintaboxit
				AlustaPlaceBoxit(tulorata);
				
				// Näytä käärintäkenttä
				ComboBox_Wrapping.Visible = true;
			}
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
			if(Globals.Tags.HMI_StartProd_RecipeSelected.Value == "" || ListBox1.SelectedItem == null)
			{	
				// Herjataan puuttuvasta tuotevalinnasta
				Globals.Tags.HMI_Error_TextValue.SetAnalog(1);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
				Globals.Popup_Error.Show();
			}
			else
			{									
				// Yhteystarkistukset
				//if ((Globals.Tags.GetTagValue("HMI_CommFault_Rob" + robottiNo) == 0) 
				//	&& (Globals.Tags.Line1_Comm_Fault_PLC1.Value == 0) 
				//	&& (Globals.Tags.GetTagValue("Line1_PLC_R" + robottiNo + "_ManAuto") == 1)) 
				//{								
					//Communications OK!
					Globals.Tags.Line1_Comm_OK.Value = 1;
								
					#region Lavapaikat	
					// Tarkistetaan, että lavapaikka on valittu
					bool valittu = false;
					foreach (FrameworkElement elementti in HaePlaceBoxit())
					{
						if (((Controls.WindowsControls.ComboBox)elementti).SelectedItem != null
							&& (string)((Controls.WindowsControls.ComboBox)elementti).SelectedItem != "")
						{
							valittu = true;
						}
					}	
					if(!valittu)
					{
						// Herjataan puuttuvasta lavapaikan valinnasta
						Globals.Tags.HMI_Error_TextValue.SetAnalog(2);
						Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
						Globals.Popup_Error.Show();
						return;	
					}
					
					// Valitaan aloitettavat lavapaikat 1 ... n
					// Huom! Tässä projektissa voi valita kehityshetkellä vain yhden lavapaikan. Usean lavapaikan valinta on vain luonnos, jota ei ole testattu. -SoPi 6/2017
					List<int> lavapaikat = new List<int>();	
						
					// Käydään valintalaatikot läpi
					foreach (Controls.WindowsControls.ComboBox lavapaikkaValitsin in HaePlaceBoxit())
					{
						if (lavapaikkaValitsin.SelectedItem != null
							&& (string)lavapaikkaValitsin.SelectedItem != "")
						{
							int lavapaikka = Convert.ToInt16(lavapaikkaValitsin.SelectedItem);
						
							// Suodatetaan duplikaatit lavapaikat pois
							if (!lavapaikat.Contains(_Konfiguraatio.RobotinLavapaikat[robottiNo][lavapaikka]))
							{
								// Tarkistetaan, että lavapaikka ei ole jo aloitettu
								if (Globals.Tags.GetTagValue("Rob" + robottiNo + "_lavap" + _Konfiguraatio.RobotinLavapaikat[robottiNo][lavapaikka] + "_pkuv") < 0.5)
								{
									// Lisätään lavapaikka listaan
									lavapaikat.Add(_Konfiguraatio.RobotinLavapaikat[robottiNo][lavapaikka]);
								}
								else
								{
									// Lavapaikka on jo aloitettu tai robotilla on väärä tieto!
									Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Aloituksessa valitulla robotin lavapaikalla " + _Konfiguraatio.RobotinLavapaikat[robottiNo][lavapaikka] + "on jo kuvio.");
									Globals.Tags.HMI_Error_TextValue.SetAnalog(3);
									Globals.Tags.HMI_Error_AdditionalInfo.Value =
										"Pallet place " + _Konfiguraatio.RobotinLavapaikat[robottiNo][lavapaikka] 
										+ " of robot " + robottiNo;
									Globals.Popup_Error.Show();
									return;
								}
							}
						}
					}
														
					// Kirjoitetaan aloitettavat lavapaikat muistiin
					// Asetetaan lavapaikan numeron mukainen bitti
					GlobalDataItem lavapaikatTagi = (GlobalDataItem)Globals.Tags.GetTag("Line1_PLC_PalletPlaces" + tulorata);
					for (int i = 1; i < lavapaikatTagi.ArraySize; i++)
					{
						if (lavapaikat.Contains(i))
						{
							lavapaikatTagi[i].Value = true;
						}
						else
						{
							lavapaikatTagi[i].Value = false;
						}
					}				
					#endregion
					
					// Valitaan robotille aloitettavat tuloradat
					List<int> tuloradat = new List<int>();
					tuloradat.Add(_Konfiguraatio.RobotinTuloradat[robottiNo][tulorata]);
				                    
					if(lavapaikat.Count > 0)
					{
						#region Aloitus robotille
						Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Aloituksen lähetys alkaa.");

						// Alustetaan command ID sanomia varten
						string Command_Id = string.Empty;
						// Luetaan kuvio
						Lavaus.Kuvio Kuvio = new Lavaus.Kuvio();
						Kuvio.Validoi = false;
						Kuvio.JSON = @"C:\Lavaus\Kuviot\" + "Kuvio" + Globals.Tags.ProdReg_PalletPattern.Value + ".json";

						// Yritetään ladata tiedosto
						try
						{
							Kuvio.Lataa();
						}
						catch (Exception ex)
						{
							// Lataus epäonnistui
							Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Kuvion " + Globals.Tags.ProdReg_PalletPattern.Value + " lataus epäonnistui: " + ex.Message);
							// Kuvion lataus epäonnistui
							Globals.Tags.HMI_Error_TextValue.SetAnalog(4);
							Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
							Globals.Popup_Error.Show();
							return;
						}

						// Jos kuvio on olemassa päivitetään näyttö
						if (Kuvio.Nykyinen != null)
						{
							Text1.Text = "Luettu: " + Kuvio.JSON;
							try
							{
								Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Aloitus kuviolla " + Globals.Tags.ProdReg_PalletPattern.Value);
								Command_Id = Globals.Robotit.robotit[robottiNo].TeeAloitus(tuloradat, lavapaikat, Globals.Tags.ProdReg_PalletPattern.Value, Kuvio);
							}
							catch (Exception ex)
							{
								Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Aloituksen teko epäonnistui: " + ex.Message);
								Globals.Tags.HMI_Error_TextValue.SetAnalog(7);
								Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
								Globals.Popup_Error.Show();
								return;
							}
							Globals.Tags.HMI_Aloitus_kesken.Value = true;
							/*
							Watchdog = new Timer((args) => {
								// Mitataan kauanko operaatioissa kestää
								
								Watchdog.Change(1, Timeout.Infinite);
							}, null, 0, Timeout.Infinite);
							*/
						}
						else
						{
							// Tyhjennetään näyttö
							Text1.Text = "Kuviota ei ole olemassa";
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
						if(valikkeet.Length < Kuvio.Nykyinen.Layers + 1)
						{
							for (int i = valikkeet.Length; i < Kuvio.Nykyinen.Layers + 1; i++) 
							{
								// Lisätään loppuun välikkeettömiä kerroksia kuvion maksimiin asti
								pahvit += ";0";
							}
						}	
                        
						// Lähetetään samat tiedot kaikille aloituksen kohteena oleville lavapaikoille
						foreach (int lavapaikka in lavapaikat)
						{
							Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Aloitetaan lavapaikka " + lavapaikka + ".");
							
							// Asetetaan välikkeet
							Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Välipahvit: '" + pahvit + "'");
							Globals.Robotit.robotit[robottiNo].AsetaPahvit(Command_Id, lavapaikka, pahvit);

							// Robotille kerrosmäärä
							Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Kerrosmäärä: " + Globals.Tags.ProdReg_LayerCount.Value);
							Globals.Robotit.robotit[robottiNo].AsetaKerrosmaara(Command_Id, lavapaikka, 
								Globals.Tags.ProdReg_LayerCount.Value);

							// Robotille paikan nopeus ja tartunta- ja jättöviive
							Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Viiveet: " + Globals.Tags.ProdReg_Robot1_Speed_Full.Value + "; " + Globals.Tags.ProdReg_Robot1_Acceleration_Full.Value + "; " + Globals.Tags.ProdReg_PickDelay.Value + "; " + Globals.Tags.ProdReg_PlaceDelay.Value);
							Globals.Robotit.robotit[robottiNo].PaikkaNopeus(Command_Id, lavapaikka, 
								Globals.Tags.ProdReg_Robot1_Speed_Full.Value, 
								Globals.Tags.ProdReg_Robot1_Acceleration_Full.Value, 
								Globals.Tags.ProdReg_PickDelay.Value, 
								Globals.Tags.ProdReg_PlaceDelay.Value);
							
							// Lavaus Offset
							Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Offset: X " + Globals.Tags.ProdReg_Robot1_X_Centering.Value + ", Y " + Globals.Tags.ProdReg_Robot1_Y_Centering.Value);
							Globals.Robotit.robotit[robottiNo].PaikkaOffset(Command_Id, lavapaikka, 
								Globals.Tags.ProdReg_Robot1_X_Centering.Value, 
								Globals.Tags.ProdReg_Robot1_Y_Centering.Value);
							
							Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Lavapaikan " + lavapaikka + " aloitus valmis.");
						}

						// Lopetetaan aloitus
						Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Aloituksen lähetys valmis.");
						Globals.Robotit.robotit[robottiNo].AloituksenLopetus(Command_Id);
						#endregion
						
						#region Aloitus logiikalle	
						// Lavatyyppi 
						Globals.Tags.SetTagValue("Line1_PLC_PalletType" + tulorata, Globals.Tags.ProdReg_PalletType.Value);
														
						// Käärintä byte 0 = ei 1 = on
						//Globals.Tags.SetTagValue("Line1_PLC_Kaarinta_TK" + tulorata, Globals.Tags.HMI_StartProd_Wrapping.Value.UShort);
						Globals.Tags.SetTagValue("Line1_PLC_WrappingProg" + tulorata, Globals.Tags.ProdReg_WrappingProgram.Value);
						
						// Lavapaikka
						// Logiikka käyttöö samoja numeroita kuin robotti, joten lavapaikat-listasta löytyy oikea
						Globals.Tags.SetTagValue("Line1_PLC_Lavapaikka_TK" + tulorata, lavapaikat.FirstOrDefault());
						
						// Reseptin rivinro omaan talteen
						Globals.Tags.SetTagValue("Line1_Rivinumero_TK" + tulorata, Globals.Tags.ProdReg_RiviNro.Value);
						
						//------------------------------------------------------------------------------------------------
						// UUDET TUOTEREKISTERIN TAGIT LOGIIKKAAN 19.10.2020
						Globals.Tags.SetTagValue("Line1_PLC_Length" + tulorata, Globals.Tags.ProdReg_Product_Length.Value);
						Globals.Tags.SetTagValue("Line1_PLC_Width" + tulorata, Globals.Tags.ProdReg_Product_Width.Value);
						Globals.Tags.SetTagValue("Line1_PLC_Height" + tulorata, Globals.Tags.ProdReg_Product_Height.Value);
						//------------------------------------------------------------------------------------------------
						
						// Odotetaan hetki, että tagit menevät varmasti logiikalle
						System.Threading.Timer aloituskasky = new System.Threading.Timer((args) => {
							// Logiikan aloituskäsky
							Globals.Tags.SetTagValue("Line1_PLC_Aloitus" + tulorata, true);
							Globals.Tags.SetTagValue("HMI_StartProduction_PLC_Aloitettu", false);
							
							}, null, 1000, System.Threading.Timeout.Infinite);
						#endregion
						
					}
					
						//Jos lavapaikkaa ei ole valittu	
					else
					{
						// Herjataan puuttuvasta lavapaikan valinnasta
						Globals.Tags.HMI_Error_TextValue.SetAnalog(2);
						Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
						Globals.Popup_Error.Show();
						return;	
					}
				//}
					//Jos aloitusehdot (kommunikointi virhe) ei kelvolliset
				//else
				//{
				//	Globals.Tags.HMI_Error_TextValue.SetAnalog(8);
				//	Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
				//	Globals.Popup_Error.Show();
				//}				
			}
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
				HaeTuotteet();

				// Siivotaan kuvion tiedot 
				Tyhjenna();
			}
		}			

		/// <summary>
		/// Tyhjentää aiemmin valitun reseptin ja piilottaa kaikki reseptin 
		/// valinnan jälkeen näytettävät kentät.
		/// </summary>
		void Tyhjenna()
		{
			//Tyhjätään aiemmin tuotantoon valittu resepti
			Globals.Tags.HMI_StartProd_RecipeSelected.Value = "";

			// Kuvion tekstit pois
			Desc_Text.Text = "";
			m_Pattern_Picture.Visibility = System.Windows.Visibility.Hidden;

			// Piilota täyttökentät
			foreach (FrameworkElement elementti in HaePlaceBoxit())
			{
				elementti.Visibility = Visibility.Hidden;
			}
			
			// Piilota käärintäkenttä
			ComboBox_Wrapping.Visible = false;
		}
	}
}