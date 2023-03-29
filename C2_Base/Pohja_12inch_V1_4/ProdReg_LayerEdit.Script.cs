namespace Neo.ApplicationFramework.Generated
{
	using Neo.ApplicationFramework.Tools.OpcClient;
	using Neo.ApplicationFramework.Tools.Recipe;
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Windows.Controls;


	/// <summary>
	/// Näyttää ja antaa muokata kerroksen välikkeitä. 
	/// Välikkeet valitaan sallituista välikkeistä väliketietokannasta.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 7.7.2017</remarks>
	public partial class ProdReg_LayerEdit
	{
		/// <summary>
		/// Väliketietokannan sisältö.
		/// </summary>
		DataSet data;

		/// <summary>
		/// Hakee välikkeet tietokannasta ja alustaa näytön.
		/// </summary>
		/// <param name="sender">this</param>
		void ProdReg_KerroksenMuokkaus_Opened(System.Object sender, System.EventArgs e)
		{

			// Parametreina saadaan
			// - Muokattavan kerroksen numero Globals.Tags.HMI_ProdReg_ValikeKerrosNro.Value
			// - Muokattavan kerroksen alkuperäinen sisältö Globals.Tags.HMI_ProdReg_ValikeKerrosValikkeet.Value

			// Tyhjennetään molemmat listat
			List_Kerros.Items.Clear();
			List_Valikkeet.Items.Clear();

			// Luetaan kaikki sallitut välikkeet valittavien listaan
			data = Globals.Valikkeet_DB.HaeKaikki();

			if (data.Tables[0] != null)
			{
				foreach (DataRow rivi in data.Tables[0].Rows)
				{
					// Lisätään välike listaan
					try
					{
						if (Convert.ToBoolean(rivi["UsedInProject"].ToString()) == true && Convert.ToInt16(rivi["Number"].ToString()) > 0)
						{
							List_Valikkeet.Items.Add(rivi[0].ToString());
						}
					}
					catch (Exception)
					{

					}
				}

				// Luetaan alkuperäinen sisältö kerrokseen
				string sisalto = Globals.Tags.HMI_ProdReg_ValikeKerrosValikkeet.Value;
				foreach (string valike in sisalto.Split(','))
				{
					// Haetaan välikkeelle nimi tietokannasta
					foreach (DataRow rivi in data.Tables[0].Rows)
					{
						try
						{
							if (rivi["Number"].ToString() == valike && Convert.ToInt16(rivi["Number"].ToString()) > 0)
							{
								// Välike löytyi tietokannasta, lisätään kerrokseen
								List_Kerros.Items.Add(rivi[0]);
								break;
							}
						}
						catch (Exception)
						{

						}
					}
				}
			}
		}

		/// <summary>
		/// Lisää valitun välikkeen kerrokseen.
		/// </summary>
		/// <param name="sender">this.Btn_Lisaa</param>
		void Btn_Lisaa_Click(System.Object sender, System.EventArgs e)
		{
			// Tarkistetaan, että jotain on valittuna
			if (List_Valikkeet.SelectedItem == null)
			{
				return;
			}

			// Lisätään kerrokseen
			List_Kerros.Items.Add(List_Valikkeet.SelectedItem.ToString());
		}

		/// <summary>
		/// Poistaa valitun välikkeen kerroksesta.
		/// </summary>
		/// <param name="sender">this.Btn_Poista</param>
		void Btn_Poista_Click(System.Object sender, System.EventArgs e)
		{
			// Tarkistetaan, että jotain on valittuna
			if (List_Kerros.SelectedItem == null)
			{
				return;
			}

			// Poistetaan valittu välike kerroksesta
			List_Kerros.Items.RemoveAt(List_Kerros.SelectedIndex);
		}

		/// <summary>
		/// Palauttaa kerroksen tiedot ja sulkee sivun
		/// </summary>
		/// <param name="sender">this.Btn_Suljesivu</param>
		void Btn_Suljesivu_Click(System.Object sender, System.EventArgs e)
		{
			string valikkeet = string.Empty;

			foreach (var valike in List_Kerros.Items)
			{
				// Haetaan tietokannasta välikkeen nimelle numero
				foreach (DataRow rivi in data.Tables[0].Rows)
				{
					try
					{
						if (rivi[0].ToString() == valike.ToString())
						{
							// Luetaan välikkeen numero
							int nro = Convert.ToInt16(rivi["Number"]);
							if (nro > 0)
							{
								// Lisätään kerroksen välikkeisiin
								valikkeet += nro.ToString() + ",";
							}
							// Ulos loopista seuraavaan välikkeeseen
							break;
						}
					}
					catch (Exception)
					{

					}
				}
			}

			// Jos viimeinen merkki on , niin poistetaan se
			if (valikkeet.EndsWith(","))
			{
				valikkeet = valikkeet.Substring(0, valikkeet.Length - 1);
			}

			// Jos kerros on tyhjä
			if (valikkeet == string.Empty)
			{
				// Merkitään tyhjäksi
				valikkeet += "0";
			}
			// Tallennetaan
			Globals.Tags.HMI_ProdReg_ValikeKerrosValikkeet.Value = valikkeet;

			// Suljetaan sivu
			this.Close();
		}

		/// <summary>
		/// Tyhjentää koko kerroksen välikkeet.
		/// </summary>
		/// <param name="sender">this.Btn_Tyhjenna</param>
		void Btn_Tyhjenna_Click(System.Object sender, System.EventArgs e)
		{
			List_Kerros.Items.Clear();
		}
	}
}

