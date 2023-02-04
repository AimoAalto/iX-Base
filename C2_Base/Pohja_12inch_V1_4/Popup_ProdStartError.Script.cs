namespace Neo.ApplicationFramework.Generated
{
    using System;
    
    
	/// <summary>
	/// Näyttää robotin virhekoodin hylätystä aloituksesta.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 7.7.2017</remarks>
    public partial class Popup_ProdStartError
    {
		/// <summary>
		/// Näyttää robotin virhekoodin sivun avautuessa.
		/// </summary>
		/// <param name="sender">this</param>
		void Popup_ProdStartError_Opened(System.Object sender, System.EventArgs e)
		{
			Text_Virhekoodi.Text = Globals.Tags.GetTagValue("Rob" + Globals.Tags.HMI_RobotNo.Value + "_Aloitusvirhe");

		}
		
		/// <summary>
		/// Nollaa robotin aloitusvirheen ikkunan sulkeutuessa, 
		/// jotta ikkuna voi avautua uudelleen.
		/// </summary>
		/// <param name="sender">this</param>
		void Btn_OK_Click(System.Object sender, System.EventArgs e)
		{
			// Sivun sulkeutuessa nollataan virhe ja sallitaan uuden sivun avautuminen
			Globals.Tags.SetTagValue("Rob" + Globals.Tags.HMI_RobotNo.Value + "_Aloitusvirhe", 0);
			
			// Suljetaan sivu
			this.Close();
		}
		
		
    }
}
