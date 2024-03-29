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

		public enum Screens
		{
			Overview = 0,
			Recipe,
			DriveMode,
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

		void SystemTagSecond_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			if (kerran == false)
			{
				NakymanValinta();
				/*
				//Neo.ApplicationFramework.Generated.S7_PLC_HMI2 = Globals.GetObjects<Neo.ApplicationFramework.Generated.S7_PLC_HMI2>();
				
				//retrieve the last date project.zip was modified i.e. when it was last transferred to the panel
				DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory());

				if (di.GetFiles("project.zip").Length > 0)
					HMI_Overview_moddate.SetString(
						di.GetFiles("project.zip")[0].LastWriteTime.ToString());
				else
				{
					string name = Assembly.GetExecutingAssembly().FullName;
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
				
				AppStart_Timer = 0;

				// Main menu painikkeiden visualisointi alustus
				Menu_MainMenu_Btn_Anim.Value = 1;*/

				kerran = true;
			}

			AppStart_Timer++;
		}

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
		/// BtnHandler / ShowScreen
		/// Method generate a screen id of showed screen by method argument
		/// </summary>
		public void BtnHandler(int panelno, Screens screen, string btn_name, int length)
		{
			lock (lockme)
			{
				if (Globals.Tags.ScreenChangePending.Value.Bool == true)
				{
					Globals.Tags.Log("Screen change pending");
					return;
				}
			
				Log(string.Format("ShowScreen {0} button: {1}", screen, btn_name));

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

					int screenid = panelno * 10000;
					screenid += ((int)screen * 100);
					screenid += num;

					SystemTagNewScreenId.SetAnalog(screenid);
					Menu_SubMenu_Btn_Anim.SetAnalog(num);
				}
				catch (Exception x)
				{
					Log(string.Format("ShowScreen {0} button: {1}. Exception: {2}", screen, btn_name, x.Message));
				}
			}
		}

		void NakymanValinta()
		{
			//Globals._Konfiguraatio.ReadSettings();

			if (Settings_PanelNumber.Value < 1)
			{
				Globals.ScreenSelection.Show();
			}
			else
			{
				SystemTagNewScreenId.SetAnalog(10001);
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
		
		void SystemTagCurrentScreenId_ValueChangeOrError(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			
		}
	}
}
