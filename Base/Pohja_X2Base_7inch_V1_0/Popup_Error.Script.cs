namespace Neo.ApplicationFramework.Generated
{
	using System;


	public enum ErrorTexts
	{
		UnexpectedError = 0, // "Unexpected error occured.","On tapahtunut odottamaton virhe",,
		ChooseProduct = 1, // "Please choose the product to be started.","Valitse aloitettava tuote",,
		ChoosePalletPlaces = 2, // "Please choose the pallet place(s) to be started on.","Valitse aloitettavat lavapaikat",,
		PalletPlaceAlreadyOnPattern = 3, // "Selected pallet place already has a pattern.Please reset the existing pattern or contact Orfer oCare Customer service.","Valittu lavapaikka on jo valittu kuvioon",,
		PatternLoadFailed = 4, // "Pattern load failed.",,,
		PatternImageLoadFailed = 5, // "Pattern picture load failed.",,,
		RecipeLoadFailed = 6, // "Loading of recipes failed.",,,
		SendStartFailed = 7, // "Sending production start command to robot failed.",,,
		CheckStartConditions = 8, // "Please check the starting conditions.",,,
		NoPatternFile = 9, // "There is no pattern file for the selected pattern number in C:\Lavaus\Kuviot\",,,
		PatternAlreadyExist = 10, // "The selected pattern number already exists.",,,
		SelectAtleastOne = 11, // "Mark at least one choice from each area.",,,
		LayerOpenFailed = 12, // "Could not open the layer.",,,
		UsePositiveNumbers = 13, // "Input fields must be positive numbers.",,,
		SheetDataFormatError = 14, // "Please input the sheet data in format:<product number><quantity>",,,
		FillAllFields = 15, // "Please fill in all the fields.",,,
		PrintingFailed = 16, // "Printing failed.",,,
		IPAddrError = 17, // "The IP-address is not valid. Please input the address in format X.X.X.X where X is a number in 0-255.",,,
		LayerSendFailed = 18, // "Could not send new layer count to robot. Layer count was not a number.",,,
		SpeedSendFailed = 19, // "Could not send new speed and delay values to robot. All values were not numbers.",,,
		DbLoadFailed = 20, // "Loading from database failed.",,,
		CheckBoxOutOfRange = 21, // "There were not enough CheckBox-elements for a section.",,,
		PalletPlaceOutOfRange = 22, // "There are not enough PlaceBoxes to select each available pallet place.",,,
		UnknownTag = 23, // "Tag was not found:",,,
		ChoosePalletType = 24, // "Choose pallet type",,,
		NoPermission = 25, // "No permission",,,"No permission"
		NotAllowedPattern = 26,
		InfeedTrackAlreadyStarted = 27,
		PalletPlaceAlreadyStarted = 28,
		MixedPalletChooseOther = 29,
		NoImageFile = 30, //
		MixedPalletSameBox = 31,
		ProductionStartError = 32,
		NothingToDelete = 33,
		RobotIdAlreadyExist = 34,
		UnknownRobotId = 35,
		NoAllowedInfeedTracks = 36,
		Info = 37,
		UnknownPatternNumber
	};
	
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
