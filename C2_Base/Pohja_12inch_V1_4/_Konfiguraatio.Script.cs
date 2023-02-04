namespace Neo.ApplicationFramework.Generated
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Neo.ApplicationFramework.Tools.OpcClient;
	
	
    
	/// <summary>
	/// Sisältää järjestelmän konfiguraation määrittelyn taustatoimintoja varten. 
	/// <list type="bullet">
	/// <listheader>
	/// <description>Määritelmät:</description>
	/// </listheader>
	/// <item>
	/// <description>Logiikkojen määrä.</description>
	/// </item>
	/// <item>
	/// <description>JSON-tallennustiedostojen polut.</description>
	/// </item>
	/// <item>
	/// <description>Mikä tulorata/lavapaikka kuuluu millekin robotille. Luo yhteyden robotin ja järjestelmän käyttämien numeroiden välille.</description>
	/// </item>
	/// <item>
	/// <description>Mitä lavatyyppejä on käytössä ja milla numeroilla niitä käsitellään.</description>
	/// </item>
	/// </list>
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 5.4.2018</remarks>
	public partial class _Konfiguraatio
	{
		public class Progress : EventArgs
		{
			public string Status { get; private set; }

			private Progress() {}

			public Progress(string status)
			{
				Status = status;
			}
		}
		
		public class Settings
		{
			// Project parameter
			// - How to add parameter!
			// - 1. add variable and property to this class. Exp. panel_number and PanelNumber
			// - 2. Create new tag named Setting_property_name to IX tag list. Exp PanelNumber
			//-------------------------
			
			// Param
			//public event System.EventHandler PanelNumberChanged;
			private int panel_number;		
			public int PanelNumber
			{
				set { this.panel_number = value; }
				get { return this.panel_number; }
			}
			
			public Settings()
			{
				panel_number = 0;// PanelNames.pn_UNKNOWN;
			}

		}
		
		public enum man_mode {ManNumber, ManMultiString};
		
		/// <summary>
		/// Logiikkojen määrä järjestelmässä. Ei ollut saatavilla järkevää tapaa
		/// laskea määritettyjen logiikkojen määrää projektista.
		/// </summary>
		public static readonly int Logiikkoja = 1;
		
		/// <summary>
		/// Kansio
		/// </summary>
		static readonly string Kansiot_Path = @"C:\Lavaus\";
		
		/// <summary>
		/// Polku,johon aikaväliasetukset on tallennettu.
		/// </summary>
		static readonly string AikavaliAsetukset_Path = Kansiot_Path+@"Asetukset\Aikavali_Asetukset.json";
		
		/// <summary>
		/// Polku,johon robotin tuloratatiedot on tallennettu.
		/// </summary>
		static readonly string RobotinTuloradatAsetukset_Path = Kansiot_Path+@"Asetukset\RobotinTuloradat_Asetukset.json";
		
		/// <summary>
		/// Polku,johon robotin lavapaikkatiedot on tallennettu.
		/// </summary>
		static readonly string RobotinLavapaikatAsetukset_Path = Kansiot_Path+@"Asetukset\RobotinLavapaikat_Asetukset.json";
		
		/// <summary>
		/// Polku,johon lavatyypit on tallennettu.
		/// </summary>
		static readonly string Lavatyypit_Path = Kansiot_Path+@"Asetukset\Lavatyypit_Asetukset.json";
		
		/// <summary>
		/// Polku, johon kuvioiden rajoitteet on tallennettu.
		/// </summary>
		public static readonly string Kuviotietolista_Path = Kansiot_Path+@"Kuviot\Kuviot.json";

		/// <summary>
		/// Polku, johon paneelikohtaiset asetukset tallennetaan
		/// </summary>
		public static readonly string Settings_Path = Kansiot_Path+@"Asetukset\Settings.json";
		
		/// <summary>
		/// Polku, johon kuvioiden rajoitteet on tallennettu.
		/// </summary>
		public static man_mode ManualMode = man_mode.ManNumber;
				
		/// <summary>
		/// Robottien tuloradat ja robotin niistä käyttämät numerot.
		/// [robotti numero,[asiakas tulorataNro, robotin sisäinen TulorataNro]]
		/// Huom. Solukohtaiset muutokset tehdään RobotinTuloradat_Asetukset.json tiedostoon joka löytyy paneelista!!!
		/// 				{1, 1}, {2, 2}
		/// </summary>
		public static Dictionary<int, Dictionary<int, int>> RobotinTuloradat = new Dictionary<int, Dictionary<int, int>>() { 
			{ 1, new Dictionary<int, int>() {
				{1, 1}
				}
			}
		}; 

		/// <summary>
		/// Robottien lavapaikat ja robotin niistä käyttämät numerot.
		/// [robotti numero,[asiakas lavapNro, robotin sisäinen LavapNro]}
		/// Huom. Solukohtaiset muutokset tehdään RobotinLavapaikat_Asetukset.json tiedostoon joka löytyy paneelista!!!
		/// </summary>
		public static Dictionary<int, Dictionary<int, int>> RobotinLavapaikat = new Dictionary<int, Dictionary<int, int>>() {
			{ 1, new Dictionary<int, int>() {
				{1, 1}
				}
			}}; 
		
		/// <summary>
		/// Projektissa käytettävät lavatyypit.
		/// Huom. Solukohtaiset muutokset tehdään Lavatyypit_Asetukset.json tiedostoon joka löytyy paneelista!!!
		/// </summary>
		public static Dictionary<int, string> Lavatyypit = new Dictionary<int, string>() {
			{ 1, "EUR" },
			{ 2, "FIN" }
		};

		public static Dictionary<string, int> Aikavalit = new Dictionary<string, int>() {
			{"Ajotiedot", 10000},
			{"RobottiWatchdog", 1000},
			{"LogiikkaWatchdog", 1000},
			{"SensorInfoUpdate", 1000}
		};

		public Settings settings = new Settings();

		/// <summary>
		/// Kirjoitetaan parametrit
		/// </summary>
		public void SaveSettings(){
			
			JsonSerialization.WriteToJsonFile<Settings>(Settings_Path, settings);
		}

		/// <summary>
		/// Luetaan parametrit
		/// </summary>
		public Settings ReadSettings(){
			
			Settings uudetasetukset;
			try
			{
				uudetasetukset = JsonSerialization.ReadFromJsonFile<Settings>(Settings_Path);
				// Save json to tags
				foreach(System.Reflection.PropertyInfo prop in uudetasetukset.GetType().GetProperties()) 
				{					
					try
					{
						var val = prop.GetValue(uudetasetukset,null);
						Globals.Tags.GetTag("Settings_"+prop.Name).Value = val;
						Globals.Tags.Log("Read settings "+prop.Name+" val "+val.ToString());
					}
					catch
					{
					}
				}
			}
			catch
			{
				// Tiedostoa ei vielä ole. Luodaan uusi
				uudetasetukset = null;
			}
							
			if (uudetasetukset == null)
			{
				SaveSettings();
				uudetasetukset = new Settings();
			}
		
			return uudetasetukset;
		}

		void ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			// Write all tag values to settings dictionary and save dictionary to JSON
			foreach(var prop in settings.GetType().GetProperties()) 
			{
				try				
				{
					if(prop.PropertyType == typeof(int))
					{
						try
						{
							int val = Globals.Tags.GetTagValueInt("Settings_"+prop.Name);
							prop.SetValue(settings, val, null);
						}
						catch{
									
						}
					}
					else if(prop.PropertyType == typeof(string))
					{
						try
						{
							string val = Globals.Tags.GetTagValueString("Settings_"+prop.Name);
							prop.SetValue(settings, val, null);
						}
						catch{
											
						}
					}
				}
				catch(Exception ex)
				{
					Globals.Tags.Log(ex.ToString()+". Error when change setting! Tag: "+prop.Name);
				}					
			}
			
			SaveSettings();
		} 
		
		/// <summary>
		/// Lataa aika-asetukset sovelluksen käynnistyessä.
		/// </summary>
		/// <param name="sender">_Konfiguraatio</param>
		void _Konfiguraatio_Created(System.Object sender, System.EventArgs e)
		{
			// Luo peruskansio jos ei olemassa
			if (!Directory.Exists(Kansiot_Path))
				Directory.CreateDirectory(Kansiot_Path);
	
			// Ladataan Robotin tuloradat
			//-----------------------------------
			Dictionary<int, Dictionary<int, int>> uudetRobotinTuloradat;
			try
			{
				uudetRobotinTuloradat = JsonSerialization.ReadFromJsonFile<Dictionary<int, Dictionary<int, int>>>(RobotinTuloradatAsetukset_Path);
			}
			catch
			{
				// Tiedostoa ei vielä ole. Luodaan uusi
				uudetRobotinTuloradat = null;
			}
			
			if (uudetRobotinTuloradat == null)
			{
				// Luo kansio jos ei olemassa
				if (!Directory.Exists(RobotinTuloradatAsetukset_Path))
					Directory.CreateDirectory(System.IO.Path.GetDirectoryName(RobotinTuloradatAsetukset_Path));
				// Tiedostossa ei ollut mitään tai sitä ei ollut
				JsonSerialization.WriteToJsonFile<Dictionary<int, Dictionary<int, int>>>(RobotinTuloradatAsetukset_Path, RobotinTuloradat);
			}
			else
			{
				RobotinTuloradat = uudetRobotinTuloradat;
			}

			// Ladataan Robotin lavapaikat
			//-----------------------------------
			Dictionary<int, Dictionary<int, int>> uudetRobotinLavapaikat;
			try
			{
				uudetRobotinLavapaikat = JsonSerialization.ReadFromJsonFile<Dictionary<int, Dictionary<int, int>>>(RobotinLavapaikatAsetukset_Path);
			}
			catch
			{
				// Tiedostoa ei vielä ole. Luodaan uusi
				uudetRobotinLavapaikat = null;
			}
					
			if (uudetRobotinLavapaikat == null)
			{
				// Tiedostossa ei ollut mitään tai sitä ei ollut
				// Luo kansio jos ei olemassa
				if (!Directory.Exists(RobotinLavapaikatAsetukset_Path))
					Directory.CreateDirectory(System.IO.Path.GetDirectoryName(RobotinLavapaikatAsetukset_Path));
		
				JsonSerialization.WriteToJsonFile<Dictionary<int, Dictionary<int, int>>>(RobotinLavapaikatAsetukset_Path, RobotinLavapaikat);
			}
			else
			{
				RobotinLavapaikat = uudetRobotinLavapaikat;
			}
	
			// Ladataan Lavatyypit
			//-----------------------------------
			Dictionary<int, string> uudetLavatyypit;
			try
			{
				uudetLavatyypit = JsonSerialization.ReadFromJsonFile<Dictionary<int, string>>(Lavatyypit_Path);
			}
			catch
			{
				// Tiedostoa ei vielä ole. Luodaan uusi
				uudetLavatyypit = null;
			}
							
			if (uudetLavatyypit == null)
			{
				// Tiedostossa ei ollut mitään tai sitä ei ollut
				// Luo kansio jos ei olemassa
				if (!Directory.Exists(Lavatyypit_Path))
					Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Lavatyypit_Path));
				JsonSerialization.WriteToJsonFile<Dictionary<int, string>>(Lavatyypit_Path, Lavatyypit);
			}
			else
			{
				Lavatyypit = uudetLavatyypit;
			}

			// Ladataan aikaväliasetukset
			//-----------------------------------
			Dictionary<string, int> uudetAikavalit;
			try
			{
				uudetAikavalit = JsonSerialization.ReadFromJsonFile<Dictionary<string, int>>(AikavaliAsetukset_Path);
			}
			catch
			{
				// Tiedostoa ei vielä ole. Luodaan uusi
				uudetAikavalit = null;
			}
					
			if (uudetAikavalit == null)
			{
				// Luo kansio jos ei olemassa
				if (!Directory.Exists(AikavaliAsetukset_Path))
					Directory.CreateDirectory(System.IO.Path.GetDirectoryName(AikavaliAsetukset_Path));
				
				// Tiedostossa ei ollut mitään tai sitä ei ollut
				JsonSerialization.WriteToJsonFile<Dictionary<string, int>>(AikavaliAsetukset_Path, Aikavalit);
			}
			else
			{
				Aikavalit = uudetAikavalit;
			}
	
			// Ladataan Parametrit
			//-----------------------------------
			// Luo kansio jos ei olemassa
			if (!Directory.Exists(Settings_Path))
				Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Settings_Path));
	
			settings = ReadSettings();
			

			// Create event to update file if settings Tag value changed
			foreach(var prop in settings.GetType().GetProperties()) 
			{					
				try
				{
					Globals.Tags.GetTag("Settings_"+prop.Name).ValueChange += ValueChange;
				}
				catch
				{
				}
			}
		}
	}
}
