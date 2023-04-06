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

			InitPatternListBox();

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

		void InitPatternListBox()
		{
			List<int> lst = new List<int>();
			foreach (int pno in Globals._Konfiguraatio.CurrentConfig.AllowedPatterns.Keys) lst.Add(pno);
			lst.Sort();
			foreach (int pno in lst) KuvioComboBox.AddString(pno, pno.ToString());
			lst.Clear();
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
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.Tags.ErrorTexts.ChoosePalletType);
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
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.Tags.ErrorTexts.ChoosePalletType);
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

		#region pattern

		string MakeString(List<int> lst)
		{
			string s = "";
			lst.Sort();
			bool next = false;
			foreach (int item in lst)
			{
				if (next)
					s = s + ", ";
				else
					next = true;
				s = s + item.ToString();
			}
			return s;
		}

		void UpdatePatternInfeedTracks(PatternInfo pi)
		{
			List<int> lst = new List<int>();
			// Luetaan, mille tuloradalle kuvio on
			foreach (int tulorata in pi.Tuloradat) lst.Add(tulorata);
			Tulorata_Text.Text = MakeString(lst);
		}

		void UpdatePatternPalletPlaces(PatternInfo pi)
		{
			List<int> lst = new List<int>();
			// Luetaan, mille lavapaikalle kuvio on
			foreach (int lavapaikka in pi.Lavapaikat) lst.Add(lavapaikka);
			Lavapaikka_Text.Text = MakeString(lst);
		}

		void UpdatePatternRobots(PatternInfo pi)
		{
			List<int> lst = new List<int>();
			// Katsotaan kummalle robotille kuvio on
			foreach (KeyValuePair<int, RobotConf> r in Globals._Konfiguraatio.CurrentConfig.Robots)
			{
				RobotConf robot = r.Value;
				foreach (int lavapaikka in pi.Lavapaikat)
				{
					if (robot.Lavapaikat.Contains(lavapaikka))
					{
						lst.Add(robot.RobotNo);
						break;
					}
				}
			}
			Robotti_Text.Text = MakeString(lst);
		}

		void UpdatePatternPallettypes(PatternInfo pi)
		{
			//Neo.ApplicationFramework.Controls.WindowsControls.ComboBox boxi = (Neo.ApplicationFramework.Controls.WindowsControls.ComboBox)PalletTypeComboBox.AdaptedObject;
			//boxi.IntervalMapper.Intervals.Clear();

			// Päivitetään lavatyypit
			bool first = true;
			int index = 0;
			foreach (int tyyppi in pi.Lavatyypit)
			{
				PalletTypeComboBox.AddString(tyyppi, Globals._Konfiguraatio.CurrentConfig.Lavatyypit[tyyppi]);
				if (first)
				{
					int pt = (int)Globals.Tags.ProdReg_PalletType.Value;
					if (Globals._Konfiguraatio.CurrentConfig.Lavatyypit.ContainsKey(pt))
					{
						PalletTypeComboBox.SelectedIndex = index;
						first = false;
					}
				}
				index++;
			}
		}

		void LoadPicture(string fname)
		{
			// Ladataan kuvion kuva
			try
			{
				Kuva_Kuvio.Image = null;
				Kuva_Kuvio.Refresh();

				// Ladataan kuvion kuva, jos on olemassa
				if (!string.IsNullOrEmpty(fname))
				{
					string name = string.Format("{0}{1}", _Konfiguraatio.PictureDirectory, fname);
					if (System.IO.File.Exists(name))
					{
						Kuva_Kuvio.Image = System.Drawing.Image.FromFile(name);
						Kuva_Kuvio.SizeMode = PictureBoxSizeMode.Zoom;
						Kuva_Kuvio.Refresh();

						Kuva_Kuvio.Visible = true; // System.Windows.Visibility.Visible;
						Kuva_Kuvio.Dock = DockStyle.Fill;
						//Kuva_Kuvio.BackColor = Color.Transparent;
					}
				}
			}
			catch (Exception ex)
			{
				// Kuvion kuvan lataaminen epäonnistui
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.Tags.ErrorTexts.PatternImageLoadFailed);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
				Globals.Popup_Error.Show();
				return;
			}
		}

		#endregion

		/// <summary>
		/// Lataa kuvion tiedot näytölle, kun tuoterekisteriin valittu 
		/// kuvionumero muuttuu.
		/// </summary>
		void LataaKuvio(System.Object sender, System.EventArgs e)
		{
			bool selected = string.IsNullOrEmpty(Globals.Tags.HMI_ProdReg_RecipeSelected.Value) == false;
			// Sallitaan/Estetään tallennusnapit
			Product_Save_As_btn.IsEnabled = selected;
			Product_Save_btn.IsEnabled = selected;

			int patternno = Globals.Tags.ProdReg_PalletPattern.Value.Int;

			System.Diagnostics.Trace.WriteLine(string.Format("Load pattern {0}", patternno));

			// Haetaan kuvion tiedot JSON-tiedostosta
			if (!Globals._Konfiguraatio.CurrentConfig.AllowedPatterns.ContainsKey(patternno))
			{
				// tuntematon kuvio
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.Tags.ErrorTexts.NotAllowedPattern);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = patternno.ToString();
				Globals.Popup_Error.Show();
				return;
			}

			PatternInfo pi = Globals._Konfiguraatio.CurrentConfig.AllowedPatterns[patternno];

			UpdatePatternInfeedTracks(pi);
			UpdatePatternPalletPlaces(pi);
			UpdatePatternRobots(pi);
			UpdatePatternPallettypes(pi);

			// Ladataan kuvion tiedot
			Lavaus.Kuvio kuvio = new Lavaus.Kuvio();
			kuvio.Validoi = false;
			kuvio.JSON = string.Format("{0}Kuvio{1}.json", _Konfiguraatio.PatternDirectory, patternno);

			// Tyhjennetään näyttö
			Desc_Text.Text = "";
			Tool_Text.Text = "";
			Boxes_Text.Text = "";
			ANPalletLength.Text = "";
			ANPalletWidth.Text = "";

			// Yritetään ladata tiedosto
			try
			{
				Kuva_Kuvio.Visible = false;

				kuvio.Lataa();

				// Jos kuvio on olemassa päivitetään näyttö
				if (kuvio.Nykyinen != null)
				{
					Desc_Text.Text = kuvio.Nykyinen.Description;
					//Text19.Text = kuvio.Nykyinen.PatternName;
					if (kuvio.Nykyinen.Tools.Count > 0)
						Tool_Text.Text = kuvio.Nykyinen.Tools[0].Name;

					// Luetaan kuviosta kerrosmäärä
					maxkerros = kuvio.Nykyinen.Layers;

					if (kuvio.Nykyinen.PalletTypes.Count > 0)
					{
						ANPalletLength.Text = string.Format("{0} mm", kuvio.Nykyinen.PalletTypes[0].Length);
						ANPalletWidth.Text = string.Format("{0} mm", kuvio.Nykyinen.PalletTypes[0].Width);
					}
					
					string s = "";
					for (int index = 0; index < kuvio.Nykyinen.ProductTypes.Count; index++)
					{
						if (index > 0) s += "\n";
						s += string.Format("[{0}] {1}", kuvio.Nykyinen.ProductTypes[index].RobotID, kuvio.Nykyinen.ProductTypes[index].Name);
					}
					Boxes_Text.Text = s;

					LoadPicture(kuvio.Nykyinen.PalletizingImageFilename);
				}
			}
			catch (Exception ex)
			{
				// Lataus epäonnistui
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.Tags.ErrorTexts.PatternLoadFailed);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
				Globals.Popup_Error.Show();
				return;
			}
		}
	}
}
