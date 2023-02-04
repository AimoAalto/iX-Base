//--------------------------------------------------------------
// Sisältää laajennuksia reseptitietokantoihin.
//--------------------------------------------------------------

namespace Neo.ApplicationFramework.Generated
{
    using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SQLite;
	using System.IO;
	using System.Reflection;
    
	
	/// <summary>
	/// Sisältää hakufunktioita Tuotetietokantaan
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 21.8.2017</remarks>
	public partial class Tuotetiedot
	{
		/// <summary>
		/// Hakee kaikki reseptit tietokannasta.
		/// </summary>
		/// <returns>Palauttaa kaikki reseptit DataSet:nä. Tulokset ovat DataSet:n 
		/// [0}-taulussa riveinä.</returns>
		public DataSet HaeKaikki()
		{
			DataSet data = new DataSet();
			
			try
			{
				using (SQLiteConnection yhteys = LuoYhteys())
				{
					// Avaa yhteys
					yhteys.Open();
				
					// Luo kaiken datan valintakomento
					SQLiteCommand komento = new SQLiteCommand("SELECT * FROM Tuotetiedot", yhteys);
			
					// Luodaan data-adapteri
					SQLiteDataAdapter adapteri = new SQLiteDataAdapter(komento);
				
					// Haetaan data
					adapteri.Fill(data);
				
					// Katkaistaan yhteys
					yhteys.Close();
				}
			}
			catch (Exception ex)
			{
				// Lataus epäonnistui
				Globals.Tags.HMI_Error_TextValue.SetAnalog(20);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
				Globals.Popup_Error.Show();
			}
			
			return data;
		}

		/// <summary>
		/// Hakee seuraavan tallennettavan reseptin rivinumeron.
		/// </summary>
		/// <returns>Palauttaa tietokannan suurin rivinumero + 1</returns>
		public int HaeMaxID()
		{
			DataSet data = new DataSet();
			
			try
			{
				using (SQLiteConnection yhteys = LuoYhteys())
				{
					// Avaa yhteys
					yhteys.Open();
				
					// Luo kaikken datan valintakomento
					SQLiteCommand komento = new SQLiteCommand("SELECT MAX(RiviNro) AS RiviNro FROM Tuotetiedot", yhteys);
			
					// Luodaan data-adapteri
					SQLiteDataAdapter adapteri = new SQLiteDataAdapter(komento);
				
					// Haetaan data
					adapteri.Fill(data);
				
					// Katkaistaan yhteys
					yhteys.Close();
				}
			}
			catch (Exception ex)
			{
				// Lataus epäonnistui
				Globals.Tags.HMI_Error_TextValue.SetAnalog(20);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
				Globals.Popup_Error.Show();
			}
			
			int ID = 1;
			if (data.Tables.Count > 0)
			{
				if (data.Tables[0].Rows.Count > 0)
				{
					// Luetaan nykyinen ID
					ID = Convert.ToInt32(data.Tables[0].Rows[0]["RiviNro"]) + 1;
				}else
				{
					// Tietokanta on tyhjä, palautetaan ID:nä 1
					ID = 1;
				}
			}
			
			return ID;
		}

		/// <summary>
		/// Hakee kaikki kuviot, joissa on jokin sallituista kuvionumeroista.
		/// </summary>
		/// <param name="kuviot">Sallitut kuvionumerot</param>
		/// <returns>Palauttaa kaikki sopivat reseptit DataSet:nä. Tulokset ovat DataSet:n
		/// [0}-taulussa riveinä.</returns>
		public DataSet HaeReseptit(int[] kuviot)
		{
			DataSet data = new DataSet();
			
			try
			{
				using (SQLiteConnection yhteys = LuoYhteys())
				{
					// Avaa yhteys
					yhteys.Open();
				
					// Luo datan valintakomento
					SQLiteCommand komento = new SQLiteCommand("SELECT * FROM Tuotetiedot WHERE Kuvio IN (" + string.Join(",", kuviot) + ");", yhteys);
							
					// Luodaan data-adapteri
					SQLiteDataAdapter adapteri = new SQLiteDataAdapter(komento);
				
					// Haetaan data
					adapteri.Fill(data);
				
					// Katkaistaan yhteys
					yhteys.Close();
				}
			}
			catch (Exception ex)
			{
				// Lataus epäonnistui
				Globals.Tags.HMI_Error_TextValue.SetAnalog(20);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
				Globals.Popup_Error.Show();
			}
			
			return data;
		}

		/// <summary>
		/// Hakee kaikki hakuehtoon sopivat reseptit, joissa on jokin sallituista
		/// kuvionumeroista. Hakuehto voi olla täysin täsmäävä tuotenumero tai osa 
		/// reseptin nimeä.
		/// </summary>
		/// <param name="kuviot">Sallitut kuvionumerot</param>
		/// <param name="hakuehto">Täysin täsmäävä tuotenumero tai osa reseptin nimeä</param>
		/// <returns>Palauttaa SQL-kyselyn tulokset DataSet:nä Tulokset ovat DataSet:n
		/// [0}-taulussa riveinä.</returns>
		public DataSet HaeReseptit(int[] kuviot, string hakuehto)
		{
			DataSet data = new DataSet();
			
			try
			{
				using (SQLiteConnection yhteys = LuoYhteys())
				{
					// Avaa yhteys
					yhteys.Open();
							
					// Luo datan valintakomento
					string komentoText = "SELECT * FROM Tuotetiedot WHERE Kuvio IN (" + string.Join(",", kuviot) + ") AND (";
							
					// Lisätään tekstikenttä hakuehdoksi
					// Jos kentässä on numero tarkistetaan tuotenumero
					int numero;
					if (int.TryParse(hakuehto, out numero))
					{
						komentoText = komentoText + "Tuotenumero LIKE " + hakuehto + " OR ";
					}
				
					// Tarkistetaan reseptinimet
					komentoText = komentoText + "FieldName LIKE @0);";
				
					SQLiteCommand komento = new SQLiteCommand(komentoText, yhteys);
				
					// Lisätään käsin kirjoitettu kenttä (Turvallisesti!)
					komento.Parameters.AddWithValue("@0", "%" + hakuehto + "%");
				
					// Luodaan data-adapteri
					SQLiteDataAdapter adapteri = new SQLiteDataAdapter(komento);
				
					// Haetaan data
					adapteri.Fill(data);
				
					// Katkaistaan yhteys
					yhteys.Close();
				}
			}
			catch (Exception ex)
			{
				// Lataus epäonnistui
				Globals.Tags.HMI_Error_TextValue.SetAnalog(20);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
				Globals.Popup_Error.Show();
			}
			
			return data;
		}
		
		/// <summary>
		/// Luo avaamattoman yhteyden tietokantaan.
		/// </summary>
		/// <returns>Palauttaa avaamattoman yhteyden tietokantaan</returns>
		SQLiteConnection LuoYhteys()
		{
			// Haetaan ohjelman polku
			string exe = Assembly.GetExecutingAssembly().Location;
			string polku = Path.GetDirectoryName(exe);

			// Luodaan yhteys polun avulla
			return new SQLiteConnection(string.Format(
				"data source={0}", Path.Combine(polku, "Database.db")));
		}

		/// <summary>
		/// Hakee reseptin nimen tietokannasta rivinumeron perusteella.
		/// </summary>
		/// <param name="riviNro">Reseptin rivinumero</param>
		/// <returns>Palauttaa reseptin nimen</returns>
		public string ReseptinNimi(int riviNro)
		{
			string reseptiNimi = "Unknown";
			DataSet data = new DataSet();

			try
			{
				using (SQLiteConnection yhteys = LuoYhteys())
				{
					// Avaa yhteys
					yhteys.Open();
				
					// Luo reseptin nimellä haun komento
					SQLiteCommand komento = new SQLiteCommand("SELECT FieldName, RiviNro FROM Tuotetiedot WHERE RiviNro = @0", yhteys);
				
					// Asetetaan parametrit (Turvallisesti!)
					komento.Parameters.AddWithValue("@0", riviNro);
				
					// Luodaan data-adapteri
					SQLiteDataAdapter adapteri = new SQLiteDataAdapter(komento);
				
					// Haetaan data
					adapteri.Fill(data);

					// Katkaistaan yhteys
					yhteys.Close();
				}
			}
			catch (Exception ex)
			{
				// Lataus epäonnistui
				Globals.Tags.HMI_Error_TextValue.SetAnalog(20);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
				Globals.Popup_Error.Show();
			}
			
			if (data.Tables.Count > 0)
			{
				if(data.Tables[0].Rows.Count == 1)
				{
					// Siirretään löydety tuloksen nimi
					reseptiNimi = data.Tables[0].Rows[0]["FieldName"].ToString();
				}
				else if (data.Tables[0].Rows.Count > 1)
				{
					// Ylimääräsiä tuloksia, hälytetään
					Globals.Tags.Alarm_ProdReg_RiviNro.Value = riviNro;
					Globals.Tags.Alarm_ProdReg_Alarm1.SetTag();					
				}
				else
				{
					// Ei tuloksia, hälytetään
					Globals.Tags.Alarm_ProdReg_RiviNro.Value = riviNro;
					Globals.Tags.Alarm_ProdReg_Alarm0.SetTag();					
				}
			}
			
			return reseptiNimi;
		}
		
		/// <summary>
		/// Hakee reseptin tuotenumeron tietokannasta rivinumeron perusteella.
		/// </summary>
		/// <param name="riviNro">Reseptin rivinumero</param>
		/// <returns>Palauttaa reseptin tuotenumeron</returns>
		public int ReseptinTuotenumero(int riviNro)
		{
			int tuotenumero = 0;
			DataSet data = new DataSet();

			try
			{
				using (SQLiteConnection yhteys = LuoYhteys())
				{
					// Avaa yhteys
					yhteys.Open();
				
					// Luo reseptin nimellä haun komento
					SQLiteCommand komento = new SQLiteCommand("SELECT Tuotenumero, RiviNro FROM Tuotetiedot WHERE RiviNro = @0", yhteys);
				
					// Asetetaan parametrit (Turvallisesti!)
					komento.Parameters.AddWithValue("@0", riviNro);
				
					// Luodaan data-adapteri
					SQLiteDataAdapter adapteri = new SQLiteDataAdapter(komento);
				
					// Haetaan data
					adapteri.Fill(data);

					// Katkaistaan yhteys
					yhteys.Close();
				}
			}
			catch (Exception ex)
			{
				// Lataus epäonnistui
				Globals.Tags.HMI_Error_TextValue.SetAnalog(20);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
				Globals.Popup_Error.Show();
			}
			
			if (data.Tables.Count > 0)
			{
				if(data.Tables[0].Rows.Count == 1)
				{
					// Siirretään löydety tuloksen tuotenumero
					tuotenumero = Convert.ToInt32(data.Tables[0].Rows[0]["Tuotenumero"]);
				}
				else if (data.Tables[0].Rows.Count > 1)
				{
					// Ylimääräsiä tuloksia, hälytetään
					Globals.Tags.Alarm_ProdReg_RiviNro.Value = riviNro;
					Globals.Tags.Alarm_ProdReg_Alarm1.SetTag();					
				}
				else
				{
					// Ei tuloksia, hälytetään
					Globals.Tags.Alarm_ProdReg_RiviNro.Value = riviNro;
					Globals.Tags.Alarm_ProdReg_Alarm0.SetTag();					
				}
			}
			
			return tuotenumero;
		}
		
		/// <summary>
		/// Tarkistaa, onko reseptin nimeä tietokannassa.
		/// </summary>
		/// <param name="resepti">Reseptin nimi</param>
		/// <returns>Palauttaa true, jos saman niminen resepti löytyy jo tietokannasta</returns>
		/// <remarks>Muokattu: 21.8.2017 SoPi - Otetaan myös rivinumero talteen päälle tallennusta varten.</remarks>
		public KeyValuePair<bool, int> ReseptiOlemassa(string resepti)
		{
			DataSet data = new DataSet();
			
			try
			{
				using (SQLiteConnection yhteys = LuoYhteys())
				{
					// Avaa yhteys
					yhteys.Open();
				
					// Luo reseptin nimellä haun komento
					SQLiteCommand komento = new SQLiteCommand("SELECT FieldName, RiviNro FROM Tuotetiedot WHERE FieldName = @0", yhteys);
				
					// Asetetaan parametrit (Turvallisesti!)
					komento.Parameters.AddWithValue("@0", resepti);
			
					// Luodaan data-adapteri
					SQLiteDataAdapter adapteri = new SQLiteDataAdapter(komento);
				
					// Haetaan data
					adapteri.Fill(data);
				
					// Katkaistaan yhteys
					yhteys.Close();
				}
			}
			catch (Exception ex)
			{
				// Lataus epäonnistui
				Globals.Tags.HMI_Error_TextValue.SetAnalog(20);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
				Globals.Popup_Error.Show();
			}

			bool olemassa = false;
			int riviNro = 0;
			if (data.Tables.Count > 0)
			{
				if(data.Tables[0].Rows.Count > 0)
				{
					olemassa = true;
					riviNro = Convert.ToInt32(data.Tables[0].Rows[0]["RiviNro"]);
				}
			}
			
			return new KeyValuePair<bool, int> (olemassa, riviNro);
		}
	}

	// Sisältää hakufunktioita Väliketietokantaan
	// Viimeksi muokattu: SoPi 7.7.2017
	public partial class Valikkeet_DB
	{
		/// <summary>
		/// Hakee kaikki välikkeet tietokannasta.
		/// </summary>
		/// <returns>Palauttaa kaikki välikkeet tietokannasta</returns>
		public DataSet HaeKaikki()
		{
			DataSet data = new DataSet();
			
			try
			{
				using (SQLiteConnection yhteys = LuoYhteys())
				{
					// Avaa yhteys
					yhteys.Open();
				
					// Luo kaikken datan valintakomento
					SQLiteCommand komento = new SQLiteCommand("SELECT * FROM Valikkeet_DB", yhteys);
			
					// Luodaan data-adapteri
					SQLiteDataAdapter adapteri = new SQLiteDataAdapter(komento);
				
					// Haetaan data
					adapteri.Fill(data);
				
					// Katkaistaan yhteys
					yhteys.Close();
				}
			}
			catch (Exception ex)
			{
				// Lataus epäonnistui
				Globals.Tags.HMI_Error_TextValue.SetAnalog(20);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
				Globals.Popup_Error.Show();
			}
			
			return data;
		}
		
		/// <summary>
		/// Luo avaamattoman yhteyden tietokantaan.
		/// </summary>
		/// <returns>Palauttaa avaamattoman yhteyden tietokantaan</returns>
		SQLiteConnection LuoYhteys()
		{
			// Haetaan ohjelman polku
			string exe = Assembly.GetExecutingAssembly().Location;
			string polku = Path.GetDirectoryName(exe);

			// Luodaan yhteys polun avulla
			return new SQLiteConnection(string.Format(
				"data source={0}", Path.Combine(polku, "Database.db")));
		}
	}
}
