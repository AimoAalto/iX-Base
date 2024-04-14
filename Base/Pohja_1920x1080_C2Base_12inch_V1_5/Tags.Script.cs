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
		#region variables

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

		private int pendingcount = 0;
		public bool TraceAll { get { return HMI_TraceAll.Value; } }

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

		#endregion

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
				// NakymanValinta();
				HMI_Settings_PanelNumber.SetAnalog(1);

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
				Menu_MainMenu_Btn_Anim.SetAnalog(0);

				kerran = true;
			}

			bool screenchanging = ScreenChangePending.Value;
			if (screenchanging)
			{
				if (pendingcount++ > 2)
				{
					pendingcount = 0;
					ScreenChangePending.ResetTag();
				}
			}
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
				GlobalDataItem g = (GlobalDataItem)GlobalDataItems.FirstOrDefault(i => i.Name == TagName);
				if (g != null)
				{
					//Log("GetTag: "+TagName);
					return g;
				}

				LightweightTag l = (LightweightTag)LightweightTags.FirstOrDefault(i => i.Name == TagName);
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
				GlobalDataItem g = (GlobalDataItem)GlobalDataItems.FirstOrDefault(i => i.Name == TagName);
				if (g != null)
				{
					//Log("GetTag: "+TagName);
					return g.Value.Int;
				}

				LightweightTag l = (LightweightTag)LightweightTags.FirstOrDefault(i => i.Name == TagName);
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
				GlobalDataItem g = (GlobalDataItem)GlobalDataItems.FirstOrDefault(i => i.Name == TagName);
				if (g != null)
				{
					//Log("GetTag: "+TagName);
					return g.Value.String;
				}

				LightweightTag l = (LightweightTag)LightweightTags.FirstOrDefault(i => i.Name == TagName);
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
				GlobalDataItem g = (GlobalDataItem)GlobalDataItems.FirstOrDefault(i => i.Name == TagName);
				if (g != null)
				{
					return g.Value;
				}

				LightweightTag l = (LightweightTag)LightweightTags.FirstOrDefault(i => i.Name == TagName);
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
		/// <exception cref="ArgumentOutOfRangeException">Tagin nimellä löytyi Lightweight-tagi</exception>
		/// <exception cref="ConfigurationFaultException">Tagia ei löydy.</exception>
		public VariantValue[] GetTagValues(string TagName)
		{
			lock (lockme)
			{
				GlobalDataItem g = (GlobalDataItem)GlobalDataItems.FirstOrDefault(i => i.Name == TagName);
				if (g != null)
				{
					return g.Values;
				}

				LightweightTag l = (LightweightTag)LightweightTags.FirstOrDefault(i => i.Name == TagName);
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
				if (TraceAll)
					System.Diagnostics.Trace.WriteLine(string.Format("SetTagValue : [{0}] {1}", TagName, v));

				// Tarkista onko perinteinen tagi
				GlobalDataItem g = (GlobalDataItem)GlobalDataItems.FirstOrDefault(i => i.Name == TagName);
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
				LightweightTag l = (LightweightTag)LightweightTags.FirstOrDefault(i => i.Name == TagName);
				if (l != null)
				{
					// VoikoLightweightTag olla string tyyppiä?
					l.SetAnalog(v);
					return;
				}

				// Jos tultiin tänne niin tagia ei löytynyt
				Log(string.Format("Unknown tag: [{0}]", TagName));
			}
		}

		#endregion
		
		#region misc

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
		
		public void LogToFile(string s)
		{
			lock (lockme)
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
				try
				{
					System.IO.File.AppendAllText(@"C:\Lavaus\Log.txt", msg + "\n");
				}
				catch (Exception x)
				{
					System.Diagnostics.Trace.WriteLine("[iX Exception] " + x.Message);
				}
			}
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
					if (Menu_SubMenu_Btn_Anim.Value.Int == num) return;

					int screenid = panelno * 10000;
					screenid += ((int)screen * 100);
					screenid += num;

					ScreenChangePending.SetTag();
					SystemTagNewScreenId.SetAnalog(screenid);
					Menu_SubMenu_Btn_Anim.SetAnalog(num);
				}
				catch (Exception x)
				{
					Log(string.Format("ShowScreen {0} button: [{1}]. Exception: {2}\n{3}", screen, btn_name, x.Message, x.InnerException));
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

			HMI_Settings_PanelNumber.SetAnalog(Globals._Konfiguraatio.CurrentConfig.PanelNo);

			int no = (int)HMI_Settings_PanelNumber.Value;
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

		#endregion

		#region events

		public void SystemTagCurrentUser_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			HMI_AdminUser.ResetTag();
			// Kirjoitetaan kulloisen käyttäjätason integer -arvo
			switch (SystemTagCurrentUser.Value.ToString())
			{
				case "Administrator":
					CurrentUserInt.Value = 3;
					HMI_AdminUser.SetTag();
					break;
				case "Supervisor":
					CurrentUserInt.Value = 2;
					HMI_AdminUser.SetTag();
					break;
				case "Operator":
					CurrentUserInt.Value = 1;
					break;
				default:
					CurrentUserInt.Value = 0;
					break;
			}
		}

		bool StartsWith(string name, string basestr, out int id)
		{
			id = -1;
			bool ret = false;
			if (name.StartsWith(basestr))
			{
				string aux = name.Substring(basestr.Length); // length of 'Line1_Manual_Area_Enabled_'
				if (int.TryParse(aux, out id))
				{
					ret = true;
				}
				else
				{
					string s = "Error in tagname " + name;
					Log(s);
				}
			}
			return ret;
		}

		public void Line1_PLC_Auto_Area_Mode_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			lock (lockme)
			{
				int id = -1;
				int value = -1;
				bool reset = false;
				string name = ((IBasicTag)sender).Name;

				//System.Diagnostics.Trace.WriteLine(name);
				if (StartsWith(name, "S7HMI_DB_ToPLC_AutoAreaCMD_ManModeSelection_", out id))
				{
					//S7HMI_DB_ToHMI_AutoAreaST_LineRunningMode_1.ResetTag();
					string tagname = string.Format("S7HMI_DB_ToHMI_AutoAreaST_LineRunningMode_{0}", id);
					value = (int)GetTagValue(tagname);
				}
				else if (StartsWith(name, "S7HMI_DB_ToHMI_AutoAreaST_LineRunningMode_", out id))
				{
					value = (int)GetTagValue(name);
					//HMI_Manual_Area_Enabled_1.ResetTag();
					string tagname = string.Format("HMI_Manual_Area_Enabled_{0}", id);
					if (value == 0) SetTagValue(tagname, false);
					reset = (bool)GetTagValue(name) == false;
				}

				if (id >= 0)
				{
					string tagname = string.Format("HMI_Internal_ManualEnabled_{0}", id);
					IBasicTag tag = GetTag(tagname);
					if (TraceAll) System.Diagnostics.Trace.WriteLine(string.Format("set : {0} = value = {1}. reset = [{2}]", tagname, value, reset));
					if (tag != null)
					{
						if (value == 0 || value >= 50 || reset)
						{
							tag.ResetTag();
							//S7HMI_DB_ToPLC_Manual_ManGroupCode1.ResetTag();
							S7HMI_DB_ToPLC_ManualCtrl_1.ResetTag();
							string stag = string.Format("S7HMI_DB_ToPLC_ManualCtrl_{0}", id);
							SetTagValue(stag, 0);
						}
						else
							tag.SetTag();
					}
				}
			}
		}

		void SystemTagNewScreenId_ValueChangeOrError(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			ScreenChangePending.ResetTag();
		}

		public void HMI_Language_Current_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			// lähetärobotille kielen vaihto viesti
			Globals.Robotit.VaihdaKieli(HMI_Language_Current.Value);
		}

		#endregion
	}
}
