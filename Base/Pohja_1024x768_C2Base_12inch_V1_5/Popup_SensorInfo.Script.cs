namespace Neo.ApplicationFramework.Generated
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Text.RegularExpressions;
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
		Timer TaustaTarkistus;

		/// <summary>
		/// Aloittaa anturin tilan tarkistuksen määritetyin aikavälein.
		/// </summary>
		/// <param name="sender">this</param>
		void Popup_SensorInfo_Opened(System.Object sender, System.EventArgs e)
		{
			// Asetetaan anturin numeerinen arvo millä haetaan tekstit listasta
			try
			{
				string anturikoodi = Regex.Replace(Globals.Tags.HMI_SensorInfo_Tunnus.Value, "[^0-9]", "");
				Tunnus = int.Parse(anturikoodi);
			}
			catch (Exception ex)
			{
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = string.Format("Unable to parse sensorcode, check parameters! [{0}]", ex.Message);
				Globals.Popup_Error.Show();
				return;
			}

			int interval = 1000;
			try
			{
				interval = Globals._Konfiguraatio.CurrentConfig.Aikavali("SensorInfoUpdate");
			}
			catch (Exception x)
			{
				Globals.Tags.Log(String.Format("SensorInfo: Interval error, use default\n{1}", x.Message));
			}

			// Päivitetään tilaa taustalla sekunnin välein kunnes ikkuna suljetaan
			TaustaTarkistus = new Timer((args) =>
				{
					// Mitataan kauanko operaatioissa kestää
					Stopwatch takeTime = new Stopwatch();
					takeTime.Start();

					// Päivitetään tila
					// UI omistaa Aliaksen, niin täytyy pyytää sitä päivittämään
					try
					{
						this.Dispatcher.Invoke((Action)(() =>
							{
								Tila = Globals.Tags.GetTagValue("PLC_Sensor_" + Globals.Tags.HMI_SensorInfo_Tunnus.Value.String);
							}));
					}
					catch (Exception x)
					{
						Globals.Tags.Log(string.Format("Popup_SensorInfo.GetTagValue: {0}", x.Message));
					}

					// Suoritetaan uudestaan intervallin kuluttua
					takeTime.Stop();
					TaustaTarkistus.Change(Math.Max(0, interval - takeTime.ElapsedMilliseconds), Timeout.Infinite);
				}, null, 0, Timeout.Infinite);

			//string kuvaus = Globals.Tags.HMI_SensorInfo_Ryhma.Value;
			//kuvaus = null;
		}

		/// <summary>
		/// Lakataan päivittämästä anturin tilaa.
		/// </summary>
		/// <param name="sender">this</param>
		void Popup_SensorInfo_Closing(System.Object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				TaustaTarkistus.Dispose();
			}
			catch
			{
			}
		}
	}
}
