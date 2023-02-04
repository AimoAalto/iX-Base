namespace Neo.ApplicationFramework.Generated
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Forms;

	
	
	/// <summary>
	/// Mahdollistaa reseptien tarkastelun ja muokkauksen.
	/// Sivun sisältämät tiedot vaihtelevat projektin mukaan.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 27.7.2017</remarks>
	public partial class ProductRegister
	{
		/// <summary>
		/// Välikkeiden muokkaussivu alustettuna
		/// </summary>
		Popup_ProdCtrl_Cardboards sivu;
		
		/// <summary>
		/// Alustaa sivun tiedot ja lataa kuviolistan valittavaksi.
		/// </summary>
		/// <param name="sender">this</param>
		void ProductRegister_Opened(System.Object sender, System.EventArgs e)
		{

			// Nollataan reseptivalinta
			Globals.Tags.ProdReg_RecipeName.Value = "";
			
			maxkerros = 20; // Alustetaan kuvion maksimikerrokset, luetaan JSON-tiedostosta.

			// Ladataan kuvion tiedot aina kun kuvionumero muuttuu
			Globals.Tags.ProdReg_PalletPattern.ValueChange += LataaKuvio;
			
			// Luetaan mahdolliset kuvionumerot
			List<int> kuvioLista;
			
			// Ladataan kaikki kuviot JSON-tiedostosta
			try 
			{	        
				List<Kuviotiedot> kuviot = Kuviotiedot.LataaKuviot();
				kuvioLista = kuviot.OrderBy(p => p.numero).Select(p => p.numero).ToList();
			}
			catch
			{
				// Tiedostoa ei ole vielä, jatketaan tyhjällä listalla
				kuvioLista = new List<int>();
			}

			foreach (int kuvio in kuvioLista)
			{
				// Lisätään kuviolistaan
				KuvioComboBox.AddString(kuvio, kuvio.ToString());
			}
			
			// Sivun latautuessa ladataan näytölle käytössä oleva resepti
			if (Globals.Tags.ProdReg_RecipeName.Value == "")
			{
				// Estetään tallennusnapit
				Product_Save_As_btn.IsEnabled = false;
				Product_Save_btn.IsEnabled = false;
				// Mitään reseptiä ei ole ladattuna, ladataan jotain
				Globals.Tags.HMI_ProdReg_Dialog_Mode.Value = 1;
				Globals.Popup_Recipe_dialog.Show();
			}
			else
			{
				// Päivitetään kuvion tiedot jos uusi tuote onkin avattu aloituksessa
				LataaKuvio(null, null);
			}
		}

		/// <summary>
		/// Avaa välikkeiden muokkaussivun.
		/// </summary>
		/// <param name="sender">this.Btn_Valikkeet</param>
		void Btn_Valikkeet_Click(System.Object sender, System.EventArgs e)
		{
			// Kirjoitetaan parametrit ennen välike sivun avausta
			// Lavapaikka Globals.Tags.HMI_PalletPlace.Value -1 => Tuoterekisteri
			Globals.Tags.HMI_ProdCtrl_Cardboards_Lavapaikka.Value = -1;
			
			// Alkuperäiset pahvit ennen muokkausta
			Globals.Tags.HMI_ProdCtrl_Cardboards_Pahvit.Value = Globals.Tags.ProdReg_Spacers.Value;
			
			// Kuvion maksimikerrot ja kerroasetus
			Globals.Tags.HMI_ProdCtrl_Cardboards_MaxKerros.Value = maxkerros; // Kuviosta luettu enimmäiskerrosmäärä
			Globals.Tags.HMI_ProdCtrl_Cardboards_Kerrosasetus.Value = Globals.Tags.ProdReg_LayerCount.Value;

			// Avataan välikkeideiden muokkaussivu ja liityttään sen sulkeutumiseventtiin
			sivu = new Popup_ProdCtrl_Cardboards();
			sivu.Closed += ValikeSivu_Closed;
			sivu.Show();
		}

		/// <summary>
		/// Lataa kuvion tiedot näytölle, kun tuoterekisteriin valittu 
		/// kuvionumero muuttuu.
		/// </summary>
		/// <param name="sender">ProdReg_PalletPattern</param>
		void LataaKuvio(System.Object sender, System.EventArgs e)
		{
			if (Globals.Tags.HMI_ProdReg_RecipeSelected.Value != "")
			{
				// Sallitaan tallennusnapit
				Product_Save_As_btn.IsEnabled = true;
				Product_Save_btn.IsEnabled = true;
			}
			else
			{
				// Estetään tallennusnapit
				Product_Save_As_btn.IsEnabled = false;
				Product_Save_btn.IsEnabled = false;
			}
			
			// Päivitä kuvion valitsin jos resepti ladattiin
			if (KuvioComboBox.Items.Contains(Globals.Tags.ProdReg_PalletPattern.Value.ToString()))
			{
				KuvioComboBox.SelectedIndex = Globals.Tags.ProdReg_PalletPattern.Value;
			}
			
			// Haetaan kuvion tiedot JSON-tiedostosta
			Kuviotiedot kuvio = Kuviotiedot.LataaKuviot()
				.Where(p => p.numero == Globals.Tags.ProdReg_PalletPattern.Value.Int).SingleOrDefault();
			
			// Luetaan, mille tuloradalle kuvio on
			Tulorata_Text.Text = "";
			bool loytyi = false;
			foreach (int tulorata in kuvio.sallitutTuloradat)
			{
				if (loytyi)
				{
					Tulorata_Text.Text = Tulorata_Text.Text + ", ";
				}
				Tulorata_Text.Text = Tulorata_Text.Text + tulorata.ToString();
				loytyi = true;
			}
			
			// Luetaan, mille lavapaikalle kuvio on
			List<int> lavapaikat = new List<int>();
			Lavapaikka_Text.Text = "";
			loytyi = false;
			foreach (int lavapaikka in kuvio.sallitutLavapaikat)
			{
				if (loytyi)
				{
					Lavapaikka_Text.Text = Lavapaikka_Text.Text + ", ";
				}
				lavapaikat.Add(lavapaikka);
				Lavapaikka_Text.Text = Lavapaikka_Text.Text + lavapaikka.ToString();
				loytyi = true;
			}
			
			// Katsotaan kummalle robotille kuvio on
			Robotti_Text.Text = "";
			loytyi = false;
			foreach (KeyValuePair<int, Dictionary<int, int>> robotti in _Konfiguraatio.RobotinLavapaikat)
			{
				foreach (int lavapaikka in lavapaikat)
				{
					if (robotti.Value.ContainsKey(lavapaikka))
					{
						if (loytyi)
						{
							Robotti_Text.Text = Robotti_Text.Text + ", ";
						}
						Robotti_Text.Text = Robotti_Text.Text + robotti.Key.ToString();
						loytyi = true;
						break;
					}
				}
			}
			
			// Päivitetään lavatyypit
			Neo.ApplicationFramework.Controls.WindowsControls.ComboBox boxi = (Neo.ApplicationFramework.Controls.WindowsControls.ComboBox)PalletTypeComboBox.AdaptedObject;
			boxi.IntervalMapper.Intervals.Clear();
			foreach (int tyyppi in kuvio.sallitutLavatyypit)
			{
				PalletTypeComboBox.AddString(tyyppi, _Konfiguraatio.Lavatyypit[tyyppi]);
			}
			if (Globals.Tags.ProdReg_PalletType.Value.Int != 0)
			{
				if (PalletTypeComboBox.Items.Contains(_Konfiguraatio.Lavatyypit[Globals.Tags.ProdReg_PalletType.Value.Int]))
				{
					PalletTypeComboBox.SelectedIndex = Globals.Tags.ProdReg_PalletType.Value.Int;
				}
			}
			// Ladataan kuvion tiedot
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
				Globals.Tags.HMI_Error_TextValue.SetAnalog(4);
				Globals.Tags.HMI_Error_AdditionalInfo. Value = ex.Message;
				Globals.Popup_Error.Show();
			}

			// Jos kuvio on olemassa päivitetään näyttö
			if (Kuvio.Nykyinen != null)
			{			
				Desc_Text.Text = Kuvio.Nykyinen.Description;
				//Text19.Text = Kuvio.Nykyinen.PatternName;
				Tool_Text.Text = Kuvio.Nykyinen.Tools[0].Name;
                
				// Luetaan kuviosta kerrosmäärä
				maxkerros = Kuvio.Nykyinen.Layers;
			}
			else
			{
				// Tyhjennetään näyttö
				Desc_Text.Text = "";
				Tool_Text.Text = "";
			}
			
			// Ladataan kuvion kuva
			try 
			{	  
				m_Kuva_Kuvio.Visibility = System.Windows.Visibility.Hidden;
				Kuva_Kuvio.Image = null;
				Kuva_Kuvio.Refresh();
				
				// Ladataan kuvion kuva, jos on olemassa
				if (Kuvio.Nykyinen.PalletizingImageFilename != null)
				{	
					string polku = @"C:\Lavaus\Kuvat\" + Kuvio.Nykyinen.PalletizingImageFilename;
					if(System.IO.File.Exists(polku))
					{
						Kuva_Kuvio.Image = System.Drawing.Image.FromFile(polku);
						Kuva_Kuvio.SizeMode = PictureBoxSizeMode.Zoom;
						Kuva_Kuvio.Refresh();

						m_Kuva_Kuvio.Visibility = System.Windows.Visibility.Visible;
						Kuva_Kuvio.Dock = DockStyle.Fill;
						//Kuva_Kuvio.BackColor = Color.Transparent;
						
					}
				}
			}
			catch (Exception ex)
			{
				// Kuvion kuvan lataaminen epäonnistui
				Globals.Tags.HMI_Error_TextValue.SetAnalog(5);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
				Globals.Popup_Error.Show();
				return;
			}
		}
		
		/// <summary>
		/// Avaa poista resepti -dialogin.
		/// </summary>
		/// <param name="sender">this.Product_Delete_Btn</param>
		void Product_Delete_Btn_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.HMI_ProdReg_Dialog_Mode.Value = 3;
		}

		/// <summary>
		/// Avaa Avaa Resepti-dialogin
		/// </summary>
		/// <param name="sender">this.Product_Open_btn2</param>
		void Product_Open_btn_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.HMI_ProdReg_Dialog_Mode.Value = 1;
		}
		
		/// <summary>
		/// Avaa tallenna nimellä -dialogin.
		/// </summary>
		/// <param name="sender">this.Product_Save_btn2</param>
		void Product_Save_As_btn_Click(System.Object sender, System.EventArgs e)
		{
			// Painettaessa tallenna avataan ikkuna vain jos jokin resepti on oikeasti näytöllä
			// Näin estetään, ettei tyhjää näyttöä (kun mitään ei ole ladattu käyttöliittymän käynnistyksen jälkeen) voida tallentaa
			// olemassa olevan reseptin päälle
			if (Globals.Tags.HMI_ProdReg_RecipeSelected.Value.ToString().Length > 0)
			{
				// Tarkistetaan, että lavatyyppi on valittuna kuvion vaihdon jälkeen
				if (PalletTypeComboBox.SelectedItem != null)
				{
					Globals.Tags.HMI_ProdReg_Dialog_Mode.Value = 2;
					Globals.Popup_Recipe_dialog.Show();
				}
				else
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog(24);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
					Globals.Popup_Error.Show();
				}
			}            
		}
		
		/// <summary>
		/// Tallentaa reseptin.
		/// </summary>
		/// <param name="sender">this.Product_Save_btn3</param>
		void Product_Save_btn_Click(System.Object sender, System.EventArgs e)
		{
			if (Globals.Tags.ProdReg_RecipeName.Value.ToString().Length > 0)
			{
				// Tarkistetaan, että lavatyyppi on valittuna kuvion vaihdon jälkeen
				if (PalletTypeComboBox.SelectedItem != null)
				{
					Globals.Tuotetiedot.SaveRecipe(Globals.Tags.ProdReg_RecipeName.Value, true);
				}
				else
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog(24);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
					Globals.Popup_Error.Show();
				}
			}
		}
		
		/// <summary>
		/// Irroittautuu kuvionumeron muutoksen seurannasta, kun sivu sulkeutuu.
		/// </summary>
		/// <param name="sender">this</param>
		void ProductRegister_Closed(System.Object sender, System.EventArgs e)
		{
			// Poistetaan kuvion muutoksen seuranta
			Globals.Tags.ProdReg_PalletPattern.ValueChange -= LataaKuvio;
		}

		/// <summary>
		/// Tallentaa muokatut välikkeet välikesivun sulkeuduttua.
		/// </summary>
		/// <param name="sender">this.sivu</param>
		void ValikeSivu_Closed(object sender, EventArgs e)
		{
			// Tallennetaan uudet välikkeet aktiiviselle tuotteelle
			Globals.Tags.ProdReg_Spacers.Value = Globals.Tags.HMI_ProdCtrl_Cardboards_Pahvit.Value;

			sivu.Closed -= ValikeSivu_Closed;
			sivu = null;
		}
		
	}	
}
