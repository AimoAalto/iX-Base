namespace Neo.ApplicationFramework.Generated
{
	using Neo.ApplicationFramework.Interfaces;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using Lavaus;
    
    
	/// <summary>
	/// Sisältää robottien yhteydet ja niiden monitoroinnin. Sisältää myös sallittujen 
	/// kuvioiden hakufunktiot, sillä kuviorajoitteet johtuvat roboteista.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 7.7.2017</remarks>
    public partial class Robotit
    {
		/// <summary>
		/// Robottien yhteydet.
		/// </summary>
		public Dictionary<int, Robotti> robotit = new Dictionary<int, Robotti>(); // Robotit
		/// <summary>
		/// Ikuisesti jatkuva robottiyhteyksien taustamonitoroinnin ajastin.
		/// </summary>
		private Timer Watchdog;

		/// <summary>
		/// Ajastaa robottien yhteyksien taustamonitoroinnin.
		/// </summary>
		/// <param name="sender">this</param>
		void Robotit_Created(System.Object sender, System.EventArgs e)
		{
			Watchdog = new Timer((args) => {
				// Mitataan kauanko operaatioissa kestää
				Stopwatch takeTime = new Stopwatch();
				takeTime.Start();
				
				// Robottien tilan tarkistus
				foreach (int robotti in _Konfiguraatio.RobotinLavapaikat.Keys)
				{
					TarkistaTila(robotti);
				}
				
				// Näytetään näytöllä, jos jollain robotilla yhteysvika
				Globals.Tags.HMI_CommFault_Robots.ResetTag();
				foreach (int robotti in robotit.Keys)
				{
					if (Globals.Tags.GetTagValue("HMI_CommFault_Rob" + robotti).Bool)
					{
						Globals.Tags.HMI_CommFault_Robots.SetTag();
						break;
					}
				}
				
				// Suoritetaan määritetyin välein (default 1s)
				takeTime.Stop();
				Watchdog.Change(Math.Max(0, _Konfiguraatio.Aikavalit["RobottiWatchdog"] - takeTime.ElapsedMilliseconds), Timeout.Infinite);
			}, null, 0, Timeout.Infinite);
		}	

		/// <summary>
		/// Hakee kaikki tuloradalle sallitut kuvionumerot.
		/// </summary>
		/// <param name="tulorata">Tuloradan numero</param>
		/// <returns>Sallittujen kuvioiden numerot</returns>
		public int[] HaeSallitutKuviot(int tulorata)
		{
			// Luetaan kuviot JSON-tiedostosta
			List<Kuviotiedot> kaikkiKuviot = Kuviotiedot.LataaKuviot();

			// Valitaan kuvionumerot, joille kelpaa tulorata
			List<int> tuloratakuviot = kaikkiKuviot
				.Where(p => p.sallitutTuloradat.Contains(tulorata))
				.Select(p => p.numero).ToList();
				
			return tuloratakuviot.ToArray();
		}
		
		/// <summary>
		/// Hakee kaikki tuloradalle ja lavapaikalle sallitut kuvionumerot.
		/// </summary>
		/// <param name="tulorata">Tuloradan numero</param>
		/// <param name="lavapaikka">Lavapaikan numero</param>
		/// <returns>Sallittujen kuvioiden numerot</returns>
		public int[] HaeSallitutKuviotLavapaikalla(int tulorata, int lavapaikka)
		{
			// Luetaan kuviot JSON-tiedostosta
			List<Kuviotiedot> kaikkiKuviot = Kuviotiedot.LataaKuviot();

			// Valitaan kuvionumerot, joille kelpaa sekä tulorata että lavapaikka
			List<int> kuviolist = kaikkiKuviot
				.Where(p => p.sallitutTuloradat.Contains(tulorata) && p.sallitutLavapaikat.Contains(lavapaikka))
				.Select(p => p.numero).ToList();

			return kuviolist.ToArray();
		}
				
		/// <summary>
		/// Siirtää robotin lähettämän tagin arvon iX:n tagiin.
		/// </summary>
		/// <param name="sender">Lavaus.Robotti</param>
		/// <param name="e">Kohdetagin tiedot ja arvo</param>
		void Robotti_TagiMuuttunut(object sender, Lavaus.TagiEventArgs e)
		{
			// Onko luku vai stringi?
			if(e.Tyyppi == Lavaus.ArvonTyyppi.luku)
			{
				// Luku
				VariantValue value = new VariantValue(Convert.ToDouble(e.Arvo,System.Globalization.CultureInfo.InvariantCulture));
				Globals.Tags.SetTagValue(e.Tagi, value);
			}
			else
			{
				// Stringi
				VariantValue value = new VariantValue(e.Arvo);
				Globals.Tags.SetTagValue(e.Tagi, value);
			}                
		}
		
		/// <summary>
		/// Näyttää ikkunan Popup_ProdStartError, kun robotin aloitus ei onnistunut.
		/// </summary>
		/// <param name="sender">Lavaus.Robotti</param>
		/// <param name="e">Robotti.Mokasit lähettämät parametrit</param>
		void Robotti_Mokasit(object sender, Lavaus.MokasitEventArgs e)
		{
			// Katsotaan mikä robotti lähetti
			foreach (var robotti in robotit)
			{
				if ((Robotti)sender == robotti.Value)
				{
					// Lisätään loki merkintä
					robotti.Value.Loki.LisaaLokiin("Aloitusvirhe: " + e.Virhe);

					// Avataan sivu vain kun virhe (esim. -1000) ei ole aktiivisena
					if (Globals.Tags.GetTagValue("Rob" + robotti.Key + "_Aloitusvirhe") >= 0)
					{
						Globals.Tags.SetTagValue("Rob" + robotti.Key + "_Aloitusvirhe", e.Virhe);
						Globals.Tags.HMI_RobotNo.Value = robotti.Key; // Kertoo ikkunalle mikä robotti
						Globals.Popup_ProdStartError.Show();
					}
				}
			}
		}
		
		/// <summary>
		/// Tallentaa paramp sanoma tiedot muuttujiin
		/// </summary>
		/// <param name="sender">Lavaus.Robotti</param>
		/// <param name="e">Robotti.Parametrisanoma lähettämät parametrit</param>
		public void Robotti_ParametritSaapunut(object sender, Lavaus.ParametrisanomaEventArgs e)
		{
			try
			{
				if(e.Tunnus == "KerrosAsetukset")
				{
					
					// Puretaan merkkijonosta luvut
					string[] viesti = e.Data.Split('$');
					string[] tiedot = viesti.Where((j, k) => k > 1).ToArray();
					
					Globals.Tags.SetTagValue("KerroksetLavalla",viesti[1]);
					const int KERROS_MAX = 10;

					if(tiedot.Length/2 > KERROS_MAX)
					{
						Globals.Tags.Log("Robotti_ParametritSaapunut: viestissä liian monta alkiota("+(tiedot.Length/2).ToString()+"). Sallittu max on "+KERROS_MAX.ToString()+". Ylimääräisiä ei kirjoitettu!");
					}
	
					for(int i=0; i<KERROS_MAX*2; i++)
					{
						try
						{
							string kerros = (i/2+1).ToString();
							if(i < tiedot.Length)
							{
								// Jaotellaan Vienti arvot parillisiin ja Tyyppi arvot parittomiin
								if(i%2 == 0)
								{
									Globals.Tags.SetTagValue("Kerros"+kerros+"_Viennit",tiedot[i]);
								}
								else
								{
									Globals.Tags.SetTagValue("Kerros"+kerros+"_Tyyppi",tiedot[i]);	
								}
							}
							else
							{
								// Nollataan loput joita ei ollut viestissä
								Globals.Tags.SetTagValue("Kerros"+kerros+"_Viennit",0);	
								Globals.Tags.SetTagValue("Kerros"+kerros+"_Tyyppi",0);	
							}
						}
						catch (Exception ex)
						{
							Globals.Tags.Log("Exception Robotti_ParametritSaapunut! - Virhe viestin purussa!" + ex.Message);
						}
					}
					Globals.Tags.Log("Robotti_ParametritSaapunut: Viesti purettu( " +e.Data.ToString() +")");
					
				}
			}
			catch (Exception ex)
			{
				Globals.Tags.Log("Exception Robotti_ParametritSaapunut! " + ex.Message);
			}	
		}
		
		/// <summary>
		/// Pyytää robottia päivittämään tilansa ja luo yhteyshälytyksen, jos robotin
		/// watchdog hälyttää.
		/// </summary>
		/// <param name="numero">Robotin numero</param>
		void TarkistaTila(int numero)
		{
			if (!robotit.ContainsKey(numero))
			{
				// Alustetaan robotti
				robotit.Add(numero, new Lavaus.Robotti(numero));
				
				// Liitytään tagien muutos eventtiin
				robotit[numero].TagiMuuttunut += Robotti_TagiMuuttunut;
				robotit[numero].Mokasit += Robotti_Mokasit;
				robotit[numero].Parametrisanoma += Robotti_ParametritSaapunut;
			}
			else
			{
				// Kysytään yhteyden tilaa
				Globals.Tags.SetTagValue("Rob" + numero + "_Tila", robotit[numero].Tila);

				// Watchdog
				Globals.Tags.SetTagValue("HMI_CommFault_Rob" + numero, robotit[numero].WatchdogHalytys);
			}
		}
    }
}
