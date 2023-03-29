namespace Neo.ApplicationFramework.Generated
{
	using System;


	/// <summary>
	/// Mahdollistaa käyttäjän sisään ja uloskirjautumisen.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 6.7.2017</remarks>
	public partial class Popup_Login
	{
		/// <summary>
		/// Liittyy käyttäjän vaihtumosen seurantaan.
		/// </summary>
		/// <param name="sender">this</param>
		void Login_Opened(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.SystemTagCurrentUser.ValueChange += SystemTagCurrentUser_ValueChange;
		}

		/// <summary>
		/// Sulkee Login-ikkunan automaattisesti, kun käyttäjä vaihtuu
		/// </summary>
		/// <param name="sender">SystemTagCurrentUser</param>
		void SystemTagCurrentUser_ValueChange(System.Object sender, Core.Api.DataSource.ValueChangedEventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Irroittautuu käyttäjän vaihtumisen seurannasta.
		/// </summary>
		/// <param name="sender">this</param>
		void Login_Closed(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.SystemTagCurrentUser.ValueChange -= SystemTagCurrentUser_ValueChange;
		}
	}
}
