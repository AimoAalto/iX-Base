//--------------------------------------------------------------
// Press F1 to get help about using script.
// To access an object that is not located in the current class, start the call with Globals.
// When using events and timers be cautious not to generate memoryleaks,
// please see the help for more information.
//---------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
	using System;
	//using System.Drawing;
	//using Neo.ApplicationFramework.Tools;
	//using Neo.ApplicationFramework.Common.Graphics.Logic;
	//using Neo.ApplicationFramework.Controls;
	//using Neo.ApplicationFramework.Interfaces;


	/// <summary>
	/// 1. jos ohjelma sammutetaan ennen kuin ajastimen tai säikeen (thread) koodi on suoritettu loppuun 
	///    viittauksen sovelluksen ja sen ikkunoiden yleisiin muuttujiin ja/tai koodiin aiheuttaa poikkeuksen, koska koodia ei enöä ole suoritettavissa
	/// </summary>

	public partial class MainScreen
	{
		#region variables
		
		System.Windows.Forms.Timer formstimer = null;
		System.Timers.Timer timerstimer = null;
		System.Threading.Timer threadingtimer = null;

		bool running = false;
		int timercount = 0;
		int timerokcount = 0;
		
		bool loop = false;
		System.Threading.Thread localthread;
		
		WorkThread wthread = null;
		int externalcount = 0;

		#endregion
		
		void Screen1_Opened(System.Object sender, System.EventArgs e)
		{
			this.Title = "DO'S and DONT'S...jotain uutta, jotain vanhaa";
		}

		#region timers

		/// <summary>
		/// Forms.Timer
		///  - ei suositella käytettäväksi
		///  - koodi ajetaan ikkunan säikeessä => vaikuttaa ikkunan toimintaan
		///    ikkunan toiminnot 'jumissa' ajastimen koodiin suorituksen ajan
		///  - jos koodin suoritus kestää enemmän aikaa kuin ajastimen liipaisuaika
		///    Koodi suoritetaan päällekkäin edellisen ajastimen liipaisun kanssa ja johtaa suorittimen muistin varausvirheeseen 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		void Btn_StartTimerForms_Click(System.Object sender, System.EventArgs e)
		{
			if (running)
			{
				System.Windows.Forms.MessageBox.Show("Odota ajastimien taustaajon loppuminen");
			}
			else if (formstimer != null || timerstimer != null || threadingtimer != null)
			{
				System.Windows.Forms.MessageBox.Show("Stop timer first!");
			}
			else
			{
				timercount = 0;
				timerokcount = 0;
				formstimer = new System.Windows.Forms.Timer();
				formstimer.Tick += new EventHandler(FormsTimerFunc);
				formstimer.Interval = 1000;
				formstimer.Start();
			}
		}

		/// <summary>
		/// Forms timer event
		/// </summary>
		/// <param name="o"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		void FormsTimerFunc(Object o, EventArgs args)
		{
			timercount++;
			Globals.Tags.HMI_TimerCount.Value = timercount;

			ANVarValue.Text = timercount.ToString();

			/// jos pitkään vievää ajoa suoritetaan ei suoritetan uudestaan
			if (!running)
			{
				running = true;

				System.Threading.Thread.Sleep(3000);

				timerokcount++;
				ANVarOkValue.Text = timerokcount.ToString();

				running = false;
			}
		}

		/// <summary>
		/// Omassa säikeessä ajettava ajastin
		///  - jos koodin suoritus kestää enemmän aikaa kuin ajastimen liipaisuaika
		///    Koodi suoritetaan päällekkäin edellisen ajastimen liipaisun kanssa ja johtaa suorittimen muistin varausvirheeseen 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		void Btn_StartTimerTimers_Click(System.Object sender, System.EventArgs e)
		{
			if (running)
			{
				System.Windows.Forms.MessageBox.Show("Odota ajastimien taustaajon loppuminen");
			}
			else if (formstimer != null || timerstimer != null || threadingtimer != null)
			{
				System.Windows.Forms.MessageBox.Show("Stop timer first!");
			}
			else
			{
				timercount = 0;
				timerokcount = 0;
				timerstimer = new System.Timers.Timer(1000);
				timerstimer.Elapsed += TimersTimerFunc;
				// AutoReset = true, ajastin suorittaa taustakoodin tasavälein automaattisesti
				// AutoReset = false, ajastin suorittaa taustakodin vain kerran asetetun ajan kuluttua
				//   jos halutaan suorittaa loopissa, pitää taustakoodissa asettaa uudestaan Enabled = true
				timerstimer.AutoReset = true;
				timerstimer.Enabled = true;
			}
		}

		/// <summary>
		/// Timers.Timer event
		/// </summary>
		/// <param name="o"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		void TimersTimerFunc(Object o, EventArgs args)
		{
			timercount++;
			Globals.Tags.HMI_TimerCount.Value = timercount;

			// Ajastin säie ei omista ikkunan objekteja ==> suora osoitus ei toimi
			// function suoritus loppuu
			//ANVarValue.Text = timercount.ToString();
			// oikea tapa päivittää ikkunan objekteja
			this.Dispatcher.Invoke((Action)(() =>
				{
				ANVarValue.Text = timercount.ToString();
				}));

			/// jos pitkään vievää ajoa suoritetaan ei suoritetan uudestaan
			if (!running)
			{
				running = true;

				System.Threading.Thread.Sleep(3000);

				timerokcount++;
				this.Dispatcher.Invoke((Action)(() =>
					{
					ANVarOkValue.Text = timerokcount.ToString();
					}));

				running = false;
			}
		}

		/// <summary>
		/// Omassa säikeessä ajettava ajastin
		///  - jos koodin suoritus kestää enemmän aikaa kuin ajastimen liipaisuaika
		///    Koodi suoritetaan päällekkäin edellisen ajastimen liipaisun kanssa ja johtaa suorittimen muistin varausvirheeseen 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		void Btn_StartTimerThreading_Click(System.Object sender, System.EventArgs e)
		{
			if (running)
			{
				System.Windows.Forms.MessageBox.Show("Odota ajastimien taustaajon loppuminen");
			}
			else if (formstimer != null || timerstimer != null || threadingtimer != null)
			{
				System.Windows.Forms.MessageBox.Show("Stop timer first!");
			}
			else
			{
				timercount = 0;
				timerokcount = 0;
				threadingtimer = new System.Threading.Timer(ThreadingTimerFunc);
				// duetime = 2000, first callback 2s
				// period = 1000, callback every 1s
				threadingtimer.Change(2000, 1000);
			}
		}

		/// <summary>
		/// Threading.Thread.Timer event
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		void ThreadingTimerFunc(object state)
		{
			timercount++;
			Globals.Tags.HMI_TimerCount.Value = timercount;

			// Ajastin säie ei omista ikkunan objekteja ==> suora osoitus ei toimi
			// tämä aiheuttaa Exceptionin, jos sitä ei käsitellä, kaatuu koko ohjelma
			try
			{
				// VÄÄRIN, ei saa käyttää
				ANVarValue.Text = timercount.ToString();
			}
			catch (Exception) { }
			// oikea tapa päivittää ikkunan objekteja
			this.Dispatcher.Invoke((Action)(() =>
				{
				ANVarValue.Text = timercount.ToString();
				}));

			/// jos pitkään vievää ajoa suoritetaan, ei suoritetan uudestaan
			if (!running)
			{
				running = true;

				System.Threading.Thread.Sleep(3000);

				timerokcount++;
				this.Dispatcher.Invoke((Action)(() =>
					{
					ANVarOkValue.Text = timerokcount.ToString();
					}));

				running = false;
			}
		}

		/// <summary>
		/// Omassa säikeessä ajettava ajastin
		///  - ajastin ajastetaan liipaistumaan vain kerran säädetyn ajan kuluttua ==> taustakoodin ajo päällekkäin ei ole mahdollista
		///  - looppi asetetaan ajastamalla ajastin uudelleen taustakoodissa
		///  - jos halutaan tasavälein ajettava koodi, pitää laskea taustakoodin suoritusaika ja säätää seuraavan ajastuksen aika sen mukaan
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		void Btn_StartTimerThreadingII_Click(System.Object sender, System.EventArgs e)
		{
			if (formstimer != null || timerstimer != null || threadingtimer != null)
			{
				System.Windows.Forms.MessageBox.Show("Stop timer first!");
			}
			else
			{
				timercount = 0;
				timerokcount = 0;
				threadingtimer = new System.Threading.Timer(ThreadingTimerFuncII);
				// duetime = 2000, first callback 2s
				// period = 1000, callback every 1s
				threadingtimer.Change(2000, System.Threading.Timeout.Infinite);
			}
		}

		/// <summary>
		/// Threading.Thread.Timer event
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		void ThreadingTimerFuncII(object state)
		{
			timercount++;
			Globals.Tags.HMI_TimerCount.Value = timercount;

			// Ajastin säie ei omista ikkunan objekteja ==> suora osoitus ei toimi
			// tämä aiheuttaa Exceptionin, jos sitä ei käsitellä, kaatuu koko ohjelma
			//ANVarValue.Text = timercount.ToString();
			// oikea tapa päivittää ikkunan objekteja
			this.Dispatcher.Invoke((Action)(() =>
				{
				ANVarValue.Text = timercount.ToString();
				}));

			/// jos pitkään vievää ajoa suoritetaan, ei suoritetan uudestaan
			if (!running)
			{
				running = true;

				System.Threading.Thread.Sleep(3000);

				timerokcount++;
				this.Dispatcher.Invoke((Action)(() =>
					{
					ANVarOkValue.Text = timerokcount.ToString();
					}));

				running = false;
			}

			threadingtimer.Change(1000, System.Threading.Timeout.Infinite);
		}

		/// <summary>
		/// Stop running timer
		/// </summary>
		/// <returns></returns>
		void StopTimers()
		{
			if (formstimer != null)
			{
				formstimer.Stop();
				formstimer = null;
			}
			else if (timerstimer != null)
			{
				timerstimer.Stop();
				timerstimer.Dispose();
				timerstimer = null;
			}
			else if (threadingtimer != null)
			{
				threadingtimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
				threadingtimer.Dispose();
				threadingtimer = null;
			}
			while (running) 
			{ 
				/* do nothing */
				System.Threading.Thread.Sleep(1);
				System.Windows.Forms.Application.DoEvents();
			}
		}

		/// <summary>
		/// Button event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		void Btn_StopTimer_Click(System.Object sender, System.EventArgs e)
		{
			StopTimers();
		}

		#endregion

		#region threads
		
		void CreateExtThread()
		{
			wthread = new WorkThread();
			wthread.State = ThreadState;
			wthread.IncCount = IncExtCounter;
		}
		
		void Btn_StartExternalThread_Click(System.Object sender, System.EventArgs e)
		{
			if (wthread == null)
			{
				CreateExtThread();
			}
			
			wthread.Loop = CBExternalLoop.Checked;
			wthread.Start();
		}
		
		void Btn_StopExternalThread_Click(System.Object sender, System.EventArgs e)
		{
			if (wthread == null)
			{
				System.Windows.Forms.MessageBox.Show("Säie ei ole käynnissä");
			}
			else
			{
				wthread.Abort();
				wthread = null;
				externalcount = 0;
				ANExternalCount.Text = externalcount.ToString();
			}
		}
		
		void CBExternalLoop_Click(System.Object sender, System.EventArgs e)
		{
			if (wthread == null)
			{
				CreateExtThread();
			}
			
			wthread.Loop = CBExternalLoop.Checked;
		}

		private void ThreadState(WorkThreadState state)
		{
			switch (state)
			{
				case WorkThreadState.Stopped:
					Globals.Tags.HMI_ExternalThreadState.Value = 2;
					break;
				case WorkThreadState.Running:
					Globals.Tags.HMI_ExternalThreadState.Value = 1;
					break;
				default:
					Globals.Tags.HMI_ExternalThreadState.Value = 0;
					break;
			}
		}
		
		private void IncExtCounter()
		{
			this.Dispatcher.Invoke((Action)(() =>
				{
				externalcount++;
				ANExternalCount.Text = externalcount.ToString();
				}));
		}

		void Btn_StartLocalThread_Click(System.Object sender, System.EventArgs e)
		{
			try 
			{	        
				if (localthread == null)
				{
					localthread = new System.Threading.Thread(LocalDoWork);
				}
				if (localthread.ThreadState == System.Threading.ThreadState.Running)
				{
					System.Windows.Forms.MessageBox.Show("Säie on jo käynnissä");
				}
				else
				{
					if (localthread.ThreadState == System.Threading.ThreadState.Suspended || localthread.ThreadState == System.Threading.ThreadState.Stopped)
					{
						localthread.Abort();
						bool bck = loop;
						loop = false;
						localthread.Join();
						localthread = null;
						localthread = new System.Threading.Thread(LocalDoWork);
						loop = bck;
					}

					localthread.Start();
				}
			}
			catch (Exception x)
			{
				System.Windows.Forms.MessageBox.Show(x.Message);
			}
		}
		
		void CBLocalLoop_Click(System.Object sender, System.EventArgs e)
		{
			loop = CBLocalLoop.Checked;
		}
		
		void LocalDoWork()
		{
			do
			{
				int i = Globals.Tags.LocalThreadMeterValue.Value;
				i++;
				if (i > 100) i = 0;
				Globals.Tags.LocalThreadMeterValue.Value = i;
				if (loop) System.Threading.Thread.Sleep(500);
			}
			while (loop);
		}
		
		void Btn_StopLocalThread_Click(System.Object sender, System.EventArgs e)
		{
			if (localthread == null)
			{
				System.Windows.Forms.MessageBox.Show("Säie ei ole käynnissä");
			}
			else
			{
				localthread.Abort();
				localthread.Join();
				localthread = null;
			}
			loop = false;
		}
		
		void StopThreads()
		{
			if (wthread != null)
			{
				wthread.Abort();
				wthread = null;
			}
		}
		
		#endregion
		
		/// <summary>
		/// When closing, stop running timer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		void MainScreen_Closing(System.Object sender, System.ComponentModel.CancelEventArgs e)
		{
			StopTimers();
			StopThreads();
		}
	}
}
