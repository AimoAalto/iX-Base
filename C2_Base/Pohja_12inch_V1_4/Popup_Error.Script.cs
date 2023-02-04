namespace Neo.ApplicationFramework.Generated
{
	using System;
    
    
	/// <summary>
	/// Näyttää käyttäjälle virheen. Virheen teksti valitaan ErrorText-kentän
	/// listasta tagilla HMI_Error_TextValue ja lisätietoja voi antaa
	/// tagilla HMI_Error_AdditionalInfo. Huomaa, että HMI_Error_AdditionalInfo
	/// -tagilla syötettyä tekstiä ei voida kääntää.
	/// Error tekstidialogi sisältää projektikohtaisia tekstejä.
	/// </summary>
	/// <example>Esimerkkikäyttö:
	/// <code>
	///catch (Exception ex)
	///{
	///	// Kuvion kuvan lataaminen epäonnistui
	///	Globals.Tags.HMI_Error_TextValue.SetAnalog(5); // "Pattern picture load failed."
	///	Globals.Tags.HMI_Error_AdditionalInfo.Value = ex.Message;
	///	Globals.Popup_Error.Show();
	///}
	/// </code>
	/// </example>
	/// <remarks>Viimeksi muokattu: SoPi 29.6.2017</remarks>
    public partial class Popup_Error
    {
		/// <summary>
		/// Siirtää tagin HMI_Error_AdditionalInfo arvon lisätiedoksi.
		/// Lisätietona voi olla esim Exceptionin viesti tai virheen aiheuttanut
		/// muuttuja.
		/// </summary>
		/// <param name="sender">this</param>
		void Popup_Error_Opened(System.Object sender, System.EventArgs e)
		{
			AdditionalInfo.Text = Globals.Tags.HMI_Error_AdditionalInfo.Value.String;
			
		}

    }
}
