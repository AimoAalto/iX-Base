namespace Neo.ApplicationFramework.Generated
{
	using Neo.ApplicationFramework.Common.Designer;
	using Neo.ApplicationFramework.Interfaces;
	using Neo.ApplicationFramework.Interfaces.Tag;
	using Neo.ApplicationFramework.Tools.OpcClient;
	using System;
	using System.Collections.Generic;
	using System.Linq;


	/// <summary>
	/// Sisältää lavalokin tallentamisen ja siihen vaadittavat seurannat.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 26.7.2017</remarks>
	public partial class Lavaloki
	{
		/// <summary>
		/// Liittyy kaikkiin tarvittaviin lavan valmistumisbittien ja robotin lavapaikan
		/// vientimäärien tagien ValueChange-eventteihin.
		/// </summary>
		/// <param name="sender">this</param>
		void Lavaloki_Created(System.Object sender, System.EventArgs e)
		{
			if (Globals.Tags.TraceAll) System.Diagnostics.Trace.WriteLine("Lavaloki Created (start)");

			try
			{
				foreach (Neo.ApplicationFramework.Generated.RobotConf robot in Globals._Konfiguraatio.CurrentConfig.Robots.Values)
				{
					foreach (int lavapaikka in robot.Lavapaikat)
					{
						string name = "";
						try
						{
							if (Globals.Tags.TraceAll) System.Diagnostics.Trace.WriteLine(string.Format("Lavaloki lavapaikka {0}, {1}", lavapaikka, robot.Lavapaikat[lavapaikka]));

							name = "Line1_PLC_LavaValmis" + lavapaikka;
							Globals.Tags.GetTag(name).ValueChange += Lava_Valmis;

							name = "Rob" + robot.RobotNo + "_lavap" + robot.Lavapaikat[lavapaikka] + "_plas";
							Globals.Tags.GetTag(name).ValueChange += Lava_Aloitettu;
						}
						catch (Exception x)
						{
							Globals.Tags.Log(String.Format("Lavaloki_Create: Unknown Tag {0} [{1}]", name, x.Message));
						}
					}
				}
			}
			catch (Exception x)
			{
				Globals.Tags.Log(String.Format("Lavaloki_Create.Exception: {0}", x.Message));
			}
		}

		/// <summary>
		/// Seuraa robotin nykyisen lavan vientimäärää ja merkitsee lavan aloitetuksi,
		/// kun lavalle asetetaan ensimmäiset kappaleet.
		/// </summary>
		/// <param name="sender">RobX_lavapX_plas</param>
		/// <exception cref="ArgumentException">Robotin tai robotin käyttämää lavapaikan numeroa ei voitu parsia sender:stä</exception>
		/// <exception cref="ConfigurationFaultException">Lavapaikkaa ei löytynyt robotin lavapaikoista _Konfiguraatiossa</exception>
		void Lava_Aloitettu(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			DesignerItemBase lahettaja_nimi = (DesignerItemBase)sender;
			IBasicTag lahettaja_arvo = (IBasicTag)sender;

			// Lava on aloitettu, kun ensimmäinen vienti on tehty
			if ((VariantValue)lahettaja_arvo.Value == 1)
			{
				int robotti = 0;
				int robottiLavapaikka = 0;
				if (!int.TryParse(lahettaja_nimi.Name.Substring(lahettaja_nimi.Name.IndexOf("_") - 1, 1), out robotti))
				{
					// Lavapaikan numeron parsinta epäonnistui
					Globals.Tags.Log("Robotin numeroa ei voitu parsia lähettäjästä: " + lahettaja_nimi.Name);
					return;
				}
				if (!int.TryParse(lahettaja_nimi.Name.Substring(lahettaja_nimi.Name.IndexOf("_", 6) - 1, 1), out robottiLavapaikka))
				{
					// Robotin numeron parsinta epäonnistui
					Globals.Tags.Log("Robotin lavapaikan numeroa ei voitu parsia lähettäjästä: " + lahettaja_nimi.Name);
					return;
				}

				int lavapaikka = -1;
				try
				{
					// Haetaan lavapaikan oma numero
					lavapaikka = Globals._Konfiguraatio.CurrentConfig.Lavapaikat.First(p => p.Value == robottiLavapaikka).Key;

					// Merkitään aloitusaika
					Globals.Tags.SetTagValue("Line1_Pallet_started" + lavapaikka, Globals.Tags.SystemTagDateTime.Value);
				}
				catch (NullReferenceException ex)
				{
					// Virheellinen konfiguraatio
					Globals.Tags.Log(string.Format("Lavapaikan tietoja ei löytynyt robotilta {0} ({1}). {2}", lahettaja_nimi.Name, robottiLavapaikka, ex.Message));
				}
			}
		}

		/// <summary>
		/// Pyytää lavalokin päivityksen, kun lava valmistuu.
		/// </summary>
		/// <param name="sender">Line1_PLC_LavaValmisX</param>
		/// <exception cref="ArgumentException">Lavapaikan numeroa ei voitu parsia sender:n nimen lopusta.</exception>
		void Lava_Valmis(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			DesignerItemBase lahettaja_nimi = (DesignerItemBase)sender;
			IBasicTag lahettaja_arvo = (IBasicTag)sender;

			// Reagoidaan vain nousevaan reunaan
			if (((VariantValue)lahettaja_arvo.Value).Bool)
			{
				int lavapaikka = -1;

				if (!int.TryParse(lahettaja_nimi.Name.Substring(lahettaja_nimi.Name.Length - 1, 1), out lavapaikka))
				{
					// lavapaikkanumeron parsinta epäonnistui
					Globals.Tags.Log("Lavapaikan numeroa ei voitu parsia lähettäjästä: " + lahettaja_nimi.Name);
					return;
				}

				// Lisätään merkintä lavalokiin
				if (Globals._Konfiguraatio.CurrentConfig.Lavapaikat.Keys.Contains(lavapaikka))
				{
					Paivita_Lavaloki(lavapaikka);
					return;
				}
			}
		}

		/// <summary>
		/// Päivittää lavalokiin merkinnän valmistuneesta lavasta. Tallentaa 
		/// Lavan aloituksen ja valmistumisen aikaleiman, lavan lavapaikan, lavan tuotteen
		/// ja lavan laatikkomäärän.
		/// </summary>
		/// <param name="lavapaikka">Lavapaikan numero</param>
		void Paivita_Lavaloki(int roboLavapaikka)
		{
			// Miltä lavapaikalta lava tulee
			int tulorata = -1;
			int robotti = 0;
			int lavapaikka = -1;

			foreach (Neo.ApplicationFramework.Generated.RobotConf robot in Globals._Konfiguraatio.CurrentConfig.Robots.Values)
			{
				// Haetaan tulorata, jolle lavapaikka on aloitettu
				foreach (int tulorataEhdokas in robot.Tuloradat)
				{
					//Tarkistetaan vain aloitetut tuloradat
					if (Globals.Tags.GetTagValue("Line1_PLC_Aloitettu" + tulorataEhdokas).Bool)
					{
						if (Globals.Tags.GetTagValue("Line1_PLC_Lavapaikka_TK" + tulorataEhdokas) == roboLavapaikka)
						{
							// Tulorata löytyi, kerätään tiedot
							tulorata = tulorataEhdokas;
							robotti = robot.RobotNo;
							foreach (KeyValuePair<int, int> paikkapari in Globals._Konfiguraatio.CurrentConfig.Lavapaikat)
							{
								if (paikkapari.Value == roboLavapaikka)
								{
									lavapaikka = paikkapari.Key;
								}
							}
						}
					}
				}
			}

			if (tulorata == -1 || robotti == 0 || lavapaikka == -1)
			{
				// Lavapaikkatieto olikin ehtinyt nollautua. Ei lavalokimerkintää
				return;
			}

			// Lavan aloitushetki
			Globals.Tags.HMI_PalletLog_Started.Value = Globals.Tags.GetTagValue("Line1_Pallet_started" + lavapaikka);

			// Lavan valmistumishetki
			Globals.Tags.HMI_PalletLog_Ended.Value = Globals.Tags.SystemTagDateTime.Value;

			// Lavapaikka, jolta lava valmistui
			Globals.Tags.HMI_PalletLog_PalletPlace.Value = lavapaikka;

			// Tuote, jota lavalla oli
			Globals.Tags.HMI_PalletLog_ProductNo.Value = Globals.Tuotetiedot.ReseptinTuotenumero(Globals.Tags.GetTagValue("Line1_Rivinumero_TK" + tulorata).Int);

			// Lavan Laatikkomäärä
			Globals.Tags.HMI_PalletLog_pcs.Value = Globals.Tags.GetTagValue("Rob" + robotti + "_lavap" + roboLavapaikka + "_plaslviim").Int;

			// Kasvatetaan lokin monitoritagin arvoa -> lokiin lisätään arvo
			Globals.Tags.HMI_LogData_Pallet_Log.Value++;

			// Merkintä robotin lokiin
			Globals.Robotit.LisaaLokiin(robotti, "Lava valmis paikalla " + roboLavapaikka);
		}
	}
}
