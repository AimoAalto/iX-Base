namespace Neo.ApplicationFramework.Generated
{
	using System;
	using System.Linq;
	using System.Threading;
	
    
	/// <summary>
	/// Näyttää kaikkien robottienvirhelokin.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 5.7.2017</remarks>
    public partial class Settings_Pan1_Scr4
    {
		/// <summary>
		/// Lukko, jolla pidetään uusi päivitys odottamassa ja ylimääräiset
		/// pyynnöt ohjataan pois.
		/// </summary>
		object GUIUpdateLock = new object();
		/// <summary>
		/// Odotusobjekti, johon odottava päivityspyyntö pysäytetään.
		/// </summary>
		ManualResetEvent SaaPaivittaa = new ManualResetEvent(true);
		
		/// <summary>
		/// Lataa ensimmäisen robotin lokin ja liittyy seuraamaan sen muutoksia.
		/// </summary>
		/// <param name="sender">this</param>
		void Settings_Robot_Opened(System.Object sender, System.EventArgs e)
		{
			try
			{
				// Avataan ensin pienin robottinumero
				RobottiNo = Globals.Robotit.robotit.Keys.Min();
			
				// Loki
				LataaLoki();
			
				// Sivun ladattua seurataan, jos lista muuttuu ja pitää päivittää
				Globals.Robotit.robotit[RobottiNo].Loki.LokiMuuttunut += Loki_LokiMuuttunut;
			}
			catch (Exception x)
			{
				System.Windows.Forms.MessageBox.Show(x.Message);
			}
		}
		
		/// <summary>
		/// Päivittää robottin virhelokin näytölle. Siirtää näkymän ja valinnan
		/// uusimpiin tapahtumiin.
		/// </summary>
		void LataaLoki()
		{
			// Tyhjennetään lokit			
			Virhelista.Items.Clear();

			// Lisätään kaikki rivit
			foreach (string a in Globals.Robotit.robotit[RobottiNo].Loki.LueLoki())
			{
				Virhelista.Items.Add(a);
			}
			
			// Scrollataan listaa mukana
			Virhelista.SelectedIndex = Virhelista.Items.Count - 1;
			Virhelista.AdaptedObject.CastTo<Neo.ApplicationFramework.Controls.WindowsControls.ListBox>()
				.ScrollIntoView(Virhelista.SelectedItem);
		}

		/// <summary>
		/// Päivittää lokin. kun se muuttuu, jos päivitys on sallittu. 
		/// Päivitys sallitaan korkeintaan kerran sekunnissa.
		/// </summary>
		/// <param name="sender">Robotti.Loki</param>
		void Loki_LokiMuuttunut(object sender, EventArgs e)
		{
			bool stop = false;
			Dispatcher.Invoke(new Action(() => stop = StopUpdate));
			// Lista on päivittynyt, päivitä myös GUI, jos sallittu
			if (!stop)
			{
				// Vain yksi päivityspyyntö voi odottaa kerrallaan
				if (Monitor.TryEnter(GUIUpdateLock))
				{
					try
					{
						// Odotetaan, että saa mennä päivittämään näytön
						SaaPaivittaa.WaitOne();
				
						// Viritetään odotus uudestaan seuraavaa odottajaa varten
						SaaPaivittaa.Reset();
					}
					finally
					{
						Monitor.Exit(GUIUpdateLock);
					}
				}
				else
				{
					// Jos lukossa on jo odottaja, lopetetaan
					return;
				}
			
				// GUI voi pyöriä eri säikeessä kuin eventti, joten
				Dispatcher.Invoke(new Action(() =>
					{
					LataaLoki(); 
					}));  
					
				// Sallitaan päivitys enintään sekunnin välein
				Thread.Sleep(1000);
			
				// Seuraava päivittäjä saa tulla sisään
				SaaPaivittaa.Set();
			}
		}
		
		/// <summary>
		/// Valitsee numerojärjestyksessä seuraavan robotin ja lataa sen lokin.
		/// </summary>
		/// <param name="sender">this.Robot_next_btn</param>
		void Robot_next_btn_Click(System.Object sender, System.EventArgs e)
		{
			if (Globals.Robotit.robotit.Count > 1)
			{
				// Irroittaudutaan muutoksen seurannasta
				Globals.Robotit.robotit[RobottiNo].Loki.LokiMuuttunut -= Loki_LokiMuuttunut;
				
				// Haetaan seuraavan robotin numero
				try
				{
					RobottiNo = Globals.Robotit.robotit.Keys.Where(p => p > RobottiNo).Min();
				}
				catch
				{
					// Ei ollut isompaa numeroa, haetaan pienin
					RobottiNo = Globals.Robotit.robotit.Keys.Min();
				}
				
				// Päivitetään näyttö
				LataaLoki();
				
				// Liitytään mutoksen seurantaan
				Globals.Robotit.robotit[RobottiNo].Loki.LokiMuuttunut += Loki_LokiMuuttunut;
			}
		}
		
		/// <summary>
		/// Valitsee numerojärjestyksessä edellisen robotin ja lataa sen lokin.
		/// </summary>
		/// <param name="sender">this.Robot_prev_btn</param>
		void Robot_prev_btn_Click(System.Object sender, System.EventArgs e)
		{
			if (Globals.Robotit.robotit.Count > 1)
			{
				// Irroittaudutaan muutoksen seurannasta
				Globals.Robotit.robotit[RobottiNo].Loki.LokiMuuttunut -= Loki_LokiMuuttunut;
				
				// Haetaan edellisen robotin numero
				try
				{
					RobottiNo = Globals.Robotit.robotit.Keys.Where(p => p < RobottiNo).Max();
				}
				catch 
				{
					// Ei ollut pienempää numeroa, haetaan suurin
					RobottiNo = Globals.Robotit.robotit.Keys.Max();
				}
				
				// Päivitetään näyttö
				LataaLoki();
				
				// Liitytään mutoksen seurantaan
				Globals.Robotit.robotit[RobottiNo].Loki.LokiMuuttunut += Loki_LokiMuuttunut;
			}
		}

		/// <summary>
		/// Irroittautuu lokin seurannasta sivun sulkeutuessa.
		/// </summary>
		/// <param name="sender">this</param>
		void Settings_Robot_Closing(System.Object sender, System.ComponentModel.CancelEventArgs e)
		{
			try 
			{
				// Sivu alkaa sulkeutua, irroittaudutaan kaikista tapahtumista ja ajastimista
				Globals.Robotit.robotit[RobottiNo].Loki.LokiMuuttunut -= Loki_LokiMuuttunut;
			}
			catch (Exception x)
			{
				System.Windows.Forms.MessageBox.Show(x.Message);
			}
		}
				
		/// <summary>
		/// Pysäyttää lokinäytön päivityksen.
		/// </summary>
		/// <param name="sender">this.Stop_update_btn</param>
		void Stop_update_btn_Click(System.Object sender, System.EventArgs e)
		{
			// Koska toggle alias ei toiminut jostain mystisestä syystä
			StopUpdate = !StopUpdate;
			
			// Päivitetään heti, jos sallittiin
			if (!StopUpdate)
			{
				LataaLoki();
			}
		}
    }
}
