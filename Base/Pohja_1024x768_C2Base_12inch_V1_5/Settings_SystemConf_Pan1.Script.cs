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
	using System.Windows;
	using System.Drawing;
	using System.Linq;
	using System.Windows.Media;


	public class AllowedPlace
	{
		public bool Selected { get; set; }
		public int Id { get; set; }
		public int RobotId { get; set; }
	}

	public partial class Settings_SystemConf_Pan1
	{
		int rno = 0;
		int pno = 0;
		object lockobject = new object();

		void Settings_SystemConf_Pan1_Opened(System.Object sender, System.EventArgs e)
		{
			CBSetSelection.SelectedIndex = 0;
			Globals.Tags.HMI_ConfGroupSet.SetAnalog(0);
			ANSettingsFilename.Text = _Konfiguraatio.ConfigFileName;
			
			if (!Globals._Konfiguraatio.ReadOk) Globals._Konfiguraatio.Read();
			ANSqlServerInstance.Text = Globals._Konfiguraatio.CurrentConfig.SqlInstanceName;
			ANSqlServerDB.Text = Globals._Konfiguraatio.CurrentConfig.SqlDatabaseName;

			Globals._Konfiguraatio.CurrentConfig.PanelNo = Globals.Tags.HMI_Settings_PanelNumber.Value.Int;

			InitSettingsFields();
		}

		#region update fields

		/// <summary>
		/// Init combobox with common values and robot selection
		/// </summary>
		/// <returns></returns>
		void InitRobotCB()
		{
			CBSelectedRobot.Items.Clear();
			CBSelectedRobot.Items.Add("");
			CBSelectedRobot.SelectedIndex = 0;
			rno = 0;

			foreach (Neo.ApplicationFramework.Generated.RobotConf r in Globals._Konfiguraatio.CurrentConfig.Robots.Values)
			{
				CBSelectedRobot.Items.Add(string.Format("Robot {0}", r.RobotNo));
				if (rno == 0)
				{
					rno = r.RobotNo;
					CBSelectedRobot.SelectedIndex = 1;
				}
			}
		}

		/// <summary>
		/// init given listbox with dictionary values (items)
		/// </summary>
		/// <param name="lb"></param>
		/// <param name="lst"></param>
		/// <returns></returns>
		void InitListBox(Neo.ApplicationFramework.Controls.Script.ListBoxAdapter lb, Dictionary<int, int> lst)
		{
			lb.Items.Clear();
			foreach (KeyValuePair<int, int> item in lst)
			{
				lb.Items.Add(item);
			}
		}

		void InitListBox(Neo.ApplicationFramework.Controls.Script.ListBoxAdapter lb, Dictionary<int, string> lst)
		{
			lb.Items.Clear();
			foreach (KeyValuePair<int, string> item in lst)
			{
				lb.Items.Add(item);
			}
		}

		void InitLists(int mode)
		{
			if (mode == 1 || mode == 0)
			{
				InitListBox(LBInfeedTracks, Globals._Konfiguraatio.CurrentConfig.Tuloradat);
			}

			if (mode == 2 || mode == 0)
			{
				InitListBox(LBPalletPlaces, Globals._Konfiguraatio.CurrentConfig.Lavapaikat);
			}

			if (mode == 3 || mode == 0)
			{
				InitListBox(LBPalletTypes, Globals._Konfiguraatio.CurrentConfig.Lavatyypit);
			}
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

		void ClearListAndEvent(Neo.ApplicationFramework.Controls.Script.ListBoxAdapter lb, System.Windows.RoutedEventHandler clicked)
		{
			// clear items and click handlers
			foreach (object item in lb.Items)
				if (item is System.Windows.Controls.CheckBox)
					((System.Windows.Controls.CheckBox)item).Click -= clicked;
			lb.Items.Clear();
		}

		void InitListBoxAndEvent(Neo.ApplicationFramework.Controls.Script.ListBoxAdapter lb, System.Windows.RoutedEventHandler clicked, Dictionary<int, int> lst, List<int> allowed)
		{
			ClearListAndEvent(lb, clicked);
			// create new items with click handler
			foreach (KeyValuePair<int, int> item in lst)
			{
				string s = string.Format("[{0}] {1}", item.Key, item.Value);
				System.Windows.Controls.CheckBox cb = CreateCheckBox(s, item.Key, allowed.Contains(item.Key), clicked);
				lb.Items.Add(cb);
			}
		}

		void InitListBoxAndEvent(Neo.ApplicationFramework.Controls.Script.ListBoxAdapter lb, System.Windows.RoutedEventHandler clicked, Dictionary<int, string> lst, List<int> allowed)
		{
			ClearListAndEvent(lb, clicked);
			// create new items with click handler
			foreach (KeyValuePair<int, string> item in lst)
			{
				string s = string.Format("[{0}] {1}", item.Key, item.Value);
				System.Windows.Controls.CheckBox cb = CreateCheckBox(s, item.Key, allowed.Contains(item.Key), clicked);
				lb.Items.Add(cb);
			}
		}

		/// <summary>
		/// list allowed pattern numbers in listbox
		/// </summary>
		/// <returns></returns>
		void InitAllowedPatterns()
		{
			LBPatternNo.Items.Clear();
			pno = 0;
			List<int> lst = Globals._Konfiguraatio.CurrentConfig.AllowedPatternNumbers();
			foreach (int no in lst)
			{
				int index = LBPatternNo.Items.Add(no);
				if (pno == 0)
				{
					pno = no;
					LBPatternNo.SelectedIndex = index;
				}
			}
		}

		/// <summary>
		/// Update listboxes for allowed infeed tracks and pallet places
		/// </summary>
		/// <param name="rno"></param>
		/// <returns></returns>
		void InitPatternAllowedLists(int pno, int mode)
		{
			if (mode == 1 || mode == 0)
			{
				InitListBoxAndEvent(LBPatternAllowedIT, CBPatternInfeedTrack_Click, Globals._Konfiguraatio.CurrentConfig.Tuloradat,
					Globals._Konfiguraatio.CurrentConfig.AllowedPatternInfeedTracks(pno));
			}

			if (mode == 2 || mode == 0)
			{
				InitListBoxAndEvent(LBPatternAllowedPP, CBPatternPalletPlace_Click, Globals._Konfiguraatio.CurrentConfig.Lavapaikat,
					Globals._Konfiguraatio.CurrentConfig.AllowedPatternPalletPlaces(pno));
			}

			if (mode == 3 || mode == 0)
			{
				InitListBoxAndEvent(LBPatternAllowedPT, CBPatternPalletType_Click, Globals._Konfiguraatio.CurrentConfig.Lavatyypit,
					Globals._Konfiguraatio.CurrentConfig.AllowedPatternPalletTypes(pno));
			}
		}

		/// <summary>
		/// init listboxes for allowed infeed tracks
		/// </summary>
		/// <param name="rno"></param>
		/// <returns></returns>
		void InitRobotAllowedLists(int rno, int mode)
		{
			if (mode == 1 || mode == 0)
			{
				InitListBoxAndEvent(LBAllowedInfeedTracks, CBInfeedTracks_Click, Globals._Konfiguraatio.CurrentConfig.Tuloradat,
					Globals._Konfiguraatio.CurrentConfig.AllowedInfeedTracks(rno));
			}

			if (mode == 2 || mode == 0)
			{
				InitListBoxAndEvent(LBAllowedPalletPlaces, CBPalletPlaces_Click, Globals._Konfiguraatio.CurrentConfig.Lavapaikat,
					Globals._Konfiguraatio.CurrentConfig.AllowedPalletPlaces(rno));
			}
		}

		/// <summary>
		/// Init settings listboxes and other fields
		/// </summary>
		/// <returns></returns>
		void InitSettingsFields()
		{
			lock (lockobject)
			{
				ANPanelNo.Value = Globals._Konfiguraatio.CurrentConfig.PanelNo;
				ANNumberOfPLC.Value = Globals._Konfiguraatio.CurrentConfig.NumberOfPLC;

				LBWTimes.Items.Clear();
				foreach (KeyValuePair<string, int> item in Globals._Konfiguraatio.CurrentConfig.Aikavalit)
				{
					//string s = string.Format("[{0}] {1}", item.Key, item.Value);
					LBWTimes.Items.Add(item);
				}

				InitRobotCB();
				InitLists(0);
				InitRobotAllowedLists(rno, 0);
				InitAllowedPatterns();
				InitPatternAllowedLists(pno, 0);

				UpdateSettingsJson();
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
				TextBoxSettings.Text = s;
			}
			catch (Exception x)
			{
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
				Globals.Popup_Error.Show();
			}
		}

		#endregion

		void BtnSave_Click(System.Object sender, System.EventArgs e)
		{
			Globals._Konfiguraatio.CurrentConfig.SqlInstanceName = ANSqlServerInstance.Text.Trim();
			Globals._Konfiguraatio.CurrentConfig.SqlDatabaseName = ANSqlServerDB.Text.Trim();
			
			Globals._Konfiguraatio.Save();
		}

		void BtnRead_Click(System.Object sender, System.EventArgs e)
		{
			Globals._Konfiguraatio.Read();
			InitSettingsFields();
		}

		void CBSetSelection_SelectionChanged(System.Object sender, System.EventArgs e)
		{
			if (CBSetSelection.SelectedIndex >= 0)
				try
				{
					Globals.Tags.HMI_ConfGroupSet.SetAnalog(CBSetSelection.SelectedIndex);
				}
				catch (Exception x)
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
					Globals.Popup_Error.Show();
				}
		}

		void CBSelectedRobot_SelectionChanged(System.Object sender, System.EventArgs e)
		{
			if (CBSelectedRobot.SelectedIndex >= 0)
				try
				{
					if (CBSelectedRobot.SelectedIndex == 0)
						rno = 0;
					else
					{
						string s = CBSelectedRobot.SelectedItem.ToString();
						s = s.Substring(6);
						if (!int.TryParse(s, out rno))
						{
							CBSelectedRobot.SelectedIndex = 0;
							rno = 0;
						}
					}
					InitRobotAllowedLists(rno, 0);
					ANDebug.Text = string.Format("[{0}] {1}", rno, CBSelectedRobot.SelectedIndex);
				}
				catch (Exception x)
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
					Globals.Popup_Error.Show();
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
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
					Globals.Popup_Error.Show();
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
					InitRobotAllowedLists(rno, 3);
					InitPatternAllowedLists(pno, 3);
				}
				else
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UsePositiveNumbers);
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
						Globals._Konfiguraatio.CurrentConfig.RemovePalletType(key);
						UpdateSettingsJson();
						InitRobotAllowedLists(rno, 3);
						InitPatternAllowedLists(pno, 3);
					}
				}
				else
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.NothingToDelete);
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
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
					Globals.Popup_Error.Show();
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
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.FillAllFields);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
					Globals.Popup_Error.Show();
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
						Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UsePositiveNumbers);
						Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
						Globals.Popup_Error.Show();
					}
				}
			}
			catch (Exception x)
			{
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
				Globals.Popup_Error.Show();
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
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.NothingToDelete);
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

		#endregion

		#region Available Infeed tracks

		void LBInfeedTracks_SelectionChanged(System.Object sender, System.EventArgs e)
		{
			if (LBInfeedTracks.SelectedIndex < 0 || LBInfeedTracks.SelectedIndex > (LBInfeedTracks.Items.Count - 1))
			{
				ANITId.Text = "";
				ANITRobotId.Text = "";
				ANITSelIndex.Text = LBInfeedTracks.SelectedIndex.ToString();
			}
			else
			{
				try
				{
					KeyValuePair<int, int> item = (KeyValuePair<int, int>)LBInfeedTracks.SelectedItem;
					ANITId.Text = item.Key.ToString();
					ANITRobotId.Text = item.Value.ToString();
					ANITSelIndex.Text = LBInfeedTracks.SelectedIndex.ToString();
				}
				catch (Exception x)
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
					Globals.Popup_Error.Show();
				}
			}
		}

		void BtnITAdd_Click(System.Object sender, System.EventArgs e)
		{
			try
			{
				string s = ANITId.Text.Trim();
				int id;
				if (int.TryParse(s, out id))
				{
					s = ANITRobotId.Text.Trim();
					int rid;
					if (int.TryParse(s, out rid))
					{
						bool isnew = true;
						if (LBInfeedTracks.Items.Count > 0)
						{
							for (int i = LBInfeedTracks.Items.Count - 1; i > 0; i--)
							{
								int key = ((KeyValuePair<int, int>)LBInfeedTracks.Items[i]).Key;
								if (key == id)
								{
									isnew = false;
									LBInfeedTracks.Items.RemoveAt(i);
									LBInfeedTracks.Items.Insert(i, new KeyValuePair<int, int>(key, rid));
									Globals._Konfiguraatio.CurrentConfig.Tuloradat[key] = rid;
								}
							}
						}

						if (isnew)
						{
							LBInfeedTracks.Items.Add(new KeyValuePair<int, int>(id, rid));
							Globals._Konfiguraatio.CurrentConfig.Tuloradat[id] = rid;
						}

						UpdateSettingsJson();
						InitRobotAllowedLists(rno, 1);
						InitPatternAllowedLists(pno, 1);
					}
					else
					{
						Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UsePositiveNumbers);
						Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
						Globals.Popup_Error.Show();
					}
				}
				else
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UsePositiveNumbers);
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

		void BtnITDelete_Click(System.Object sender, System.EventArgs e)
		{
			try
			{
				if (LBInfeedTracks.Items.Count > 0)
				{
					if (LBInfeedTracks.SelectedIndex >= 0 && LBInfeedTracks.SelectedIndex < LBInfeedTracks.Items.Count)
					{
						int key = ((KeyValuePair<int, int>)LBInfeedTracks.Items[LBInfeedTracks.SelectedIndex]).Key;
						LBInfeedTracks.Items.RemoveAt(LBInfeedTracks.SelectedIndex);
						Globals._Konfiguraatio.CurrentConfig.RemoveInfeedTrack(key);
						UpdateSettingsJson();
						InitRobotAllowedLists(rno, 1);
						InitPatternAllowedLists(pno, 1);
					}
				}
				else
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.NothingToDelete);
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
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
					Globals.Popup_Error.Show();
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
						InitRobotAllowedLists(rno, 2);
						InitPatternAllowedLists(pno, 2);
					}
					else
					{
						Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UsePositiveNumbers);
						Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
						Globals.Popup_Error.Show();
					}
				}
				else
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UsePositiveNumbers);
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
						Globals._Konfiguraatio.CurrentConfig.RemovePalletPlace(key);
						UpdateSettingsJson();
						InitRobotAllowedLists(rno, 2);
						InitPatternAllowedLists(pno, 2);
					}
				}
				else
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.NothingToDelete);
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

		#endregion

		#region Robot instance

		void BtnRobotAdd_Click(System.Object sender, System.EventArgs e)
		{
			try
			{
				int no = Convert.ToInt16(ANNewRobotNo.Value);
				if (Globals._Konfiguraatio.CurrentConfig.Robots.ContainsKey(no))
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.RobotIdAlreadyExist);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
					Globals.Popup_Error.Show();
				}
				else
				{
					Globals._Konfiguraatio.CurrentConfig.AddRobot(no);
					UpdateSettingsJson();
					InitRobotCB();
				}
			}
			catch (Exception x)
			{
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
				Globals.Popup_Error.Show();
			}
		}

		void BtnRobotDelete_Click(System.Object sender, System.EventArgs e)
		{
			try
			{
				int no = Convert.ToInt16(ANNewRobotNo.Value);
				if (Globals._Konfiguraatio.CurrentConfig.Robots.ContainsKey(no))
				{
					if (System.Windows.Forms.MessageBox.Show(
						string.Format("Haluatko poistaa robotin [{0}]", no),
						"Robotin poisto",
						System.Windows.Forms.MessageBoxButtons.YesNo,
						System.Windows.Forms.MessageBoxIcon.Question,
						System.Windows.Forms.MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
					{
						Globals._Konfiguraatio.CurrentConfig.RemoveRobot(no);
						UpdateSettingsJson();
						rno = 0;
						InitRobotCB();
						InitRobotAllowedLists(rno, 0);
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
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
				Globals.Popup_Error.Show();
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
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
					Globals.Popup_Error.Show();
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
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.PatternAlreadyExist);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = "";
					Globals.Popup_Error.Show();
				}
				else
				{
					Globals._Konfiguraatio.CurrentConfig.AddPattern(no);
					UpdateSettingsJson();
					InitAllowedPatterns();
					pno = no;
					InitPatternAllowedLists(pno, 0);
					LBPatternNo.SelectedItem = no;
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
						UpdateSettingsJson();
						InitAllowedPatterns();
						ANNewPatternNo.Text = "";
						pno = 0;
						InitPatternAllowedLists(pno, 0);
						if (LBPatternNo.Items.Count > 0) LBPatternNo.SelectedIndex = 0;
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
					InitPatternAllowedLists(pno, 0);
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
						UpdateSettingsJson();
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
						UpdateSettingsJson();
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
						UpdateSettingsJson();
					}
				}
				catch (Exception x)
				{
					Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
					Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
					Globals.Popup_Error.Show();
				}
		}

		#endregion
	}
}
