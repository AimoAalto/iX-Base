//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
	using Neo.ApplicationFramework.Interfaces;
	using Neo.ApplicationFramework.Interfaces.Tag;
	using Neo.ApplicationFramework.Tools.OpcClient;
	using System;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Collections.Generic;


	public partial class Tags
	{
		public enum ErrorTexts
		{
			UnexpectedError = 0, // "Unexpected error occured.","On tapahtunut odottamaton virhe",,
			ChooseProduct = 1, // "Please choose the product to be started.","Valitse aloitettava tuote",,
			ChoosePalletPlaces = 2, // "Please choose the pallet place(s) to be started on.","Valitse aloitettavat lavapaikat",,
			PalletPlaceAlreadyOnPattern = 3, // "Selected pallet place already has a pattern.Please reset the existing pattern or contact Orfer oCare Customer service.","Valittu lavapaikka on jo valittu kuvioon",,
			PatternLoadFailed = 4, // "Pattern load failed.",,,
			PatternImageLoadFailed = 5, // "Pattern picture load failed.",,,
			RecipeLoadFailed = 6, // "Loading of recipes failed.",,,
			SendStartFailed = 7, // "Sending production start command to robot failed.",,,
			CheckStartConditions = 8, // "Please check the starting conditions.",,,
			NoPatternFile = 9, // "There is no pattern file for the selected pattern number in C:\Lavaus\Kuviot\",,,
			PatternAlreadyExist = 10, // "The selected pattern number already exists.",,,
			SelectAtleastOne = 11, // "Mark at least one choice from each area.",,,
			LayerOpenFailed = 12, // "Could not open the layer.",,,
			UsePositiveNumbers = 13, // "Input fields must be positive numbers.",,,
			SheetDataFormatError = 14, // "Please input the sheet data in format:<product number><quantity>",,,
			FillAllFields = 15, // "Please fill in all the fields.",,,
			PrintingFailed = 16, // "Printing failed.",,,
			IPAddrError = 17, // "The IP-address is not valid. Please input the address in format X.X.X.X where X is a number in 0-255.",,,
			LayerSendFailed = 18, // "Could not send new layer count to robot. Layer count was not a number.",,,
			SpeedSendFailed = 19, // "Could not send new speed and delay values to robot. All values were not numbers.",,,
			DbLoadFailed = 20, // "Loading from database failed.",,,
			CheckBoxOutOfRange = 21, // "There were not enough CheckBox-elements for a section.",,,
			PalletPlaceOutOfRange = 22, // "There are not enough PlaceBoxes to select each available pallet place.",,,
			UnknownTag = 23, // "Tag was not found:",,,
			ChoosePalletType = 24, // "Choose pallet type",,,
			NoPermission = 25, // "No permission",,,"No permission"
			NotAllowedPattern = 26,
			InfeedTrackAlreadyStarted = 27,
			PalletPlaceAlreadyStarted = 28,
			MixedPalletChooseOther = 29,
			NoImageFile = 30, //
			MixedPalletSameBox = 31,
			ProductionStartError = 32,
			NothingToDelete = 33,
			RobotIdAlreadyExist = 34,
			UnknownRobotId = 35,
			NoAllowedInfeedTracks = 36,
			Info = 37
		};

		public enum Screens
		{
			Overview = 0,
			Recipe,
			Robots,
			Diagnostics,
			Manual,
			Production,
			Alarms,
			Settings
		};

		private readonly object lockme = new object();

		private readonly bool traceall = false;
		public bool TraceAll { get { return traceall; } }

		/// <summary>
		/// Sekunteja mittaava apuajastin, joka käynnistyy, 
		/// kun logiikkayhteys on muodostettu
		/// </summary>
		public int AppStart_Timer = 0;
		/// <summary>
		/// Ohjelman käynnistyttyä kerran tehtäville asioille
		/// </summary>
		private bool kerran = false;

		public List<string> __Log = new List<string>();

		/// <summary>
		/// Suorittaa asioita sekunnin välein. Suorittaa kerran sovelluksen
		/// avautuessa tehtävät asiat kerran-bitin avulla.
		/// </summary>
		/// <param name="sender">SystemTagSecond</param>
		void SystemTagSecond_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			if (kerran == false)
			{
				// jos näkymänvalinta ei ole käytössä, pitää paneli asettaa 'käsin', jotta valikot toimivat
				NakymanValinta();
				//Globals.Tags.Settings_PanelNumber.SetAnalog(1);

				//Neo.ApplicationFramework.Generated.S7_PLC_HMI2 = Globals.GetObjects<Neo.ApplicationFramework.Generated.S7_PLC_HMI2>();

				//retrieve the last date project.zip was modified i.e. when it was last transferred to the panel
				DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory());

				if (di.GetFiles("project.zip").Length > 0)
					HMI_Overview_moddate.SetString(di.GetFiles("project.zip")[0].LastWriteTime.ToString());
				else
				{
					string name = Assembly.GetExecutingAssembly().Location;
					try
					{
						FileInfo fi = new FileInfo(name);
						HMI_Overview_moddate.SetString(fi.LastAccessTime.ToString());
					}
					catch (Exception)
					{
						HMI_Overview_moddate.SetString("N/A");
					}
				}
				Log("Application Started");

				AppStart_Timer = 0;

				// Main menu painikkeiden visualisointi alustus
				HMI_MainMenu_BtnAnim.SetAnalog(0);

				kerran = true;
			}
		}

		void HMI_Comm_Watchdog_From_PLC1_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			AppStart_Timer++;
		}

		#region tags

		/// <summary>
		/// Hakee tagin nimen perusteella.
		/// </summary>
		/// <param name="TagName">Tagin nimi</param>
		/// <returns>Tagi IBasicTag-interfacena.</returns>
		/// <exception cref="ConfigurationFaultException">Tagia ei löydy.</exception>
		public IBasicTag GetTag(string TagName)
		{
			lock (lockme)
			{
				GlobalDataItem g = (GlobalDataItem)Globals.Tags.GlobalDataItems.FirstOrDefault(i => i.Name == TagName);
				if (g != null)
				{
					//Log("GetTag: "+TagName);
					return g;
				}

				LightweightTag l = (LightweightTag)Globals.Tags.LightweightTags.FirstOrDefault(i => i.Name == TagName);
				if (l != null)
				{
					//Log("GetLightWeightTag: "+TagName);
					return l;
				}

				Log(string.Format("Unknown tag: [{0}]", TagName));

				return null;
			}
		}

		/// <summary>
		/// Hakee int arvon tagin nimen perusteella.
		/// </summary>
		/// <param name="TagName">Tagin nimi</param>
		/// <returns>Tagi IBasicTag-interfacena.</returns>
		/// <exception cref="ConfigurationFaultException">Tagia ei löydy.</exception>
		public int GetTagValueInt(string TagName)
		{
			lock (lockme)
			{
				GlobalDataItem g = (GlobalDataItem)Globals.Tags.GlobalDataItems.FirstOrDefault(i => i.Name == TagName);
				if (g != null)
				{
					//Log("GetTag: "+TagName);
					return g.Value.Int;
				}

				LightweightTag l = (LightweightTag)Globals.Tags.LightweightTags.FirstOrDefault(i => i.Name == TagName);
				if (l != null)
				{
					//Log("GetLightWeightTag: "+TagName);
					return l.Value.Int;
				}

				Log(string.Format("Unknown tag: [{0}]", TagName));

				return 0;
			}
		}

		/// <summary>
		/// Hakee string arvon tagin nimen perusteella.
		/// </summary>
		/// <param name="TagName">Tagin nimi</param>
		/// <returns>Tagi IBasicTag-interfacena.</returns>
		/// <exception cref="ConfigurationFaultException">Tagia ei löydy.</exception>
		public string GetTagValueString(string TagName)
		{
			lock (lockme)
			{
				GlobalDataItem g = (GlobalDataItem)Globals.Tags.GlobalDataItems.FirstOrDefault(i => i.Name == TagName);
				if (g != null)
				{
					//Log("GetTag: "+TagName);
					return g.Value.String;
				}

				LightweightTag l = (LightweightTag)Globals.Tags.LightweightTags.FirstOrDefault(i => i.Name == TagName);
				if (l != null)
				{
					//Log("GetLightWeightTag: "+TagName);
					return l.Value.String;
				}

				Log(string.Format("Unknown tag: [{0}]", TagName));

				return "";
			}
		}

		/// <summary>
		/// Hakee tagin arvon sen nimen perusteella.
		/// </summary>
		/// <param name="TagName">Tagin nimi</param>
		/// <returns>Tagin arvo.</returns>
		/// <exception cref="ConfigurationFaultException">Tagia ei löydy.</exception>
		public VariantValue GetTagValue(string TagName)
		{
			lock (lockme)
			{
				GlobalDataItem g = (GlobalDataItem)Globals.Tags.GlobalDataItems.FirstOrDefault(i => i.Name == TagName);
				if (g != null)
				{
					return g.Value;
				}

				LightweightTag l = (LightweightTag)Globals.Tags.LightweightTags.FirstOrDefault(i => i.Name == TagName);
				if (l != null)
				{
					return l.Value;
				}

				Log(string.Format("Unknown tag: [{0}]", TagName));

				return null;
			}
		}

		/// <summary>
		/// Hakee tagin arvot nimen perusteella.
		/// </summary>
		/// <param name="TagName">Tagin nimi</param>
		/// <returns>Arvot taulukkona. Null jos tagia ei löydy.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Tagin nimellä löytyi Lightweight-tagi</exception>		/// <exception cref="ConfigurationFaultException">Tagia ei löydy.</exception>
		/// <exception cref="ConfigurationFaultException">Tagia ei löydy.</exception>
		public VariantValue[] GetTagValues(string TagName)
		{
			lock (lockme)
			{
				GlobalDataItem g = (GlobalDataItem)Globals.Tags.GlobalDataItems.FirstOrDefault(i => i.Name == TagName);
				if (g != null)
				{
					return g.Values;
				}

				LightweightTag l = (LightweightTag)Globals.Tags.LightweightTags.FirstOrDefault(i => i.Name == TagName);
				if (l != null)
				{
					// Voiko lightweight tagilla olla monta arvoa?
					Log(string.Format("GetTagValues - LightweightTag -  tag: [{0}]", TagName));
					//throw new ArgumentOutOfRangeException("GetTagValues - LightweightTag - TagName");
					return null;
				}

				Log(string.Format("Unknown tag: [{0}]", TagName));

				return null;
			}
		}

		/// <summary>
		/// Etsii tagin nimen perusteella ja astettaa sille arvon.
		/// </summary>
		/// <param name="TagName">Tagin nimi</param>
		/// <param name="v">Tagin uusi arvo</param>
		/// <exception cref="ArgumentException">Tagia ei löydy.</exception>
		public void SetTagValue(string TagName, VariantValue v)
		{
			lock (lockme)
			{
				// Tarkista onko perinteinen tagi
				GlobalDataItem g = (GlobalDataItem)Globals.Tags.GlobalDataItems.FirstOrDefault(i => i.Name == TagName);
				if (g != null)
				{
					if (g.DataType == Interop.DataSource.BEDATATYPE.DT_STRING)
					{
						g.SetString(v);
					}
					else if (g.DataType == Interop.DataSource.BEDATATYPE.DT_BOOLEAN)
					{
						if (v.Bool)
						{
							g.SetTag();
						}
						else
						{
							g.ResetTag();
						}
					}
					else if (g.DataType == Interop.DataSource.BEDATATYPE.DT_DATETIME)
					{
						g.Value = v.DateTime;
					}
					else
					{
						g.SetAnalog(v);
					}

					return;
				}

				// Tarkista onko iX 2.2 tagi
				LightweightTag l = (LightweightTag)Globals.Tags.LightweightTags.FirstOrDefault(i => i.Name == TagName);
				if (l != null)
				{
					// VoikoLightweightTag olla string tyyppiä?
					l.SetAnalog(v);
					return;
				}

				// Jos tultiin tänne niin tagia ei löytynyt
				Log(string.Format("Unknown tag: [{0}]", TagName));

				return;
			}
		}

		#endregion

		/// <summary>
		/// IX inner log
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public void Log(string s)
		{
			while (__Log.Count > 1000)
			{
				__Log.RemoveAt(0);
			}

			string msg = string.Format("{0}: {1}", DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss.fff"), s);
			// Write to iX log
			__Log.Add(msg);
			// Write to DebugView or any trace listener
			System.Diagnostics.Trace.WriteLine("[iX] " + msg);
		}

		/// <summary>
		/// BtnHandler / ShowScreen
		/// Method generate a screen id of showed screen by method argument
		/// </summary>
		public void BtnHandler(int panelno, Screens screen, string btn_name, int length)
		{
			lock (lockme)
			{
				if (ScreenChangePending.Value.Bool == true)
				{
					Log("Screen change pending");
					return;
				}

				string msg = string.Format("ShowScreen {0} button: {1}", screen, btn_name);
				if (TraceAll)
				{
					Log(msg);
					System.Diagnostics.Trace.WriteLine(msg);
				}

				// Jos napissa ei ole tekstiä eli 'ei ole käytössä' lopetetaan tähän
				if (length == 0) return;

				try
				{
					string aux = "";

					// Erotetaan napin nimestä numero
					for (int i = 0; i < btn_name.Length; i++)
					{
						if (Char.IsDigit(btn_name[i]))
							aux += btn_name[i];
					}

					int num = Convert.ToInt16(aux);
					if (Globals.Tags.HMI_SubMenu_BtnAnim.Value.Int == num) return;

					int screenid = panelno * 10000;
					screenid += ((int)screen * 100);
					screenid += num;

					HMI_SubMenu_BtnAnim.SetAnalog(num);
					ScreenChangePending.SetTag();

					SystemTagNewScreenId.SetAnalog(screenid);
				}
				catch (Exception x)
				{
					Log(string.Format("ShowScreen {0} button: {1}. Exception: {2}", screen, btn_name, x.Message));
				}
			}
		}

		/// <summary>
		/// Valitaan näytettävä paneli
		///  - valintaikkuna näytetään jos panelinumeroa ei ole asetettu (koodissa, "tag-initial value" jne.)
		/// </summary>
		/// <returns></returns>
		void NakymanValinta()
		{
			try
			{
				if (!Globals._Konfiguraatio.ReadOk) Globals._Konfiguraatio.Read();
			}
			catch (Exception x)
			{
				Log(x.Message);
			}

			Globals.Tags.Settings_PanelNumber.SetAnalog(Globals._Konfiguraatio.CurrentConfig.PanelNo);

			int no = (int)Globals.Tags.Settings_PanelNumber.Value;
			if (no < 1)
			{
				Globals.ScreenSelection.Show();
			}
			else
			{
				try
				{
					SystemTagNewScreenId.SetAnalog((10000 * no) + 1);
				}
				catch (Exception x)
				{
					Log(x.Message);
					SystemTagNewScreenId.SetAnalog(10001);
				}
			}
		}

		void SystemTagCurrentUser_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			// Kirjoitetaan kulloisen käyttäjätason integer -arvo
			switch (SystemTagCurrentUser.Value.ToString())
			{
				case "Administrator":
					CurrentUserInt.Value = 3;
					break;
				case "Supervisor":
					CurrentUserInt.Value = 2;
					break;
				case "Operator":
					CurrentUserInt.Value = 1;
					break;
				default:
					CurrentUserInt.Value = 0;
					break;
			}
		}

		void Line1_PLC_Auto_Area_Mode_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			lock (lockme)
			{
				int id = -1;
				int value = -1;
				bool reset = false;
				string name = ((IBasicTag)sender).Name;

				//System.Diagnostics.Trace.WriteLine(name);

				if (name.StartsWith("Line1_Manual_Area_Enabled_"))
				{
					bool sval = (bool)GetTagValue(name);
					string aux = name.Substring(26); // length of 'Line1_Manual_Area_Enabled_'

					if (int.TryParse(aux, out id))
					{
						string tagname = string.Format("Line1_PLC_Auto_Area_Mode_C{0}", id);
						value = (int)GetTagValue(tagname);
					}
					else
					{
						string s = "Error in tagname" + name;
						System.Diagnostics.Trace.WriteLine(s);
						Log(s);
					}
				}
				else if (name.StartsWith("Line1_PLC_Auto_Area_Mode_C"))
				{
					string aux = name.Substring(26); // length of 'Line1_PLC_Auto_Area_Mode_C'
					if (int.TryParse(aux, out id))
					{
						value = (int)GetTagValue(name);

						string tagname = string.Format("Line1_Manual_Area_Enabled_{0}", id);
						if (value == 0) SetTagValue(tagname, false);
						reset = (bool)GetTagValue(name) == false;
					}
					else
					{
						string s = "Ei ole numero" + aux;
						System.Diagnostics.Trace.WriteLine(s);
						Log(s);
					}
				}

				if (id >= 0)
				{
					string tagname = string.Format("Line1_Internal_ManualEnabled_{0}", id);
					IBasicTag tag = GetTag(tagname);
					if (TraceAll) System.Diagnostics.Trace.WriteLine(string.Format("set : {0} = value = {1}. reset = [{2}]", tagname, value, reset));
					if (tag != null)
					{
						if (value >= 50 || reset)
						{
							tag.ResetTag();
							Line1_HMI1_ManualCtrlNr.ResetTag();
						}
						else
							tag.SetTag();
					}
				}
			}
		}
		
		void SystemTagNewScreenId_ValueChangeOrError(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			Globals.Tags.ScreenChangePending.ResetTag();
		}
	}
}
