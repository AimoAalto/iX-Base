namespace Neo.ApplicationFramework.Generated
{
    using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
    
    
	/// <summary>
	/// Näyttää anturin tarkemmat tiedot ja ajantasaisen tilan.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 7.7.2017</remarks>
    public partial class Popup_SensorInfo
    {			
		/// <summary>
		/// Anturin tilan päivityksen taustatoiminto
		/// </summary>
		System.Threading.Timer TaustaTarkistus;
		
		/// <summary>
		/// Aloittaa anturin tilan tarkistuksen määritetyin aikavälein.
		/// </summary>
		/// <param name="sender">this</param>
		void Popup_SensorInfo_Opened(System.Object sender, System.EventArgs e)
		{

			// Päivitetään tilaa taustalla sekunnin välein kunnes ikkuna suljetaan
			TaustaTarkistus = new System.Threading.Timer((args) =>
				{
					// Mitataan kauanko operaatioissa kestää
					Stopwatch takeTime = new Stopwatch();
					takeTime.Start();
	
					// Päivitetään tila
					// UI omistaa Aliaksen, niin täytyy pyytää sitä päivittämään
					this.Dispatcher.Invoke((Action)(() =>
						{
							Tila = Globals.Tags.GetTagValue("PLC_Sensor_" + Globals.Tags.HMI_SensorInfo_Tunnus.Value.String);
						}));
					
					// Suoritetaan uudestaan intervallin kuluttua
					takeTime.Stop();
					TaustaTarkistus.Change(Math.Max(0, _Konfiguraatio.Aikavalit["SensorInfoUpdate"] - takeTime.ElapsedMilliseconds), Timeout.Infinite);
				}, null, 0, Timeout.Infinite);

            string kuvaus = Globals.Tags.HMI_SensorInfo_Ryhma.Value;
            kuvaus = null;
		}
		
		/// <summary>
		/// Lakataan päivittämästä anturin tilaa.
		/// </summary>
		/// <param name="sender">this</param>
		void Popup_SensorInfo_Closing(System.Object sender, System.ComponentModel.CancelEventArgs e)
		{
			TaustaTarkistus.Dispose();	
		}

    }
}
