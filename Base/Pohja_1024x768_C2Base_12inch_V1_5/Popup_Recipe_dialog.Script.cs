namespace Neo.ApplicationFramework.Generated
{
	using Neo.ApplicationFramework.Tools.Recipe;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Linq;


	/// <summary>
	/// Lataa, luo uuden tai poistaa reseptejä tietokannasta.
	public partial class Popup_Recipe_dialog
	{
		/// <summary>
		/// Lataa reseptit näytölle sivun avautuessa.
		/// </summary>
		/// <param name="sender">this</param>
		void Recipe_dialog_Opened(System.Object sender, System.EventArgs e)
		{

			// Tyhjätään reseptilista ennen uutta latausta
			ListBox1.Items.Clear();

			// Ei oletusvalintaa
			ReseptiKentta.Text = "";

			// Jos tallenna nimellä, tallennettavan tuotenumero
			if (Globals.Tags.HMI_ProdReg_Dialog_Mode.Value == 2)
			{
				TuotenumeroKentta.Text = Globals.Tags.HMI_ProdReg_ProductNo.Value.ToString();
			}
			else
			{
				TuotenumeroKentta.Text = "";
			}

			// Haetaan reseptien nimet ja rivinumerot
			try
			{
				// Tehdään kysely
				DataSet data1 = Globals.Tuotetiedot.HaeKaikki();

				if (data1.Tables.Count > 0)
				{
					// Käydään kaikki reseptit läpi
					List<Resepti> Reseptit = new List<Resepti>();
					foreach (DataRow rivi in data1.Tables[0].Rows)
					{
						// Lisätään resepti listaan
						Reseptit.Add(new Resepti()
						{
							Nimi = rivi["FieldName"].ToString(),
							Numero = Convert.ToInt32(rivi["Tuotenumero"]),
							RiviNro = Convert.ToInt32(rivi["RiviNro"])
						});
					}

					// Luetaan kaikki reseptit näytölle halutussa järjestyksessä
					foreach (Resepti r in Reseptit.OrderBy(i => i.Numero).ThenBy(j => j.Nimi))
					{
						// Lisätään resepti näytölle
						ListBox1.Items.Add(r);
					}
				}
			}
			catch (Exception ex)
			{
				// Reseptien lataus epäonnistui
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.RecipeLoadFailed);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message + "; " + ex.InnerException.Message;
				Globals.Popup_Error.Show();
			}
		}

		/// <summary>
		/// Näyttää valitun reseptin tiedot ja valitsee reseptin taustalle 
		/// latausta varten.
		/// </summary>
		/// <param name="sender">this.ListBox1</param>
		void ListBox1_SelectionChanged(System.Object sender, System.EventArgs e)
		{
			// Luetaan nimi näytölle muokattavaksi
			Resepti r = (Resepti)ListBox1.SelectedItem;
			ReseptiKentta.Text = r.Nimi;
			if (Globals.Tags.HMI_ProdReg_Dialog_Mode.Value != 2)
			{
				TuotenumeroKentta.Text = r.Numero.ToString();
			}
			// Valitaan resepti myös taustalle
			Globals.Tags.HMI_ProdReg_RecipeSelected.Value = r.Nimi;
		}

		/// <summary>
		/// Lataa, tallentaa uuden tai poistaa reseptin perustuen, mikä dialogi avattiin.
		/// </summary>
		/// <param name="sender">this.OK_btn</param>
		/// <remarks>Muokattu 21.8.2017 SoPi - Tallenna nimellä päälle tallennettaessa käytetään vanhan reseptin rivinumeroa.</remarks>
		void OK_btn_Click(System.Object sender, System.EventArgs e)
		{
			try
			{
				// 1 = Ladataan resepti
				if (Globals.Tags.HMI_ProdReg_Dialog_Mode.Value == 1)
				{
					// Käytetään viimeksi valittua resepti, ei käsin muokattavissa olevaa nimeä
					if (ListBox1.SelectedItem != null)
					{
						// Luetaan valittu resepti
						Resepti r = (Resepti)ListBox1.SelectedItem;
						Globals.Tuotetiedot.LoadRecipe(r.Nimi);
						Globals.Tags.HMI_ProdReg_ProductName.Value = r.Nimi;

						// Suljetaan ikkuna
						this.Close();
					}
				}

				// 2 = Tallennetaan resepti
				if (Globals.Tags.HMI_ProdReg_Dialog_Mode.Value == 2)
				{
					// Luetaan reseptin nimi laatikosta
					string resepti = ReseptiKentta.Text;

					if (resepti.Length < 1)
					{
						// Tyhjää nimeä ei saa tallentaa
						return;
					}

					// Tarkistetaan onko tälläinen resepti jo olemassa
					KeyValuePair<bool, int> olemassa = Globals.Tuotetiedot.ReseptiOlemassa(resepti);
					if (olemassa.Key)
					{
						// Käytetään vanhan reseptin IDtä, ettei tule duplikaatteja
						Globals.Tags.HMI_ProdReg_RiviNro.Value = olemassa.Value;

						// Muokataan vanhaa reseptiä
						Globals.Tags.HMI_ProdReg_ProductName.Value = resepti;
						Globals.Tuotetiedot.SaveRecipe(resepti, true);
					}
					else
					{
						// Tallennetaan uutena
						// Luetaan tietokannan maksimi ID
						int ID = Globals.Tuotetiedot.HaeMaxID();

						// Päivitetään reseptin tietoihin uusi ID
						Globals.Tags.HMI_ProdReg_RiviNro.Value = ID;

						// Tallennetaan resepti
						Globals.Tags.HMI_ProdReg_ProductName.Value = resepti;
						Globals.Tuotetiedot.SaveRecipe(resepti);
					}

					// Suljetaan ikkuna
					this.Close();
				}

				// 3 = Poistetaan resepti
				if (Globals.Tags.HMI_ProdReg_Dialog_Mode.Value == 3)
				{
					// Luetaan valittu resepti
					if (ListBox1.SelectedItem != null)
					{
						// Luetaan valittu resepti
						Resepti r = (Resepti)ListBox1.SelectedItem;
						Globals.Tuotetiedot.DeleteRecipe(r.Nimi);
						// Tyhjennetään valittu resepti
						Globals.Tags.HMI_ProdReg_ProductName.Value = "";

						// Suljetaan ikkuna
						this.Close();
					}
				}
			}
			catch (Exception ex)
			{
				Globals.Tags.HMI_Error_TextValue.SetAnalog((int)Neo.ApplicationFramework.Generated.ErrorTexts.UnexpectedError);
				Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
				Globals.Popup_Error.Show();
			}
		}
	}

	/// <summary>
	/// Luokka reseptin järkevään esittämiseen ListBox:ssa.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 20.6.2017</remarks>
	public class Resepti
	{
		/// <summary>
		/// Reseptin yksilöllinen nimi.
		/// </summary>
		public string Nimi { get; set; }
		/// <summary>
		/// Reseptin tuotenumero.
		/// </summary>
		public int Numero { get; set; }
		/// <summary>
		/// Reseptin yksilöllinen rivinumero tietokannassa.
		/// </summary>
		public int RiviNro { get; set; }

		public int KuvioNro { get; set; }

		/// <summary>
		/// Näyttää reseptin tiedot järkevässä muodossa ListBox:ssa.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			//return string.Format("{0} {1,-35} [{2}]", Numero, Nimi, KuvioNro);
			return string.Format("{0} {1}", Numero, Nimi);
		}

	}
}
