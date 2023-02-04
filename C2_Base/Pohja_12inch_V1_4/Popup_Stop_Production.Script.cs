namespace Neo.ApplicationFramework.Generated
{
	using Neo.ApplicationFramework.Interfaces;
	using Neo.ApplicationFramework.Interfaces.Tag;
	using System;
	using System.Collections.Generic;
	using System.Threading;
	
    
	/// <summary>
	/// Antaa mahdollisuuden lopettaa tuloradan tuotannon.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 5.4.2018</remarks>
    public partial class Popup_Stop_Production
    {
		/// <summary>
		/// Hakee ajossa olevan tuotteen nimen näytölle ja sitoo lopetusvaiheen
		/// oikeaan tulorataan sivun avautuessa.
		/// </summary>
		/// <param name="sender">this</param>
		void Stop_Production_Opened(System.Object sender, System.EventArgs e)
		{

			int tulorata = Globals.Tags.HMI_Overview_track_selected.Value;
			
			// Haetaan ajossa oleva tuote
			AnalogNumeric2.Value = Globals.Ajotiedot.HaeTuloradanTuote(tulorata);
			
			// Sidotaan Lopetusvaihe-Alias oikeaan tagiin
			IBasicTag lopetusTagi = Globals.Tags.GetTag("Line1_PLC_Lopetusvaiheet" + tulorata);
			TulorataX_Lopetusvaihe = (VariantValue)lopetusTagi.Value;
			lopetusTagi.ValueChange += Line1_PLC_LopetusvaiheetX_ValueChange;
		}
		
		/// <summary>
		/// Lähettää lopetuspyynnön logiikalle ja robotille.
		/// Jos lopetus on jo käynnissä, pyytää logikkaa pakottamaan/ohittamaan 
		/// lopetuksen vaiheen.
		/// HUOM! Lopetuksen pakotuksen toimintaa ei ole tehty.
		/// </summary>
		/// <param name="sender">this.StopProd_btn</param>
		/// <exception cref="ConfigurationFaultException">Robotin numeroa ei voitu päätellä tuloradasta.</exception>
		void StopProd_btn_Click(System.Object sender, System.EventArgs e)
		{
			int tulorata = Globals.Tags.HMI_Overview_track_selected.Value;
			
//			if (!Globals.Tags.GetTagValue("Line1_PLC_Lopetus" + tulorata).Bool)
//			{
				// Hae tuloradan numero robotilla robotin lopetusta varten
				int robottiNo = 0;
				int robottiTulorata = -1;
				
				// Katsotaan mille robotille tulorata on
				foreach (KeyValuePair<int, Dictionary<int, int>> robotti in _Konfiguraatio.RobotinTuloradat)
				{
					if (robotti.Value.ContainsKey(tulorata))
					{
						robottiNo = robotti.Key;
						robottiTulorata = robotti.Value[tulorata];
						break;
					}
				}
				if(robottiNo == 0)
				{
					// Robotin numeron parsinta epäonnistui
					throw new ConfigurationFaultException("Robotin numeroa ei voitu löytää tuloradan avulla:", "Tulorata " + tulorata);
				}
								
				// Asetetaan logiikan lopetusbitti
				Globals.Tags.SetTagValue("Line1_PLC_Lopetus" + tulorata, true);
				
				// Lähetetään lopetus robotille
				Globals.Robotit.robotit[robottiNo].Loki.LisaaLokiin("Lopetetaan tulorata " + robottiTulorata + ".");
				Globals.Robotit.robotit[robottiNo].TeeLopetus(robottiTulorata);	
				
				//this.Close();
//			}
//			else // Lopetus on jo käynnissä
//			{
//				// Kysytään, haluaako pakottaa lopetuksen
//				// Resetoidaan vahvistuksen kyselyn odotusvahti
//				((Popup_Confirmation)Globals.Popup_Confirmation.AdaptedObject).konfirmaatioVahvistus.Reset();
//
//				// Kysytään vahvistusta
//				Globals.Tags.HMI_Confirmation_Text.SetAnalog(4);
//				Globals.Tags.HMI_Confirmation_Value.Value = "";
//				Globals.Popup_Confirmation.Show();
//				
//				// Tulostetaan tai ei tulosteta, kun vastaus saadaan
//				Thread toiminta = new Thread(() => 
//					{
//						// Odotetaan vastausta
//						((Popup_Confirmation)Globals.Popup_Confirmation.AdaptedObject).konfirmaatioVahvistus.WaitOne();
//							
//						// Luetaan vastaus
//						if (Globals.Tags.HMI_Confirmation_OK.Value.Bool)
//						{
//							// Lähetetään lopetuksen pakotuspyyntö logiikalle
//							// TODO
//						}
//					}
//					);
//				
//				// Käynnistetään lanka odottamaan vastausta
//				toiminta.Start();
//				
//				// Suljetaan ikkuna
//				this.Close();
//			}
		}
    
		/// <summary>
		/// Päivittää tuloradan lopetusvaiheen.
		/// </summary>
		/// <param name="sender">Line1_PLC_LopetusvaiheetX</param>
		void Line1_PLC_LopetusvaiheetX_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			IBasicTag lahettaja_arvo = (IBasicTag)sender;

			// Päivitetään vaiheen numero Aliakseen vaiheen tekstiä varten
			TulorataX_Lopetusvaihe = ((VariantValue)lahettaja_arvo.Value).Short;
			System.Diagnostics.Trace.WriteLine("[iX] Event: Line1_PLC_LopetusvaiheetX_ValueChange " + TulorataX_Lopetusvaihe);

		}
		
		/// <summary>
		/// Irtautuu lopetusvaiheen päivityksestä
		/// </summary>
		/// <param name="sender">this</param>
		void Stop_Production_Closed(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.GetTag("Line1_PLC_Lopetusvaiheet" + Globals.Tags.HMI_Overview_track_selected.Value).ValueChange -= Line1_PLC_LopetusvaiheetX_ValueChange;
		}
		
		void CloseMe_ValueChanged(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			int tulorata = Globals.Tags.HMI_Overview_track_selected.Value;
			if (tulorata == Globals.Tags.Stop_Production_CloseMe.Value)
				Close();
		}

	}
}
