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
    

	/// <summary>
	/// System configuration class
	/// </summary>
	public class Configuration__
	{
		#region variables
		
		private int numberofplc = 1;
		private Dictionary<string, int> aikavalit = new Dictionary<string, int>();
		
		public Dictionary<string, int> Aikavalit
		{
			get { if (aikavalit == null) aikavalit = new Dictionary<string, int>(); return aikavalit; }
			set { if (value != null) aikavalit = value; }
		}
		public int NumberOfPLC { get { return numberofplc; } set { if (value < 1) numberofplc = 1; else numberofplc = value; } }

		#endregion

		public int Aikavali(string key) { return (aikavalit.ContainsKey(key)) ? aikavalit[key] : 1000; }
		
		public Configuration__()
		{
			aikavalit.Add("Ajotiedot", 10000);
			aikavalit.Add("LogiikkaWatchdog", 1000);
			aikavalit.Add("SensorInfoUpdate", 1000);
		}
	}
	
	public partial class _Konfiguraatio
	{
		private Configuration__ config = new Configuration__();
		
		public Configuration__ CurrentConfig
		{
			get { if (config == null) config = new Configuration__(); return config; }
			set { if (value != null) config = value; else config = new Configuration__(); }
		}
	}
}
