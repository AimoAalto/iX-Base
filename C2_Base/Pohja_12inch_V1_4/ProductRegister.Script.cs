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

		bool PossibleCombination(List<int> lst)
		{
			foreach (int pno in lst)
			{
				foreach (int tr in Globals._Konfiguraatio.CurrentConfig.AllowedPatterns[pno].Tuloradat)
				{
					foreach (int item in lst)
					{
						if (item != pno)
						{
							if (Globals._Konfiguraatio.CurrentConfig.AllowedPatterns[item].Tuloradat.Contains(tr)) return false;
						}
					}
				}
			}
			return true;
		}

		void CBPatternList_Click(System.Object sender, System.EventArgs e)
		{
			try
			{
				string bck = Globals.Tags.ProdReg_InfeedTrackPattern.Value;
				bool checkcombination = ((System.Windows.Controls.CheckBox)sender).IsChecked == true;
				List<int> lst = new List<int>();
				bool next = false;
				string s = "";

				foreach (System.Windows.Controls.StackPanel o in LBPatterns.Items)
				{
					if (o.Children.Count > 0)
					{
						System.Windows.Controls.CheckBox cb = (System.Windows.Controls.CheckBox)o.Children[0];
						if (cb.IsChecked == true)
						{
							int tag = (int)cb.Tag;
							lst.Add(tag);
							if (next)
								s += ",";
							else
								next = true;
							s += tag.ToString();
						}
					}
				}

				if (checkcombination)
				{
					if (!PossibleCombination(lst))
					{
						((System.Windows.Controls.CheckBox)sender).IsChecked = false;
						s = bck;
					}
				}

				Globals.Tags.ProdReg_InfeedTrackPattern.SetString(s);
			}
			catch (Exception x)
			{
				System.Windows.Forms.MessageBox.Show(x.Message);
			}
		}

		void InitPatternListBox()
		{
			LBPatterns.Items.Clear();
			foreach (int pno in Globals._Konfiguraatio.CurrentConfig.AllowedPatterns.Keys)
			{
				System.Windows.Controls.StackPanel parent = new System.Windows.Controls.StackPanel();
				parent.Orientation = System.Windows.Controls.Orientation.Horizontal;
				parent.Tag = pno;
				// Lisätään kuviolistaan
				System.Windows.Controls.CheckBox cb = CreateCheckBox(string.Format("{0}  ", pno), pno, CBPatternList_Click);
				parent.Children.Add(cb);

				foreach (int tr in Globals._Konfiguraatio.CurrentConfig.Tuloradat.Keys)
				{
					System.Windows.Controls.CheckBox cbtr = CreateCheckBox(string.Format("{0}  ", tr), tr, null);
					cbtr.IsChecked = Globals._Konfiguraatio.CurrentConfig.IsAllowedPatternInfeedTrack(pno, tr);
					//cbtr.IsHitTestVisible = false;
					cbtr.IsEnabled = false;
					parent.Children.Add(cbtr);
				}
				LBPatterns.Items.Add(parent);
			}
		}

		System.Windows.Controls.CheckBox CreateCheckBox(string title, object tag, System.Windows.RoutedEventHandler clicked
			/*, bool ischecked*/)
		{
			System.Windows.Controls.CheckBox cb = new System.Windows.Controls.CheckBox();
			cb.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
			cb.Content = title;
			cb.Tag = tag;
			//cb.IsChecked = ischecked;
			if (clicked != null) cb.Click += clicked;
			return cb;
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

		void UpdatePatternSelection()
		{
			int pno = Globals.Tags.ProdReg_PalletPattern.Value;
			List<int> lst = new List<int>();
			string s = Globals.Tags.ProdReg_InfeedTrackPattern.Value;
			string[] values = s.Split(',');
			if (values.Length > 0)
			{
				int no = 0;
				foreach (string item in values)
					if (int.TryParse(item, out no)) lst.Add(no);
			}

			int index = 0;
			foreach (System.Windows.Controls.StackPanel o in LBPatterns.Items)
			{
				int ptag = (int)o.Tag;

				if (pno == ptag)
					LBPatterns.SelectedIndex = index;

				if (o.Children.Count > 0)
				{
					System.Windows.Controls.CheckBox cb = (System.Windows.Controls.CheckBox)o.Children[0];
					int tag = (int)cb.Tag;
					cb.IsChecked = lst.Contains(tag);
				}
				index++;
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
			Lavaus.Kuvio Kuvio = new Lavaus.Kuvio();
			Kuvio.Validoi = false;
			Kuvio.JSON = string.Format("{0}Kuvio{1}.json", _Konfiguraatio.PatternDirectory, patternno);

			// Tyhjennetään näyttö
			Desc_Text.Text = "";
			Tool_Text.Text = "";
			ANPalletLength.Text = "";
			ANPalletWidth.Text = "";

			// Yritetään ladata tiedosto
			try
			{
				Kuva_Kuvio.Visible = false;

				Kuvio.Lataa();

				// Jos kuvio on olemassa päivitetään näyttö
				if (Kuvio.Nykyinen != null)
				{
					Desc_Text.Text = Kuvio.Nykyinen.Description;
					//Text19.Text = Kuvio.Nykyinen.PatternName;
					if (Kuvio.Nykyinen.Tools.Count > 0)
						Tool_Text.Text = Kuvio.Nykyinen.Tools[0].Name;

					// Luetaan kuviosta kerrosmäärä
					maxkerros = Kuvio.Nykyinen.Layers;

					if (Kuvio.Nykyinen.PalletTypes.Count > 0)
					{
						ANPalletLength.Text = string.Format("{0} mm", Kuvio.Nykyinen.PalletTypes[0].Length);
						ANPalletWidth.Text = string.Format("{0} mm", Kuvio.Nykyinen.PalletTypes[0].Width);
					}

					LoadPicture(Kuvio.Nykyinen.PalletizingImageFilename);
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

		void LBPatterns_SelectionChanged(System.Object sender, System.EventArgs e)
		{
			if (LBPatterns.SelectedIndex >= 0)
			{
				System.Windows.Controls.StackPanel lbitem = (System.Windows.Controls.StackPanel)LBPatterns.SelectedItem;
				Globals.Tags.ProdReg_PalletPattern.SetAnalog((int)lbitem.Tag);
			}
		}

		void ANProductNo_ValueChanged(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine(string.Format("Patterns str {0}", Globals.Tags.ProdReg_InfeedTrackPattern.Value));
			UpdatePatternSelection();
		}
	}
}
