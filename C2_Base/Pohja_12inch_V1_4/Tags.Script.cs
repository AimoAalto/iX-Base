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
		
		public enum Screens	{
			Overview = 0, 
			Recipe, 
			Robots, 
			Diagnostics,
			Manual, 
			Production,
			Alarms,
			Settings
		};  
		
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
			if(kerran == false)
			{	
				NakymanValinta();
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
				Menu_MainMenu_Btn_Anim.SetAnalog(0);

				kerran = true;
			}
		}	
		
		/// <summary>
		/// Kasvattaa apuajastinta, kun logiikkayhteys on muodostettu.
		/// </summary>
		/// <param name="sender">Line1_PLC_SystemsSecond</param>
		void Line1_PLC_SystemsSecond_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			AppStart_Timer++;
		}
			
		/// <summary>
		/// Hakee tagin nimen perusteella.
		/// </summary>
		/// <param name="TagName">Tagin nimi</param>
		/// <returns>Tagi IBasicTag-interfacena.</returns>
		/// <exception cref="ConfigurationFaultException">Tagia ei löydy.</exception>
		public IBasicTag GetTag(string TagName)
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

		/// <summary>
		/// Hakee int arvon tagin nimen perusteella.
		/// </summary>
		/// <param name="TagName">Tagin nimi</param>
		/// <returns>Tagi IBasicTag-interfacena.</returns>
		/// <exception cref="ConfigurationFaultException">Tagia ei löydy.</exception>
		public int GetTagValueInt(string TagName)
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

		/// <summary>
		/// Hakee string arvon tagin nimen perusteella.
		/// </summary>
		/// <param name="TagName">Tagin nimi</param>
		/// <returns>Tagi IBasicTag-interfacena.</returns>
		/// <exception cref="ConfigurationFaultException">Tagia ei löydy.</exception>
		public string GetTagValueString(string TagName)
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
		
		/// <summary>
		/// Hakee tagin arvon sen nimen perusteella.
		/// </summary>
		/// <param name="TagName">Tagin nimi</param>
		/// <returns>Tagin arvo.</returns>
		/// <exception cref="ConfigurationFaultException">Tagia ei löydy.</exception>
		public VariantValue GetTagValue(string TagName)
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
		
		/// <summary>
		/// Hakee tagin arvot nimen perusteella.
		/// </summary>
		/// <param name="TagName">Tagin nimi</param>
		/// <returns>Arvot taulukkona. Null jos tagia ei löydy.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Tagin nimellä löytyi Lightweight-tagi</exception>		/// <exception cref="ConfigurationFaultException">Tagia ei löydy.</exception>
		/// <exception cref="ConfigurationFaultException">Tagia ei löydy.</exception>
		public VariantValue[] GetTagValues(string TagName)
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
				Globals.Robotit.robotit[1].Loki.LisaaLokiin("GetTagValues - LightweightTag - TagName");
				//throw new ArgumentOutOfRangeException("GetTagValues - LightweightTag - TagName");                
			}

			Log(string.Format("Unknown tag: [{0}]", TagName));
			
			return null;
		}	
		
		/// <summary>
		/// Etsii tagin nimen perusteella ja astettaa sille arvon.
		/// </summary>
		/// <param name="TagName">Tagin nimi</param>
		/// <param name="v">Tagin uusi arvo</param>
		/// <exception cref="ArgumentException">Tagia ei löydy.</exception>
		public void SetTagValue(string TagName, VariantValue v)
		{
			// Tarkista onko perinteinen tagi
			GlobalDataItem g = (GlobalDataItem)Globals.Tags.GlobalDataItems.FirstOrDefault(i => i.Name == TagName);
			if (g != null)
			{                
				if(g.DataType == Interop.DataSource.BEDATATYPE.DT_STRING)
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

		public Neo.ApplicationFramework.Interfaces.IScreenAdapter GetProperty(string name)
		{
			Type type = typeof(Globals); // MyClass is static class with static properties
			foreach (var p in type.GetProperties())
			{
				if(p.Name == name)
				{
					var v = p.GetValue(null, null); // static classes cannot be instanced, so use null...
					return (Neo.ApplicationFramework.Interfaces.IScreenAdapter)v;
				}
			}
			return null;
		}	
		
		/// <summary>
		/// BtnHandler / ShowScreen
		/// Method generate a screen id of showed screen by method argument
		/// </summary>
		public void BtnHandler(int panelno, Screens screen, string btn_name, int length)
		{
			string msg = string.Format("ShowScreen {0} button: {1}", screen, btn_name);
			Log(msg);
			System.Diagnostics.Trace.WriteLine(msg);

			// Jos napissa ei ole tekstiä eli 'ei ole käytössä' lopetetaan tähän
			if (length == 0) return;
			
			try 
			{
				string aux = "";
			
				// Erotetaan napin nimestä numero
				for (int i = 0; i < btn_name.Length; i++){
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
		
		void NakymanValinta()
		{
			Globals._Konfiguraatio.ReadSettings();
				
			if(Globals.Tags.Settings_PanelNumber.Value < 1)
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
		
				

		void Line1_PLC_Auto_Area_Mode_C1_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			if(Line1_PLC_Auto_Area_Mode_C1.Value < 50)
			{
				Line1_Internal_ManualEnabled_1.Value = true;
			}
			else
			{
				Line1_Internal_ManualEnabled_1.Value = false;
			}
		}
		
		void Line1_PLC_Auto_Area_Mode_C2_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			if(Line1_PLC_Auto_Area_Mode_C2.Value < 50)
			{
				Line1_Internal_ManualEnabled_2.Value = true;
			}
			else
			{
				Line1_Internal_ManualEnabled_2.Value = false;
			}
		}
		
		void Line1_PLC_Auto_Area_Mode_C3_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			if(Line1_PLC_Auto_Area_Mode_C3.Value < 50)
			{
				Line1_Internal_ManualEnabled_3.Value = true;
			}
			else
			{
				Line1_Internal_ManualEnabled_3.Value = false;
			}
		}
		
		void Line1_PLC_Auto_Area_Mode_C4_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			if(Line1_PLC_Auto_Area_Mode_C4.Value < 50)
			{
				Line1_Internal_ManualEnabled_4.Value = true;
			}
			else
			{
				Line1_Internal_ManualEnabled_4.Value = false;
			}
		}
		
		void Line1_PLC_Auto_Area_Mode_C5_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			if(Line1_PLC_Auto_Area_Mode_C5.Value < 50)
			{
				Line1_Internal_ManualEnabled_5.Value = true;
			}
			else
			{
				Line1_Internal_ManualEnabled_5.Value = false;
			}
		}
		
		void Line1_Manual_Area_Enabled_6_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			if(Line1_PLC_Auto_Area_Mode_C6.Value < 50)
			{
				Line1_Internal_ManualEnabled_6.Value = true;
			}
			else
			{
				Line1_Internal_ManualEnabled_6.Value = false;
			}
		}
		
		void Line1_Manual_Area_Enabled_7_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			if(Line1_PLC_Auto_Area_Mode_C7.Value < 50)
			{
				Line1_Internal_ManualEnabled_7.Value = true;
			}
			else
			{
				Line1_Internal_ManualEnabled_7.Value = false;
			}
		}
		
		void Line1_Manual_Area_Enabled_8_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			if(Line1_PLC_Auto_Area_Mode_C8.Value < 50)
			{
				Line1_Internal_ManualEnabled_8.Value = true;
			}
			else
			{
				Line1_Internal_ManualEnabled_8.Value = false;
			}
		}
    }
}
