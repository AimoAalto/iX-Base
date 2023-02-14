namespace Neo.ApplicationFramework.Generated
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	public class PatternInfo
	{
		private List<int> lavatyypit = new List<int>();
		private List<int> tuloradat = new List<int>();
		private List<int> lavapaikat = new List<int>();
		
		/// <summary>
		/// Kuviolle sallitut lavatyypit. Numerojen merkitykset on määritetty
		/// _Konfiguraatioon.
		/// </summary>
		public List<int> Lavatyypit
		{ 
			get { if (lavatyypit == null) lavatyypit = new List<int> (); return lavatyypit; } 
			set { if (value != null) lavatyypit = value; } 
		}
		/// <summary>
		/// Kuviolle sallitut tuloradat.
		/// </summary>
		public List<int> Tuloradat
		{ 
			get { if (tuloradat == null) tuloradat = new List<int> (); return tuloradat; } 
			set { if (value != null) tuloradat = value; } 
		}
		/// <summary>
		/// Kuviolle sallitut lavapaikat.
		/// </summary>
		public List<int> Lavapaikat
		{ 
			get { if (lavapaikat == null) lavapaikat = new List<int> (); return lavapaikat; } 
			set { if (value != null) lavapaikat = value; } 
		}
	}
	
	/// <summary>
	/// RobotConfiguraton class
	/// </summary>
	public class RobotConf
	{
		public int RobotNo { get; set; }
		/// <summary>
		/// Robottien tuloradat ja robotin niistä käyttämät numerot.
		/// [asiakas tulorataNro, robotin sisäinen TulorataNro]
		/// Huom. Solukohtaiset muutokset tehdään RobotinTuloradat_Asetukset.json tiedostoon joka löytyy paneelista!!!
		/// 	{1, 1}, {2, 2}
		/// </summary>
		/*private Dictionary<int, int> tuloradat = new Dictionary<int, int>();
		public Dictionary<int, int> Tuloradat {
			get { if (tuloradat == null) tuloradat = new Dictionary<int, int>(); return tuloradat; }
			set { if (value != null) tuloradat = value; }
		}*/
		private List<int> tuloradat = new List<int>();
		public List<int> Tuloradat
		{ 
			get { if (tuloradat == null) tuloradat = new List<int> (); return tuloradat; } 
			set { if (value != null) tuloradat = value; } 
		}

		/// <summary>
		/// Robottien lavapaikat ja robotin niistä käyttämät numerot.
		/// [asiakas lavapNro, robotin sisäinen LavapNro]
		/// Huom. Solukohtaiset muutokset tehdään RobotinLavapaikat_Asetukset.json tiedostoon joka löytyy paneelista!!!
		/// </summary>
		/*private Dictionary<int, int> lavapaikat = new Dictionary<int, int>();
		public Dictionary<int, int> Lavapaikat {
			get { if (lavapaikat == null) lavapaikat = new Dictionary<int, int>(); return lavapaikat; }
			set { if (value != null) lavapaikat = value; }
		}*/
		private List<int> lavapaikat = new List<int>();
		public List<int> Lavapaikat
		{ 
			get { if (lavapaikat == null) lavapaikat = new List<int> (); return lavapaikat; } 
			set { if (value != null) lavapaikat = value; } 
		}
		
		public RobotConf(int no, int trk, int lpk)
		{
			RobotNo = no;
			for (int i = 1; i <= trk; i++) tuloradat.Add(i);
			for (int i = 1; i <= lpk; i++) lavapaikat.Add(i);
		}
	}

	/// <summary>
	/// System configuration class
	/// </summary>
	public class Configuration__
	{
		private int numberofplc = 1;
		private Dictionary<int, RobotConf> robots = new Dictionary<int, RobotConf>();
		private Dictionary<int, string> lavatyypit = new Dictionary<int, string>();
		private Dictionary<string, int> aikavalit = new Dictionary<string, int>();
		private Dictionary<int, PatternInfo> patterns = new Dictionary<int, PatternInfo>();
		/// <summary>
		/// Robottien lavapaikat ja robotin niistä käyttämät numerot.
		/// [asiakas lavapNro, robotin sisäinen LavapNro]
		/// Huom. Solukohtaiset muutokset tehdään RobotinLavapaikat_Asetukset.json tiedostoon joka löytyy paneelista!!!
		/// </summary>
		private Dictionary<int, int> lavapaikat = new Dictionary<int, int>();
		/// <summary>
		/// Robottien tuloradat ja robotin niistä käyttämät numerot.
		/// [asiakas tulorataNro, robotin sisäinen TulorataNro]
		/// Huom. Solukohtaiset muutokset tehdään RobotinTuloradat_Asetukset.json tiedostoon joka löytyy paneelista!!!
		/// 	{1, 1}, {2, 2}
		/// </summary>
		private Dictionary<int, int> tuloradat = new Dictionary<int, int>();
		
		public int PanelNo { get; set; }
		public Dictionary<int, RobotConf> Robots
		{ 
			get { if (robots == null) robots = new Dictionary<int, RobotConf>(); return robots; } 
			set { if (value != null) robots = value; } 
		}
		public Dictionary<int, string> Lavatyypit
		{ 
			get { if (lavatyypit == null) lavatyypit = new Dictionary<int, string>(); return lavatyypit; } 
			set { if (value != null) lavatyypit = value; } 
		}
		public Dictionary<string, int> Aikavalit
		{ 
			get { if (aikavalit == null) aikavalit = new Dictionary<string, int>(); return aikavalit; } 
			set { if (value != null) aikavalit = value; } 
		}
		public Dictionary<int, int> Lavapaikat
		{ 
			get { if (lavapaikat == null) lavapaikat = new Dictionary<int, int>(); return lavapaikat; } 
			set { if (value != null) lavapaikat = value; } 
		}
		public Dictionary<int, int> Tuloradat
		{ 
			get { if (tuloradat == null) tuloradat = new Dictionary<int, int>(); return tuloradat; }
			set { if (value != null) tuloradat = value; } 
		}
		public Dictionary<int, PatternInfo> AllowedPatterns
		{ 
			get { if (patterns == null) patterns = new Dictionary<int, PatternInfo>(); return patterns; }
			set { if (value != null) patterns = value; } 
		}
		[Newtonsoft.Json.JsonIgnore]
		public int NumberOfRobots { get { if (robots == null) return 0; else return robots.Count; } }
		public int NumberOfPLC { get { return numberofplc; } set { if (value < 1) numberofplc = 1; else numberofplc = value; } }

		public int GetRobotNoByIndex(int index)
		{
			int ret = -1;
			if (robots == null) robots = new Dictionary<int, RobotConf>();
			if (robots.Count == 0)
			{
				robots.Add(1, new RobotConf(1, 4, 4));
			}
			if (index >= 0 && index < robots.Count)
			{
				int i = 0;
				foreach (int key in robots.Keys)
				{
					ret = robots[key].RobotNo;
					if (i == index) break;
					i++;
				}
			}
			return ret;
			}

		public void AddRobot(int no)
		{
			if (robots == null) robots = new Dictionary<int, RobotConf>();
			if (!robots.ContainsKey(no))
			{
				robots.Add(no, new RobotConf(no, 4, 4));
			}
		}

		public void RemoveRobot(int no)
		{
			if (robots == null) robots = new Dictionary<int, RobotConf>();
			if (robots.ContainsKey(no))
			{
				robots.Remove(no);
			}
		}

		public void AddRobotInfeedTrack(int no, int tulorata)
		{
			if (robots == null) robots = new Dictionary<int, RobotConf>();
			if (robots.ContainsKey(no))
			{
				robots[no].Tuloradat[tulorata] = tulorata;
			}
		}

		public void RemoveRobotInfeedTrack(int no, int tulorata)
		{
			if (robots == null) robots = new Dictionary<int, RobotConf>();
			if (robots.ContainsKey(no))
			{
				if (robots[no].Tuloradat.Contains(tulorata))
					robots[no].Tuloradat.Remove(tulorata);
			}
		}

		public void AddRobotPalletPlace(int no, int lavapaikka)
		{
			if (robots == null) robots = new Dictionary<int, RobotConf>();
			if (robots.ContainsKey(no))
			{
				robots[no].Lavapaikat[lavapaikka] = lavapaikka;
			}
		}

		public void RemoveRobotPalletPlace(int no, int lavapaikka)
		{
			if (robots == null) robots = new Dictionary<int, RobotConf>();
			if (robots.ContainsKey(no))
			{
				if (robots[no].Lavapaikat.Contains(lavapaikka))
					robots[no].Lavapaikat.Remove(lavapaikka);
			}
		}

		public bool GetRobotinTulorata(int tr, out int rno, out int rtrno)
		{
			rno = 0;
			rtrno = (-1);
			foreach (RobotConf r in robots.Values)
			{
				if (r.Tuloradat.Contains(tr))
				{
					rno = r.RobotNo;
					rtrno = r.Tuloradat[tr];
					break;
				}
			}
			return (rno > 0);
		}

		public bool GetRobotinLavapaikka(int lp, out int rno, out int rlpno)
		{
			rno = 0;
			rlpno = (-1);
			foreach (RobotConf r in robots.Values)
			{
				if (r.Lavapaikat.Contains(lp))
				{
					rno = r.RobotNo;
					rlpno = r.Lavapaikat[lp];
					break;
				}
			}
			return (rno > 0);
		}

		public int GetRobottiNumeroByTulorata(int tulorata)
		{
			int ret = 0;
			foreach (RobotConf robotti in Robots.Values)
			{
				if (robotti.Tuloradat.Contains(tulorata))
				{
					ret = robotti.RobotNo;
					break;
				}
			}
			return ret;
		}

		public List<int> GetSallitutLavapaikat(int robotti, int tulorata)
		{
			List<int> lpt = new List<int>();

			if (robots.ContainsKey(robotti))
			{
				// Katso mille lavapaikoille on olemassa yhteinen kuvio tuloradan kanssa
				foreach (int lavapaikka in robots[robotti].Lavapaikat)
				{
					// Yritetään hakea yhdistelmälle kuvio
					try 
					{	        
						int[] kuviot = Globals.Robotit.HaeSallitutKuviotLavapaikalla(tulorata, lavapaikka);
						if (kuviot.Length > 0) lpt.Add(lavapaikka);
					}
					catch (Exception x)
					{
						// Kuvioa ei ollut (muu virhe), jatketaan
						Globals.Tags.Log("GetSallitutLavapaikat" + x.Message);
					}
				}
			}
			return lpt;
		}
		
		public bool IsAllowedPalletPlace(int robotti, int lavapaikka)
		{
			bool ret = false;
			if (robots.ContainsKey(robotti))
			{
				ret = robots[robotti].Lavapaikat.Contains(lavapaikka);
			}
			return ret;
		}
		
		public bool IsAllowedInfeedTrack(int robotti, int tulorata)
		{
			bool ret = false;
			if (robots.ContainsKey(robotti))
			{
				ret = robots[robotti].Tuloradat.Contains(tulorata);
			}
			return ret;
		}

		public bool AddPattern(int no)
		{
			bool ret = false;
			if (!patterns.ContainsKey(no))
			{
				patterns.Add(no, new PatternInfo());
				ret = true;
			}
			return ret;
		}
		
		public bool RemovePattern(int no)
		{
			bool ret = false;
			if (patterns.ContainsKey(no))
			{
				patterns.Remove(no);
				ret = true;
			}
			return ret;
		}
		
		public bool AddPatternInfeedTrack(int no, int tulorata)
		{
			bool ret = false;
			if (patterns.ContainsKey(no))
			{
				if (!patterns[no].Tuloradat.Contains(tulorata))
					patterns[no].Tuloradat.Add(tulorata);
			}
			return ret;
		}
		
		public bool RemovePatternInfeedTrack(int no, int tulorata)
		{
			bool ret = false;
			if (patterns.ContainsKey(no))
			{
				if (patterns[no].Tuloradat.Contains(tulorata))
					patterns[no].Tuloradat.Remove(tulorata);
			}
			return ret;
		}
		
		public bool AddPatternPalletPlace(int no, int lavapaikka)
		{
			bool ret = false;
			if (patterns.ContainsKey(no))
			{
				if (!patterns[no].Lavapaikat.Contains(lavapaikka))
					patterns[no].Lavapaikat.Add(lavapaikka);
			}
			return ret;
		}
		
		public bool RemovePatternPalletPlace(int no, int lavapaikka)
		{
			bool ret = false;
			if (patterns.ContainsKey(no))
			{
				if (patterns[no].Lavapaikat.Contains(lavapaikka))
					patterns[no].Lavapaikat.Remove(lavapaikka);
			}
			return ret;
		}
		
		public bool AddPatternPalletType(int no, int lavatyyppi)
		{
			bool ret = false;
			if (patterns.ContainsKey(no))
			{
				if (!patterns[no].Lavatyypit.Contains(lavatyyppi))
					patterns[no].Lavatyypit.Add(lavatyyppi);
			}
			return ret;
		}
		
		public bool RemovePatternPalletType(int no, int lavatyyppi)
		{
			bool ret = false;
			if (patterns.ContainsKey(no))
			{
				if (patterns[no].Lavatyypit.Contains(lavatyyppi))
					patterns[no].Lavatyypit.Remove(lavatyyppi);
			}
			return ret;
		}
		
		public bool IsAllowedPatternPalletPlace(int no, int lavapaikka)
		{
			bool ret = false;
			if (patterns.ContainsKey(no))
			{
				ret = patterns[no].Lavapaikat.Contains(lavapaikka);
			}
			return ret;
		}
		
		public bool IsAllowedPatternInfeedTrack(int no, int tulorata)
		{
			bool ret = false;
			if (patterns.ContainsKey(no))
			{
				ret = patterns[no].Tuloradat.Contains(tulorata);
			}
			return ret;
		}

		public bool IsAllowedPatternPalletType(int no, int lavatyyppi)
		{
			bool ret = false;
			if (patterns.ContainsKey(no))
			{
				ret = patterns[no].Lavatyypit.Contains(lavatyyppi);
			}
			return ret;
		}

		public int Aikavali(string key)
		{
			if (aikavalit.ContainsKey(key))
				return aikavalit[key];
			else
				return 1000;
		}
		
		public string Lavatyyppi(int key)
		{
			if (lavatyypit.ContainsKey(key))
				return Lavatyypit[key];
			else
				return "Unknown";
		}
		
		public Configuration__()
		{
			robots.Add(1, new RobotConf(1, 4, 4));
			//robots.Add(2, new RobotConf(2, 4, 4));
			lavatyypit.Add(1, "EUR");
			lavatyypit.Add(2, "FIN");
			aikavalit.Add("Ajotiedot", 10000);
			aikavalit.Add("RobottiWatchdog", 1000);
			aikavalit.Add("LogiikkaWatchdog", 1000);
			aikavalit.Add("SensorInfoUpdate", 1000);
			for (int i = 1; i <= 4; i++) tuloradat.Add(i, i);
			for (int i = 1; i <= 4; i++) lavapaikat.Add(i, i);
		}
	}

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
		public enum man_mode { ManNumber, ManMultiString };

		/// <summary>
		/// Polku, johon kuvioiden rajoitteet on tallennettu.
		/// </summary>
		public static man_mode ManualMode = man_mode.ManNumber;

		private object lockme = new object();
		private Configuration__ config = new Configuration__();
		private bool savetodisk = true;
		private static string lavauspath = @"C:\Lavaus\";
		private static string path = lavauspath + @"Asetukset\Conf.json";
		/// <summary>
		/// Polku, johon kuvioiden rajoitteet on tallennettu.
		/// </summary>
		public static readonly string Kuviotietolista_Path = lavauspath + @"Kuviot\Kuviot.json";
		
		public bool ReadOk { get; set; }
		
		public Configuration__ CurrentConfig 
		{ 
			get { if (config == null) config = new Configuration__(); return config; } 
			set { if (value != null) config = value; else config = new Configuration__(); } 
		}
		
		public void Read()
		{
			lock (lockme)
				try 
				{
					if (savetodisk)
					{
						config = JsonSerialization.ReadFromJsonFile<Configuration__>(path);
						string s = Newtonsoft.Json.JsonConvert.SerializeObject(CurrentConfig);
						Globals.Tags.ConfCurrentConfig.SetString(s);
					}
					else
					{
						// non-volatiletag
						string s = Globals.Tags.ConfCurrentConfig.Value;
						if (!string.IsNullOrEmpty(s))
							config = Newtonsoft.Json.JsonConvert.DeserializeObject<Configuration__>(s);
						else
							throw new Exception("blank confdata, use current value");
					}
					
					Globals.Tags.Settings_PanelNumber.SetAnalog(config.PanelNo);
					ReadOk = true;
				}
				catch (Exception)
				{
					//System.Diagnostics.Trace.WriteLine(x.Message);
					// call function must have handler to exception or application will crash
					throw;
				}
		}
		
		public void Save()
		{
			lock (lockme)
				try 
				{
					if (savetodisk)
					{
						// Luo peruskansio jos ei olemassa
						if (!Directory.Exists(lavauspath)) Directory.CreateDirectory(lavauspath);
						if (!Directory.Exists(System.IO.Path.GetDirectoryName(path))) Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
				
						JsonSerialization.WriteToJsonFile<Configuration__>(path, config); 
					}
					// non-volatiletag
					// backup last value
					string s = Globals.Tags.ConfCurrentConfig.Value;
					Globals.Tags.ConfLastConfig.SetString(s);
					s = Newtonsoft.Json.JsonConvert.SerializeObject(CurrentConfig);
					Globals.Tags.ConfCurrentConfig.SetString(s);
				}
				catch (Exception)
				{
					//System.Diagnostics.Trace.WriteLine(x.Message);
					// call function must have handler to exception or application will crash
					throw;
				}
		}

/*
		void ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			// Write all tag values to settings dictionary and save dictionary to JSON
			foreach (var prop in settings.GetType().GetProperties())
			{
				try
				{
					if (prop.PropertyType == typeof(int))
					{
						try
						{
							int val = Globals.Tags.GetTagValueInt("Settings_" + prop.Name);
							prop.SetValue(settings, val, null);
						}
						catch (Exception x)
						{
							Globals.Tags.Log(String.Format("ValueChange: {0}, [{1}]", prop.Name, x.Message));
						}
					}
					else if (prop.PropertyType == typeof(string))
					{
						try
						{
							string val = Globals.Tags.GetTagValueString("Settings_" + prop.Name);
							prop.SetValue(settings, val, null);
						}
						catch (Exception x)
						{
							Globals.Tags.Log(String.Format("ValueChange: {0}, [{1}]", prop.Name, x.Message));
						}
					}
				}
				catch (Exception ex)
				{
					Globals.Tags.Log(ex.ToString() + ". Error when change setting! Tag: " + prop.Name);
				}
			}

			SaveSettings();
		}
		
		private void UpdateSettings
		{
		}
*/
		
		/// <summary>
		/// Lataa asetukset sovelluksen käynnistyessä.
		/// </summary>
		/// <param name="sender">_Konfiguraatio</param>
		void _Konfiguraatio_Created(System.Object sender, System.EventArgs e)
		{
			if (Globals.Tags.TraceAll) System.Diagnostics.Trace.WriteLine("_Konfiguraatio Created (start)");         
			
			try 
			{	        
				Read();
			}
			catch (Exception x)
			{
				System.Diagnostics.Trace.WriteLine(x.Message);
				Globals.Tags.Log(x.Message);
			}
			
			// Create event to update file if settings Tag value changed
			/*foreach (var prop in settings.GetType().GetProperties())
			{
			try
			{
			Globals.Tags.GetTag("Settings_" + prop.Name).ValueChange += ValueChange;
			}
			catch (Exception x)
			{
			Globals.Tags.Log(String.Format("KonfiguraatioCreated: {0}, [{1}]", prop.Name, x.Message));
			}
			}*/
		}
	}
}
