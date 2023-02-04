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
		int robotti = 1;
		
		void Robots_Pan1_Scr1_Opened(System.Object sender, System.EventArgs e)
		{
			
		}

		
		/// Lähettää numeronapin painalluksen robotille.
		void Button_Virtoip_1_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Kuittaus 1 - " + Numero.Value);
			Globals.Robotit.robotit[robotti].KuittaaHairio(1, Convert.ToInt16(Numero.Value));
		}
		
		/// Lähettää numeronapin painalluksen robotille.
		void Button_Virtoip_2_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Kuittaus 2 - " + Numero.Value);
			Globals.Robotit.robotit[robotti].KuittaaHairio(2, Convert.ToInt16(Numero.Value));
		}
		
		/// Lähettää numeronapin painalluksen robotille.
		void Button_Virtoip_3_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Kuittaus 3 - " + Numero.Value);
			Globals.Robotit.robotit[robotti].KuittaaHairio(3, Convert.ToInt16(Numero.Value));
		}
		
		/// Lähettää numeronapin painalluksen robotille.
		void Button_Virtoip_4_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Kuittaus 4 - " + Numero.Value);
			Globals.Robotit.robotit[robotti].KuittaaHairio(4, Convert.ToInt16(Numero.Value));
		}
		
		/// Lähettää numeronapin painalluksen robotille.
		void Button_Virtoip_5_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Kuittaus 5 - " + Numero.Value);
			Globals.Robotit.robotit[robotti].KuittaaHairio(5, Convert.ToInt16(Numero.Value));
		}
		
		/// Lähettää numeronapin painalluksen robotille.
		void Button_Virtoip_6_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Kuittaus 6 - " + Numero.Value);
			Globals.Robotit.robotit[robotti].KuittaaHairio(6, Convert.ToInt16(Numero.Value));	
		}
		
		/// Lähettää numeronapin painalluksen robotille.
		void Button_Virtoip_7_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Kuittaus 7 - " + Numero.Value);
			Globals.Robotit.robotit[robotti].KuittaaHairio(7, Convert.ToInt16(Numero.Value));
		}
		
		/// Lähettää numeronapin painalluksen robotille.		
		void Button_Virtoip_8_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Kuittaus 8 - " + Numero.Value);
			Globals.Robotit.robotit[robotti].KuittaaHairio(8, Convert.ToInt16(Numero.Value));
		}
		
		/// Lähettää robotille pyynnön ajaa huoltoasemaan.
		void Button_ServicePos_Click(System.Object sender, System.EventArgs e)
		{
			Globals.Robotit.robotit[robotti].Loki.LisaaLokiin("Ajopyyntö huoltoasemaan.");
			Globals.Robotit.robotit[robotti].AjaHuoltoon();
			Globals.Tags.Rob1_ServicePosSent.Value = true;
		}

	}
}
