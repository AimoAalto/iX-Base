namespace Neo.ApplicationFramework.Generated
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Windows;
	using System.Windows.Forms;
	using System.Windows.Media;
    
    /// <summary>
	/// Mahdollistaa sallittujen kuvioiden ja niille sallittujen 
	/// tuloratojen/lavapaikkojen/lavatyyppien määritelyn.
	/// Kuviojen tietoja säilytetään D:\Lavaus\Kuviot.json -JSON
	/// -tiedostossa.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 2.8.2017</remarks>
    public partial class Settings_Pan1_Scr5
    {
		/// <summary>
		/// Projektin tuloradat
		/// </summary>
		List<int> tuloradat = new List<int>();
		/// <summary>
		/// Projektin lavapaikat
		/// </summary>
		List<int> lavapaikat = new List<int>();
		/// <summary>
		/// Kuviolista
		/// </summary>
		List<Kuviotiedot> kuviot;
		/// <summary>
		/// ListBoxista valittu kuvio
		/// </summary>
		Kuviotiedot valittuKuvio = new Kuviotiedot();
		
		/// <summary>
		/// Lataa tarvittavat tiedot sivun avautuessa.
		/// <list type="bullet">
		/// <item>
		/// <description>Liittyy kuuntelemaan HMI_PatternListEditor_AddPattern.ValueChangea uuden kuvion lisäystä varten</description>
		/// </item>
		/// <item>
		/// <description>Päivittää kuviolistan.</description>
		/// </item>
		/// <item>
		/// <description>Tyhjentää edellisen kuvion tiedot.</description>
		/// </item>
		/// <item>
		/// <description>Kerää järjestelmän tuloradat/lavapaikat/lavatyypit _Konfiguraatiosta.</description>
		/// </item>
		/// <item>
		/// <description>Päivittää tuloratojen/lavapaikkojen/lavatyyppien valintalaatikot.</description>
		/// </item>
		/// </list>
		/// </summary>
		/// <param name="sender">this</param>
		void Settings_PatternList_Editor_Opened(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.HMI_PatternListEditor_AddPattern.ValueChange += HMI_PatternListEditor_AddPattern_ValueChanged;
			
			// Päivitetään kuviolista
			Paivita_Kuviot();
			
			// Tyhjennetään näyttö
			Desc_Text.Text = "";
			Tool_Text.Text = "";	  
			m_Kuva_Kuvio.Visibility = System.Windows.Visibility.Hidden;
			Kuva_Kuvio.Image = null;
			Kuva_Kuvio.Refresh();
			
			// Haetaan konfiguraatiosta tuloratojen määrä
			tuloradat.Clear();
			foreach (KeyValuePair<int, Dictionary<int, int>> robotti in _Konfiguraatio.RobotinTuloradat)
			{
				foreach (int tulorata in robotti.Value.Keys)
				{
					// _Konfiguraatiossa on tarkistus, ettei ole duplikaatteja tuloratoja
					tuloradat.Add(tulorata);
				}
			}
			
			// Haetaan konfiguraatiosta lavapaikkojen määrä
			lavapaikat.Clear();
			foreach (KeyValuePair<int, Dictionary<int, int>> robotti in _Konfiguraatio.RobotinLavapaikat)
			{
				foreach (int lavapaikka in robotti.Value.Keys)
				{
					// _Konfiguraatiossa on tarkistus, ettei ole duplikaatteja tuloratoja
					lavapaikat.Add(lavapaikka);
				}
			}
			
			// Tarkistetaan, onko tuloradoissa/lavapaikoissa/lavatyypeissä
			// jo oikeat tiedot -> viimeinen merkki on numero
			// Tekstit resetoituvat kielen vaihdossa, jolloin tämä pitää tehdä uudestaan
			if (!char.IsDigit(Infeed1.Text[Infeed1.Text.Length-1]))
			{
				// Näytetään oikea määrä asioita ja lisätään perään tunnistetiedot
				Nayta_Tuloradat();
			
				Nayta_Lavapaikat();
			
				Nayta_Lavatyypit();
			}
		}
		
		/// <summary>
		/// Poistaa valitun kuvion listasta ja tallentaa muutoksen JSON-tiedostoon.
		/// Kutsuu kuviolistan päivitystä Paivita_Kuviot poiston jälkeen.
		/// </summary>
		/// <param name="sender">this.Delete_btn</param>
		void Delete_btn_Click(System.Object sender, System.EventArgs e)
		{
			// Poista kuvio listasta
			kuviot.Remove(valittuKuvio);
			
			// Tallennetaan JSON-tiedostoon
			Kuviotiedot.TallennaKuviot(kuviot);
			
			// Päivitetään kuviolista
			Paivita_Kuviot();
		}
	
		/// <summary>
		/// Hakee sopivat näytön elementit nimen perusteella.
		/// </summary>
		/// <param name="nimenOsa">Pätkä elementin nimestä</param>
		/// <returns>Sopivat elementit</returns>
		List<FrameworkElement> HaeElementit(string nimenOsa)
		{
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
				if (lapsi.Name.Contains(nimenOsa))
				{
					// Lisätään listaan
					elementit.Add(lapsi);
				}
			}
			
			return elementit;
		}
		
		/// <summary>
		/// Lisää uuden kuvionumeron listaan, jos syötettä numeroa ei ole jo olemassa.
		/// Uusi kuvio lisätään vain Pattern_ListBoxiin, jotta kuviota ei voi tallentaa
		/// ilman muokkaamatta ensin. Jos kuvionumero on jo olemassa, kuviota ei tallenneta 
		/// ja käyttäjälle ilmoitetaan duplikaatista numerosta näyttämällä 
		/// Popup_PatternList_Error. Lisäksi tarkistetaan, että kuvionumerolle on olemassa
		/// kuviotiedosto D:\Lavaus\Kuviot\Kuvio[numero].txt.
		/// </summary>
		/// <param name="sender">HMI_PatternListEditor_AddPattern</param>
		void HMI_PatternListEditor_AddPattern_ValueChanged(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			if (Globals.Tags.HMI_PatternListEditor_AddPattern.Value.Bool)
			{
				if (Globals.Tags.HMI_PatternListEditor_Number.Value.Int > 0)
				{
					// Tarkistetaan, että numeroa ei jo ole
					if (!kuviot.Select(p => p.numero).Contains(Globals.Tags.HMI_PatternListEditor_Number.Value.Int))
					{
						// Tarkistetaan, että kuviolle on kuviotiedosto
						string polku = @"C:\Lavaus\Kuviot\Kuvio" + Globals.Tags.HMI_PatternListEditor_Number.Value.Int + ".json";
						if (System.IO.File.Exists(polku))
						{
							Kuviotiedot uusiKuvio = new Kuviotiedot(Globals.Tags.HMI_PatternListEditor_Number.Value);
			
							// Lisää uusi numero kuviolistaan
							Pattern_ListBox.Items.Add(uusiKuvio);
			
							// Valitaan uusi kuvio muokattavaksi
							Pattern_ListBox.SelectedItem = uusiKuvio;
						}
						else
						{
							// Herjataan että kuviotiedostoa ei ole
							Globals.Tags.HMI_Error_TextValue.SetAnalog(9);
							Globals.Tags.HMI_Error_AdditionalInfo.Value = "Kuvio" + Globals.Tags.HMI_PatternListEditor_Number.Value.Int + ".json";
							Globals.Popup_Error.Show();
						}
					}
					else
					{
						// Herjataan duplikaatista numerosta
						Globals.Tags.HMI_Error_TextValue.SetAnalog(10);
						Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
						Globals.Popup_Error.Show();
					}
				}
			}
		}
	
		/// <summary>
		/// Järjestää elementtilistan elementtien nimen lopussa sijaitsevan numeron
		/// perusteella.
		/// </summary>
		/// <param name="lista">Järjestettävä lista</param>
		/// <param name="numeronIdeksi">Numeron alkuindeksi elementtien nimessä</param>
		/// <returns>Järjestetty elementtilista</returns>
		List<FrameworkElement> JarjestaLista(List<FrameworkElement> lista, int numeronIdeksi)
		{
			// Järjestetään lista lopussa olevan numeron perusteella
			lista.Sort(delegate(FrameworkElement x, FrameworkElement y)
				{
					int xNumero, yNumero;
					int.TryParse(x.Name.Substring(numeronIdeksi), out xNumero);
					int.TryParse(y.Name.Substring(numeronIdeksi), out yNumero);
					return xNumero.CompareTo(yNumero);
				});

			return lista;
		}
		
		/// <summary>
		/// Näyttää näytöllä oikean määrän lavapaikkoja ja lisää niihin numerot.
		/// Lisää numerot this.lavapaikat -listasta esiintymisjärjestyksessä.
		/// </summary>
		void Nayta_Lavapaikat()
		{
			// Haetaan lavapaikkaelementit			
			List<FrameworkElement> lavapaikkaElementit = HaeElementit("PalletPlace");
						
			// Järjestetään lista niin näytetään vain alkupäästä
			// Elementin nimi on m_PalletPlaceX
			lavapaikkaElementit = JarjestaLista(lavapaikkaElementit, 13);
			
			// Piilotetaan ensin kaikki lavapaikat
			foreach (FrameworkElement elementti in lavapaikkaElementit)
			{
				elementti.Visibility = Visibility.Hidden;
			}
			
			// Muokataan näkyviin oikea määrä lavapaikkoja
			for (int i = 0; i < lavapaikat.Count; i++)
			{
				// Lisätään tekstiin lavapaikan numero
				((Controls.WindowsControls.CheckBox)lavapaikkaElementit.ElementAt(i)).Text += " " + lavapaikat[i];
				
				// Asetetaan laatikko näkyviin
				lavapaikkaElementit.ElementAt(i).Visibility = Visibility.Visible;
			}
		}
		
		/// <summary>
		/// Näyttää näytöllä oikean määrän lavatyyppejä ja lisää niihin tyyppien nimet.
		/// Lisää lavatyyppien nimet _Konfiguraatio.Lavatyypit Dictionarysta esiintymisjärjestyksessä. 
		/// </summary>
		void Nayta_Lavatyypit()
		{	
			// Haetaan lavatyyppielementit			
			List<FrameworkElement> lavatyyppiElementit = HaeElementit("PalletType");
						
			// Järjestetään lista niin näytetään vain alkupäästä
			// Elementin nimi on m_PalletTypeX
			lavatyyppiElementit = JarjestaLista(lavatyyppiElementit, 12);
			
			// Piilotetaan ensin kaikki lavatyypit
			foreach (FrameworkElement elementti in lavatyyppiElementit)
			{
				elementti.Visibility = Visibility.Hidden;
			}
			
			// Muokataan näkyviin oikea määrä lavapaikkoja
			for (int i = 0; i < _Konfiguraatio.Lavatyypit.Keys.Count; i++)
			{
				// Lisätään tekstiin lavatyypin nimi
				((Controls.WindowsControls.CheckBox)lavatyyppiElementit.ElementAt(i)).Text += " " + _Konfiguraatio.Lavatyypit.ElementAt(i).Value;
				
				// Asetetaan laatikko näkyviin
				lavatyyppiElementit.ElementAt(i).Visibility = Visibility.Visible;
			}			
		}
		
		/// <summary>
		/// Näyttää näytöllä oikean määrän tuloratoja ja lisää niihin numerot.
		/// Lisää numerot this.tuloradat-listasta esiintymisjärjestyksessä.
		/// </summary>
		void Nayta_Tuloradat()
		{
			// Haetaan tulorataelementit			
			List<FrameworkElement> tulorataElementit = HaeElementit("Infeed");
						
			// Järjestetään lista niin näytetään vain alkupäästä
			// Elementin nimi on m_InfeedX
			tulorataElementit = JarjestaLista(tulorataElementit, 8);
			
			// Piilotetaan ensin kaikki tuloradat
			foreach (FrameworkElement elementti in tulorataElementit)
			{
				elementti.Visibility = Visibility.Hidden;
			}
			
			// Muokataan näkyviin oikea määrä tuloratoja
			for (int i = 0; i < tuloradat.Count; i++)
			{
				// Lisätään tekstiin tuloradan numero
				((Controls.WindowsControls.CheckBox)tulorataElementit.ElementAt(i)).Text += " " + tuloradat[i];
				
				// Asetetaan laatikko näkyviin
				tulorataElementit.ElementAt(i).Visibility = Visibility.Visible;
			}
		}
		
		/// <summary>
		/// Päivittää kuviolistan taustalle ja Pattern_ListBox:iin.
		/// Lataa kaikki kuviot JSON-tiedostosta ja täyttää ListBoxin.
		/// </summary>
		void Paivita_Kuviot()
		{
			// Ladataan kaikki kuviot JSON-tiedostosta
			kuviot = Kuviotiedot.LataaKuviot();
			
			// Listataan kuviot ListBoxiin
			Pattern_ListBox.Items.Clear();
			foreach (Kuviotiedot kuvio in kuviot.OrderBy(p => p.numero).ToList())
			{
				Pattern_ListBox.Items.Add(kuvio);
			}
		}	
		
		/// <summary>
		/// Päivittää näytön kuviovalinnan muuttuessa. Päivittää 
		/// tuloradat/lavapaikat/lavatyypit, lataa kuviotiedoston tiedot ja
		/// Lataa kuviotiedostoon merkityn kuvan.
		/// </summary>
		/// <param name="sender">this.Pattern_ListBox</param>
		void Pattern_ListBox_SelectionChanged(System.Object sender, System.EventArgs e)
		{
			// Valitaan kuvio taustalle tai tyhjätään valinta
			if (Pattern_ListBox.SelectedItem != null)
			{
				valittuKuvio = (Kuviotiedot)Pattern_ListBox.SelectedItem;
			}
			else
			{
				valittuKuvio = new Kuviotiedot();
			}
			
			// Tuloratojen päivitys
			try
			{
				// Haetaan tulorataelementit			
				List<FrameworkElement> tulorataElementit = JarjestaLista(HaeElementit("Infeed"), 8);
				
				for (int i = 0; i < tuloradat.Count; i++)
				{
					((Controls.WindowsControls.CheckBox)tulorataElementit.ElementAt(i)).IsChecked = 
						valittuKuvio.sallitutTuloradat.Contains(tuloradat[i]);
				}
			}
			catch
			{
				// Tulorataelementtejä ei ollut riittävästi
				Globals.Tags.HMI_Error_TextValue.SetAnalog(21);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = "tulorataElementit";
				Globals.Popup_Error.Show();
			} 
			
			// Lavapaikkojen päivitys
			try
			{
				// Haetaan lavapaikkaelementit			
				List<FrameworkElement> lavapaikkaElementit = JarjestaLista(HaeElementit("PalletPlace"), 13);
				
				for (int i = 0; i < lavapaikat.Count; i++)
				{
					((Controls.WindowsControls.CheckBox)lavapaikkaElementit.ElementAt(i)).IsChecked = 
						valittuKuvio.sallitutLavapaikat.Contains(lavapaikat[i]);
				}
			}
			catch 
			{
				// Lavapaikkaelementtejä ei ollut riittävästi
				Globals.Tags.HMI_Error_TextValue.SetAnalog(21);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = "lavapaikkaElementit";
				Globals.Popup_Error.Show();
			} 
			
			// Lavatyyppien päivitys
			try
			{
				// Haetaan lavatyyppielementit			
				List<FrameworkElement> lavatyyppiElementit = JarjestaLista(HaeElementit("PalletType"), 12);
				
				for (int i = 0; i < _Konfiguraatio.Lavatyypit.Count; i++)
				{
					((Controls.WindowsControls.CheckBox)lavatyyppiElementit.ElementAt(i)).IsChecked = 
						valittuKuvio.sallitutLavatyypit.Contains(_Konfiguraatio.Lavatyypit.ElementAt(i).Key);
				}
			}
			catch 
			{
				// Lavatyyppielementtejä ei ollut riittävästi
				Globals.Tags.HMI_Error_TextValue.SetAnalog(21);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = "lavatyyppiElementit";
				Globals.Popup_Error.Show();
			} 
			
			// Kuvion tietojen näyttö
			PatternNo_txt.Value = valittuKuvio.numero;
			
			if (Pattern_ListBox.SelectedItem != null)
			{
				// Ladataan kuvion tiedot
				Lavaus.Kuvio Kuvio = new Lavaus.Kuvio();
				Kuvio.Validoi = false;
				Kuvio.JSON = @"C:\Lavaus\Kuviot\" + "Kuvio" + valittuKuvio.numero + ".json";

				// Yritetään ladata tiedosto
				try
				{
					Kuvio.Lataa();
				}
				catch (Exception ex)
				{
					// Lataus epäonnistui
					Globals.Tags.HMI_Error_TextValue.SetAnalog(4);
					Globals.Tags.HMI_Error_AdditionalInfo. Value = ex.Message;
					Globals.Popup_Error.Show();
				}

				// Jos kuvio on olemassa päivitetään näyttö
				if (Kuvio.Nykyinen != null)
				{			
					Desc_Text.Text = Kuvio.Nykyinen.Description;
					Tool_Text.Text = Kuvio.Nykyinen.Tools[0].Name;
				}
				else
				{
					// Tyhjennetään näyttö
					Desc_Text.Text = "";
					Tool_Text.Text = "";
				}
			
				// Ladataan kuvion kuva
				m_Kuva_Kuvio.Visibility = System.Windows.Visibility.Hidden;
				Kuva_Kuvio.Image = null;
				Kuva_Kuvio.Refresh();
			
				// Ladataan kuvion kuva, jos on olemassa
				if (Kuvio.Nykyinen != null)
				if (Kuvio.Nykyinen.PalletizingImageFilename != null)
				{	
					string polku = @"C:\Lavaus\Kuvat\" + Kuvio.Nykyinen.PalletizingImageFilename;
					Globals.Tags.Log(polku);
					if(System.IO.File.Exists(polku))
					{
						Kuva_Kuvio.Image = Image.FromFile(polku);
						Kuva_Kuvio.SizeMode = PictureBoxSizeMode.Zoom;
						Kuva_Kuvio.Refresh();

						m_Kuva_Kuvio.Visibility = System.Windows.Visibility.Visible;
						Kuva_Kuvio.Dock = DockStyle.Fill;
					}
				}				
			}
		}
		
		/// <summary>
		/// Lopettaa HMI_PatternListEditor_AddPattern.ValueChange valvonnan.
		/// Koska eventtiin liitytään joka kerta sivun avautuessa, täytyy siitä myös irroittautua
		/// muistivuodon estämiseksi.
		/// </summary>
		/// <param name="sender">this</param>
		void Settings_PatternList_Editor_Closed(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.HMI_PatternListEditor_AddPattern.ValueChange -= HMI_PatternListEditor_AddPattern_ValueChanged;
		}
		
		/// <summary>
		/// Tallentaa kuvion JSON-tiedostoon perustuen näytön elementtien valintoihin.
		/// Valitut tuloradat/lavapaikat/lavatyypit tallennetaan listoina kuvionumeroon
		/// sidottuna. Kaikissa listoissa pitää olla valittuna vähintään yksi elementti,
		/// jotta tallennuksen annetaan mennä läpi. Tallentamisen jälkeen funktio pyytää
		/// kuviolistan päivitystä kutsumalla Paivita_Kuviot.
		/// </summary>
		/// <param name="sender">this.Save_btn</param>
		void Save_btn_Click(System.Object sender, System.EventArgs e)
		{
			if (valittuKuvio != new Kuviotiedot())
			{
				// Haetaan tulorataelementit			
				List<FrameworkElement> tulorataElementit = JarjestaLista(HaeElementit("Infeed"), 8);
								
				// Kerätään uusi tuloratojen lista
				List<int> uudetTuloradat = new List<int>();
				for (int i = 0; i < tuloradat.Count; i++)
				{
					// Lisätään tulorata sallittujen listaan, jos se on valittu
					if (((Controls.WindowsControls.CheckBox)tulorataElementit.ElementAt(i)).IsChecked.GetValueOrDefault())
					{
						uudetTuloradat.Add(tuloradat[i]);
					}
				}
			
				// Haetaan lavapaikkaelementit			
				List<FrameworkElement> lavapaikkaElementit = JarjestaLista(HaeElementit("PalletPlace"), 13);
								
				// Kerätään uusi lavapaikkojen lista
				List<int> uudetLavapaikat = new List<int>();
				for (int i = 0; i < lavapaikat.Count; i++)
				{
					// Lisätään lavapaikka sallittujen listaan, jos se on valittu
					if (((Controls.WindowsControls.CheckBox)lavapaikkaElementit.ElementAt(i)).IsChecked.GetValueOrDefault())
					{
						uudetLavapaikat.Add(lavapaikat[i]);
					}
				}
				
				// Haetaan lavatyyppielementit			
				List<FrameworkElement> lavatyyppiElementit = JarjestaLista(HaeElementit("PalletType"), 12);
								
				// Kerätään uusi lavatyyppien lista
				List<int> uudetLavatyypit = new List<int>();
				for (int i = 0; i < _Konfiguraatio.Lavatyypit.Keys.Count; i++)
				{
					// Lisätään lavatyyppi sallittujen listaan, jos se on valittu
					if (((Controls.WindowsControls.CheckBox)lavatyyppiElementit.ElementAt(i)).IsChecked.GetValueOrDefault())
					{
						uudetLavatyypit.Add(_Konfiguraatio.Lavatyypit.ElementAt(i).Key);
					}
				}
				
				// Tarkistetaan, että kaikkiin listoihin tuli vähintään yksi ruksi
				if (!uudetTuloradat.Any() || !uudetLavapaikat.Any() || !uudetLavatyypit.Any())
				{
					// Ei tallennetakaan ja herjataan!
					Globals.Tags.HMI_Error_TextValue.SetAnalog(11);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
					Globals.Popup_Error.Show();
					return;
				}

				// Korvataan kuvion tiedot listassa uusilla tiedoilla
				int indeksi = kuviot.IndexOf(valittuKuvio);
				if (indeksi != -1) // Kuvio löytyi
				{
					kuviot[indeksi].sallitutTuloradat = uudetTuloradat;
					kuviot[indeksi].sallitutLavapaikat = uudetLavapaikat;
					kuviot[indeksi].sallitutLavatyypit = uudetLavatyypit;
					valittuKuvio = kuviot[indeksi];
				}
				else // Juuri lisätty kuvionumero
				{
					valittuKuvio.sallitutTuloradat = uudetTuloradat;
					valittuKuvio.sallitutLavapaikat = uudetLavapaikat;
					valittuKuvio.sallitutLavatyypit = uudetLavatyypit;
					kuviot.Add(valittuKuvio);
				}
			
				// Tallennetaan JSON-tiedostoon
				Kuviotiedot.TallennaKuviot(kuviot);
				
				// Päivitetään kuviolista
				Paivita_Kuviot();
			}
		}
    }
}
