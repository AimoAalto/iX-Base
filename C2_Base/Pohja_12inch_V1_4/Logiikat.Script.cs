namespace Neo.ApplicationFramework.Generated
{
	using Neo.ApplicationFramework.Tools.OpcClient;
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
		object lockme = new object();
		/// <summary>
		/// Ikuisesti jatkuva logiikkayhteyksien taustamonitoroinnin ajastin.
		/// </summary>
		Timer Watchdog;
		/// <summary>
		/// Logiikkakohtainen epäonnistuneiden tarkistuskertojen laskuri ennen hälytystä.
		/// </summary>
		Dictionary<int, int> Watchdog_Wait = new Dictionary<int, int>();

		/// <summary>
		/// Ajastaa logiikkojen yhteyden taustamonitoroinnin sovelluksen käynnistyessä.
		/// </summary>
		/// <param name="sender">this</param>
		void Logiikat_Created(System.Object sender, System.EventArgs e)
		{
			if (Globals.Tags.TraceAll) System.Diagnostics.Trace.WriteLine("Lokiikat Created (start)");
			int interval = 10000;
			try
			{
				interval = Globals._Konfiguraatio.CurrentConfig.Aikavali("LogiikkaWatchdog");
			}
			catch (Exception x)
			{
				Globals.Tags.Log(String.Format("Logiikat_Created: Interval error, use default\n{1}", x.Message));
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
		/// </summary>
		/// <param name="numero">Logiikan numero tageissa</param>
		void TarkistaTila(int numero)
		{
			lock (lockme)
				try
				{
					// Ensimmäinen tarkistuskerta, lisätään lokiikka numero (objekti) listaan 
					if (!Watchdog_Wait.ContainsKey(numero))
					{
						Watchdog_Wait.Add(numero, 0);
					}

					// Pakotetaan tagi luku, esitetään että yhteys on kunnossa vaikka tulisi 
					// Bad Station hälytystä. Silloin on vain tagien määrittelyssä vikaa
					Neo.ApplicationFramework.Interfaces.Tag.IBasicTag tag = Globals.Tags.GetTag("HMI_Comm_Watchdog_From_PLC" + numero);

					if (tag != null)
						tag.Read();
					else
						System.Diagnostics.Trace.WriteLine(string.Format("[NULL] Virhe tagin [HMI_Comm_Watchdog_From_PLC{0}] haussa", numero));

					// Tarkistetaan onko logiikan arvo muuttunut
					if (Globals.Tags.GetTagValue("HMI_Comm_Watchdog_From_PLC" + numero) != Globals.Tags.GetTagValue("HMI_Comm_Watchdog_From_PLC" + numero + "_Old"))
					{
						// On muuttunut, tallennetaan uusi arvo
						Globals.Tags.SetTagValue("HMI_Comm_Watchdog_From_PLC" + numero + "_Old", Globals.Tags.GetTagValue("HMI_Comm_Watchdog_From_PLC" + numero));
						// Kirjoitetaan logiikalle uusi arvo
						Globals.Tags.SetTagValue("HMI_Comm_Watchdog_To_PLC" + numero, Globals.Tags.GetTagValue("HMI_Comm_Watchdog_From_PLC" + numero + "_Old"));

						// Nollataan virhe ajastin
						Watchdog_Wait[numero] = 0;
					}
					else
					{
						// Arvo on pysynyt samana eli logiikka ei ole hereillä, kasvatetaan virhelaskuria
						Watchdog_Wait[numero] += 1;
					}

					// Odotellaan hetki ennen hälyttämistä
					if (Watchdog_Wait[numero] > 7)
					{
						// Hälytetään yhteys poikki
						Globals.Tags.SetTagValue("Line1_Comm_Fault_PLC" + numero, 1);
					}
					else
					{
						// Aika on, nollataan hälytys
						Globals.Tags.SetTagValue("Line1_Comm_Fault_PLC" + numero, 0);
					}
				}
				catch (Exception x)
				{
					System.Diagnostics.Trace.WriteLine(string.Format("Exception [Lokiikat.TarkistaTila] {0}", x.Message));
				}
		}
	}
}
