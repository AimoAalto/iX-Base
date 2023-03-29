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
		object lockme = new object();
		/// <summary>
		/// Robottien yhteydet.
		/// </summary>
		Dictionary<int, Robotti> robotit = new Dictionary<int, Robotti>(); // Robotit
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
			/**/
			if (Globals.Tags.TraceAll) System.Diagnostics.Trace.WriteLine("Robotit Created (start)");
			int interval = 1000;
			try
			{
				interval = Globals._Konfiguraatio.CurrentConfig.Aikavali("RobottiWatchdog");
			}
			catch (Exception x)
			{
				Globals.Tags.Log(String.Format("Robotit_Created: Interval error, use default\n{1}", x.Message));
			}

			Watchdog = new Timer((args) =>
			{
				// Mitataan kauanko operaatioissa kestää
				Stopwatch takeTime = new Stopwatch();
				takeTime.Start();

				// Robottien tilan tarkistus
				// Näytetään näytöllä, jos jollain robotilla yhteysvika
				Globals.Tags.HMI_CommFault_Robots.ResetTag();
				foreach (int robotti in robotit.Keys)
				{
					TarkistaTila(robotti);
					if (Globals.Tags.GetTagValue("HMI_CommFault_Rob" + robotti).Bool) Globals.Tags.HMI_CommFault_Robots.SetTag();
				}

				takeTime.Stop();

				if (Globals.Tags.TraceAll) System.Diagnostics.Trace.WriteLine(string.Format("Robotit time : {0} (ticks)", takeTime.ElapsedTicks));

				// Suoritetaan määritetyin välein (default 1s)
				Watchdog.Change(Math.Max(0, interval - takeTime.ElapsedMilliseconds), Timeout.Infinite);
			}, null, 0, Timeout.Infinite);
			/**/
		}

		/// <summary>
		/// Get loki object for selected robot
		/// </summary>
		/// <param name="robotti"></param>
		/// <returns></returns>
		public Lavaus.Virheloki GetLoki(int robotti)
		{
			Exists(robotti);
			return robotit[robotti].Loki;
		}

		/// <summary>
		/// get first robott object
		/// create if non exists
		/// </summary>
		/// <returns></returns>
		public int First()
		{
			if (robotit.Count == 0)
				Exists(1); // atleast one robot
			return robotit.Keys.First();
		}

		/// <summary>
		/// get next robot from robotit list
		/// returns first if selected is last
		/// </summary>
		/// <param name="no"></param>
		/// <returns></returns>
		public int Next(int no)
		{
			int ret = -1;

			if (robotit.Count == 0)
				Exists(1); // atleast one robot

			bool getnext = false;

			foreach (int key in robotit.Keys)
			{
				if (getnext)
				{
					ret = key;
					break;
				}
				if (key == no) getnext = true;
			}

			if (ret < 0) ret = First();

			return ret;
		}

		/// <summary>
		/// get previous robot object from robotit list
		/// </summary>
		/// <param name="no"></param>
		/// <returns></returns>
		public int Previous(int no)
		{
			int ret = -1;

			foreach (int key in robotit.Keys)
			{
				if (key == no) break;
				ret = key;
			}

			if (ret < 0) ret = First();

			return ret;
		}

		/// <summary>
		/// Siirtää robotin lähettämän tagin arvon iX:n tagiin.
		/// </summary>
		/// <param name="sender">Lavaus.Robotti</param>
		/// <param name="e">Kohdetagin tiedot ja arvo</param>
		void Robotti_TagiMuuttunut(object sender, Lavaus.TagiEventArgs e)
		{
			// Onko luku vai stringi?
			if (e.Tyyppi == Lavaus.ArvonTyyppi.luku)
			{
				// Luku
				VariantValue value = new VariantValue(Convert.ToDouble(e.Arvo, System.Globalization.CultureInfo.InvariantCulture));
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
				if (e.Tunnus == "KerrosAsetukset")
				{

					// Puretaan merkkijonosta luvut
					string[] viesti = e.Data.Split('$');
					string[] tiedot = viesti.Where((j, k) => k > 1).ToArray();

					Globals.Tags.SetTagValue("KerroksetLavalla", viesti[1]);
					const int KERROS_MAX = 10;

					if (tiedot.Length / 2 > KERROS_MAX)
					{
						Globals.Tags.Log("Robotti_ParametritSaapunut: viestissä liian monta alkiota(" + (tiedot.Length / 2).ToString() + "). Sallittu max on " + KERROS_MAX.ToString() + ". Ylimääräisiä ei kirjoitettu!");
					}

					for (int i = 0; i < KERROS_MAX * 2; i++)
					{
						try
						{
							string kerros = (i / 2 + 1).ToString();
							if (i < tiedot.Length)
							{
								// Jaotellaan Vienti arvot parillisiin ja Tyyppi arvot parittomiin
								if (i % 2 == 0)
								{
									Globals.Tags.SetTagValue("Kerros" + kerros + "_Viennit", tiedot[i]);
								}
								else
								{
									Globals.Tags.SetTagValue("Kerros" + kerros + "_Tyyppi", tiedot[i]);
								}
							}
							else
							{
								// Nollataan loput joita ei ollut viestissä
								Globals.Tags.SetTagValue("Kerros" + kerros + "_Viennit", 0);
								Globals.Tags.SetTagValue("Kerros" + kerros + "_Tyyppi", 0);
							}
						}
						catch (Exception ex)
						{
							Globals.Tags.Log("Exception Robotti_ParametritSaapunut! - Virhe viestin purussa!" + ex.Message);
						}
					}
					Globals.Tags.Log("Robotti_ParametritSaapunut: Viesti purettu( " + e.Data.ToString() + ")");

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
		private void TarkistaTila(int numero)
		{
			lock (lockme)
				if (Exists(numero))
				{
					// Kysytään yhteyden tilaa
					Globals.Tags.SetTagValue("Rob" + numero + "_Tila", robotit[numero].Tila);

					// Watchdog
					Globals.Tags.SetTagValue("HMI_CommFault_Rob" + numero, robotit[numero].WatchdogHalytys);
				}
		}

		/// <summary>
		/// Tarkistaa onko robotti objecti olemassa listalla
		/// robotti luodaan sen puuttuessa
		/// </summary>
		/// <param name="robotti">Robotin numero</param>
		/// <returns>exists = false, jos robottiobjekti luotiin</returns>
		public bool Exists(int numero)
		{
			bool exists = true;
			if (!robotit.ContainsKey(numero))
			{
				// Alustetaan robotti
				robotit.Add(numero, new Lavaus.Robotti(numero));

				// Liitytään tagien muutos eventtiin
				robotit[numero].TagiMuuttunut += Robotti_TagiMuuttunut;
				robotit[numero].Mokasit += Robotti_Mokasit;
				robotit[numero].Parametrisanoma += Robotti_ParametritSaapunut;
				exists = false;
			}
			return exists;
		}

		/// <summary>
		/// Kuittaa robotti häiriö
		/// </summary>
		/// <param name="robotti"></param>
		/// <param name="id"></param>
		/// <param name="num"></param>
		/// <returns></returns>
		public void KuittaaHairio(int robotti, int id, Int16 num)
		{
			try
			{
				Exists(robotti); // jos robotti ei ole olemassa, se luodaan

				robotit[robotti].Loki.LisaaLokiin(string.Format("Kuittaus {0} - {1}", id, num));
				robotit[robotti].KuittaaHairio(id, num);
			}
			catch (Exception ex)
			{
				Globals.Tags.Log("Exception KuittaaHairio! " + ex.Message);
			}
		}

		/// <summary>
		/// Huoltoasemaan ajopyyntö bobotille
		/// </summary>
		/// <param name="robotti"></param>
		/// <returns></returns>
		public void AjaHuoltoon(int robotti)
		{
			try
			{
				Exists(robotti); // jos robotti ei ole olemassa, se luodaan

				robotit[robotti].Loki.LisaaLokiin("Ajopyyntö huoltoasemaan.");
				robotit[robotti].AjaHuoltoon();
			}
			catch (Exception ex)
			{
				Globals.Tags.Log("Exception AjaHuoltoon! " + ex.Message);
			}
		}

		/// <summary>
		/// Lisää viesti lokiin
		/// </summary>
		/// <param name="robotti"></param>
		/// <param name="msg"></param>
		/// <returns></returns>
		public void LisaaLokiin(int robotti, string msg)
		{
			try
			{
				Exists(robotti); // jos robotti ei ole olemassa, se luodaan

				robotit[robotti].Loki.LisaaLokiin(msg);
			}
			catch (Exception ex)
			{
				Globals.Tags.Log("Exception LisaaLokiin! " + ex.Message);
			}
		}

		/// <summary>
		/// Lavanvaihtopyyntö bobotille
		/// </summary>
		/// <param name="robotti"></param>
		/// <param name="lavapaikka"></param>
		/// <returns></returns>
		public void TeeLavanvaihto(int robotti, int lavapaikka)
		{
			try
			{
				Exists(robotti); // jos robotti ei ole olemassa, se luodaan

				robotit[robotti].TeeLavanvaihto(lavapaikka);
			}
			catch (Exception ex)
			{
				Globals.Tags.Log("Exception TeeLavanvaihto! " + ex.Message);
			}
		}

		/// <summary>
		/// Huoltoasemaan ajopyyntö bobotille
		/// </summary>
		/// <param name="robotti"></param>
		/// <param name="lavapaikka"></param>
		/// <returns></returns>
		public void TeeRyhmittelynLavanvaihto(int robotti, int lavapaikka)
		{
			try
			{
				Exists(robotti); // jos robotti ei ole olemassa, se luodaan

				robotit[robotti].TeeRyhmittelynLavanvaihto(lavapaikka);
			}
			catch (Exception ex)
			{
				Globals.Tags.Log("Exception TeeRyhmittelynLavanvaihto! " + ex.Message);
			}
		}

		public void PaikkaNopeus(int robotti, int paikka, int nopeus, int kiihtyvyys, double tartuntaviive, double jattoviive)
		{
			try
			{
				Exists(robotti); // jos robotti ei ole olemassa, se luodaan

				robotit[robotti].PaikkaNopeus(paikka, nopeus, kiihtyvyys, tartuntaviive, jattoviive);
			}
			catch (Exception ex)
			{
				Globals.Tags.Log("Exception PaikkaNopeus! " + ex.Message);
			}
		}

		public void PaikkaNopeus(int robotti, string command_id, int paikka, int nopeus, int kiihtyvyys, double tartuntaviive, double jattoviive)
		{
			try
			{
				Exists(robotti); // jos robotti ei ole olemassa, se luodaan

				robotit[robotti].PaikkaNopeus(command_id, paikka, nopeus, kiihtyvyys, tartuntaviive, jattoviive);
			}
			catch (Exception ex)
			{
				Globals.Tags.Log("Exception PaikkaNopeus! " + ex.Message);
			}
		}

		public void Nopeus(int robotti, int nopeus_tyhja, int kiihtyvyys_tyhja, int nopeus_lava, int kiihtyvyys_lava, int nopeus_pahvi, int kiihtyvyys_pahvi)
		{
			try
			{
				Exists(robotti); // jos robotti ei ole olemassa, se luodaan

				robotit[robotti].Nopeus(nopeus_tyhja, kiihtyvyys_tyhja, nopeus_lava, kiihtyvyys_lava, nopeus_pahvi, kiihtyvyys_pahvi);
			}
			catch (Exception ex)
			{
				Globals.Tags.Log("Exception Nopeus! " + ex.Message);
			}
		}

		public void PaikkaOffset(int robotti, string command_id, int paikka, double X, double Y)
		{
			try
			{
				Exists(robotti); // jos robotti ei ole olemassa, se luodaan

				robotit[robotti].PaikkaOffset(command_id, paikka, X, Y);
			}
			catch (Exception ex)
			{
				Globals.Tags.Log("Exception PaikkaOffset! " + ex.Message);
			}
		}

		public void AsetaKerrosmaara(int robotti, int roboLavapaikka, Int16 kerrokset)
		{
			try
			{
				Exists(robotti); // jos robotti ei ole olemassa, se luodaan

				robotit[robotti].AsetaKerrosmaara(roboLavapaikka, kerrokset);
			}
			catch (Exception ex)
			{
				Globals.Tags.Log("Exception AsetaKerrosmaara! " + ex.Message);
			}
		}

		public void AsetaKerrosmaara(int robotti, string command_id, int lavapaikka, Int16 kerrokset)
		{
			try
			{
				Exists(robotti); // jos robotti ei ole olemassa, se luodaan

				robotit[robotti].AsetaKerrosmaara(command_id, lavapaikka, kerrokset);
			}
			catch (Exception ex)
			{
				Globals.Tags.Log("Exception AsetaKerrosmaara! " + ex.Message);
			}
		}

		public string TeeAloitus(int robotti, List<int> tuloradat, List<int> lavapaikat, int pattern, Lavaus.Kuvio kuvio)
		{
			string ret = "";
			try
			{
				Exists(robotti); // jos robotti ei ole olemassa, se luodaan

				ret = robotit[robotti].TeeAloitus(tuloradat, lavapaikat, pattern, kuvio);
			}
			catch (Exception ex)
			{
				Globals.Tags.Log("Exception TeeAloitus! " + ex.Message);
			}
			return ret;
		}

		public void TeeLopetus(int robotti, int tulorata)
		{
			try
			{
				Exists(robotti); // jos robotti ei ole olemassa, se luodaan

				robotit[robotti].TeeLopetus(tulorata);
			}
			catch (Exception ex)
			{
				Globals.Tags.Log("Exception TeeLopetus! " + ex.Message);
			}
		}

		public void AsetaPahvit(int robotti, int lavapaikka, string pahvit)
		{
			try
			{
				Exists(robotti); // jos robotti ei ole olemassa, se luodaan

				robotit[robotti].AsetaPahvit(lavapaikka, pahvit);
			}
			catch (Exception ex)
			{
				Globals.Tags.Log("Exception AsetaPahvit! " + ex.Message);
			}
		}

		public void AsetaPahvit(int robotti, string command_id, int lavapaikka, string pahvit)
		{
			try
			{
				Exists(robotti); // jos robotti ei ole olemassa, se luodaan

				robotit[robotti].AsetaPahvit(command_id, lavapaikka, pahvit);
			}
			catch (Exception ex)
			{
				Globals.Tags.Log("Exception AsetaPahvit! " + ex.Message);
			}
		}

		public void AloituksenLopetus(int robotti, string command_id)
		{
			try
			{
				Exists(robotti); // jos robotti ei ole olemassa, se luodaan

				robotit[robotti].AloituksenLopetus(command_id);
			}
			catch (Exception ex)
			{
				Globals.Tags.Log("Exception AloituksenLopetus! " + ex.Message);
			}
		}
	}
}
