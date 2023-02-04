namespace Neo.ApplicationFramework.Generated
{
	using Neo.ApplicationFramework.Controls.Script; 
	using Neo.ApplicationFramework.Tools.OpcClient;
	using Neo.ApplicationFramework.Tools.Recipe;
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Windows.Controls;
	   
	
	/// <summary>
	/// Mahdollistaa projektissa käytettävien välikkeiden muokkaamisen.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 7.7.2017</remarks>
	public partial class Popup_Settings_ValikeEdit
	{
		/// <summary>
		/// Väliketietokannan sisältö.
		/// </summary>
		DataSet data;
		
		/// <summary>
		/// Päivittää välikkeet näytölle sivun avautuessa.
		/// </summary>
		/// <param name="sender">this</param>
		void StartProduction_ValikeEdit_Opened(System.Object sender, System.EventArgs e)
		{
			// Päivitetään tietokannasta välikkeet näytölle
			Paivita();
		}
		
		/// <summary>
		/// Lisää valitun välikkeen projektin käytettäväksi ja päivittää näytön.
		/// </summary>
		/// <param name="sender">Add_btn</param>
		void Add_btn_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.HMI_StartProd_ValikeSelected.Value = (string) ListBox2.SelectedItem;
			
			Globals.Valikkeet_DB.LoadRecipe(Globals.Tags.HMI_StartProd_ValikeSelected.Value);
			
			Globals.Tags.HMI_ProdReg_Valike_OnUse.Value = true;
		
			Globals.Valikkeet_DB.SaveRecipe(Globals.Tags.HMI_StartProd_ValikeSelected.Value.ToString());
			
			Paivita();
		}
		
		/// <summary>
		/// Sallii poistonapin käytön, kun hiiri on viimeksi käynyt sallittujen
		/// välikkeiden puolella.
		/// </summary>
		/// <param name="sender">this.ListBox1</param>
		void ListBox1_MouseEnter(System.Object sender, System.EventArgs e)
		{
			Add_btn.IsEnabled = false;
			Remove_btn.IsEnabled = true;
		}
		
		/// <summary>
		/// Sallii lisäysnapin käytön, kun hiiri on viimeksi käynyt kaikkien 
		/// välikkeiden puolella.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		void ListBox2_MouseEnter(System.Object sender, System.EventArgs e)
		{
			Add_btn.IsEnabled = true;
			Remove_btn.IsEnabled = false;
		}
	
		/// <summary>
		/// Päivittää molemmat listat.
		/// </summary>
		void Paivita()
		{
			// Tyhjennetään listat aluksi
			ListBox1.Items.Clear();
			ListBox2.Items.Clear();

			// Haetaan projektissa sallitut välikkeet
			data = Globals.Valikkeet_DB.HaeKaikki();

			if (data.Tables[0] != null)
			{
				foreach (DataRow rivi in data.Tables[0].Rows)
				{
					try
					{
						// Lisätään kaikki välikkeet valittavaksi
						ListBox2.Items.Add(rivi[0]);

						// Lisätään käytössä olevat käytössä oleviin
						if (Convert.ToBoolean(rivi["UsedInProject"].ToString()) == true && Convert.ToInt16(rivi["Number"].ToString()) > 0)
						{
							ListBox1.Items.Add(rivi[0]);
						}
					}
					catch (Exception)
					{

					}
				}
			}
		}		
		
		/// <summary>
		/// Poistaa valitun välikkeen projektin käytöstä.
		/// </summary>
		/// <param name="sender">this.Remove_btn</param>
		void Remove_btn_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.HMI_StartProd_ValikeSelected.Value = (string) ListBox1.SelectedItem;
		    
			Globals.Valikkeet_DB.LoadRecipe(Globals.Tags.HMI_StartProd_ValikeSelected.Value);
			
			Globals.Tags.HMI_ProdReg_Valike_OnUse.Value = false;
			
			Globals.Valikkeet_DB.SaveRecipe(Globals.Tags.HMI_StartProd_ValikeSelected.Value.ToString());

			Paivita();
		}
		
	}
}