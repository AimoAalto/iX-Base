namespace Neo.ApplicationFramework.Generated
{
	using System;


	/// <summary>
	/// Näyttää robotin näytön, tietoja sijainnista ja nopeusasetuksista ja 
	/// mahdollistaa numeronappien käytön myös käyttöliittymästä.
	/// Robotin numero luokan ominaisuutena skriptissä.
	/// </summary>
	/// <remarks>Viimeksi muokattu 22.3.2018</remarks>
	public partial class Robots_Pan1_Scr1
	{
		void Robots_Pan1_Scr1_Opened(System.Object sender, System.EventArgs e)
		{
			Globals.Tags.HMI_RobotNo.SetAnalog(1);
		}
	}
}
