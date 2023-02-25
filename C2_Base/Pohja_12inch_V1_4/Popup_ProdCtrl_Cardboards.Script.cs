namespace Neo.ApplicationFramework.Generated
{
	using System;
	using System.Collections.Generic;
	using System.Windows;
	using System.Windows.Media;
    
	
	/// <summary>
	/// Näyttää ja antaa muokata ajossa olevan lavapaikan tai reseptin
	/// välikkeitä.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 7.7.2017</remarks>
	public partial class Popup_ProdCtrl_Cardboards
	{
		/// <summary>
		/// Elementtien tallennuspaikka.
		/// </summary>
		List<FrameworkElement> Elementit = new List<FrameworkElement>();
		/// <summary>
		/// Muokattavan lavapaikan numero tai -1 jos muokataan tuotereksiterissä.
		/// </summary>
		int lavapaikka = Globals.Tags.HMI_ProdCtrl_Cardboards_Lavapaikka.Value;
		/// <summary>
		/// Alkuperäiset pahvit. Tulevat joko tuoterekisteristä tai robotilta.
		/// </summary>
		string pahvit = Globals.Tags.HMI_ProdCtrl_Cardboards_Pahvit.Value;
		/// <summary>
		/// Lavan maksimikerrokset.
		/// </summary>
		int maxkerros = Globals.Tags.HMI_ProdCtrl_Cardboards_MaxKerros.Value;
		/// <summary>
		/// Lavan asetetut kerrokset.
		/// </summary>
		int kerrosasetus = Globals.Tags.HMI_ProdCtrl_Cardboards_Kerrosasetus.Value;
		/// <summary>
		/// Kerroksen muokkaussivu alustettuna.
		/// </summary>
		ProdReg_LayerEdit sivu;
		
		/// <summary>
		/// Alustaa ja päivittää sivun sen avautuessa.
		/// </summary>
		/// <param name="sender">this</param>
		void ProdCtrl_Cardboards_Opened(System.Object sender, System.EventArgs e)
		{
			// Haetaan kaikki kerros ja välipahvi elementit näytöltä            
			Elementit.Clear();
			HaeLapsi(this);
			
			// Päivitetään tilanne
			PaivitaValikkeet(true);	
		}

		/// <summary>
		/// Hakee visuaalisen elementin lapset, joiden nimessä on Kerros tai Pahvi, 
		/// ja lisää ne Elementit-listaan. funktio kutsuu rekursiivisesti itseään 
		/// elementtien todentamiseksi.
		/// </summary>
		/// <param name="nyk">Elementti, jonka lapset haetaan tai nykyinen tarkesteltava elementti</param>
		/// <returns>Kerros tai Pahvi child elementti</returns>
		FrameworkElement HaeLapsi(FrameworkElement nyk)
		{
			// Katsotaan onko tämä se elementti mistä ollaan kiinnostuneita
			if (nyk.Name.Contains("Kerros") || nyk.Name.Contains("Pahvi"))
			{
				// Otetaan talteen
				Elementit.Add(nyk);

				// On eli palautetaan tämä, ei mennä syvemmälle                
				return nyk;
			}

			// Haetaan lapsien lukumäärä
			var Lapsia = VisualTreeHelper.GetChildrenCount(nyk);

			// Käydään kaikki lapset läpi rekursiivisesti
			for(int i = 0; i < Lapsia; i++)
			{
				try
				{
					// Haetaan nykyisen elementin lapsi
					var elementti = (FrameworkElement)VisualTreeHelper.GetChild(nyk, i);
					// Jos on olemassa niin mennään tarkistamaan elementin lapsen lapset
					if (elementti != null)
					{
						HaeLapsi(elementti);
					}
				}
				catch(Exception)
				{

				}
			}
			// Kaikki lapset käyty, palautetaan null
			return null;
		}
		
		/// <summary>
		/// Avaa kerroksen muokkausikkunan, kun kerrosta painetaan.
		/// </summary>
		/// <param name="sender">this.KerrosX</param>
		void KerrosElementti_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{            
			try
			{
				// Luetaan kerroksen numero lähettäjästä
				string vertailu = "Kerros";
				var elementti = (FrameworkElement)sender;
				int index = elementti.Name.IndexOf(vertailu);
				int kerros = Convert.ToInt16(elementti.Name.Substring(index + vertailu.Length));

				// Tallennetaan kerroksen numero
				Globals.Tags.HMI_ProdReg_ValikeKerrosNro.Value = kerros;
                 
				// Tallennetaan kerroksen nykytila muokkauksen alkuarvoksi
				Globals.Tags.HMI_ProdReg_ValikeKerrosValikkeet.Value = LueKerroksenValikkeet(kerros, pahvit);
                
				// Avataan tietojen muokkaussivu
				sivu = new ProdReg_LayerEdit();
				sivu.Closed += sivu_Closed;
				sivu.Show();
			}
			catch(Exception ex)
			{
				Globals.Tags.HMI_Error_TextValue.SetAnalog(12);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
				Globals.Popup_Error.Show();
			}
		}

		/// <summary>
		/// Tarkistaa välikkeiden määrän ja tallentaa välipahvit tuoterekisteriin 
		/// tai lähettää ne robotille riippuen mistä ikkuna avattiin.
		/// </summary>
		/// <param name="sender">this.laheta_btn</param>
		/// <exception cref="ConfigurationFaultException">Robotin numeroa ei voitu päätellä lavapaikan numerosta.</exception>
		void Button_Laheta_Click(System.Object sender, System.EventArgs e)
		{
			// Tarkistetaan, ettei sanoma ala tai lopu erottimeen
			if(pahvit.EndsWith(";") || pahvit.EndsWith(","))
			{
				pahvit = pahvit.Substring(0, pahvit.Length - 1);
			}
			if (pahvit.StartsWith(";") || pahvit.StartsWith(","))
			{
				pahvit.Remove(0, 1);
			}

			// Tarkistetaan, että sanomassa on tarpeeksi kerroksia
			string[] valikkeet = pahvit.Split(';');
			// Varmistetaan että stringissä on jotain
			if (pahvit == "")
			{
				pahvit = "0";
			}
			// Välikkeitä pitää olla lavan kerrokset + aluskerros
			if (valikkeet.Length < maxkerros + 1)
			{
				for (int i = valikkeet.Length; i < maxkerros + 1; i++)
				{
					// Lisätään loppuun välikkeettömiä kerroksia kuvion maksimiin asti
					pahvit += ";0";
				}
			}
			
			if (Globals.Security.CurrentUser == "Administrator")
			{
				System.Windows.Forms.MessageBox.Show("Pahvistring: " + pahvit);
			}
			
			//Lähetä robotille, jos sivua ei avattu tuoterekisteristä
			if(lavapaikka > -1)
			{
				// Katsotaan kummalle robotille tulorata on
				int robottiNo = 0;
				int roboLavapaikka = 0;

				Globals._Konfiguraatio.CurrentConfig.GetRobotinLavapaikka(Globals.Tags.HMI_PalletPlace.Value, out robottiNo, out roboLavapaikka);
				
				if(robottiNo == 0)
				{
					// Robotin numeron parsinta epäonnistui
					MessageBox.Show("Robotin numeroa ei voitu löytää lavapaikan avulla:", "Lavapaikka " + lavapaikka);
				}
				else
				{
					// Lähetetään sanoma robotille
					Globals.Robotit.AsetaPahvit(robottiNo, roboLavapaikka, pahvit);
				}
			}
			else				
			{
				// Tarjotaan pahveja tuoterekisterille
				Globals.Tags.HMI_ProdCtrl_Cardboards_Pahvit.Value = pahvit;
			}
			
			// Suljetaan sivu
			this.Close();
		}
		
		/// <summary>
		/// Palauttaa yhden kerroksen välikkeet välikestringistä
		/// </summary>
		/// <param name="kerros">Luettavan kerroksen numero</param>
		/// <param name="pahvit">Koko välikemerkkijono</param>
		/// <returns>Kerroksen välikemerkkijono</returns>
		string LueKerroksenValikkeet(int kerros, string pahvit)
		{
			// Kerros 0 vastaa alusvälikkeitä, kerros 1 ... n tuotteiden väliin vietäviä
			string[] kerrokset = pahvit.Split(';');

			// Tarkistetaan, että kerroksia on tarpeeksi
			if (kerros < kerrokset.Length)
			{
				// Palautetaan kerroksen sisältö
				return kerrokset[kerros];
			}

			// Kerrosta ei ollutkaan olemassa, joten se on varmaan tyhjä
			return "0";
		}

		/// <summary>
		/// Tarkistaa pahvistringista onko valitussa kerroksessa välikkeitä vai ei
		/// </summary>
		/// <param name="kerros">Kerroksen numero</param>
		/// <param name="pahvit">Kerroksen välikkeiden merkkijono</param>
		/// <returns>True, jos kerroksessa on välikkeitä.</returns>
		bool OnkoKerroksessaValikkeita(int kerros, string pahvit)
		{
			// Kerros 0 vastaa alusvälikkeitä, kerros 1 ... n tuotteiden väliin vietäviä
			string[] kerrokset = pahvit.Split(';');

			// Tarkistetaan, että kerroksia on tarpeeksi
			if (kerros < kerrokset.Length)
			{
				// Luetaan kerroksen sisältö välikkeiksi
				string[] valikkeet = kerrokset[kerros].Split(',');

				foreach(string valike in valikkeet)
				{
					try
					{
						if(Convert.ToInt16(valike) > 0)
						{
							// On joku välike kerroksessa,
							return true;
						}
					}
					catch(Exception)
					{ 

					}
				}
			}

			// Ei löytynyt sopivaa kerrosta
			return false;
		}

		/// <summary>
		/// Päivittää ikkunan elementit.
		/// </summary>
		/// <param name="ekakerta">Ikkunan ensimmäinen päivityskerta.</param>
		void PaivitaValikkeet(bool ekakerta)
		{
			// Käydään kaikki sivun kiinnostavat kontrollit
			foreach (FrameworkElement elementti in Elementit)
			{
				if (elementti != null)
				{
					// Kerroslaatikot sen mukaan paljonko kuviossa on maksimikerrokset
					if (elementti.Name.Contains("Kerros") == true)
					{
						// Kun näyttö päivitetään ensimmäistä kertaa liitytään eventteihin
						if (ekakerta == true)
						{
							elementti.MouseDown += KerrosElementti_MouseDown;
						}

						// Luetaan kerrosnumero nimestä
						int kerros = 0;
						try
						{
							string vertailu = "Kerros";
							int index = elementti.Name.IndexOf(vertailu);
							string a = elementti.Name.Substring(index + vertailu.Length);
							kerros = Convert.ToInt16(elementti.Name.Substring(index + vertailu.Length));
						}
						catch (Exception)
						{

						}

						// Onko kerros mahdollinen tässä lavassa
						if (kerros > 0 && kerros <= maxkerros)
						{
							elementti.Visibility = System.Windows.Visibility.Visible;
							
							// Yli nykyisen kerrosasetuksen ovat kerrokset esitetään himmeänä
							if(kerros > kerrosasetus)
							{
								elementti.Opacity = 0.2;
							}
							else
							{
								elementti.Opacity = 1;
							}
						}
						else
						{
							elementti.Visibility = System.Windows.Visibility.Hidden;
						}
					}

					// Välipahvit sen mukaan missä väleissä on pahveja robotilla
					if (elementti.Name.Contains("Pahvi") == true)
					{
						// Luetaan kerrosnumero nimestä
						int kerros = 0;
						try
						{
							string vertailu = "Pahvi";
							int index = elementti.Name.IndexOf(vertailu);
							string a = elementti.Name.Substring(index + vertailu.Length);
							kerros = Convert.ToInt16(elementti.Name.Substring(index + vertailu.Length));

							// Tarkistetaan onko kerroksessa välikkeitä
							if (OnkoKerroksessaValikkeita(kerros, this.pahvit) == true)
							{
								elementti.Visibility = System.Windows.Visibility.Visible;
								
								// Yli nykyisen kerrosasetuksen ovat kerrokset esitetään himmeänä
								if(kerros > kerrosasetus)
								{
									elementti.Opacity = 0.5;
								}
								else
								{
									elementti.Opacity = 1;
								}
							}
							else
							{
								elementti.Visibility = System.Windows.Visibility.Hidden;
							}
						}
						catch (Exception)
						{

						}
					}
				}
			}

			// Lava näkyviin aina
			Kerros0.Visible = true;
		}

		/// <summary>
		/// Siivoaa kerroksen muokkaussivun ikkunan sulkeutuessa.
		/// </summary>
		/// <param name="sender">this</param>
		void ProdCtrl_Cardboards_Closing(System.Object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Irroittaudutaan eventistä ja tuhotaan muokkaussivu
			if (sivu != null)
			{
				sivu.Closed -= sivu_Closed;
				sivu.Dispose();
				sivu = null;
			}
		}

		/// <summary>
		/// Tallentaa muutokset kerroksen muokkaussivun sulkeuduttua.
		/// </summary>
		/// <param name="sender">this.sivu</param>
		void sivu_Closed(object sender, EventArgs e)
		{
			// Tallennetaan muokattu tieto
			TallennaValikkeetKerrokseen(Globals.Tags.HMI_ProdReg_ValikeKerrosNro.Value, Globals.Tags.HMI_ProdReg_ValikeKerrosValikkeet.Value);

			// Sivulle palattaessa päivitetään välikkeet
			PaivitaValikkeet(false);
		}

		/// <summary>
		/// Tallentaa yhden kerroksen tiedot välikemerkkijonoon.
		/// </summary>
		/// <param name="kerros">Tallennettavan kerroksen numero</param>
		/// <param name="valikkeet">Kerroksen välikemerkkijono</param>
		void TallennaValikkeetKerrokseen(int kerros, string valikkeet)
		{
			// Kerros 0 vastaa alusvälikkeitä, kerros 1 ... n tuotteiden väliin vietäviä
			List<string> kerrokset = new List<string>();
			kerrokset.InsertRange(0,  this.pahvit.Split(';'));
           
			// Tarkistetaan, että kerroksia on tarpeeksi
			if (kerros < kerrokset.Count)
			{
				// Tallennetaan
				kerrokset[kerros] = valikkeet;
			}
			else
			{
				// Pitää luoda kerroksia...
				for(int i = kerrokset.Count;i < kerros; i++)
				{
					kerrokset.Add("0");
				}
				// Lopuksi lisätään vielä se haluttukin data
				kerrokset.Add(valikkeet);
			}

			// Kasataan uusi pahvit stringi
			string _pahvit = string.Empty;
			foreach(string rivi in kerrokset)
			{
				// Siltä varalta, että jostain syystä tulisi tyhjiä kerroksia purusta..
				if (rivi.Length > 0)
				{
					_pahvit += rivi + ";";
				}
			}
            
			// Poistetaan viimeinen erotin ja tallennetaan
			this.pahvit = _pahvit.Substring(0, _pahvit.Length - 1);
		}
	}
}
