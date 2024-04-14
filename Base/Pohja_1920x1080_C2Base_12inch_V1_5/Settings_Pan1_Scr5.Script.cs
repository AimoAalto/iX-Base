namespace Neo.ApplicationFramework.Generated
{
	using System;
	using System.Collections.Generic;
	using System.Windows;
	using System.Drawing;
	using System.Linq;
	using System.Windows.Media;

	/// <summary>
	/// Mahdollistaa sallittujen kuvioiden ja niille sallittujen 
	/// tuloratojen/lavapaikkojen/lavatyyppien määritelyn.
	/// Kuviojen tietoja säilytetään 'muuttujissa' _konfikuraatio.CurrentConfig.AllowedPattern 
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 2.8.2017</remarks>
	public partial class Settings_Pan1_Scr5
	{
		int pno = 0;

		void Settings_PatternList_Editor_Opened(System.Object sender, System.EventArgs e)
		{
			UpdateSettingsFields();
		}

		#region pattern

		/// <summary>
		/// Update settings listboxes and other fields
		/// </summary>
		/// <returns></returns>
		void UpdateSettingsFields()
		{
			UpdateAllowedPatterns();

			UpdatePatternAllowedPlaces(pno);

			if (LBPatternNo.Items.Count > 0)
				LBPatternNo.SelectedIndex = 0;
		}

		/// <summary>
		/// update allowed patterns listbox
		/// </summary>
		/// <returns></returns>
		void UpdateAllowedPatterns()
		{
			LBPatternNo.Items.Clear();
			pno = 0;
			List<int> lst = Globals._Konfiguraatio.CurrentConfig.AllowedPatternNumbers();
			foreach (int no in lst)
			{
				if (pno == 0) pno = no;
				LBPatternNo.Items.Add(no);
			}
		}

		void ClearListBoxItems(Neo.ApplicationFramework.Controls.Script.ListBoxAdapter l, System.Windows.RoutedEventHandler clicked)
		{
			foreach (object item in l.Items)
			{
				if (item is System.Windows.Controls.CheckBox)
				{
					((System.Windows.Controls.CheckBox)item).Click -= clicked;
				}
			}
			l.Items.Clear();
		}

		System.Windows.Controls.CheckBox CreateCheckBox(string title, object tag, bool ischecked, System.Windows.RoutedEventHandler clicked)
		{
			System.Windows.Controls.CheckBox cb = new System.Windows.Controls.CheckBox();
			cb.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
			cb.Content = title;
			cb.Tag = tag;
			cb.IsChecked = ischecked;
			cb.Click += clicked;
			return cb;
		}

		/// <summary>
		/// Update listboxes for allowed infeed tracks and pallet places
		/// </summary>
		/// <param name="rno"></param>
		/// <returns></returns>
		void UpdatePatternAllowedPlaces(int pno)
		{
			ClearListBoxItems(LBPatternAllowedIT, CBPatternInfeedTrack_Click);
			foreach (KeyValuePair<int, int> item in Globals._Konfiguraatio.CurrentConfig.Tuloradat)
			{
				string s = string.Format("[{0}] {1}", item.Key, item.Value);
				System.Windows.Controls.CheckBox cb = CreateCheckBox(s, item.Key,
					Globals._Konfiguraatio.CurrentConfig.IsAllowedPatternInfeedTrack(pno, item.Key), CBPatternInfeedTrack_Click);
				LBPatternAllowedIT.Items.Add(cb);
			}

			ClearListBoxItems(LBPatternAllowedPP, CBPatternPalletPlace_Click);
			foreach (KeyValuePair<int, int> item in Globals._Konfiguraatio.CurrentConfig.Lavapaikat)
			{
				string s = string.Format("[{0}] {1}", item.Key, item.Value);
				System.Windows.Controls.CheckBox cb = CreateCheckBox(s, item.Key,
					Globals._Konfiguraatio.CurrentConfig.IsAllowedPatternPalletPlace(pno, item.Key), CBPatternPalletPlace_Click);
				LBPatternAllowedPP.Items.Add(cb);
			}

			ClearListBoxItems(LBPatternAllowedPT, CBPatternPalletType_Click);
			foreach (KeyValuePair<int, string> item in Globals._Konfiguraatio.CurrentConfig.Lavatyypit)
			{
				string s = string.Format("[{0}] {1}", item.Key, item.Value);
				System.Windows.Controls.CheckBox cb = CreateCheckBox(s, item.Key,
					Globals._Konfiguraatio.CurrentConfig.IsAllowedPatternPalletType(pno, item.Key), CBPatternPalletType_Click);
				LBPatternAllowedPT.Items.Add(cb);
			}
		}

		bool LoadPattern(int pattern_no)
		{
			bool ret = false;

			// Ladataan kuvion tiedot
			Lavaus.Kuvio kuvio = new Lavaus.Kuvio();
			kuvio.Validoi = false;
			kuvio.JSON = string.Format(@"{0}Kuvio{1}.json",
				Neo.ApplicationFramework.Generated._Konfiguraatio.PatternDirectory, pattern_no);

			Desc_Text.Text = "";
			Tool_Text.Text = "";
			Boxes_Text.Text = "";

			// Yritetään ladata tiedosto
			if (System.IO.File.Exists(kuvio.JSON))
			{
				try
				{
					kuvio.Lataa();
					if (kuvio.Nykyinen != null)
					{
						Desc_Text.Text = kuvio.Nykyinen.Description;

						if (kuvio.Nykyinen.Tools.Count > 0)
							Tool_Text.Text = kuvio.Nykyinen.Tools[0].Name;

						string s = "";
						for (int index = 0; index < kuvio.Nykyinen.ProductTypes.Count; index++)
						{
							if (index > 0) s += "\n";
							s += string.Format("[{0}] {1}", kuvio.Nykyinen.ProductTypes[index].RobotID, kuvio.Nykyinen.ProductTypes[index].Name);
						}
						Boxes_Text.Text = s;

						LoadPatternPicture(kuvio.Nykyinen.PalletizingImageFilename);

						ret = true;
					}
				}
				catch (Exception ex)
				{
					// Lataus epäonnistui
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.PatternLoadFailed);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
					Globals.Popup_Error.Show();
				}
			}
			else
			{
				// Lataus epäonnistui
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.NoPatternFile);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = kuvio.JSON;
				Globals.Popup_Error.Show();
			}

			return ret;
		}

		bool LoadPatternPicture(string fname)
		{
			bool ret = false;

			// Ladataan kuvion kuva
			Kuva_Kuvio.Visible = false; //System.Windows.Visibility.Hidden;
			Kuva_Kuvio.Image = null;
			Kuva_Kuvio.Refresh();

			if (!string.IsNullOrEmpty(fname))
			{
				string name = string.Format("{0}{1}", _Konfiguraatio.PictureDirectory, fname);
				if (System.IO.File.Exists(name))
				{
					Kuva_Kuvio.Image = Image.FromFile(name);
					Kuva_Kuvio.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
					Kuva_Kuvio.Refresh();

					Kuva_Kuvio.Visible = true;
					Kuva_Kuvio.Dock = System.Windows.Forms.DockStyle.Fill;

					ret = true;
				}
				else
				{
					// Lataus epäonnistui
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.NoImageFile);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = fname;
					Globals.Popup_Error.Show();
				}
			}

			return ret;
		}

		#endregion

		#region Allowed Pattern mouse/keyboard events

		void BtnPatternAdd_Click(System.Object sender, System.EventArgs e)
		{
			try
			{
				int no = Convert.ToInt16(ANNewPatternNo.Value);
				if (Globals._Konfiguraatio.CurrentConfig.AllowedPatterns.ContainsKey(no))
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.PatternAlreadyExist);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
					Globals.Popup_Error.Show();
				}
				else
				{
					Globals._Konfiguraatio.CurrentConfig.AddPattern(no);
					// UpdateSettingsJson();
					UpdateAllowedPatterns();
					pno = no;
					UpdatePatternAllowedPlaces(no);
				}
			}
			catch (Exception x)
			{
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
				Globals.Popup_Error.Show();
			}
		}

		void BtnPatternDelete_Click(System.Object sender, System.EventArgs e)
		{
			try
			{
				int no = Convert.ToInt16(ANNewPatternNo.Value);
				if (Globals._Konfiguraatio.CurrentConfig.AllowedPatterns.ContainsKey(no))
				{
					if (System.Windows.Forms.MessageBox.Show(
						string.Format("Haluatko poistaa Kuvion [{0}]", no),
						"Kuvion poisto",
						System.Windows.Forms.MessageBoxButtons.YesNo,
						System.Windows.Forms.MessageBoxIcon.Question,
						System.Windows.Forms.MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
					{
						Globals._Konfiguraatio.CurrentConfig.RemovePattern(no);
						// UpdateSettingsJson();
						UpdateAllowedPatterns();
						UpdatePatternAllowedPlaces(no);
					}
				}
				else
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnknownRobotId);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
					Globals.Popup_Error.Show();
				}
			}
			catch (Exception x)
			{
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
				Globals.Popup_Error.Show();
			}
		}

		void LBPatternNo_SelectionChanged(System.Object sender, System.EventArgs e)
		{
			try
			{
				if (LBPatternNo.SelectedIndex == -1)
				{
					ANPatternNo.Text = "";
					pno = 0;
				}
				else
				{
					ANPatternNo.Text = LBPatternNo.SelectedItem.ToString();
					pno = (int)LBPatternNo.SelectedItem;
					UpdatePatternAllowedPlaces(pno);

					LoadPattern(pno);
				}
			}
			catch (Exception x)
			{
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
				Globals.Popup_Error.Show();
			}
		}

		void CBPatternPalletPlace_Click(System.Object sender, System.EventArgs e)
		{
			if (pno >= 0)
				try
				{
					if (sender is System.Windows.Controls.CheckBox)
					{
						int tag = (int)((System.Windows.Controls.CheckBox)sender).Tag;
						if (((System.Windows.Controls.CheckBox)sender).IsChecked == true)
						{
							Globals._Konfiguraatio.CurrentConfig.AddPatternPalletPlace(pno, tag);
						}
						else
						{
							Globals._Konfiguraatio.CurrentConfig.RemovePatternPalletPlace(pno, tag);
						}
						// UpdateSettingsJson();
					}
				}
				catch (Exception x)
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
					Globals.Popup_Error.Show();
				}
		}

		void CBPatternInfeedTrack_Click(System.Object sender, System.EventArgs e)
		{
			if (pno >= 0)
				try
				{
					if (sender is System.Windows.Controls.CheckBox)
					{
						int tag = (int)((System.Windows.Controls.CheckBox)sender).Tag;
						if (((System.Windows.Controls.CheckBox)sender).IsChecked == true)
						{
							Globals._Konfiguraatio.CurrentConfig.AddPatternInfeedTrack(pno, tag);
						}
						else
						{
							Globals._Konfiguraatio.CurrentConfig.RemovePatternInfeedTrack(pno, tag);
						}
						// UpdateSettingsJson();
					}
				}
				catch (Exception x)
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
					Globals.Popup_Error.Show();
				}
		}

		void CBPatternPalletType_Click(System.Object sender, System.EventArgs e)
		{
			if (pno >= 0)
				try
				{
					if (sender is System.Windows.Controls.CheckBox)
					{
						int tag = (int)((System.Windows.Controls.CheckBox)sender).Tag;
						if (((System.Windows.Controls.CheckBox)sender).IsChecked == true)
						{
							Globals._Konfiguraatio.CurrentConfig.AddPatternPalletType(pno, tag);
						}
						else
						{
							Globals._Konfiguraatio.CurrentConfig.RemovePatternPalletType(pno, tag);
						}
						// UpdateSettingsJson();
					}
				}
				catch (Exception x)
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
					Globals.Popup_Error.Show();
				}
		}

		void BtnRead_Click(System.Object sender, System.EventArgs e)
		{
			Globals._Konfiguraatio.Read();
			UpdateSettingsFields();
		}

		void BtnSave_Click(System.Object sender, System.EventArgs e)
		{
			Globals._Konfiguraatio.Save();
		}

		#endregion

	}
}
