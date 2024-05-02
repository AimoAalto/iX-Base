namespace Neo.ApplicationFramework.Generated
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;


	/// <summary>
	/// Näyttää lavapaikan tuotantotiedot ja robotin asetukset. Robotin asetuksia voi
	/// muokata ja lähettää robotille, joka ottaa muutokset käyttöön omien asetustensa 
	/// mukaisesti.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 22.3.2018</remarks>
	public partial class Popup_Palletplace_info
	{
		/// <summary>
		/// Lavapaikan robotin numero.
		/// </summary>
		int robottiNo = 0;
		/// <summary>
		/// Lavapaikan numero robotilla.
		/// </summary>
		int roboLavapaikka = 0;

		/// <summary>
		/// Täyttää robotin arvot näytön kenttiin, kun sivu avataan.
		/// </summary>
		/// <param name="sender">this</param>
		/// <exception cref="ConfigurationFaultException">Robotin numeroa ei löytynyt lavapaikalle.</exception>
		void Popup_Palletplace_info_Opened(System.Object sender, System.EventArgs e)
		{
			ReadInfo();
		}

		private void ReadInfo()
		{
			// Haetaan robotin numero ja robotin käyttämä lavapaikan numero
			Globals._Konfiguraatio.CurrentConfig.GetRobotinLavapaikka(Globals.Tags.HMI_PalletPlace.Value, out robottiNo, out roboLavapaikka);
			
			if (robottiNo == 0)
			{
				// Robotin numeron parsinta epäonnistui
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnknownRobotId);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = "Pallet place: " + Globals.Tags.HMI_PalletPlace.Value;
				Globals.Popup_Error.Show();
			}

			string tagname = string.Format("Rob{0}_lavap{1}_pkuv",robottiNo, roboLavapaikka);
			// Robotin kuvionumero
			KuvioNr.Value = Globals.Tags.GetTagValue(tagname);
			
			//Lavapaikan tuotenimi	
			if (Globals.Tags.GetTagValue(tagname) < 1)
				ANProduct.Visible = false;
			else
				ANProduct.Visible = true;
			ANProduct.Value = Globals.Ajotiedot.HaeLavapaikanTuote(Globals.Tags.HMI_PalletPlace.Value);

			//Luetaan robotilta alkuarvot tageihin
			tagname = string.Format("Rob{0}_lavap{1}_pkerasetus",robottiNo, roboLavapaikka);
			ANKerrokset.Value = Globals.Tags.GetTagValue(tagname);
		
			tagname = string.Format("Rob{0}_lavap{1}_pmaxkerros",robottiNo, roboLavapaikka);
			Text_MaxKerros.Text = " / " + Globals.Tags.GetTagValue(tagname).ToString();
		
			tagname = string.Format("Rob{0}_lavap{1}_pnopker",robottiNo, roboLavapaikka);
			tuote_nop.Value = Globals.Tags.GetTagValue(tagname);
		
			tagname = string.Format("Rob{0}_lavap{1}_pkiiker",robottiNo, roboLavapaikka);
			tuote_kii.Value = Globals.Tags.GetTagValue(tagname);
		
			tagname = string.Format("Rob{0}_lavap{1}_ptarviive",robottiNo, roboLavapaikka);
			LukuTartuntaviive.Value = Globals.Tags.GetTagValue(tagname);
		
			tagname = string.Format("Rob{0}_lavap{1}_pjatviive",robottiNo, roboLavapaikka);
			LukuJattoviive.Value = Globals.Tags.GetTagValue(tagname);
			
			tagname = string.Format("Rob{0}_lavap{1}_plasl",robottiNo, roboLavapaikka);
			ANProductOnPallet.Value = Globals.Tags.GetTagValue(tagname);
			
			tagname = string.Format("Rob{0}_lavap{1}_pmaxvie",robottiNo, roboLavapaikka);
			ANMaxOnPallet.Value = Globals.Tags.GetTagValue(tagname);
			
			ANStartedInfeed.Value = Globals.Tags.GetTagValueString(string.Format("S7HMI_PPStates_Place_{0}_InfeedLine", roboLavapaikka));
			bool b = (bool)Globals.Tags.GetTagValue(string.Format("S7HMI_PPStates_Place_{0}_ReadyForProdStart", roboLavapaikka));
			CBCanStart.Checked = b;
		}
		
		/// <summary>
		/// Avaa välikkeiden muokkaussivun.
		/// </summary>
		/// <param name="sender">this.Btn_Valikkeet</param>
		void Btn_Valikkeet_Click(System.Object sender, System.EventArgs e)
		{
			try
			{
				// Kirjoitetaan parametrit ennen välike sivun avausta
				// Lavapaikka Globals.Tags.HMI_PalletPlace.Value -1 => Tuoterekisteri
				Globals.Tags.HMI_ProdCtrl_Cardboards_Lavapaikka.Value = Globals.Tags.HMI_PalletPlace.Value;

				// Alkuperäiset pahvit ennen muokkausta
				string tagname = "Rob" + robottiNo + "_lavap" + roboLavapaikka + "_ppahvit";
				Globals.Tags.HMI_ProdCtrl_Cardboards_Pahvit.Value = Globals.Tags.GetTagValue(tagname);

				// Kuvion maksimikerrot ja kerroasetus
				Globals.Tags.HMI_ProdCtrl_Cardboards_MaxKerros.Value = Globals.Tags.GetTagValue("Rob" + robottiNo + "_lavap" + roboLavapaikka + "_pmaxkerros"); // Kuviosta luettu enimmäiskerrosmäärä
				Globals.Tags.HMI_ProdCtrl_Cardboards_Kerrosasetus.Value = Globals.Tags.GetTagValue("Rob" + robottiNo + "_lavap" + roboLavapaikka + "_pkerasetus");

				// Avataan välikkeideiden muokkaussivu
				Globals.Popup_ProdCtrl_Cardboards.Show();
			}
			catch (Exception x)
			{
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = x.Message + "; " + x.InnerException.Message;
				Globals.Popup_Error.Show();
			}
		}

		/// <summary>
		/// Kokoaa laatikoihin kirjoitetut tiedot ja lähettää ne robotille.
		/// Lähettää nopeuden, kiihtyvyyden, tartuntaviiveen ja jättöviiveen.
		/// </summary>
		/// <param name="sender">this.laheta</param>
		void Btn_SendChanges_Click(System.Object sender, System.EventArgs e)
		{
			try
			{
				int nopeus, kiihtyvyys;
				double tartuntaviive, jattoviive;

				// Tarkistetaan, että kaikki arvot ovat lukuja
				nopeus = Convert.ToInt16(tuote_nop.Value);
				kiihtyvyys = Convert.ToInt16(tuote_kii.Value);
				tartuntaviive = Convert.ToDouble(LukuTartuntaviive.Value);
				jattoviive = Convert.ToDouble(LukuJattoviive.Value);

				// Lähetetään Robotille
				Globals.Robotit.LisaaLokiin(robottiNo, "Uudet tuotenopeudet: " + tuote_nop.Value + ", " + tuote_kii.Value + ", " + LukuTartuntaviive.Value + ", " + LukuJattoviive.Value);
				Globals.Robotit.PaikkaNopeus(robottiNo, roboLavapaikka, nopeus, kiihtyvyys, tartuntaviive, jattoviive);
			}
			catch (Exception ex)
			{
				// Kaikki arvot ei ollut lukuja
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.SpeedSendFailed);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
				Globals.Popup_Error.Show();
			}

			try
			{
				// Lähetetään kerrosmäärä robotille
				Globals.Robotit.LisaaLokiin(robottiNo, "Uusi kerrosmäärä: " + ANKerrokset.Value);
				Globals.Robotit.AsetaKerrosmaara(robottiNo, roboLavapaikka, Convert.ToInt16(ANKerrokset.Value));
			}
			catch (Exception ex)
			{
				// Kerrosmäärä ei ollut luku
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.LayerSendFailed);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
				Globals.Popup_Error.Show();
			}
		}
		
		// send change pallet msg to robot
		void BtnChangePallet_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.TeeLavanvaihto(robottiNo, roboLavapaikka);
		}
		
		void Btn_Refresh_Click(System.Object sender, System.EventArgs e)
		{
			ReadInfo();
		}
	}
}
