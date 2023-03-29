namespace Neo.ApplicationFramework.Generated
{
	using System;
	using System.Collections.Generic;


	/// <summary>
	/// Kysyy käyttäjältä vahvistuksen lavanvaihtoon.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 22.3.2018</remarks>
	public partial class Popup_NewPallet
	{

		void Popup_NewPallet_Opened(System.Object sender, System.EventArgs e)
		{

		}

		/// <summary>
		/// Pyytää robottia tekemään lavanvaihdon vahvistettaessa.
		/// </summary>
		/// <param name="sender">this.Button2</param>
		void Button_OK_Click(System.Object sender, System.EventArgs e)
		{
			// Miltä lavapaikalta lava tulee
			int lavapaikka = Globals.Tags.HMI_PalletChange.Value.Int;
			int robotti = 0;
			int roboLavapaikka = -1;

			// Haetaan mikä robottilavapaikka on kyseessä
			int r;
			int l;
			if (Globals._Konfiguraatio.CurrentConfig.GetRobotinLavapaikka(lavapaikka, out r, out l))
			{
				robotti = r;
				roboLavapaikka = l;
			}

			// Lokimerkintä
			Globals.Robotit.LisaaLokiin(robotti,
				"Lavanvaihto lavapaikalle " + roboLavapaikka +
				". Tuotteita lavalla: " + Globals.Tags.GetTagValue("Rob" + robotti + "_lavap" + roboLavapaikka + "_plasl"));

			Globals.Robotit.TeeLavanvaihto(robotti, roboLavapaikka);

			// Resetoi näkyvyys
			Globals.Tags.HMI_Overview_pallet_resets.Value = false;
		}
	}
}
