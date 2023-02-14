//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	public class AllowedPlace
	{
		public bool Selected { get; set; }
		public int Id { get; set; }
		public int RobotId {get; set; }
	}
	
	public partial class Settings_SystemConf_Pan1
	{
		int rno = 0;
		int pno = 0;
		
		void Settings_SystemConf_Pan1_Opened(System.Object sender, System.EventArgs e)
		{
			CBSetSelection.SelectedIndex = 0;
			Globals.Tags.HMI_ConfGroupSet.SetAnalog(0);
			
			Globals._Konfiguraatio.CurrentConfig.PanelNo = Globals.Tags.Settings_PanelNumber.Value.Int;
			UpdateRobotCB();
			UpdateSettingsFields();
		}
		
		#region update fields
		
		/// <summary>
		/// update combobox with common values and robot selection
		/// </summary>
		/// <returns></returns>
		void UpdateRobotCB()
		{			
			CBSelectedRobot.Items.Clear();
			rno = 0;
			foreach (Neo.ApplicationFramework.Generated.RobotConf r in Globals._Konfiguraatio.CurrentConfig.Robots.Values)
			{
				CBSelectedRobot.Items.Add(string.Format("Robot {0}", r.RobotNo));
				if (rno == 0)
				{
					rno = r.RobotNo;
					CBSelectedRobot.SelectedIndex = 0;
				}
			}
		}
		
		/// <summary>
		/// Update settings listboxes and other fields
		/// </summary>
		/// <returns></returns>
		void UpdateSettingsFields()
		{
			ANPanelNo.Value = Globals._Konfiguraatio.CurrentConfig.PanelNo;
			ANNumberOfPLC.Value = Globals._Konfiguraatio.CurrentConfig.NumberOfPLC;
			
			LBPalletTypes.Items.Clear();
			foreach (KeyValuePair<int, string> item in Globals._Konfiguraatio.CurrentConfig.Lavatyypit)
			{
				string s = string.Format("[{0}] {1}", item.Key, item.Value);
				int i = LBPalletTypes.Items.Add(item);
			}
			
			LBWTimes.Items.Clear();
			foreach (KeyValuePair<string, int> item in Globals._Konfiguraatio.CurrentConfig.Aikavalit)
			{
				string s = string.Format("[{0}] {1}", item.Key, item.Value);
				int i = LBWTimes.Items.Add(item);
			}
			
			LBInfeedPlaces.Items.Clear();			
			foreach (KeyValuePair<int, int> item in Globals._Konfiguraatio.CurrentConfig.Tuloradat)
			{
				string s = string.Format("[{0}] {1}", item.Key, item.Value);
				int i = LBInfeedPlaces.Items.Add(item);
			}
			
			LBPalletPlaces.Items.Clear();			
			foreach (KeyValuePair<int, int> item in Globals._Konfiguraatio.CurrentConfig.Lavapaikat)
			{
				string s = string.Format("[{0}] {1}", item.Key, item.Value);
				int i = LBPalletPlaces.Items.Add(item);
			}

			UpdateRobotAllowedPlaces(rno);

			UpdateAllowedPatterns();
			
			UpdatePatternAllowedPlaces(pno);

			UpdateSettingsJson();
		}
		
		void UpdateAllowedPatterns()
		{
			LBPatternNo.Items.Clear();
			pno = 0;
			foreach (int no in Globals._Konfiguraatio.CurrentConfig.AllowedPatterns.Keys)
			{
				if (pno == 0) pno = no;
				LBPatternNo.Items.Add(no);
			}
		}

		void ClearListBoxItems(Neo.ApplicationFramework.Controls.Script.ListBoxAdapter l)
		{
			foreach (object item in l.Items)
			{
				if (item is System.Windows.Controls.CheckBox)
				{
					((System.Windows.Controls.CheckBox)item).Click -= CBInfeedTracks_Click;
				}
			}
			l.Items.Clear();
		}
		
		/// <summary>
		/// Update listboxes for allowed infeed tracks and pallet places
		/// </summary>
		/// <param name="rno"></param>
		/// <returns></returns>
		void UpdateRobotAllowedPlaces(int rno)
		{
			ClearListBoxItems(LBAllowedPalletPlaces);
			foreach (KeyValuePair<int, int> item in Globals._Konfiguraatio.CurrentConfig.Lavapaikat)
			{
				string s = string.Format("[{0}] {1}", item.Key, item.Value);
				System.Windows.Controls.CheckBox cb = new System.Windows.Controls.CheckBox();
				cb.Content = s;
				cb.Tag = item.Key;
				cb.IsChecked = Globals._Konfiguraatio.CurrentConfig.IsAllowedPalletPlace(rno, item.Key);
				cb.Click += CBInfeedTracks_Click;
				LBAllowedPalletPlaces.Items.Add(cb);
			}
			
			ClearListBoxItems(LBAllowedInfeedPlaces);
			foreach (KeyValuePair<int, int> item in Globals._Konfiguraatio.CurrentConfig.Tuloradat)
			{
				string s = string.Format("[{0}] {1}", item.Key, item.Value);
				System.Windows.Controls.CheckBox cb = new System.Windows.Controls.CheckBox();
				cb.Content = s;
				cb.Tag = item.Key;
				cb.IsChecked = Globals._Konfiguraatio.CurrentConfig.IsAllowedInfeedTrack(rno, item.Key);
				cb.Click += CBPalletPlaces_Click;
				LBAllowedInfeedPlaces.Items.Add(cb);
			}
		}

		/// <summary>
		/// Update listboxes for allowed infeed tracks and pallet places
		/// </summary>
		/// <param name="rno"></param>
		/// <returns></returns>
		void UpdatePatternAllowedPlaces(int pno)
		{
			ClearListBoxItems(LBPatternAllowedIT);
			foreach (KeyValuePair<int, int> item in Globals._Konfiguraatio.CurrentConfig.Tuloradat)
			{
				string s = string.Format("[{0}] {1}", item.Key, item.Value);
				System.Windows.Controls.CheckBox cb = new System.Windows.Controls.CheckBox();
				cb.Content = s;
				cb.Tag = item.Key;
				cb.IsChecked = Globals._Konfiguraatio.CurrentConfig.IsAllowedPatternInfeedTrack(pno, item.Key);
				cb.Click += CBPatternInfeedTrack_Click;
				LBPatternAllowedIT.Items.Add(cb);
			}
			
			ClearListBoxItems(LBPatternAllowedPP);
			foreach (KeyValuePair<int, int> item in Globals._Konfiguraatio.CurrentConfig.Lavapaikat)
			{
				string s = string.Format("[{0}] {1}", item.Key, item.Value);
				System.Windows.Controls.CheckBox cb = new System.Windows.Controls.CheckBox();
				cb.Content = s;
				cb.Tag = item.Key;
				cb.IsChecked = Globals._Konfiguraatio.CurrentConfig.IsAllowedPatternPalletPlace(pno, item.Key);
				cb.Click += CBPatternPalletPlace_Click;
				LBPatternAllowedPP.Items.Add(cb);
			}
			
			ClearListBoxItems(LBPatternAllowedPT);
			foreach (KeyValuePair<int, string> item in Globals._Konfiguraatio.CurrentConfig.Lavatyypit)
			{
				string s = string.Format("[{0}] {1}", item.Key, item.Value);
				System.Windows.Controls.CheckBox cb = new System.Windows.Controls.CheckBox();
				cb.Content = s;
				cb.Tag = item.Key;
				cb.IsChecked = Globals._Konfiguraatio.CurrentConfig.IsAllowedPatternPalletType(pno, item.Key);
				cb.Click += CBPatternPalletType_Click;
				LBPatternAllowedPT.Items.Add(cb);
			}
		}
		
		/// <summary>
		/// Updatejson textbox on screen
		/// </summary>
		/// <returns></returns>
		void UpdateSettingsJson()
		{
			try
			{
				string s = Newtonsoft.Json.JsonConvert.SerializeObject(
					Globals._Konfiguraatio.CurrentConfig, 
					Newtonsoft.Json.Formatting.Indented);
				TextBox.Text = s;
			}
			catch (Exception x)
			{
				System.Windows.Forms.MessageBox.Show(x.Message);
			}
		}
		
		#endregion
		
		void BtnSave_Click(System.Object sender, System.EventArgs e)
		{
			Globals._Konfiguraatio.Save();
		}
		
		void BtnRead_Click(System.Object sender, System.EventArgs e)
		{
			Globals._Konfiguraatio.Read();
			UpdateSettingsFields();
		}
	
		void CBSetSelection_SelectionChanged(System.Object sender, System.EventArgs e)
		{
			if (CBSetSelection.SelectedIndex >= 0)
				try
				{
					Globals.Tags.HMI_ConfGroupSet.SetAnalog(CBSetSelection.SelectedIndex);
				}
				catch {}
		}
		
		void CBSelectedRobot_SelectionChanged(System.Object sender, System.EventArgs e)
		{
			if (rno >= 0)
			try 
			{
				rno = Globals._Konfiguraatio.CurrentConfig.GetRobotNoByIndex(CBSelectedRobot.SelectedIndex);
				UpdateRobotAllowedPlaces(rno);
				ANDebug.Text = string.Format("[{0}] {1}", rno, CBSelectedRobot.SelectedIndex);
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message);
			}
		}
		
		#region Pallettypes

		void LBPalletTypes_SelectionChanged(System.Object sender, System.EventArgs e)
		{
			if (LBPalletTypes.SelectedIndex < 0 || LBPalletTypes.SelectedIndex > (LBPalletTypes.Items.Count - 1))
			{
				ANPalletType.Text = "";
				ANPalletName.Text = "";
				ANPTSelIndex.Text = LBPalletTypes.SelectedIndex.ToString();
			}
			else
			{
				try
				{
					KeyValuePair<int, string> item = (KeyValuePair<int, string>)LBPalletTypes.SelectedItem;
					ANPalletType.Text = item.Key.ToString();
					ANPalletName.Text = item.Value.ToString();
					ANPTSelIndex.Text = LBPalletTypes.SelectedIndex.ToString();
				}
				catch (Exception x)
				{
					MessageBox.Show(x.Message);
				}
			}
		}
		
		void BtnPTAdd_Click(System.Object sender, System.EventArgs e)
		{
			try 
			{
				string s = ANPalletType.Text.Trim();
				string name = ANPalletName.Text.Trim();
				int type;
				if (int.TryParse(s, out type))
				{
					bool isnew = true;
					if (string.IsNullOrEmpty(name)) name = type.ToString();
					if (LBPalletTypes.Items.Count > 0)
					{
						for (int i = LBPalletTypes.Items.Count - 1; i > 0; i--)
						{
							int key = ((KeyValuePair<int, string>)LBPalletTypes.Items[i]).Key;
							if (key == type)
							{
								isnew = false;
								LBPalletTypes.Items.RemoveAt(i);
								LBPalletTypes.Items.Insert(i, new KeyValuePair<int, string>(key, name));
								Globals._Konfiguraatio.CurrentConfig.Lavatyypit[key] = name;
							}
						}
					}
					
					if (isnew)
					{
						LBPalletTypes.Items.Add(new KeyValuePair<int, string>(type, name));
						Globals._Konfiguraatio.CurrentConfig.Lavatyypit[type] = name;
					}
				
					UpdateSettingsJson();
				}
				else
				{
					MessageBox.Show("Pallet type is not number");
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message);
			}
		}
		
		void BtnPTDelete_Click(System.Object sender, System.EventArgs e)
		{
			try 
			{
				if (LBPalletTypes.Items.Count > 0)
				{
					if (LBPalletTypes.SelectedIndex >= 0 && LBPalletTypes.SelectedIndex < LBPalletTypes.Items.Count)
					{
						int key = ((KeyValuePair<int, string>)LBPalletTypes.Items[LBPalletTypes.SelectedIndex]).Key;
						LBPalletTypes.Items.RemoveAt(LBPalletTypes.SelectedIndex);
						Globals._Konfiguraatio.CurrentConfig.Lavatyypit.Remove(key);
						UpdateSettingsJson();
					}
				}
				else
				{
					MessageBox.Show("Nothing to delete");
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message);
			}
		}
		
		#endregion
		
		#region WatchDog times
		
		void LBWTimes_SelectionChanged(System.Object sender, System.EventArgs e)
		{
			if (LBWTimes.SelectedIndex < 0 || LBWTimes.SelectedIndex > (LBWTimes.Items.Count - 1))
			{
				ANWName.Text = "";
				ANWTime.Text = "";
				ANWSelIndex.Text = LBWTimes.SelectedIndex.ToString();
			}
			else
			{
				try
				{
					KeyValuePair<string, int> item = (KeyValuePair<string, int>)LBWTimes.SelectedItem;
					ANWName.Text = item.Key.ToString();
					ANWTime.Text = item.Value.ToString();
					ANWSelIndex.Text = LBWTimes.SelectedIndex.ToString();
				}
				catch (Exception x)
				{
					MessageBox.Show(x.Message);
				}
			}
		}
		
		void BtnWAdd_Click(System.Object sender, System.EventArgs e)
		{
			try 
			{
				string name = ANWName.Text.Trim();
				if (string.IsNullOrEmpty(name))
				{
					MessageBox.Show("Watchdog name is empty");
				}
				else
				{
					string s = ANWTime.Text.Trim();
					int time;
					if (int.TryParse(s, out time))
					{
						bool isnew = true;
						if (LBWTimes.Items.Count > 0)
						{
							for (int i = LBWTimes.Items.Count - 1; i > 0; i--)
							{
								string key = ((KeyValuePair<string, int>)LBWTimes.Items[i]).Key;
								if (key == name)
								{
									isnew = false;
									LBWTimes.Items.RemoveAt(i);
									LBWTimes.Items.Insert(i, new KeyValuePair<string, int>(key, time));
									Globals._Konfiguraatio.CurrentConfig.Aikavalit[key] = time;
								}
							}
						}
					
						if (isnew)
						{
							LBWTimes.Items.Add(new KeyValuePair<string, int>(name, time));
							Globals._Konfiguraatio.CurrentConfig.Aikavalit[name] = time;
						}
				
						UpdateSettingsJson();
					}
					else
					{
						MessageBox.Show("Watchdog time is not number");
					}
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message);
			}
		}
		
		void BtnWDelete_Click(System.Object sender, System.EventArgs e)
		{
			try 
			{
				if (LBWTimes.Items.Count > 0)
				{
					if (LBWTimes.SelectedIndex >= 0 && LBWTimes.SelectedIndex < LBWTimes.Items.Count)
					{
						string key = ((KeyValuePair<string, int>)LBWTimes.Items[LBWTimes.SelectedIndex]).Key;
						LBWTimes.Items.RemoveAt(LBWTimes.SelectedIndex);
						Globals._Konfiguraatio.CurrentConfig.Aikavalit.Remove(key);
						UpdateSettingsJson();
					}
				}
				else
				{
					MessageBox.Show("Nothing to delete");
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message);
			}
		}
		
		#endregion
		
		#region Available Infeed places
		
		void LBInfeedPlaces_SelectionChanged(System.Object sender, System.EventArgs e)
		{
			if (LBInfeedPlaces.SelectedIndex < 0 || LBInfeedPlaces.SelectedIndex > (LBInfeedPlaces.Items.Count - 1))
			{
				ANIPId.Text = "";
				ANIPRobotId.Text = "";
				ANIPSelIndex.Text = LBInfeedPlaces.SelectedIndex.ToString();
			}
			else
			{
				try
				{
					KeyValuePair<int, int> item = (KeyValuePair<int, int>)LBInfeedPlaces.SelectedItem;
					ANIPId.Text = item.Key.ToString();
					ANIPRobotId.Text = item.Value.ToString();
					ANIPSelIndex.Text = LBInfeedPlaces.SelectedIndex.ToString();
				}
				catch (Exception x)
				{
					MessageBox.Show(x.Message);
				}
			}
		}
		
		void BtnIPAdd_Click(System.Object sender, System.EventArgs e)
		{
			try 
			{
				string s = ANIPId.Text.Trim();
				int id;
				if (int.TryParse(s, out id))
				{
					s = ANIPRobotId.Text.Trim();
					int rid;
					if (int.TryParse(s, out rid))
					{
						bool isnew = true;
						if (LBInfeedPlaces.Items.Count > 0)
						{
							for (int i = LBInfeedPlaces.Items.Count - 1; i > 0; i--)
							{
								int key = ((KeyValuePair<int, int>)LBInfeedPlaces.Items[i]).Key;
								if (key == id)
								{
									isnew = false;
									LBInfeedPlaces.Items.RemoveAt(i);
									LBInfeedPlaces.Items.Insert(i, new KeyValuePair<int, int>(key, rid));
									Globals._Konfiguraatio.CurrentConfig.Tuloradat[key] = rid;
								}
							}
						}
					
						if (isnew)
						{
							LBInfeedPlaces.Items.Add(new KeyValuePair<int, int>(id, rid));
							Globals._Konfiguraatio.CurrentConfig.Tuloradat[id] = rid;
						}
				
						UpdateSettingsJson();
					}
					else
					{
						MessageBox.Show("Infeed Robot Id is not number");
					}
				}
				else
				{
					MessageBox.Show("Inffeed Id is not number");
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message);
			}	
		}
		
		void BtnIPDelete_Click(System.Object sender, System.EventArgs e)
		{
			try 
			{
				if (LBInfeedPlaces.Items.Count > 0)
				{
					if (LBInfeedPlaces.SelectedIndex >= 0 && LBInfeedPlaces.SelectedIndex < LBInfeedPlaces.Items.Count)
					{
						int key = ((KeyValuePair<int, int>)LBInfeedPlaces.Items[LBInfeedPlaces.SelectedIndex]).Key;
						LBInfeedPlaces.Items.RemoveAt(LBInfeedPlaces.SelectedIndex);
						Globals._Konfiguraatio.CurrentConfig.Tuloradat.Remove(key);
						UpdateSettingsJson();
					}
				}
				else
				{
					MessageBox.Show("Nothing to delete");
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message);
			}
		}
		
		#endregion
		
		#region Available Pallet places
		
		void LBPalletPlaces_SelectionChanged(System.Object sender, System.EventArgs e)
		{
			if (LBPalletPlaces.SelectedIndex < 0 || LBPalletPlaces.SelectedIndex > (LBPalletPlaces.Items.Count - 1))
			{
				ANPPId.Text = "";
				ANPPRobotId.Text = "";
				ANPPSelIndex.Text = LBPalletPlaces.SelectedIndex.ToString();
			}
			else
			{
				try
				{
					KeyValuePair<int, int> item = (KeyValuePair<int, int>)LBPalletPlaces.SelectedItem;
					ANPPId.Text = item.Key.ToString();
					ANPPRobotId.Text = item.Value.ToString();
					ANPPSelIndex.Text = LBPalletPlaces.SelectedIndex.ToString();
				}
				catch (Exception x)
				{
					MessageBox.Show(x.Message);
				}
			}
		}
		
		void BtnPPAdd_Click(System.Object sender, System.EventArgs e)
		{
			try 
			{
				string s = ANPPId.Text.Trim();
				int id;
				if (int.TryParse(s, out id))
				{
					s = ANPPRobotId.Text.Trim();
					int rid;
					if (int.TryParse(s, out rid))
					{
						bool isnew = true;
						if (LBPalletPlaces.Items.Count > 0)
						{
							for (int i = LBPalletPlaces.Items.Count - 1; i > 0; i--)
							{
								int key = ((KeyValuePair<int, int>)LBPalletPlaces.Items[i]).Key;
								if (key == id)
								{
									isnew = false;
									LBPalletPlaces.Items.RemoveAt(i);
									LBPalletPlaces.Items.Insert(i, new KeyValuePair<int, int>(key, rid));
									Globals._Konfiguraatio.CurrentConfig.Lavapaikat[key] = rid;
								}
							}
						}
					
						if (isnew)
						{
							LBPalletPlaces.Items.Add(new KeyValuePair<int, int>(id, rid));
							Globals._Konfiguraatio.CurrentConfig.Lavapaikat[id] = rid;
						}
				
						UpdateSettingsJson();
					}
					else
					{
						MessageBox.Show("Infeed Robot Id is not number");
					}
				}
				else
				{
					MessageBox.Show("Inffeed Id is not number");
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message);
			}
		}
		
		void BtnPPDelete_Click(System.Object sender, System.EventArgs e)
		{
			try 
			{
				if (LBPalletPlaces.Items.Count > 0)
				{
					if (LBPalletPlaces.SelectedIndex >= 0 && LBPalletPlaces.SelectedIndex < LBPalletPlaces.Items.Count)
					{
						int key = ((KeyValuePair<int, int>)LBPalletPlaces.Items[LBPalletPlaces.SelectedIndex]).Key;
						LBPalletPlaces.Items.RemoveAt(LBPalletPlaces.SelectedIndex);
						Globals._Konfiguraatio.CurrentConfig.Lavapaikat.Remove(key);
						UpdateSettingsJson();
					}
				}
				else
				{
					MessageBox.Show("Nothing to delete");
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message);
			}
		}
		
		#endregion
		
		#region Robot instance
		
		void BtnRobotAdd_Click(System.Object sender, System.EventArgs e)
		{
			try 
			{
				int no = Convert.ToInt16(ANNewRobotNo.Value);
				if (Globals._Konfiguraatio.CurrentConfig.Robots.ContainsKey(no))
				{
					MessageBox.Show("Robotti on jo olemassa");
				}
				else
				{
					Globals._Konfiguraatio.CurrentConfig.AddRobot(no);
					UpdateSettingsJson();
					UpdateRobotCB();
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message);
			}
		}
		
		void BtnRobotDelete_Click(System.Object sender, System.EventArgs e)
		{
			try 
			{
				int no = Convert.ToInt16(ANNewRobotNo.Value);
				if (Globals._Konfiguraatio.CurrentConfig.Robots.ContainsKey(no))
				{
					if (MessageBox.Show(
						string.Format("Haluatko poistaa robotin [{0}]", no), 
						"Robotin poisto", 
						System.Windows.Forms.MessageBoxButtons.YesNo, 
						System.Windows.Forms.MessageBoxIcon.Question, 
						System.Windows.Forms.MessageBoxDefaultButton.Button2) == DialogResult.Yes)
					{
						Globals._Konfiguraatio.CurrentConfig.RemoveRobot(no);
						UpdateSettingsJson();
						UpdateRobotCB();
					}
				}
				else
				{
					MessageBox.Show("Tuntematon robottinumero");
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message);
			}
		}
		
		#endregion
		
		#region Available robot infeed places
		
		void CBInfeedTracks_Click(System.Object sender, System.EventArgs e)
		{
			try
			{
				if (sender is System.Windows.Controls.CheckBox)
				{
					int tag = (int)((System.Windows.Controls.CheckBox)sender).Tag;
					if (((System.Windows.Controls.CheckBox)sender).IsChecked == true)
					{
						Globals._Konfiguraatio.CurrentConfig.AddRobotInfeedTrack(rno, tag);
					}
					else
					{
						Globals._Konfiguraatio.CurrentConfig.RemoveRobotInfeedTrack(rno, tag);
					}
					UpdateSettingsJson();
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message);
			}
		}
		
		#endregion
		
		#region Available robot pallet places
		
		void CBPalletPlaces_Click(System.Object sender, System.EventArgs e)
		{
			if (rno >= 0)
			try
			{
				if (sender is System.Windows.Controls.CheckBox)
				{
					int tag = (int)((System.Windows.Controls.CheckBox)sender).Tag;
					if (((System.Windows.Controls.CheckBox)sender).IsChecked == true)
					{
						Globals._Konfiguraatio.CurrentConfig.AddRobotPalletPlace(rno, tag);
					}
					else
					{
						Globals._Konfiguraatio.CurrentConfig.RemoveRobotPalletPlace(rno, tag);
					}
					UpdateSettingsJson();
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message);
			}	
		}
		
		#endregion
		
		#region Allowed Pattern settings
		
		void BtnPatternAdd_Click(System.Object sender, System.EventArgs e)
		{
			try 
			{
				int no = Convert.ToInt16(ANNewPatternNo.Value);
				if (Globals._Konfiguraatio.CurrentConfig.AllowedPatterns.ContainsKey(no))
				{
					MessageBox.Show("Kuvionumero on jo olemassa");
				}
				else
				{
					Globals._Konfiguraatio.CurrentConfig.AddPattern(no);
					UpdateSettingsJson();
					UpdateAllowedPatterns();
					pno = no;
					UpdatePatternAllowedPlaces(no);
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message);
			}
		}
		
		void BtnPatternDelete_Click(System.Object sender, System.EventArgs e)
		{
			try 
			{
				int no = Convert.ToInt16(ANNewPatternNo.Value);
				if (Globals._Konfiguraatio.CurrentConfig.AllowedPatterns.ContainsKey(no))
				{
					if (MessageBox.Show(
						string.Format("Haluatko poistaa Kuvion [{0}]", no), 
						"Kuvion poisto", 
						System.Windows.Forms.MessageBoxButtons.YesNo, 
						System.Windows.Forms.MessageBoxIcon.Question, 
						System.Windows.Forms.MessageBoxDefaultButton.Button2) == DialogResult.Yes)
					{
						Globals._Konfiguraatio.CurrentConfig.RemovePattern(no);
						UpdateSettingsJson();
						UpdateAllowedPatterns();
						UpdatePatternAllowedPlaces(no);
					}
				}
				else
				{
					MessageBox.Show("Tuntematon robottinumero");
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message);
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
				}
			}
			catch (Exception x)
			{
				MessageBox.Show(x.Message);
			}	
		}
		
		void CBPatternPalletPlace_Click(System.Object sender, System.EventArgs e)
		{
			if (rno >= 0)
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
						UpdateSettingsJson();
					}
				}
				catch (Exception x)
				{
					MessageBox.Show(x.Message);
				}
		}

		void CBPatternInfeedTrack_Click(System.Object sender, System.EventArgs e)
		{
			if (rno >= 0)
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
						UpdateSettingsJson();
					}
				}
				catch (Exception x)
				{
					MessageBox.Show(x.Message);
				}	
		}

		void CBPatternPalletType_Click(System.Object sender, System.EventArgs e)
		{
			if (rno >= 0)
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
						UpdateSettingsJson();
					}
				}
				catch (Exception x)
				{
					MessageBox.Show(x.Message);
				}	
		}
		
		#endregion
	}
}
