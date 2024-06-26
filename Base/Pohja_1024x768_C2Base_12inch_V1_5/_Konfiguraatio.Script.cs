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
			get { if (lavatyypit == null) lavatyypit = new List<int>(); return lavatyypit; }
			set { if (value != null) lavatyypit = value; }
		}
		/// <summary>
		/// Kuviolle sallitut tuloradat.
		/// </summary>
		public List<int> Tuloradat
		{
			get { if (tuloradat == null) tuloradat = new List<int>(); return tuloradat; }
			set { if (value != null) tuloradat = value; }
		}
		/// <summary>
		/// Kuviolle sallitut lavapaikat.
		/// </summary>
		public List<int> Lavapaikat
		{
			get { if (lavapaikat == null) lavapaikat = new List<int>(); return lavapaikat; }
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
		private List<int> tuloradat = new List<int>();
		public List<int> Tuloradat
		{
			get { if (tuloradat == null) tuloradat = new List<int>(); return tuloradat; }
			set { if (value != null) tuloradat = value; }
		}

		/// <summary>
		/// Robottien lavapaikat ja robotin niistä käyttämät numerot.
		/// [asiakas lavapNro, robotin sisäinen LavapNro]
		/// Huom. Solukohtaiset muutokset tehdään RobotinLavapaikat_Asetukset.json tiedostoon joka löytyy paneelista!!!
		/// </summary>
		private List<int> lavapaikat = new List<int>();
		public List<int> Lavapaikat
		{
			get { if (lavapaikat == null) lavapaikat = new List<int>(); return lavapaikat; }
			set { if (value != null) lavapaikat = value; }
		}

		/// <summary>
		/// Robot constructor
		/// </summary>
		/// <param name="no">Robot number</param>
		/// <param name="trk">Number of Infeed tracks</param>
		/// <param name="lpk">Number of Pallet places</param>
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
		#region variables

		private int numberofplc = 1;
		private Dictionary<string, int> aikavalit = new Dictionary<string, int>();
		private Dictionary<int, string> lavatyypit = new Dictionary<int, string>();
		/// <summary>
		/// Robottien tuloradat ja robotin niistä käyttämät numerot.
		/// [asiakas tulorataNro, robotin sisäinen TulorataNro]
		/// Huom. Solukohtaiset muutokset tehdään RobotinTuloradat_Asetukset.json tiedostoon joka löytyy paneelista!!!
		/// 	{1, 1}, {2, 2}
		/// </summary>
		private Dictionary<int, int> tuloradat = new Dictionary<int, int>();
		/// <summary>
		/// Robottien lavapaikat ja robotin niistä käyttämät numerot.
		/// [asiakas lavapNro, robotin sisäinen LavapNro]
		/// Huom. Solukohtaiset muutokset tehdään RobotinLavapaikat_Asetukset.json tiedostoon joka löytyy paneelista!!!
		/// </summary>
		private Dictionary<int, int> lavapaikat = new Dictionary<int, int>();
		private Dictionary<int, RobotConf> robots = new Dictionary<int, RobotConf>();
		private Dictionary<int, PatternInfo> patterns = new Dictionary<int, PatternInfo>();

		public int PanelNo { get; set; }
		
		public int AreaNo { get; set; }

		public int DefaultManualScreen { get; set; }

		public int DefaultManualScreenGroup2 { get; set; }
		
		public Dictionary<string, int> Aikavalit
		{
			get { if (aikavalit == null) aikavalit = new Dictionary<string, int>(); return aikavalit; }
			set { if (value != null) aikavalit = value; }
		}
		public Dictionary<int, string> Lavatyypit
		{
			get { if (lavatyypit == null) lavatyypit = new Dictionary<int, string>(); return lavatyypit; }
			set { if (value != null) lavatyypit = value; }
		}
		public Dictionary<int, int> Tuloradat
		{
			get { if (tuloradat == null) tuloradat = new Dictionary<int, int>(); return tuloradat; }
			set { if (value != null) tuloradat = value; }
		}
		public Dictionary<int, int> Lavapaikat
		{
			get { if (lavapaikat == null) lavapaikat = new Dictionary<int, int>(); return lavapaikat; }
			set { if (value != null) lavapaikat = value; }
		}
		public Dictionary<int, RobotConf> Robots
		{
			get { if (robots == null) robots = new Dictionary<int, RobotConf>(); return robots; }
			set { if (value != null) robots = value; }
		}
		public Dictionary<int, PatternInfo> AllowedPatterns
		{
			get { if (patterns == null) patterns = new Dictionary<int, PatternInfo>(); return patterns; }
			set { if (value != null) patterns = value; }
		}
		[Newtonsoft.Json.JsonIgnore]
		public int NumberOfRobots { get { if (robots == null) return 0; else return robots.Count; } }
		public int NumberOfPLC { get { return numberofplc; } set { if (value < 1) numberofplc = 1; else numberofplc = value; } }

		private string _SqlInstanceName = "localhost\\SQLEXPRESS";
		private string _SqlDatabaseName = "Wipak3";
		private bool _UseLocalUserCredentials = true; // if true username and password are obsolsete
		public string SqlInstanceName { get { return _SqlInstanceName; } set { if (!string.IsNullOrEmpty(value)) _SqlInstanceName = value; } }
		public string SqlDatabaseName { get { return _SqlDatabaseName; } set { if (!string.IsNullOrEmpty(value)) _SqlDatabaseName = value; } }
		public bool UseLocalUserCredentials { get { return _UseLocalUserCredentials; } set { _UseLocalUserCredentials = value; } }

		#endregion

		public void RemovePalletType(int no)
		{
			/*foreach (KeyValuePair<int, RobotConf> r in robots)
			{
				RobotConf robot = r.Value;
				if (robot.Lavatyypit.Contains(no)) robot.Lavatyypit.Remove(no);
			}*/
			foreach (KeyValuePair<int, PatternInfo> p in patterns)
			{
				PatternInfo pattern = p.Value;
				if (pattern.Lavatyypit.Contains(no)) pattern.Lavatyypit.Remove(no);
			}
			Lavatyypit.Remove(no);
		}

		public void RemoveInfeedTrack(int no)
		{
			foreach (KeyValuePair<int, RobotConf> r in robots)
			{
				RobotConf robot = r.Value;
				if (robot.Tuloradat.Contains(no)) robot.Tuloradat.Remove(no);
			}
			foreach (KeyValuePair<int, PatternInfo> p in patterns)
			{
				PatternInfo pattern = p.Value;
				if (pattern.Tuloradat.Contains(no)) pattern.Tuloradat.Remove(no);
			}
			Tuloradat.Remove(no);
		}

		public void RemovePalletPlace(int no)
		{
			foreach (KeyValuePair<int, RobotConf> r in robots)
			{
				RobotConf robot = r.Value;
				if (robot.Lavapaikat.Contains(no)) robot.Lavapaikat.Remove(no);
			}
			foreach (KeyValuePair<int, PatternInfo> p in patterns)
			{
				PatternInfo pattern = p.Value;
				if (pattern.Lavapaikat.Contains(no)) pattern.Lavapaikat.Remove(no);
			}
			Lavapaikat.Remove(no);
		}

		public int GetRobotNoByIndex(int index)
		{
			int ret = -1;
			if (robots == null) robots = new Dictionary<int, RobotConf>();
			if (robots.Count == 0)
			{
				robots.Add(1, new RobotConf(1, 0, 0));
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
				robots.Add(no, new RobotConf(no, tuloradat.Count, lavapaikat.Count));
		}

		public void RemoveRobot(int no)
		{
			if (robots == null) robots = new Dictionary<int, RobotConf>();
			if (robots.ContainsKey(no))
				robots.Remove(no);
		}

		public void AddRobotInfeedTrack(int no, int tulorata)
		{
			if (robots == null) robots = new Dictionary<int, RobotConf>();
			if (robots.ContainsKey(no))
				if (!robots[no].Tuloradat.Contains(tulorata))
				{
					robots[no].Tuloradat.Add(tulorata);
					robots[no].Tuloradat.Sort();
				}
		}

		public void RemoveRobotInfeedTrack(int no, int tulorata)
		{
			if (robots == null) robots = new Dictionary<int, RobotConf>();
			if (robots.ContainsKey(no))
				if (robots[no].Tuloradat.Contains(tulorata))
					robots[no].Tuloradat.Remove(tulorata);
		}

		public void AddRobotPalletPlace(int no, int lavapaikka)
		{
			if (robots == null) robots = new Dictionary<int, RobotConf>();
			if (robots.ContainsKey(no))
			{
				if (!robots[no].Lavapaikat.Contains(lavapaikka))
				{
					robots[no].Lavapaikat.Add(lavapaikka);
					robots[no].Lavapaikat.Sort();
				}
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
					rtrno = Tuloradat[tr];
					break;
				}
			}
			return (rno > 0);
		}

		/// <summary>
		/// Robot Palletplace
		/// </summary>
		/// <param name="lp">Palletplace</param>
		/// <param name="rno">(out) Robot Number</param>
		/// <param name="rlpno">(out) Robot Palletplace number</param>
		/// <returns></returns>
		public bool GetRobotinLavapaikka(int lp, out int rno, out int rlpno)
		{
			rno = 0;
			rlpno = (-1);
			foreach (RobotConf r in robots.Values)
			{
				if (r.Lavapaikat.Contains(lp))
				{
					rno = r.RobotNo;
					rlpno = Lavapaikat[lp];
					break;
				}
			}
			return (rno > 0);
		}

		public int GetRobottiNumeroByTulorata__(int tulorata)
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

		/// <summary>
		/// List allowed infeed tracks for robot
		/// </summary>
		/// <param name="robotti">Robot number</param>
		/// <returns></returns>
		public List<int> AllowedInfeedTracks(int robotti) { return robots.ContainsKey(robotti) ? robots[robotti].Tuloradat : new List<int>(); }

		/// <summary>
		/// List allowed pallet places for robot
		/// </summary>
		/// <param name="robotti">Robot number</param>
		/// <returns></returns>
		public List<int> AllowedPalletPlaces(int robotti) { return robots.ContainsKey(robotti) ? robots[robotti].Lavapaikat : new List<int>(); }

		/// <summary>
		/// List allowed infeed tracks for robots infeed track
		/// </summary>
		/// <param name="robotti">Robot number</param>
		/// <param name="tulorata">infeed track</param>
		/// <returns></returns>
		public List<int> AllowedPalletPlaces(int robotti, int tulorata)
		{
			List<int> lpt = new List<int>();

			if (robots.ContainsKey(robotti))
			{
				// Katso mille lavapaikoille on olemassa yhteinen kuvio tuloradan kanssa
				foreach (int lavapaikka in robots[robotti].Lavapaikat)
				{
					// Yritetään hakea yhdistelmälle kuvio
					if (PatternIsAllowedAnyInfeedTrack_PalletPlace(tulorata, lavapaikka))
						if (!lpt.Contains(lavapaikka))
							lpt.Add(lavapaikka);
				}
			}
			lpt.Sort();
			return lpt;
		}

		public bool IsAllowedPalletPlace(int robotti, int lavapaikka) { return robots.ContainsKey(robotti) ? robots[robotti].Lavapaikat.Contains(lavapaikka) : false; }
		public bool IsAllowedInfeedTrack(int robotti, int tulorata) { return robots.ContainsKey(robotti) ? robots[robotti].Tuloradat.Contains(tulorata) : false; }

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
				{
					patterns[no].Tuloradat.Add(tulorata);
					patterns[no].Tuloradat.Sort();
				}
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
				{
					patterns[no].Lavapaikat.Add(lavapaikka);
					patterns[no].Lavapaikat.Sort();
				}
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
				{
					patterns[no].Lavatyypit.Add(lavatyyppi);
					patterns[no].Lavatyypit.Sort();
				}
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

		private readonly List<int> empty = new List<int>();

		/// <summary>
		/// Allowed infeed tracks for pattern
		/// </summary>
		/// <param name="no">Patterns number</param>
		/// <returns></returns>
		public List<int> AllowedPatternInfeedTracks(int no) { return patterns.ContainsKey(no) ? patterns[no].Tuloradat : empty; }

		/// <summary>
		/// Allowed palletplaces for pattern
		/// </summary>
		/// <param name="no">Patterns number</param>
		/// <returns></returns>
		public List<int> AllowedPatternPalletPlaces(int no) { return patterns.ContainsKey(no) ? patterns[no].Lavapaikat : empty; }

		/// <summary>
		/// Allowed pallet types for pattern
		/// </summary>
		/// <param name="no">Patterns number</param>
		/// <returns></returns>
		public List<int> AllowedPatternPalletTypes(int no) { return patterns.ContainsKey(no) ? patterns[no].Lavatyypit : empty; }

		/// <summary>
		/// IsAllowed infeed track for pattern
		/// </summary>
		/// <param name="no">Pattern number</param>
		/// <param name="tulorata">Infeed tracs</param>
		/// <returns></returns>
		public bool IsAllowedPatternInfeedTrack(int no, int tulorata) { return patterns.ContainsKey(no) ? patterns[no].Tuloradat.Contains(tulorata) : false; }

		/// <summary>
		/// IsAllowed pallet place for pattern
		/// </summary>
		/// <param name="no">Pattern number</param>
		/// <param name="tulorata">Pallet place</param>
		/// <returns></returns>
		public bool IsAllowedPatternPalletPlace(int no, int lavapaikka) { return patterns.ContainsKey(no) ? patterns[no].Lavapaikat.Contains(lavapaikka) : false; }

		/// <summary>
		/// IsAllowed infeed track for palletplace
		/// </summary>
		/// <param name="no">Infeed track</param>
		/// <param name="tulorata">Pallet place</param>
		/// <returns></returns>
		public bool IsAllowedPatternPalletType(int no, int lavatyyppi) { return patterns.ContainsKey(no) ? patterns[no].Lavatyypit.Contains(lavatyyppi) : false; }

		/// <summary>
		/// IsAllowed palletplace for infeed track
		/// </summary>
		/// <param name="no">Infeed track</param>
		/// <param name="tulorata">Pallet place</param>
		/// <returns></returns>
		public bool PatternIsAllowedAnyInfeedTrack_PalletPlace(int tulorata, int lavapaikka)
		{
			foreach (KeyValuePair<int, PatternInfo> item in patterns)
				if (((PatternInfo)item.Value).Tuloradat.Contains(tulorata) && ((PatternInfo)item.Value).Lavapaikat.Contains(lavapaikka))
					return true;
			return false;
		}

		/// <summary>
		/// list allowed pattern numbers
		/// </summary>
		/// <returns></returns>
		public List<int> AllowedPatternNumbers()
		{
			List<int> lst = new List<int>();
			foreach (KeyValuePair<int, PatternInfo> item in patterns) lst.Add(item.Key);
			lst.Sort();
			return lst;
		}

		public List<int> PatternsForInfeed__(int tulorata)
		{
			List<int> lst = new List<int>();
			foreach (KeyValuePair<int, PatternInfo> item in patterns)
				if (((PatternInfo)item.Value).Tuloradat.Contains(tulorata)) lst.Add(item.Key);
			lst.Sort();
			return lst;
		}

		public List<int> PatternAllowedPalletPlaces(int robotti, int kuviono)
		{
			List<int> lst = new List<int>();
			if (robots.ContainsKey(robotti))
			{
				foreach (int lavapaikka in robots[robotti].Lavapaikat)
					if (IsAllowedPatternPalletPlace(kuviono, lavapaikka)) lst.Add(lavapaikka);
			}
			lst.Sort();
			return lst;
		}

		public List<int> PatternsForInfeed(int tulorata, int lavapaikka)
		{
			List<int> lst = new List<int>();
			foreach (KeyValuePair<int, PatternInfo> item in patterns)
			{
				if (((PatternInfo)item.Value).Tuloradat.Contains(tulorata)
					&& (((PatternInfo)item.Value).Lavapaikat.Contains(lavapaikka) || lavapaikka == 0))
					lst.Add(item.Key);
			}
			lst.Sort();
			return lst;
		}

		/// <summary>
		/// palauttaa aikavälin avaimen perusteella
		/// 
		/// Oletus aikaväli = 1000
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns></returns>
		public int Aikavali(string key) { return (aikavalit.ContainsKey(key)) ? aikavalit[key] : 1000; }
		public string Lavatyyppi(int key) { return (lavatyypit.ContainsKey(key)) ? Lavatyypit[key] : "Unknown"; }

		public Configuration__()
		{
			//robots.Add(1, new RobotConf(1, 0, 0));
			//robots.Add(2, new RobotConf(2, 0, 0));
			//lavatyypit.Add(1, "EUR");
			//lavatyypit.Add(2, "FIN");
			aikavalit.Add("Ajotiedot", 10000);
			aikavalit.Add("RobottiWatchdog", 1000);
			aikavalit.Add("LogiikkaWatchdog", 1000);
			aikavalit.Add("SensorInfoUpdate", 1000);
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
		public static man_mode ManualMode = man_mode.ManNumber;

		private readonly object lockme = new object();
		private Configuration__ config = new Configuration__();
		/// <summary>
		/// Polku, johon kuvioiden rajoitteet on tallennettu.
		/// </summary>
		private static readonly string lavauspath = @"C:\Lavaus\";
		public static readonly string ConfigFileName = lavauspath + @"Asetukset\Conf.json";
		public static readonly string PatternDirectory = lavauspath + @"Kuviot\";
		public static readonly string PictureDirectory = lavauspath + @"Kuvat\";
		private string conf_filename = "";
		public string CurrentConfigFileName { get { return string.IsNullOrEmpty(conf_filename) ? ConfigFileName : conf_filename; } set { conf_filename = value; } }

		/// <summary>
		/// tiedosto, johon kuvioiden rajoitteet on tallennettu.
		/// </summary>

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
					bool b = (bool)Globals.Tags.HMI_Conf_UseOnlyNVBD.Value;
					if (b)
					{
						// non-volatiletag
						string s = Globals.Tags.HMI_Conf_CurrentConfig.Value;
						if (!string.IsNullOrEmpty(s))
							config = Newtonsoft.Json.JsonConvert.DeserializeObject<Configuration__>(s);
					}
					else
					{
						try
						{
							config = JsonSerialization.ReadFromJsonFile<Configuration__>(CurrentConfigFileName);
						}
						catch (Exception)
						{
							config = new Configuration__();
						}
						string s = Newtonsoft.Json.JsonConvert.SerializeObject(CurrentConfig);
						Globals.Tags.HMI_Conf_CurrentConfig.SetString(s);
					}

					Globals.Tags.HMI_Settings_PanelNumber.SetAnalog(config.PanelNo);
					ReadOk = true;
				}
				catch (Exception x)
				{
					System.Diagnostics.Trace.WriteLine(x.Message);
					// call function must have handler to exception or application will crash
					throw;
				}
		}

		public void Save()
		{
			lock (lockme)
				try
				{
					string last = Globals.Tags.HMI_Conf_LastConfig.Value;
					if (last == null) last = "";
					string current = Globals.Tags.HMI_Conf_LastConfig.Value;
					if (current == null) current = "";
					string s = Newtonsoft.Json.JsonConvert.SerializeObject(CurrentConfig);
					if (last.CompareTo(s) != 0)
					{
						// non-volatiletag
						Globals.Tags.HMI_Conf_LastConfig.SetString(current);
						Globals.Tags.HMI_Conf_CurrentConfig.SetString(s);

						// backup last value
						// Luo peruskansio jos ei olemassa
						if (!Directory.Exists(lavauspath)) Directory.CreateDirectory(lavauspath);
						if (!Directory.Exists(System.IO.Path.GetDirectoryName(ConfigFileName))) Directory.CreateDirectory(System.IO.Path.GetDirectoryName(ConfigFileName));
						// !!!! if not used static config filename, check that directory exists
						JsonSerialization.WriteToJsonFile<Configuration__>(CurrentConfigFileName, config);
					}
				}
				catch (Exception)
				{
					//System.Diagnostics.Trace.WriteLine(x.Message);
					// call function must have handler to exception or application will crash
					throw;
				}
		}

		/// <summary>
		/// Save PanelNo setting value
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		void ValueChangePN(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			if (Globals.Tags.HMI_Settings_PanelNumber.Value.Int > 0 && config.PanelNo > 0)
			{
				Save();
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

				private void CreateChangeEvents
				{
					// Create event to update file if settings Tag value changed
					foreach (var prop in settings.GetType().GetProperties())
					{
						try
						{
							Globals.Tags.GetTag("Settings_" + prop.Name).ValueChange += ValueChange;
						}
						catch (Exception x)
						{
							Globals.Tags.Log(String.Format("KonfiguraatioCreated: {0}, [{1}]", prop.Name, x.Message));
						}
					}
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

				if (CurrentConfig.AreaNo == 0) CurrentConfig.AreaNo = 1;
				Globals.Tags.HMI_Area.SetAnalog(CurrentConfig.AreaNo);

				// PanelNo change event
				Globals.Tags.HMI_Settings_PanelNumber.ValueChange += ValueChangePN;

				foreach (int r in CurrentConfig.Robots.Keys)
				{
					Globals.Robotit.Exists(r);
				}
			}
			catch (Exception x)
			{
				System.Diagnostics.Trace.WriteLine(x.Message);
				Globals.Tags.Log(x.Message);
			}
		}
	}
}
