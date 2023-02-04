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
			foreach (KeyValuePair<int, Dictionary<int, int>> robottiLavapaikat in _Konfiguraatio.RobotinLavapaikat)
			{
				if (robottiLavapaikat.Value.ContainsKey(lavapaikka))
				{
					robotti = robottiLavapaikat.Key;
					roboLavapaikka = robottiLavapaikat.Value[lavapaikka];
				}
			}

			// Lokimerkintä
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin(
				"Lavanvaihto lavapaikalle " + roboLavapaikka +
				". Tuotteita lavalla: " + Globals.Tags.GetTagValue("Rob" + robotti + "_lavap" + roboLavapaikka + "_plasl"));

			Globals.Robotit.robotit[robotti].TeeLavanvaihto(roboLavapaikka);
		}
    }
}
