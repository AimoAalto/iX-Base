namespace Neo.ApplicationFramework.Generated
{
	//using Neo.ApplicationFramework.Tools.OpcClient;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;


	/// <summary>
	/// Sisältää logiikoiden yhteyksien taustamonitoroinnin.
	/// </summary>
	/// <remarks>Viimeksi muokattu SoPi 6.7.2017</remarks>
	public partial class Logiikat
	{
		readonly object lockme = new object();
		/// <summary>
		/// Ikuisesti jatkuva logiikkayhteyksien taustamonitoroinnin ajastin.
		/// </summary>
		Timer Watchdog;
		/// <summary>
		/// Logiikkakohtainen epäonnistuneiden tarkistuskertojen laskuri ennen hälytystä.
		/// </summary>
		readonly Dictionary<int, int> Watchdog_Wait = new Dictionary<int, int>();

		/// <summary>
		/// Ajastaa logiikkojen yhteyden taustamonitoroinnin sovelluksen käynnistyessä.
		/// </summary>
		/// <param name="sender">this</param>
		void Logiikat_Created(System.Object sender, System.EventArgs e)
		{
			if (Globals.Tags.TraceAll) System.Diagnostics.Trace.WriteLine("Logiikat Created (start)");
			int interval = 10000;
			try
			{
				interval = Globals._Konfiguraatio.CurrentConfig.Aikavali("LogiikkaWatchdog");
			}
			catch (Exception x)
			{
				Globals.Tags.Log(String.Format("Logiikat_Created: Interval error, use default\n{0}", x.Message));
			}

			/**/
			Watchdog = new Timer((args) =>
				{
				// Mitataan kauanko operaatioissa kestää
				Stopwatch takeTime = new Stopwatch();
				takeTime.Start();

				// Tarkista kaikkien logiikkojen tila
				for (int i = 1; i <= Globals._Konfiguraatio.CurrentConfig.NumberOfPLC; i++)
				{
					if (Globals.Tags.TraceAll) System.Diagnostics.Trace.WriteLine(string.Format("Lokiikat TilaTarkitus [{0}]", i));
					TarkistaTila(i);
				}

				takeTime.Stop();

				if (Globals.Tags.TraceAll) System.Diagnostics.Trace.WriteLine(string.Format("Lokiikat time : {0} (ticks) {1} (ms)", takeTime.ElapsedTicks, takeTime.ElapsedMilliseconds));

				// Suoritetaan määritetyin välein (default 1s)
				Watchdog.Change(Math.Max(0, interval - takeTime.ElapsedMilliseconds), Timeout.Infinite);
				}, null, 0, Timeout.Infinite);
			/**/
		}

		/// <summary>
		/// Tarkistaa, onko logiikka päivittänyt From_PLC-tagia ja vastaa päivittämällä
		/// To_PLC-tagia. Jos logiikka ei ole päivittänyt tagia, hälytetään yhteyden 
		/// katkeamisesta 7 tarkistuksen jälkeen.
		/// ix <> PLX communikointi
		/// </summary>
		/// <param name="numero">Logiikan numero tageissa</param>
		public void TarkistaTila(int numero)
		{
			lock (lockme)
				try
				{
					#region WatchDog

					// Ensimmäinen tarkistuskerta, lisätään lokiikka numero (objekti) listaan 
					if (!Watchdog_Wait.ContainsKey(numero))
					{
						Watchdog_Wait.Add(numero, 0);
					}

					// Pakotetaan tagi luku, esitetään että yhteys on kunnossa vaikka tulisi 
					// Bad Station hälytystä. Silloin on vain tagien määrittelyssä vikaa
					string tohmitag_name = string.Format("S7HMI_DB_ToHMI_WatchDog_{0}", numero);
					string tohmitag_old_name = string.Format("S7HMI_DB_ToHMI_WatchDogOld_{0}", numero);
					string toplctag_name = string.Format("S7HMI_DB_ToPLC_WatchDog_{0}", numero);

					Neo.ApplicationFramework.Interfaces.Tag.IBasicTag ToHMItag = Globals.Tags.GetTag(tohmitag_name);
					//((Neo.ApplicationFramework.Interfaces.Tag.IBasicTag)Globals.Tags.S7HMI_DB_ToHMI_WatchDog_1).Read();
					/**
					if (ToHMItag != null)
					ToHMItag.Read();
					else
					System.Diagnostics.Trace.WriteLine(string.Format("[NULL] Virhe tagin [{0}] haussa", tohmitag_name));
					/**/
					Neo.ApplicationFramework.Interfaces.Tag.IBasicTag ToHMItagOld = Globals.Tags.GetTag(tohmitag_old_name);
					if (ToHMItagOld == null)
						System.Diagnostics.Trace.WriteLine(string.Format("[NULL] Virhe tagin [{0}] haussa", tohmitag_old_name));

					Neo.ApplicationFramework.Interfaces.Tag.IBasicTag ToPLCtag = Globals.Tags.GetTag(toplctag_name);
					if (ToPLCtag == null)
						System.Diagnostics.Trace.WriteLine(string.Format("[NULL] Virhe tagin [{0}] haussa", toplctag_name));

					bool tags_ok = (ToHMItag != null) && (ToHMItagOld != null) && (ToPLCtag != null);

					if (tags_ok)
					{
						int tohmival = (int)Globals.Tags.GetTagValue(tohmitag_name);
						int toplcval = (int)Globals.Tags.GetTagValue(toplctag_name);
						int tohmival_old = (int)Globals.Tags.GetTagValue(tohmitag_old_name);

						//System.Diagnostics.Trace.WriteLine(string.Format("Lokiikat.TarkistaTila : {0}, {1}, {2}", tohmival, toplcval, tohmival_old));

						// Tarkistetaan onko logiikan arvo muuttunut
						if (tohmival != tohmival_old)
						{
							// On muuttunut, tallennetaan uusi arvo
							ToHMItagOld.SetAnalog(tohmival);
							// Kirjoitetaan logiikalle uusi arvo
							ToPLCtag.SetAnalog(tohmival);

							// Nollataan virhe ajastin
							Watchdog_Wait[numero] = 0;

							if (numero == 1) Globals.Tags.AppStart_Timer++;
						}
						else
						{
							// Arvo on pysynyt samana eli logiikka ei ole hereillä, kasvatetaan virhelaskuria
							Watchdog_Wait[numero] += 1;
						}
					}

					string name = string.Format("HMI_CommFault_PLC{0}", numero);
					// Odotellaan hetki ennen hälyttämistä
					if (Watchdog_Wait[numero] > 7 || !tags_ok)
					{
						// Hälytetään yhteys poikki
						Globals.Tags.SetTagValue(name, 1);
					}
					else
					{
						// tagi arvo muuttunut, nollataan hälytys
						Globals.Tags.SetTagValue(name, 0);
					}

					#endregion
				}
				catch (Exception x)
				{
					System.Diagnostics.Trace.WriteLine(string.Format("Exception [Lokiikat.TarkistaTila] {0}\n{1}", x.Message, x.StackTrace));
				}
		}

		/// <summary>
		/// code from ChatGTP
		/// </summary>
		/// <param name="companyPrefix"></param>
		/// <param name="serialNumber"></param>
		/// <returns></returns>
		public string GenerateSSCC(string companyPrefix, string serialNumber)
		{
			// Pad company prefix and serial number with zeros
			companyPrefix = companyPrefix.PadLeft(6, '0');
			serialNumber = serialNumber.PadLeft(11, '0');
			// Concatenate elements to form SSCC
			string sscc = string.Format("{0}{1}", companyPrefix, serialNumber);
			// Calculate and append the checksum
			int sum = 0;
			for (int i = 0; i < sscc.Length; i++)
			{
				int digit = int.Parse(sscc[i].ToString());
				sum += (i % 2 == 0) ? digit * 3 : digit;
			}
			int modValue = sum % 10;
			int checkDigit = (modValue == 0) ? 0 : 10 - modValue;
			sscc += checkDigit.ToString()[0];

			// Globals.Tags.LogToFile(string.Format("new SSCC : {0}", sscc));
			
			return sscc;
		}

		#region S7 buffer handling
		
		/*
		#region Get/Set 32 bit unsigned value (S7 UDInt) 0..4294967296

		public static UInt32 GetUDIntAt(byte[] Buffer, int Pos)
		{
			UInt32 Result;
			Result = Buffer[Pos]; Result <<= 8;
			Result |= Buffer[Pos + 1]; Result <<= 8;
			Result |= Buffer[Pos + 2]; Result <<= 8;
			Result |= Buffer[Pos + 3];
			return Result;
		}
		public static void SetUDIntAt(byte[] Buffer, int Pos, UInt32 Value)
		{
			Buffer[Pos + 3] = (byte)(Value & 0xFF);
			Buffer[Pos + 2] = (byte)((Value >> 8) & 0xFF);
			Buffer[Pos + 1] = (byte)((Value >> 16) & 0xFF);
			Buffer[Pos] = (byte)((Value >> 24) & 0xFF);
		}

		#endregion
		
		#region Get/Set 64 bit unsigned value (S7 ULint) 0..1844 6744 0737 0955 1616

		public static UInt64 GetULIntAt(byte[] Buffer, int Pos)
		{
			UInt64 Result;
			Result = Buffer[Pos]; Result <<= 8;
			Result |= Buffer[Pos + 1]; Result <<= 8;
			Result |= Buffer[Pos + 2]; Result <<= 8;
			Result |= Buffer[Pos + 3]; Result <<= 8;
			Result |= Buffer[Pos + 4]; Result <<= 8;
			Result |= Buffer[Pos + 5]; Result <<= 8;
			Result |= Buffer[Pos + 6]; Result <<= 8;
			Result |= Buffer[Pos + 7];
			return Result;
		}

		public static void SetULintAt(byte[] Buffer, int Pos, UInt64 Value)
		{
			Buffer[Pos + 7] = (byte)(Value & 0xFF);
			Buffer[Pos + 6] = (byte)((Value >> 8) & 0xFF);
			Buffer[Pos + 5] = (byte)((Value >> 16) & 0xFF);
			Buffer[Pos + 4] = (byte)((Value >> 24) & 0xFF);
			Buffer[Pos + 3] = (byte)((Value >> 32) & 0xFF);
			Buffer[Pos + 2] = (byte)((Value >> 40) & 0xFF);
			Buffer[Pos + 1] = (byte)((Value >> 48) & 0xFF);
			Buffer[Pos] = (byte)((Value >> 56) & 0xFF);
		}

		#endregion
		*/

		#endregion
	}
}
