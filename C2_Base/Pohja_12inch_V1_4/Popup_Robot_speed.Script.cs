namespace Neo.ApplicationFramework.Generated
{
	using System;


	/// <summary>
	/// Näyttää ja antaa muokata robotin nopeusarvoja kesken tuotannon.
	/// </summary>
	/// <remarks>Viimeksi muokattu: SoPi 22.3.2018</remarks>
	public partial class Popup_Robot_speed
	{
		/// <summary>
		/// Alustaa sivun arvot robottinumeron mukaan sivun avautuessa.
		/// </summary>
		/// <param name="sender">this</param>
		void Popup_Robot_speed_Opened(System.Object sender, System.EventArgs e)
		{
			// Arvot talteen tageista
			tyhja_nop.Value = Globals.Tags.GetTagValue("Rob" + Globals.Tags.HMI_RobotNo.Value + "_nopker");
			tyhja_kii.Value = Globals.Tags.GetTagValue("Rob" + Globals.Tags.HMI_RobotNo.Value + "_kiiker");
			lava_nop.Value = Globals.Tags.GetTagValue("Rob" + Globals.Tags.HMI_RobotNo.Value + "_nopker0");
			lava_kii.Value = Globals.Tags.GetTagValue("Rob" + Globals.Tags.HMI_RobotNo.Value + "_kiiker0");
			pahvi_nop.Value = Globals.Tags.GetTagValue("Rob" + Globals.Tags.HMI_RobotNo.Value + "_nopker1");
			pahvi_kii.Value = Globals.Tags.GetTagValue("Rob" + Globals.Tags.HMI_RobotNo.Value + "_kiiker1");
		}

		/// <summary>
		/// Lähettää nopeusarvot robotille.
		/// </summary>
		/// <param name="sender">this.laheta</param>
		void Button_Laheta_Click(System.Object sender, System.EventArgs e)
		{
			int rno = Globals.Tags.HMI_RobotNo.Value;
			Globals.Robotit.LisaaLokiin(rno, "Uudet nopeusarvot: " + tyhja_nop.Value + ", " + tyhja_kii.Value + ", " + lava_nop.Value + ", " + lava_kii.Value + ", " + pahvi_nop.Value + ", " + pahvi_kii.Value);
			Globals.Robotit.Nopeus(rno,
				Convert.ToInt16(tyhja_nop.Value), Convert.ToInt16(tyhja_kii.Value),
				Convert.ToInt16(lava_nop.Value), Convert.ToInt16(lava_kii.Value),
				Convert.ToInt16(pahvi_nop.Value), Convert.ToInt16(pahvi_kii.Value));
		}
	}
}
