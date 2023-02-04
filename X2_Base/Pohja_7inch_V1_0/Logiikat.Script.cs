namespace Neo.ApplicationFramework.Generated
{
	using Neo.ApplicationFramework.Tools.OpcClient;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;    
    
	
	/// Sisältää logiikoiden yhteyksien taustamonitoroinnin.
    public partial class Logiikat
	{
		
		public static readonly int Logiikkoja = 1;

		int LogiikkaWatchdog = 1000; // aikaväli 1s
		
		/// Ikuisesti jatkuva logiikkayhteyksien taustamonitoroinnin ajastin.
		Timer Watchdog;

		/// Logiikkakohtainen epäonnistuneiden tarkistuskertojen laskuri ennen hälytystä.
		Dictionary<int, int> Watchdog_Wait = new Dictionary<int, int>();

		/// Ajastaa logiikkojen yhteyden taustamonitoroinnin sovelluksen käynnistyessä.
		void Logiikat_Created(System.Object sender, System.EventArgs e)
		{
			Watchdog = new Timer((args) => {
				// Mitataan kauanko operaatioissa kestää
				Stopwatch takeTime = new Stopwatch();
				takeTime.Start();
				
				// Tarkista kaikkien logiikkojen tila
				for (int i = 1; i <= Logiikkoja; i++)
				{
					TarkistaTila(i);
				}
				
				// Suoritetaan määritetyin välein (default 1s)
				takeTime.Stop();
				Watchdog.Change(Math.Max(0, LogiikkaWatchdog - takeTime.ElapsedMilliseconds), Timeout.Infinite);
			}, null, 0, Timeout.Infinite);
		}
		
		/// Tarkistaa, onko logiikka päivittänyt From_PLC-tagia ja vastaa päivittämällä
		/// To_PLC-tagia. Jos logiikka ei ole päivittänyt tagia, hälytetään yhteyden 
		/// katkeamisesta 7 tarkistuksen jälkeen.
		void TarkistaTila(int numero)
		{
			try 
			{	        
				// Ensimmäinen tarkistuskerta, lisätään hälytyksen viiveen listaan
				if (!Watchdog_Wait.ContainsKey(numero))
				{
					Watchdog_Wait.Add(numero, 0);
				}
		
				// Pakotetaan tagi luku, esitetään että yhteys on kunnossa vaikka tulisi 
				// Bad Station hälytystä. Silloin on vain tagien määrittelyssä vikaa
				Globals.Tags.GetTag("HMI_Comm_Watchdog_From_PLC" + numero).Read();
		
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
				if (Watchdog_Wait[numero] > 10)
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
				Globals.Tags.Log(string.Format("Logiikat.Tarkistatila Exception: {0}", x.Message));
			}
		}
    }
}
